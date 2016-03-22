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

        private bool cancel;

        private bool nameCreation;

        private bool gameCreation;

        public bool gameRunning;

        public bool gamePending;

        public bool Cancel
        {
            set
            {
                cancel = value;
            }
            get
            {
                return cancel;
            }
        }

        public bool NameCreation
        {
            set
            {
                nameCreation = value;
            }
        }

        public bool GameCreation
        {
            set
            {
                gameCreation = value;
            }
        }
        public BoggleModel(string serverDest)
        {
            server = serverDest;
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

        public async void runGame()
        {
            using(HttpClient client = CreateClient())
            {
                if (nameCreation)
                {

                }
            }
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
                    
                    if(deserResult.GameState == "active" && !gameCreated)
                    {
                        gameSetup(deserResult);
                    }
                    else if(deserResult.GameState == "active")
                    {
                        gameCurrentState(deserResult);
                    }
                    else if()

                    Console.WriteLine(gameID);
                }
                else
                {
                    
                }
            }
        }

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
