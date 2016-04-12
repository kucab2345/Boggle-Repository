using CustomNetworking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Boggle;

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
            Console.WriteLine(s);
            if (s != null)
            {
                if (lineCount == 1)
                {
                    Regex r = new Regex(@"^(\S+)\s+(\S+)");
                    Match m = r.Match(s);
                    Console.WriteLine("Method: " + m.Groups[1].Value);
                    MethodType = m.Groups[1].Value; 
                    Console.WriteLine("URL: " + m.Groups[2].Value);
                    URLAddress = m.Groups[2].Value;
                }
                if (MethodType.Equals("GET"))
                {
                    ss.BeginReceive(ContentReceived, null);
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
                    case ("BriefStatus"):
                        {
                            BriefStatus(s);
                            break;
                        }
                    case ("FullStatus"):
                        {
                            FullStatus(s);
                            break;
                        }
                }
                Person p = JsonConvert.DeserializeObject<Person>(s);
                Console.WriteLine(p.Name + " " + p.Eyes);

                // Call service method

                string result =
                    JsonConvert.SerializeObject(
                            new Person { Name = "June", Eyes = "Blue" },
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                ss.BeginSend("HTTP/1.1 200 OK\n", Ignore, null);
                ss.BeginSend("Content-Type: application/json\n", Ignore, null);
                ss.BeginSend("Content-Length: " + result.Length + "\n", Ignore, null);
                ss.BeginSend("\r\n", Ignore, null);
                ss.BeginSend(result, (ex, py) => { ss.Shutdown(); }, null);
            }
        }

        private void FullStatus(string s)
        {
            throw new NotImplementedException();
        }

        private void BriefStatus(string s)
        {
            throw new NotImplementedException();
        }

        private void PlayWord(string s)
        {
            throw new NotImplementedException();
        }

        private void CancelGame(string s)
        {
            throw new NotImplementedException();
        }

        private void JoinGame(string s)
        {
            throw new NotImplementedException();
        }

        private void CreateUser(string s)
        {
            throw new NotImplementedException();
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
                if(Regex.IsMatch(URLAddress, " /BoggleService.svc/games/:GameID"))
                {
                    return "PlayWord";
                }
            }

        }
        private void Ignore(Exception e, object payload)
        {
        }
    }

    public class Person
    {
        public String Name { get; set; }
        public String Eyes { get; set; }
    }
}