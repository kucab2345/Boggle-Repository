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
        /// This tells whether there is an active game happening or not.
        /// </summary>
        public bool GamePlaying { get; set; }

        /// <summary>
        /// This tells whether we are pending on a game request or not.  
        /// </summary>
        public bool GamePending { get; set; }

        /// <summary>
        /// This tells whether the game is finished or not.
        /// </summary>
        public bool GameCompleted { get; set; }

        /// <summary>
        /// This tells whether the game needs to be created on the board, or if the board has been set up
        /// </summary>
        public bool GameCreation { get; set;}

        /// <summary>
        /// This is the amount of time left that the server has for the game, retrieved from the server.
        /// </summary>
        public int GameTime { get; set; }

        /// <summary>
        /// This is the score of the first player, retrieved from the server
        /// </summary>
        public int Player1Score { get; set; }

        /// <summary>
        /// This is the score of the second player, retrieved from the server
        /// </summary>
        public int Player2Score { get; set; }

        /// <summary>
        /// This is the list of words and the scores that the first player will have at the end of the game.
        /// </summary>
        public List<string> player1Words { get; set; }

        /// <summary>
        /// This is the list of words and the scores that the second player will have at the end of the game.
        /// </summary>
        public List<string> player2Words { get; set; }

        /// <summary>
        /// The constructor for the class.  Sets up the lists and sets the serverAddress to the field inputted by the user.
        /// </summary>
        /// <param name="serverDest"></param>
        public BoggleModel(string serverDest)
        {
            serverAddress = serverDest;

            player1Words = new List<string>();
            player2Words = new List<string>();
        }

        /// <summary>
        /// runs the game, getting the results of the time, and both players scores during the game from the server.
        /// If the user is the first player, will tell when the game has started and call the gameSetup() method.
        /// When the game is finished, it will set GameCompleted to true, causing the controller to properly get all results.
        /// </summary>
        /// <param name="ct">token used to cancel the task if the user wishes to</param>
        /// <returns></returns>
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

                catch (AggregateException)
                {
                    GamePending = false;
                    GamePlaying = false;
                }
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Retrieves the final board state from the server, setting up the lists of the wrods and scores from both users at the end of the game.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Sets up the inital results from the server, including both player's names, the initial scores(should be zero unless the other player was able to cnnect and play sooner)
        /// and time of the game.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Private method that is used to create a httpclient for each of the server requests.
        /// Uses serverAddress as the base address.
        /// </summary>
        /// <returns></returns>
        private HttpClient CreateClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(serverAddress);

            // Tell the server that the client will accept this particular type of response data
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }

        /// <summary>
        /// Creates the user from the server response.  Uses the userName input to send to the server to retrieve a user token
        /// </summary>
        /// <param name="userName">INput for the nickname that the user will have</param>
        /// <param name="ct">Token to canccel the task</param>
        /// <returns></returns>
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

                catch (AggregateException)
                {

                    GamePending = false;
                    GamePlaying = false;
                }

                return Task.FromResult(0);
            }
        }

        /// <summary>
        /// Creates a game, sending the server the userToken and the gameTime to create a gameID.  If the user is the first player, then it will go to pending,
        /// If the user is the second player, then it will immediately go to gameSetup()
        /// </summary>
        /// <param name="gameTime">The amount of time that the user wants the game to go</param>
        /// <param name="ct">token to cancel the task</param>
        /// <returns></returns>
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
                            GamePending = true;
                            GamePlaying = true;
                            Console.WriteLine(gameID);
                        }
                        else if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Task gameMake = new Task(() => gameSetup());
                            gameMake.Start();
                            GamePending = false;
                            GameCreation = true;
                            GamePlaying = true;
                            gameMake.Wait();
                        }
                    }
                }
                catch (AggregateException)
                {
                    GamePending = false;
                    GamePlaying = false;
                }
            }
            return Task.FromResult(0);
        }
       
        /// <summary>
        /// Sends the server a request to cancel a pending game request.
        /// </summary>
        /// <param name="ct">token used to cancel the task</param>
        /// <returns></returns>
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
                catch (AggregateException)
                {
                    
                }
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Sends the server a word, allowing the server to grade the score.
        /// </summary>
        /// <param name="word">Word to be scored</param>
        /// <param name="ct">token to cancel the task</param>
        /// <returns></returns>
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
                    }
                }

                catch (AggregateException)
                {

                    GamePending = false;
                    GamePlaying = false;
                }
            }
            return Task.FromResult(0);
        }

    }
}
