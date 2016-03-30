using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Web;
using System.ServiceModel.Web;
using Newtonsoft.Json;

using static System.Net.HttpStatusCode;
using System.Linq;

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
        private static int gameID = 0;
        private static BoggleBoard board = new BoggleBoard();
        string dictionaryContents = File.ReadAllText(HttpContext.Current.Server.MapPath("dictionary.txt"));

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
            lock (sync)
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
                    AllGames[cancelGameID].GameState = "completed";
                }
                else
                {
                    SetStatus(Forbidden);
                }
            }
        }

        public string GetBriefGamestatus(string GameID)
        {
            lock (sync)
            {
                if (!AllGames.ContainsKey(GameID))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                SetStatus(OK);

                TimeSpan current = DateTime.Now.TimeOfDay;
                double result = current.Subtract(AllGames[GameID].StartGameTime).TotalSeconds;
                int times = Convert.ToInt32(result);

                int TimeRemaining;


                if (int.TryParse(AllGames[GameID].TimeLeft, out TimeRemaining) && (TimeRemaining - times >= 0))
                {
                    AllGames[GameID].TimeLeft = (TimeRemaining - times).ToString();
                }

                else
                {
                    AllGames[GameID].TimeLeft = "0";
                }

                if (times <= 0)
                {
                    AllGames[GameID].GameState = "completed";
                }

                dynamic var = new ExpandoObject();
                var.GameState = AllGames[GameID].GameState;
                var.TimeLeft = AllGames[GameID].TimeLeft;
                var.Player1 = AllGames[GameID].Player1;
                var.Player2 = AllGames[GameID].Player2;
                string stringResult = JsonConvert.SerializeObject(var);
                return stringResult;
            }
        }

        public string GetFullGameStatus(string GameID)
        {
            lock (sync)
            {
                if (!AllGames.ContainsKey(GameID))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                SetStatus(OK);

                TimeSpan current = DateTime.Now.TimeOfDay;
                double result = current.Subtract(AllGames[GameID].StartGameTime).TotalSeconds;
                int times = Convert.ToInt32(result);
                int TimeRemaining;

                if (int.TryParse(AllGames[GameID].TimeLeft, out TimeRemaining) && (TimeRemaining - times >= 0))
                {
                    AllGames[GameID].TimeLeft = (TimeRemaining - times).ToString();
                }

                else
                {
                    AllGames[GameID].TimeLeft = "0";
                }

                if (times <= 0)
                {
                    AllGames[GameID].GameState = "completed";
                }
                string stringResult;
                if (AllGames[GameID].GameState == "completed")
                {
                    dynamic var = new ExpandoObject();
                    var = AllGames[GameID];
                    var.Player1.WordsPlayed = AllGames[GameID].Player1.WordsPlayed;
                    var.Player2.WordsPlayed = AllGames[GameID].Player2.WordsPlayed;
                    stringResult = JsonConvert.SerializeObject(var);
                    return stringResult;
                }
                stringResult = JsonConvert.SerializeObject(AllGames[GameID]);
                return stringResult;
            }
        }

        public string JoinGame(GameJoin info)
        {
            lock(sync){
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
                dynamic var = new ExpandoObject();
                foreach (KeyValuePair<string, GameStatus> game in AllGames)
                {
                    if (game.Value.GameState == "pending")
                    {
                        game.Value.Player2 = AllPlayers[info.UserToken];
                        SetStatus(Created);
                        setupGame(info.TimeLimit, game.Key);
                        
                        var.GameID = game.Key;

                        return JsonConvert.SerializeObject(var);
                    }
                }

                gameID += 1;
                
                AllGames.Add(gameID.ToString(), new GameStatus());
                SetStatus(Accepted);
                AllGames[gameID.ToString()].Player1 = AllPlayers[info.UserToken];
                AllGames[gameID.ToString()].GameState = "pending";
       
                var.GameID = gameID;

                return JsonConvert.SerializeObject(var);
            }
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
            lock (sync)
            {
                if (words.UserToken == null || words.UserToken.Trim().Length == 0 || !AllPlayers.ContainsKey(words.UserToken))
                {
                    SetStatus(Forbidden);
                    return null;
                }

                if (words.Word == null || words.Word.Trim().Length == 0 || !AllGames.ContainsKey(GameID))
                {
                    SetStatus(Forbidden);
                    return null;
                }

                if (AllGames[GameID].GameState != "active")
                {
                    SetStatus(Conflict);
                    return null;
                }


                if (AllPlayers[words.UserToken].WordsPlayed == null)
                {
                    AllPlayers[words.UserToken].WordsPlayed = new List<WordScore>();
                }

                int userScore;
                int.TryParse(AllPlayers[words.UserToken].Score, out userScore);
                int WordScoreResult = ScoreWord(words.Word, words.UserToken);
                AllPlayers[words.UserToken].Score = (userScore + WordScoreResult).ToString();
                dynamic var = new ExpandoObject();
                var.Score = WordScoreResult;
                SetStatus(OK);
                return JsonConvert.SerializeObject(var);

            }
        }
        private int ScoreWord(string word, string userToken)
        {
            bool legalWord = searchDictionary(word.Trim().ToUpper());
            string currentWord = word.Trim();

            if (legalWord == true)
            {
                if (currentWord.Length < 3 || AllPlayers[userToken].WordsPlayed.Any(x => x.Word == currentWord))
                {
                    return 0;
                }
                else if (currentWord.Length == 3 || currentWord.Length == 4)
                {
                    return 1;
                }
                else if (currentWord.Length == 5)
                {
                    return 2;
                }
                else if (currentWord.Length == 6)
                {
                    return 3;
                }
                else if (currentWord.Length == 7)
                {
                    return 5;
                }
                else
                {
                    return 11;
                }
            }
            else
            {
                return -1;
            }
        }

        private bool searchDictionary(string key)
        {
            if (dictionaryContents.Contains(key))
            {
                return true;
            }
            else
            {
                return false;
            }
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
                    AllPlayers[userID].UserToken = userID;
                    AllPlayers[userID].Nickname = user.Nickname;
                    dynamic var = new ExpandoObject();
                    var.UserToken = userID;
                    return JsonConvert.SerializeObject(var) ;
                }
            }
        }
    }
}
