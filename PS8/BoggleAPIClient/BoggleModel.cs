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
        private string nickname;

        private string gameID;

        private string userToken;

        private HttpClient client;

        public BoggleModel(string serverDest)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(serverDest);

            // Tell the server that the client will accept this particular type of response data
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public void createUser(string userName)
        {
            nickname = userName;
            using (client)
            {
                dynamic data = new ExpandoObject();
                data.Nickname = userName;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("http://bogglecs3500s16.azurewebsites.net/BoggleService.svc/users", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    // The deserialized response value is an object that describes the new repository.
                    string result = response.Content.ReadAsStringAsync().Result;
                    userToken = result.Remove(0, 14);
                    userToken = userToken.Trim('}');
                    userToken = userToken.Trim('"');
                    userToken = userToken.Trim('\\');
                    Console.WriteLine(userToken);
                }
                else
                {
                    Console.WriteLine("Error creating user: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
        }

        public void createGame(int gameTime)
        {
            using (client)
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;
                data.TimeLimit = gameTime;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("http://bogglecs3500s16.azurewebsites.net/BoggleService.svc/games", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    // The deserialized response value is an object that describes the new repository.
                    string result = response.Content.ReadAsStringAsync().Result;
                    gameID = result;
                    Console.WriteLine(gameID);
                }
                else
                {
                    Console.WriteLine("Error creating game: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
        }
    }
}
