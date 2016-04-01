using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Boggle
{
    [ServiceContract]
    public interface IBoggleService
    {
        /// <summary>
        /// Sends back index.html as the response body.
        /// </summary>
        [WebGet(UriTemplate = "/api")]
        Stream API();

        /// <summary>
        /// Sends a Post request to Server to Register a User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST", UriTemplate = "/users")]
        TokenScoreGameIDReturn RegisterUser(UserInfo user);

        /// <summary>
        /// Sends a Post request to the server to Join a Game
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST", UriTemplate = "/games")]
        TokenScoreGameIDReturn JoinGame(GameJoin info);
        /// <summary>
        /// Sends a Put Request to Cancel A Pending Game
        /// </summary>
        /// <param name="userToken"></param>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "PUT", UriTemplate = "/games")]
        void CancelGame(UserGame userToken);
        /// <summary>
        /// Put Request to the Server when playing a word
        /// </summary>
        /// <param name="words"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "PUT", UriTemplate = "/games/{GameID}")]
        TokenScoreGameIDReturn playWord(UserGame words, string GameID);
        /// <summary>
        /// Get Request to get back the brief game status
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns></returns>
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/games/{GameID}?Brief=yes")]
        GameStatus GetBriefGamestatus(string GameID);
        /// <summary>
        /// Get Request to get back the full game status
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns></returns>
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/games/{GameID}")]
        GameStatus GetFullGameStatus(string GameID);
    }
}
