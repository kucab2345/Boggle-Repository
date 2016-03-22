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

        private string gameID;

        private string userToken;

        private HttpClient client;

        private string server;

        private bool cancel;

        private bool nameCreation;

        private bool gameCreation;

        public bool Cancel
        {
            set
            {
                cancel = value;
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

        public async void createUser(string userName)
        {
            nickname = userName;
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.Nickname = userName;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> responseResult = new Task<HttpResponseMessage> (() => client.PostAsync("users", content).Result);
                responseResult.Start();
                
                HttpResponseMessage response = await responseResult;
                
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
                    throw new Exception();
                    Console.WriteLine("Error creating user: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
        }

        public async Task createGame(int gameTime)
        {
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;
                data.TimeLimit = gameTime;

                Task<HttpResponseMessage> responseResult = new Task<HttpResponseMessage>(() => client.PostAsync("games", data).Result);
                responseResult.Start();

                HttpResponseMessage response = await responseResult;

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
        }

        public async Task getGameStatus()
        {
            using(HttpClient client = CreateClient())
            {
                String url = String.Format("games/{0}", gameID);

                Task<HttpResponseMessage> responseResult = new Task<HttpResponseMessage>(() => client.GetAsync(url).Result);
                responseResult.Start();

                HttpResponseMessage response = await responseResult;
                

                if (response.IsSuccessStatusCode)
                {
                    
                }
                else
                {
                    Console.WriteLine("Error Cancelling game request: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
        }

        public async Task cancelJoinRequest()
        {
            using(HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> responseResult = new Task<HttpResponseMessage>(() => client.PutAsync("games", content).Result);
                responseResult.Start();

                HttpResponseMessage response = await responseResult;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Game cancelled");
                }
                else
                {
                    Console.WriteLine("Error Cancelling game request: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
        }
    }
}
