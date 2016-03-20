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
    class BoggleClient
    {
        private string nickname;

        private string gameID;

        private string userToken;

        private HttpClient client;

        public BoggleClient(string serverDest)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(serverDest);
        }

        private void createUser(string userName)
        {
            using (client)
            {
                dynamic data = new ExpandoObject();
                data.name = "Nickname";
                data.description = userName;
                data.has_issues = false;

                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("/user", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    // The deserialized response value is an object that describes the new repository.
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic userStringToken = JsonConvert.DeserializeObject(result);
                    userToken = userStringToken;
                    
                }
                else
                {
                    Console.WriteLine("Error creating user: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
        }
    }
}
