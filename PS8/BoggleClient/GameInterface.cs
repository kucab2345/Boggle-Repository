using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    /// <summary>
    /// This interface denotes which aspects of the view the controller can access and manipulate.
    /// </summary>
    public interface GameInterface
    {
        /// <summary>
        /// End of Game summary method. Passes the players' word lists back to the view
        /// </summary>
        /// <param name="Player1List"></param>
        /// <param name="Player2List"></param>
        void EndGame(List<string> Player1List, List<string> Player2List);
        /// <summary>
        /// CreateGameEvent Action. Strings are the nickname, gametime, and server address
        /// </summary>
        event Action<string, string, string> CreateGameEvent;
        /// <summary>
        /// CancelGameEvent Action, cancels the pending game
        /// </summary>
        event Action CancelGameEvent;
        /// <summary>
        /// EndCancelEvent Action, ends the Cancellation Event
        /// </summary>
        event Action EndCancelEvent;
        /// <summary>
        /// WordEnteredEvent, passes the string being entered and sent to the server for validity
        /// </summary>
        event Action<string> WordEnteredEvent;
        /// <summary>
        /// Player1Name string property
        /// </summary>
        string Player1Name { get; set; }
        /// <summary>
        /// Player2Name string property
        /// </summary>
        string Player2Name { get; set; }
        /// <summary>
        /// Player1Score string property
        /// </summary>
        string Player1Score { get; set; }
        /// <summary>
        /// Player2Score string property
        /// </summary>
        string Player2Score { get; set; }
        /// <summary>
        /// Messages to be displayed in message boxes
        /// </summary>
        string Message { set; }
        /// <summary>
        /// cancelbutton bool, true if cancel button is depressed
        /// </summary>
        bool cancelbutton { get; set; }
        /// <summary>
        /// endRequestButton bool, true if cancellation of a cancel request is true
        /// </summary>
        bool EndRequestButton { get; set; }
        /// <summary>
        /// 16 character array of that represents the tiles of the board
        /// </summary>
        char[] Board { set; }
        /// <summary>
        /// Timer string property. Represents time left in current game. 0 means game is over. Must be between 5 - 120, inclusive (measured in seconds)
        /// </summary>
        string Timer { set; }
        /// <summary>
        /// CancelButton text string property
        /// </summary>
        string cancelbuttonText { get; set; }
        /// <summary>
        /// Calls the ResetBoard method, resets all fields to be empty, say, at the end of a game
        /// </summary>
        void ResetBoard();
        /// <summary>
        /// Resets focus to the word submission box after joining a game
        /// </summary>
        void WordFocus();
        /// <summary>
        /// Updates the scoreboard of the current game and time
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        void boardScoreUpdate(string v1, string v2, string v3);
    }
}
