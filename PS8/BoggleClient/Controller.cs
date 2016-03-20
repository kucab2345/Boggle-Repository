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
        }
        private void CreateGameHandler(string nickname, string timeLimit, string server)
        {
            mainClient = new BoggleModel(server);
            mainClient.createUser(nickname);
        }
    }
}
