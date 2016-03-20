using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    public class Controller
    {
        public GameInterface game;
        public Controller(GameInterface view)
        {
            game = view;

            game.CreateGameEvent += CreateGameHandler;
        }
        private void CreateGameHandler(string nickname, string timeLimit, string server)
        {

        }
    }
}
