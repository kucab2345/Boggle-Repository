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
        private static Dictionary<string, GameStatus> AllGames = new Dictionary<string, GameStatus>();
        private static Dictionary<string, UserInfo> AllPlayers = new Dictionary<string, UserInfo>();
        private static readonly object sync = new object();
        int gameID = 0;
        BoggleBoard board = new BoggleBoard();

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
            string cancelGameID = null;
            foreach (KeyValuePair<string, GameStatus> games in AllGames)
            {
                if (games.Value.GameState == "pending" && games.Value.Player1.UserToken == userToken)
                {
                    cancelGameID = games.Key;
                }
            }
            if (cancelGameID != null)
            {
                SetStatus(OK);
                AllGames.Remove(cancelGameID);
            }
            else
            {
                SetStatus(Forbidden);
            }
        }

        public string GetBriefGamestatus(string GameID)
        {
            TimeSpan current = DateTime.Now.TimeOfDay;
            double result = current.Subtract(AllGames[GameID].StartGameTime).TotalSeconds;
            AllGames[GameID].TimeLeft = Convert.ToInt32(result).ToString();
            throw new NotImplementedException();
        }

        public string GetFullGameStatus(string GameID)
        {
            TimeSpan current = DateTime.Now.TimeOfDay;
            double result = current.Subtract(AllGames[GameID].StartGameTime).TotalSeconds;
            AllGames[GameID].TimeLeft = Convert.ToInt32(result).ToString();
            throw new NotImplementedException();
        }

        public string JoinGame(GameJoin info)
        {
            if (info.UserToken == null || info.UserToken.Trim().Length == 0 || !AllPlayers.ContainsKey(info.UserToken))
            {
                SetStatus(Forbidden);
                return null;
            }
            int test;
            if (!int.TryParse(info.TimeLimit, out test))
            {
                SetStatus(Forbidden);
                return null;
            }

            if (test < 5 || test > 120)
            {
                SetStatus(Forbidden);
                return null;
            }

            foreach (KeyValuePair<string, GameStatus> game in AllGames)
            {
                if (game.Value.GameState == "pending")
                {
                    if (game.Value.Player1.UserToken == info.UserToken)
                    {
                        SetStatus(Conflict);
                        return null;
                    }
                }
            }

            foreach (KeyValuePair<string, GameStatus> game in AllGames)
            {
                if (game.Value.GameState == "pending")
                {
                    game.Value.Player2 = AllPlayers[info.UserToken];
                    SetStatus(Created);
                    setupGame(info.TimeLimit, game.Key);
                    return game.Key;
                }
            }

            gameID += 1;
            AllGames.Add(gameID.ToString(), new GameStatus());
            AllGames[gameID.ToString()].Player1 = AllPlayers[info.UserToken];
            AllGames[gameID.ToString()].GameState = "pending";
            return gameID.ToString();
        }

        private void setupGame(string timeLimit, string gameID)
        {
            int time1;
            int time2;
            int.TryParse(AllGames[gameID].TimeLimit, out time1);
            int.TryParse(timeLimit, out time2);

            AllGames[gameID].TimeLimit = ((time1 + time2) / 2).ToString();

            AllGames[gameID].GameState = "active";

            AllGames[gameID].Board = board.ToString();
            AllGames[gameID].StartGameTime = DateTime.Now.TimeOfDay;
        }

        public string playWord(UserGame words, string GameID)
        {
            if (words.UserToken == null || words.UserToken.Trim().Length == 0 || !AllPlayers.ContainsKey(words.UserToken))
            {
                SetStatus(Forbidden);
                return null;
            }

            if(words.Word == null || words.Word.Trim().Length == 0 || !AllGames.ContainsKey(GameID))
            {
                SetStatus(Forbidden);
                return null;
            }

            if(AllGames[GameID].GameState != "active")
            {
                SetStatus(Conflict);
                return null;
            }

            if()
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
                    SetStatus(Created);
                    string userID = Guid.NewGuid().ToString();
                    AllPlayers.Add(userID, user);
                    return userID;
                }
            }
        }
    }
}
