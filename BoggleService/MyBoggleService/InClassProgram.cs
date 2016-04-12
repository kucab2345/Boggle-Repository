using CustomNetworking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Boggle;
using System.IO;

//By Henry and Ryan
namespace SimpleWebServer
{
    public class WebServer
    {
        public static void Main()
        {
            new WebServer();
            Console.Read();
        }

        private TcpListener server;

        public WebServer()
        {
            server = new TcpListener(IPAddress.Any, 60000);
            server.Start();
            server.BeginAcceptSocket(ConnectionRequested, null);
        }

        private void ConnectionRequested(IAsyncResult ar)
        {
            Socket s = server.EndAcceptSocket(ar);
            server.BeginAcceptSocket(ConnectionRequested, null);
            new HttpRequest(new StringSocket(s, new UTF8Encoding()));
        }
    }

    class HttpRequest
    {
        private BoggleService Service = new BoggleService();
        private StringSocket ss;
        private int lineCount;
        private int contentLength;
        private string MethodType;
        private string URLAddress;

        public HttpRequest(StringSocket stringSocket)
        {
            this.ss = stringSocket;
            ss.BeginReceive(LineReceived, null);
        }

        private void LineReceived(string s, Exception e, object payload)
        {
            lineCount++;
            
            if (s != null)
            {
                if (lineCount == 1)
                {
                    Regex r = new Regex(@"^(\S+)\s+(\S+)");
                    Match m = r.Match(s);
                    
                    MethodType = m.Groups[1].Value; 
                    
                    URLAddress = m.Groups[2].Value;
                }
                if (MethodType.Equals("GET") && URLAddress == "/")
                {
                    getAPI();
                }
                if (MethodType.Equals("GET"))
                {
                    GetContent(s);
                }
                if (s.StartsWith("Content-Length:"))
                {
                    contentLength = Int32.Parse(s.Substring(16).Trim());
                }
                if (s == "\r")
                {
                    ss.BeginReceive(ContentReceived, null, contentLength);
                }
                else
                {
                    ss.BeginReceive(LineReceived, null);
                }
            }
        }


        private void getAPI()
        {
            ss.BeginSend("HTTP/1.1 200 OK\n", Ignore, null);
            var API = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "..\\index.html");
            ss.BeginSend("Content-Length: text/html\r\n", Ignore, null);
            ss.BeginSend("\r\n", Ignore, null);
            ss.BeginSend(API, (ex, py) => { ss.Shutdown(); }, null);
        }

