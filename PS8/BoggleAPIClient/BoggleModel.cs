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
    /// <summary>
    /// This classis the model class for the Boggle Client.  This will handle all primary server interactions and data that is received/submitted to the server.
    /// </summary>
    public class BoggleModel
    {
        /// <summary>
        /// This is the name of the first player in the game.
        /// </summary>
        public string player1Name;

        /// <summary>
        /// This is the name of the second player in the game.
        /// </summary>
        public string player2Name;

        /// <summary>
        /// This is the gameID token that the server sends back when you create/join a game.  
        /// This is used to ensure that the right game is being used when submitting words to, or retrieving data about a game.
        /// </summary>
        public string gameID;

        /// <summary>
        /// This is the token that the server uses to identify the user.  This is server created when a user is created.
        /// </summary>
        private string userToken;

        /// <summary>
        /// This is the client that is used to communicate with the server.  There is a new instance created every time when there is a new request made to the server.
        /// </summary>
        private HttpClient client;

        /// <summary>
        /// This is a string of what the user has inputed for their desired server address.
        /// </summary>
        private string serverAddress;

        /// <summary>
        /// This will contain the board state of the game, received from the server when the game is made.
        /// </summary>
        public char[] boardState;

        /// <summary>
        /// This tells whether there is an active game happening or not.  This allows the controller to only need to do brief pings 
        /// </summary>
        public bool GamePlaying { get; set; }

        public bool GamePending { get; set; }
        public bool GameCompleted { get; set; }
        public bool GameCreation { get; set;}
        public int GameTime { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        
        private bool playerIs1;


        public List<string> player1Words { get; set; }

        public List<string> player2Words { get; set; }

        public BoggleModel(string serverDest)
        {
            serverAddress = serverDest;

            player1Words = new List<string>();
            player2Words = new List<string>();
        }

        public Task playGame(CancellationToken ct)
        {
            using (HttpClient client = CreateClient())
            {
                String url = String.Format("games/{0}?Brief=yes", gameID);
                try
                {
                    HttpResponseMessage response = client.GetAsync(url, ct).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        dynamic deserResult = JsonConvert.DeserializeObject(result);

                        if (GamePending && deserResult.GameState == "active")
                        {
                            GamePending = false;
                            GameCreation = true;
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
                                GameTime = generalInt;
                            }

                            parsedResult = deserResult.Player1.Score;
                            if (int.TryParse(parsedResult, out generalInt))
                            {
                                Player1Score = generalInt;
                            }

                            parsedResult = deserResult.Player2.Score;
                            if (int.TryParse(parsedResult, out generalInt))
                            {
                                Player2Score = generalInt;
                            }

                        }
                        if (deserResult.GameState == "completed")
                        {
                            GameCompleted = true;
                            GamePlaying = false;
                        }

                    }
                    else
                    {

                    }
                }

                catch (AggregateException e)
                {
                    GamePending = false;
                    GamePlaying = false;
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
                        GameTime = generalInt;
                    }

                    parsedResult = deserResult.Player1.Score;
                    if (int.TryParse(parsedResult, out generalInt))
                    {
                        Player1Score = generalInt;
                    }

                    parsedResult = deserResult.Player2.Score;
                    if (int.TryParse(parsedResult, out generalInt))
                    {
                        Player2Score = generalInt;
                    }

                    foreach (var item in deserResult.Player1.WordsPlayed)
                    {
                        string word = item.Word;
                        string score = item.Score;
                        player1Words.Add("Word: " + word + " Score: " + score);

                    }

                    foreach (var item in deserResult.Player2.WordsPlayed)
                    {
                        string word = item.Word;
                        string score = item.Score;
                        player2Words.Add("Word: " + word + " Score: " + score);

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
                        GameTime = generalInt;
                    }
                    player1Name = deserResult.Player1.Nickname;
                    player2Name = deserResult.Player2.Nickname;
                    parsedResult = deserResult.Player1.Score;
                    if (int.TryParse(parsedResult, out generalInt))
                    {
                        Player1Score = generalInt;
                    }

                    parsedResult = deserResult.Player2.Score;
                    if (int.TryParse(parsedResult, out generalInt))
                    {
                        Player2Score = generalInt;
                    }

                }
            }
            return Task.FromResult(0);

        }

        private HttpClient CreateClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(serverAddress);

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

                try
                {
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
                }

                catch (AggregateException e)
                {

                    GamePending = false;
                    GamePlaying = false;
                }

                return Task.FromResult(0);
            }
        }

        public Task createGame(int gameTime, CancellationToken ct)
        {
            GameCompleted = false;
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;
                data.TimeLimit = gameTime;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                try
                {
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
                            GameCreation = true;
                            GamePlaying = true;
                            gameMake.Wait();
                        }
                    }
                    else
                    {
                        
                    }
                }
                catch (AggregateException e)
                {
                    GamePending = false;
                    GamePlaying = false;
                }
            }
            return Task.FromResult(0);
        }
       

        public Task cancelJoinRequest(CancellationToken ct)
        {
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                try {
                    HttpResponseMessage response = client.PutAsync("games", content, ct).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Game cancelled");
                        GamePending = false;
                        GamePlaying = false;
                        
                    }
                    else
                    {
                        //throw new Exception();
                    }
                }
                catch (AggregateException e)
                {
                    
                }
            }
            return Task.FromResult(0);
        }

        public Task submitWord(string word, CancellationToken ct)
        {
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;
                data.Word = word;

                String url = String.Format("games/{0}", gameID);

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                try
                {
                    HttpResponseMessage response = client.PutAsync(url, content, ct).Result;

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
                                Player1Score += numberScore;
                            }
                            else
                            {
                                Player2Score += numberScore;
                            }
                        }
                    }
                }

                catch (AggregateException e)
                {

                    GamePending = false;
                    GamePlaying = false;
                }
            }
            return Task.FromResult(0);
        }

    }
}
