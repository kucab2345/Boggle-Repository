using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoggleAPIClient;
using System.Threading;

namespace BoggleClient
{
    public class Controller
    {
        BoggleModel mainClient;
        private GameInterface game;

        CancellationTokenSource cts;
        public Controller(GameInterface view)
        { 
            game = view;
            game.CreateGameEvent += CreateGameHandler;
            game.CancelGameEvent += CancelGameHandler;
            game.WordEnteredEvent += WordEnteredHandler;
            cts = new CancellationTokenSource();
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
            Task createUser = new Task (() =>mainClient.createUser(nickname, cts.Token));
            int.TryParse(timeLimit, out gameTime);
            createUser.Start();
            await createUser;
            Task createGame = new Task(() => mainClient.createGame(gameTime, cts.Token));
            createGame.Start();
            await createGame;
            game.cancelbutton = true;
            while (mainClient.GamePlaying)
            {
                Task playGame = new Task(() => mainClient.playGame(cts.Token));
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
                Task endGame = new Task(() =>mainClient.finalBoardSetup());
                endGame.Start();
                await endGame;
                boardEndScoreUpdate();
            }

        }

        private void boardScoreUpdate()
        {
            game.Player1Score = mainClient.player1Score.ToString();
            game.Player2Score = mainClient.player2Score.ToString();
            game.Timer = mainClient.gameTime.ToString();
         
        }

        private void boardEndScoreUpdate()
        {
            game.Player1Score = mainClient.player1Score.ToString();
            game.Player2Score = mainClient.player2Score.ToString();
            game.Timer = mainClient.gameTime.ToString();
            game.Board = new Char[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
            game.EndGame(mainClient.player1Words, mainClient.player2Words);
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
            if (mainClient.GamePending)
            {
                cts.Cancel();
                Task cancelGame = new Task(() => mainClient.cancelJoinRequest());
                cancelGame.Start();
                await cancelGame;
                Console.WriteLine();
                if (mainClient.cancel)
                {
                    game.cancelbutton = false;
                }
                cts = new CancellationTokenSource();
            }
        }
    }
}
