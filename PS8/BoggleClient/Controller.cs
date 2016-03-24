using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoggleAPIClient;
using System.Threading;

namespace BoggleClient
{
    /// <summary>
    /// Main Controller class for the client. Communicates to the view via Interface "game"
    /// </summary>
    public class Controller
    {
        /// <summary>
        /// Current BoggleModel client
        /// </summary>
        BoggleModel mainClient;
        /// <summary>
        /// Interface that the Controller interacts with to access the view
        /// </summary>
        private GameInterface game;
        /// <summary>
        /// This token is used to cancel game tasks in the event that the player cancels it
        /// </summary>
        CancellationTokenSource cts;
        /// <summary>
        /// This token is used to cancel the Cancel Request task
        /// </summary>
        CancellationTokenSource cancelRequestToken;

        /// <summary>
        /// Constructor for the controller, it access the view through the interface
        /// </summary>
        /// <param name="view"></param>
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
        /// <summary>
        /// Async method that submits words to the server
        /// </summary>
        /// <param name="obj"></param>
        private async void WordEnteredHandler(string obj)
        {
            Task Scoring = new Task(() => mainClient.submitWord(obj, cts.Token));
            Scoring.Start();
            await Scoring;
        }
        /// <summary>
        /// Ends the cancellation request if user calls for a game to be cancelled
        /// </summary>
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
        /// <summary>
        /// Handles the initial creation of a game. Waits for the server to 
        /// ping back with a positive game joined response, removes the cancel button
        /// and its associated task, and draws the new board state
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="timeLimit"></param>
        /// <param name="server"></param>
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
        /// <summary>
        /// Update the board's score after each ping
        /// </summary>
        private void boardScoreUpdate()
        {
            game.Player1Score = mainClient.Player1Score.ToString();
            game.Player2Score = mainClient.Player2Score.ToString();
            game.Timer = mainClient.GameTime.ToString();
         
        }
        /// <summary>
        /// Method called by the CreateGame method above. Setups the board by bringing in the two players' names, 
        /// setting their scores, and setting the game timer.
        /// </summary>
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
        /// <summary>
        /// Handles the actual cancellation of a game request. Creates a seperate task for the cancellation thread to run on
        /// and cancels the game if the cancel game button is depressed.
        /// </summary>
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
