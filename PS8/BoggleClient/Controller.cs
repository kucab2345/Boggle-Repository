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

        CancellationTokenSource cancelRequestToken;

        

        public Controller(GameInterface view)
        { 
            game = view;
            game.CreateGameEvent += CreateGameHandler;
            game.CancelGameEvent += CancelGameHandler;
            game.WordEnteredEvent += WordEnteredHandler;
            game.EndCancelEvent += EndCancelHandler;
            cts = new CancellationTokenSource();
            cancelRequestToken = new CancellationTokenSource();
        }

        private async void WordEnteredHandler(string obj)
        {
            Task Scoring = new Task(() => mainClient.submitWord(obj, cts.Token));
            Scoring.Start();
            await Scoring;
        }

        public void EndCancelHandler()
        {
            cancelRequestToken.Cancel();
           
            if (mainClient.GamePending)
            {
                game.cancelbutton = true;
                game.EndRequestButton = true;
            }
            else
            {
                game.cancelbutton = false;
                game.EndRequestButton = false;
            }
            cancelRequestToken = new CancellationTokenSource(); 
        }

        private async void CreateGameHandler(string nickname, string timeLimit, string server)
        {
            mainClient = new BoggleModel(server);
            int gameTime;
            Task createUser = new Task (() =>mainClient.createUser(nickname, cts.Token));
            int.TryParse(timeLimit, out gameTime);
            createUser.Start();
            try {
                await createUser;
                Task createGame = new Task(() => mainClient.createGame(gameTime, cts.Token));
                createGame.Start();
                await createGame;
                game.cancelbutton = true;
                game.cancelbuttonText = "Cancel Pending Game...";
                while (mainClient.GamePlaying)
                {
                    Task playGame = new Task(() => mainClient.playGame(cts.Token));
                    playGame.Start();
                    await playGame;
                    if (mainClient.GameCreation)
                    {
                        boardSetup();
                        game.cancelbuttonText = "Exit Game...";
                    }
                    game.boardScoreUpdate(mainClient.Player1Score.ToString(), mainClient.Player2Score.ToString(), mainClient.GameTime.ToString());
                    await Task.Delay(1000);
                }
                if (mainClient.GameCompleted)
                {
                    Task endGame = new Task(() => mainClient.finalBoardSetup());
                    game.cancelbutton = false;
                    endGame.Start();
                    await endGame;
                    game.EndGame(mainClient.player1Words,mainClient.player2Words);
                    game.ResetBoard();
                }
                else
                {
                    game.ResetBoard();

                }
            }
            catch (Exception e)
            {
                game.Message = "There has been an error in the application." + "\n" + e.Message;
                game.ResetBoard();
            }
            

        }

        private void boardScoreUpdate()
        {
            game.Player1Score = mainClient.Player1Score.ToString();
            game.Player2Score = mainClient.Player2Score.ToString();
            game.Timer = mainClient.GameTime.ToString();
         
        }


        private void boardSetup()
        {
            game.Board = mainClient.boardState;
            game.Player1Name = mainClient.player1Name;
            game.Player2Name = mainClient.player2Name;
            game.Player1Score = mainClient.Player1Score.ToString();
            game.Player2Score = mainClient.Player2Score.ToString();
            game.Timer = mainClient.GameTime.ToString();
            mainClient.GameCreation = false;
            game.WordFocus();
        }

        private async void CancelGameHandler()
        {
            if (mainClient.GamePending)
            {
                cts.Cancel();
                Task cancelGame = new Task(() => mainClient.cancelJoinRequest(cancelRequestToken.Token));
                cancelGame.Start();
                try {
                    game.EndRequestButton = true;
                    await cancelGame;
                    game.cancelbutton = false;
                    game.ResetBoard();
                    game.Message = "Game Cancelled";
                }
                catch(AggregateException e)
                {
                    game.Message = "There has been an error in the application." + "\n" + e.Message;
                }
                
                game.EndRequestButton = false;
                cts = new CancellationTokenSource();
            }
            else
            {
                cts.Cancel();
                game.cancelbutton = false;
                game.Message = "You have left the game.";
                cts = new CancellationTokenSource();
            }
        }
    }
}
