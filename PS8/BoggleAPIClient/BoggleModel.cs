using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace BoggleAPIClient
{
    public class BoggleModel
    {
        public string nickname;

        public string gameID;

        private string userToken;

        private HttpClient client;

        private string server;

        public char[] boardState;

        public bool GamePlaying { get; set; }

        private bool gamePending;
        public bool gameCompleted;
        public bool gameCreation;
        public int gameTime;
        public int player1Score;
        public int player2Score;

        public string player2Name;

        public BoggleModel(string serverDest)
        {
            server = serverDest;
        }

        public Task playGame()
        {
            using (HttpClient client = CreateClient())
            {
                String url = String.Format("games/{0}/Brief=yes", gameID);

                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic deserResult = JsonConvert.DeserializeObject(result);

                    if(gamePending && deserResult.GameState == "active")
                    {
                        gamePending = false;
                        gameCreation = true;
                        Task gameMake = new Task(() => gameSetup());
                        gameMake.Start();
                        gameMake.Wait();
                    }
                    else if(deserResult.GameState == "active")
                    {
                        int generalInt;
                        if (int.TryParse(deserResult.TimeLeft, out generalInt))
                        {
                            gameTime = generalInt;
                        }

                        if (int.TryParse(deserResult.Player1.Score, out generalInt))
                        {
                            player1Score = generalInt;
                        }

                        if (int.TryParse(deserResult.Player2.Score, out generalInt))
                        {
                            player2Score = generalInt;
                        }

                    }
                    if(deserResult.GameState == "completed")
                    {
                        gameCompleted = true;
                    }
                    
                }
                else
                {

                }
            }
            return Task.FromResult(0);
        }

        private Task gameSetup()
        {
            using (HttpClient client = CreateClient())
            {
                String url = String.Format("games/{0}, gameID");

                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic deserResult = JsonConvert.DeserializeObject(result);

                    boardState = deserResult.Board.ToCharArray();

                    player2Name = deserResult.Player2.Nickname;

                    int generalInt;
                    if (int.TryParse(deserResult.TimeLeft, out generalInt))
                    {
                        gameTime = generalInt;
                    }

                    if (int.TryParse(deserResult.Player1.Score, out generalInt))
                    {
                        player1Score = generalInt;
                    }

                    if (int.TryParse(deserResult.Player2.Score, out generalInt))
                    {
                        player2Score = generalInt;
                    }
                }
            }
                    return Task.FromResult(0);
            
        }

        private HttpClient CreateClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(server);

            // Tell the server that the client will accept this particular type of response data
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }



        public Task createUser(string userName)
        {
            nickname = userName;
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.Nickname = userName;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("users", content).Result;
                
                //HttpResponseMessage response = responseResult;
                
                if (response.IsSuccessStatusCode)
                {
                    // The deserialized response value is an object that describes the new repository.
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic deserResult = JsonConvert.DeserializeObject(result);
                    userToken = deserResult.UserToken;
                    Console.WriteLine(userToken);
                }
                else
                {
                    //throw new Exception();
                    Console.WriteLine("Error creating user: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }

                return Task.FromResult(0);
            }
        }

        public Task createGame(int gameTime)
        {
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;
                data.TimeLimit = gameTime;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("games", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    // The deserialized response value is an object that describes the new repository.
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic deserResult = JsonConvert.DeserializeObject(result);
                    gameID = deserResult.GameID;
                    gamePending = true;
                    Console.WriteLine(gameID);
                }
                else
                {
                    Console.WriteLine("Error creating game: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
            return Task.FromResult(0);
        }
        /*
        public async void getGameStatus()
        {
            using(HttpClient client = CreateClient())
            {
                String url = String.Format("games/{0}/Brief=yes", gameID);

                Task<HttpResponseMessage> responseResult = new Task<HttpResponseMessage>(() => client.GetAsync(url).Result);
                

                HttpResponseMessage response = await responseResult;
                

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic deserResult = JsonConvert.DeserializeObject(result);
                    
                    //if(deserResult.GameState == "active" && !gameCreated)
                    //{
                    //    gameSetup(deserResult);
                    //}
                    //else if(deserResult.GameState == "active")
                    //{
                    //    gameCurrentState(deserResult);
                    //}
                    //else if()

                    Console.WriteLine(gameID);
                }
                else
                {
                    
                }
            }
        }
        */
        public Task cancelJoinRequest()
        {
            using(HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsync("games", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Game cancelled");
                    gamePending = false;
                }
                else
                {
                    //throw new Exception();
                }
            }
            return Task.FromResult(0);
        }
        
    }
}
