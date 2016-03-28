using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;

using static System.Net.HttpStatusCode;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {
        /// <summary>
        /// The most recent call to SetStatus determines the response code used when
        /// an http response is sent.
        /// </summary>
        /// <param name="status"></param>
        /// 
        private static Dictionary<string, GameStatus> AllGames;
        private static Dictionary<string, UserInfo> AllPlayers;
        private static readonly object sync = new object();


        private static void SetStatus(HttpStatusCode status)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = status;
        }

        /// <summary>
        /// Returns a Stream version of index.html.
        /// </summary>
        /// <returns></returns>
        public Stream API()
        {
            SetStatus(OK);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "index.html");
        }

        public void CancelGame(string userToken)
        {
            throw new NotImplementedException();
        }

        public string GetBriefGamestatus(string GameID)
        {
            throw new NotImplementedException();
        }

        public string GetFullGameStatus(string GameID)
        {
            throw new NotImplementedException();
        }

        public string JoinGame(GameJoin info)
        {
            throw new NotImplementedException();
        }


        public string playWord(UserGame words, string GameID)
        {
            throw new NotImplementedException();
        }

        public string RegisterUser(UserInfo user)
        {
            lock (sync)
            {
                if (user.Nickname == null || user.Nickname.Trim().Length == 0)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                else
                {
                    string userID = Guid.NewGuid().ToString();
                    AllPlayers.Add(userID, user);
                    return userID;
                }
            }
        }
    }
}
