using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace BoggleAPIClient
{
    public class BoggleModel
    {
        public string player1Name;

        public string gameID;

        private string userToken;

        private HttpClient client;

        private string server;

        public char[] boardState;

        public bool GamePlaying { get; set; }

        public bool GamePending;
        public bool gameCompleted;
        public bool gameCreation;
        public int gameTime;
        public int player1Score;
        public int player2Score;
        public string player2Name;
        private bool playerIs1;
        public bool cancel;
        

        public List<string> player1Words;
        public List<string> player2Words;

        public BoggleModel(string serverDest)
        {
            server = serverDest;
            player1Words = new List<string>();
            player2Words = new List<string>();
        }

        public Task playGame(CancellationToken ct)
        {
            using (HttpClient client = CreateClient())
            {
                String url = String.Format("games/{0}", gameID);

                HttpResponseMessage response = client.GetAsync(url, ct).Result;

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic deserResult = JsonConvert.DeserializeObject(result);

                    if (GamePending && deserResult.GameState == "active")
                    {
                        GamePending = false;
                        gameCreation = true;
                        Task gameMake = new Task(() => gameSetup());
                        gameMake.Start();
                        gameMake.Wait();
                    }
                    else if (deserResult.GameState == "active")
                    {
                        int generalInt;
                        string parsedResult = deserResult.TimeLeft;
                        if (int.TryParse(parsedResult, out generalInt))
                        {
                            gameTime = generalInt;
                        }

                        parsedResult = deserResult.Player1.Score;
                        if (int.TryParse(parsedResult, out generalInt))
                        {
                            player1Score = generalInt;
                        }

                        parsedResult = deserResult.Player2.Score;
                        if (int.TryParse(parsedResult, out generalInt))
                        {
                            player2Score = generalInt;
                        }

                    }
                    if (deserResult.GameState == "completed")
                    {
                        gameCompleted = true;
                        GamePlaying = false;
                    }

                }
                else
                {

                }
            }
            return Task.FromResult(0);
        }

        public Task finalBoardSetup()
        {

            using (HttpClient client = CreateClient())
            {
                String url = String.Format("games/{0}", gameID);

                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic deserResult = JsonConvert.DeserializeObject(result);
                    int generalInt;
                    string parsedResult = deserResult.TimeLeft;
                    if (int.TryParse(parsedResult, out generalInt))
                    {
                        gameTime = generalInt;
                    }

                    parsedResult = deserResult.Player1.Score;
                    if (int.TryParse(parsedResult, out generalInt))
                    {
                        player1Score = generalInt;
                    }

                    parsedResult = deserResult.Player2.Score;
                    if (int.TryParse(parsedResult, out generalInt))
                    {
                        player2Score = generalInt;
                    }

                    foreach(var item in deserResult.Player1.WordsPlayed)
                    {
                        string word = item.Word;
                        if(word != null)
                        player1Words.Add(word);
                        
                    }

                    foreach (var item in deserResult.Player2.WordsPlayed)
                    {
                        string word = item.Word;
                        player2Words.Add(word);
                        
                    }
                }
                else
                {

                }
            }
            return Task.FromResult(0);
            throw new NotImplementedException();
        }

        private Task gameSetup()
        {
            using (HttpClient client = CreateClient())
            {
                String url = String.Format("games/{0}", gameID);

                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic deserResult = JsonConvert.DeserializeObject(result);
                    int generalInt;
                    string parsedResult = deserResult.Board;
                    boardState = parsedResult.ToCharArray();

                    parsedResult = deserResult.TimeLeft;
                    if (int.TryParse(parsedResult, out generalInt))
                    {
                        gameTime = generalInt;
                    }
                    player1Name = deserResult.Player1.Nickname;
                    player2Name = deserResult.Player2.Nickname;
                    parsedResult = deserResult.Player1.Score;
                    if (int.TryParse(parsedResult, out generalInt))
                    {
                        player1Score = generalInt;
                    }

                    parsedResult = deserResult.Player2.Score;
                    if (int.TryParse(parsedResult, out generalInt))
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



        public Task createUser(string userName, CancellationToken ct)
        {
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.Nickname = userName;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("users", content, ct).Result;

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

        public Task createGame(int gameTime, CancellationToken ct)
        {
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;
                data.TimeLimit = gameTime;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("games", content, ct).Result;

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic deserResult = JsonConvert.DeserializeObject(result);
                    gameID = deserResult.GameID;
                    if (response.StatusCode == HttpStatusCode.Accepted)
                    {
                        // The deserialized response value is an object that describes the new repository.
                        playerIs1 = true;
                        GamePending = true;
                        GamePlaying = true;
                        Console.WriteLine(gameID);
                    }
                    else if (response.StatusCode == HttpStatusCode.Created)
                    {
                        Task gameMake = new Task(() => gameSetup());
                        gameMake.Start();
                        playerIs1 = false;
                        gameCreation = true;
                        GamePlaying = true;
                        gameMake.Wait();
                    }
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
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsync("games", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Game cancelled");
                    GamePending = false;
                    GamePlaying = false;
                    cancel = true;
                }
                else
                {
                    //throw new Exception();
                }
            }
            return Task.FromResult(0);
        }

        public Task submitWord(string word)
        {
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;
                data.Word = word;

                String url = String.Format("games/{0}", gameID);

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic deserResult = JsonConvert.DeserializeObject(result);
                    string score = deserResult.Score;

                    int numberScore;
                    if (int.TryParse(score, out numberScore))
                    {
                        if (playerIs1)
                        {
                            player1Score += numberScore;
                        }
                        else
                        {
                            player2Score += numberScore;
                        }
                    }
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