        private void GetContent(string s)
        {
            Regex r = new Regex(@"^/BoggleService.svc/games/(\d+)$");
            Regex r1 = new Regex(@"^/BoggleService.svc/games/\d+$");
            Match m = r.Match(URLAddress);
            string GameID = m.Groups[1].Value;

            Regex rbrief = new Regex(@"^/BoggleService.svc/games/\d+\?brief=(.*)$");
            Match mbrief = rbrief.Match(URLAddress);
            string briefLine = mbrief.Groups[1].Value;

            if(!r1.IsMatch(URLAddress) && !rbrief.IsMatch(URLAddress))
            {
                ss.BeginSend("HTTP:/1.1 404 Not Found\r\n", (ex, py) => { ss.Shutdown(); }, null);
                return;
            }

            GameStatus user = JsonConvert.DeserializeObject<GameStatus>(s);

            GameStatus var = Service.GetFullGameStatus(GameID);

            string result = JsonConvert.SerializeObject(var, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            ss.BeginSend("HTTP/1.1 200 OK\r\n", Ignore, null);
            ss.BeginSend("Content-Type: application/json\r\n", Ignore, null);
            ss.BeginSend("Content-Length: " + result.Length + "\r\n", Ignore, null);
            ss.BeginSend("\r\n", Ignore, null);
            ss.BeginSend(result, (ex, py) => { ss.Shutdown(); }, null);
        }

        private void ContentReceived(string s, Exception e, object payload)
        {
            if (s != null)
            {
                string method = methodChooser();
                switch (method)
                {
                    case ("CreateUser"):
                        {
                            CreateUser(s);
                            break;
                        }

                    case ("JoinGame"):
                        {
                            JoinGame(s);
                            break;
                        }
                    case ("CancelGame"):
                        {
                            CancelGame(s);
                            break;
                        }
                    case ("PlayWord"):
                        {
                            PlayWord(s);
                            break;
                        }
                    default:
                        {
                            ss.BeginSend("HTTP:/1.1 404 Not Found\r\n", (ex, py) => { ss.Shutdown(); }, null);
                            break;
                         }
             
                }
                
            }
        }
        

        private void PlayWord(string s)
        {
            UserGame user = JsonConvert.DeserializeObject<UserGame>(s);

            Regex r = new Regex(@"^/BoggleService.svc/games/(\d+)$");
            Match m  = r.Match(URLAddress);

            TokenScoreGameIDReturn var = Service.playWord(user, m.Groups[1].Value);

            string result = JsonConvert.SerializeObject(var, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            ss.BeginSend("HTTP/1.1 " + (int)Service.ActualStatus + Service.ActualStatus.ToString() + "\r\n", Ignore, null);
            ss.BeginSend("Content-Type: application/json\r\n", Ignore, null);
            ss.BeginSend("Content-Length: " + result.Length + "\r\n", Ignore, null);
            ss.BeginSend("\r\n", Ignore, null);
            ss.BeginSend(result, (ex, py) => { ss.Shutdown(); }, null);
            
        }

        private void CancelGame(string s)
        {
            UserGame user = JsonConvert.DeserializeObject<UserGame>(s);

            Service.CancelGame(user);

            ss.BeginSend("HTTP/1.1 " + (int)Service.ActualStatus + Service.ActualStatus.ToString() + "\r\n", (ex, py) => { ss.Shutdown(); }, null);
            
            
        }

        private void JoinGame(string s)
        {
            GameJoin user = JsonConvert.DeserializeObject<GameJoin>(s);

            TokenScoreGameIDReturn var = Service.JoinGame(user);

            string result = JsonConvert.SerializeObject(var, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            ss.BeginSend("HTTP/1.1 " + (int)Service.ActualStatus + Service.ActualStatus.ToString() + "\r\n", Ignore, null);
            ss.BeginSend("Content-Type: application/json\r\n", Ignore, null);
            ss.BeginSend("Content-Length: " + result.Length + "\r\n", Ignore, null);
            ss.BeginSend("\r\n", Ignore, null);
            ss.BeginSend(result, (ex, py) => { ss.Shutdown(); }, null);
            
        }

        private void CreateUser(string s)
        {
            UserInfo user = JsonConvert.DeserializeObject<UserInfo>(s);
            
            TokenScoreGameIDReturn var = Service.RegisterUser(user);

            string result = JsonConvert.SerializeObject(var,new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            ss.BeginSend("HTTP/1.1 " + (int)Service.ActualStatus + Service.ActualStatus.ToString() + "\r\n", Ignore, null);
            ss.BeginSend("Content-Type: application/json\r\n", Ignore, null);
            ss.BeginSend("Content-Length: " + result.Length + "\r\n", Ignore, null);
            ss.BeginSend("\r\n", Ignore, null);
            ss.BeginSend(result, (ex, py) => { ss.Shutdown(); }, null);
            
        }

        private string methodChooser()
        {
            if(MethodType == "POST")
            {
                if(Regex.IsMatch(URLAddress, "/BoggleService.svc/users"))
                {
                    return "CreateUser";
                }

                if(Regex.IsMatch(URLAddress, "/BoggleService.svc/games"))
                {
                    return "JoinGame";
                }
            }
            else if(MethodType == "PUT")
            {
                if(Regex.IsMatch(URLAddress, "/BoggleService.svc/games"))
                {
                    return "CancelGame";
                }
                if(Regex.IsMatch(URLAddress, "/BoggleService.svc/games/+d\\"))
                {
                    return "PlayWord";
                }
            }

            return "Failure";

        }
        private void Ignore(Exception e, object payload)
        {
        }
    }
}