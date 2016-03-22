using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoggleAPIClient;

namespace BoggleClient
{
    public class Controller
    {
        BoggleModel mainClient;
        private GameInterface game;
        public Controller(GameInterface view)
        { 
          
            game = view;
            game.CreateGameEvent += CreateGameHandler;
            game.CancelGameEvent += CancelGameHandler;
        }
        private async void CreateGameHandler(string nickname, string timeLimit, string server)
        {
            mainClient = new BoggleModel(server);
            Task createUser = new Task (() =>mainClient.createUser(nickname));
            createUser.Start();
            createUser.Wait();
            game.Player1Name = mainClient.nickname;
           
        }
        private void CancelGameHandler()
        {
            mainClient.cancelJoinRequest();
        }
    }
}
