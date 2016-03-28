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

        [WebInvoke(Method = "POST", UriTemplate = "/users")]
        string RegisterUser(UserInfo user);

        [WebInvoke(Method = "POST", UriTemplate = "/games")]
        string JoinGame(GameJoin info);

        [WebInvoke(Method = "PUT", UriTemplate = "/games")]

        void CancelGame(string userToken);

        [WebInvoke(Method = "PUT", UriTemplate = "/games/{GameID}")]
        string playWord(UserGame words, string GameID);

        [WebGet(UriTemplate = "/games/{GameID}?Brief=yes")]
        string GetBriefGamestatus(string GameID);

        [WebGet(UriTemplate = "/games/{GameID}")]
        string GetFullGameStatus(string GameID);
    }
}
