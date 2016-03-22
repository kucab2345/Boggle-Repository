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
            game.WordEnteredEvent += WordEnteredHandler;
        }

        private async void WordEnteredHandler(string obj)
        {
            Task Scoring = new Task(() => mainClient.submitWord(obj));
            Scoring.Start();
            await Scoring;
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
                boardScoreUpdate();
                await Task.Delay(1000);
            }
            if (mainClient.gameCompleted)
            {
                //mainClient.finalBoardSetup();
            }

        }

        private void boardScoreUpdate()
        {
            game.Player1Score = mainClient.player1Score.ToString();
            game.Player2Score = mainClient.player2Score.ToString();
            game.Timer = mainClient.gameTime.ToString();
        }

        private void boardSetup()
        {
            game.Board = mainClient.boardState;
            game.Player1Name = mainClient.player1Name;
            game.Player2Name = mainClient.player2Name;
            game.Player1Score = mainClient.player1Score.ToString();
            game.Player2Score = mainClient.player2Score.ToString();
            game.Timer = mainClient.gameTime.ToString();
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
