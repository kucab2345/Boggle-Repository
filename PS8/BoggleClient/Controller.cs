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
            int gameTime;
            Task createUser = new Task (() =>mainClient.createUser(nickname));
            createUser.Start();
            int.TryParse(timeLimit, out gameTime);
            await createUser;
            Task createGame = new Task(() => mainClient.createGame(gameTime));
            createGame.Start();
            game.Player1Name = mainClient.nickname;
            await createGame;
            game.cancelbutton = true;
            while (mainClient.GamePlaying)
            {
                Task playGame = new Task(() => mainClient.playGame());
                playGame.Start();
                await playGame;
                if (mainClient.gameCreation)
                {
                    boardSetup();
                    game.cancelbutton = false;
                }
                await Task.Delay(1000);
            }
            if (mainClient.gameCompleted)
            {
                //mainClient.finalBoardSetup();
            }

        }

        private void boardSetup()
        {
            game.Player2Name = mainClient.player2Name;
            game.Player1Score = mainClient.player1Score.ToString();
            game.Player2Score = mainClient.player2Score.ToString();
            mainClient.gameCreation = false;
        }

        private async void CancelGameHandler()
        {
            Task cancelGame = new Task(() =>mainClient.cancelJoinRequest());
            await cancelGame;
            game.cancelbutton = false;
        }
    }
}
