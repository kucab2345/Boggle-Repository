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
using System.Text;

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
        private static HashSet<string> dictionaryContents = new HashSet<string>(File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\dictionary.txt"));
        private static BoggleBoard board = new BoggleBoard();
       
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

        public void CancelGame(UserGame endUser)
        {
            lock (sync)
            {
                string cancelGameID = null;
                foreach (KeyValuePair<string, GameStatus> games in AllGames)
                {
                    if (games.Value.GameState == "pending" && games.Value.Player1.UserToken == endUser.UserToken)
                    {
                        cancelGameID = games.Key;
                    }
                }
                if (cancelGameID != null)
                {
                    SetStatus(OK);
                    AllGames[cancelGameID].Player1 = null;
                }
                else
                {
                    SetStatus(Forbidden);
                }
            }
        }

        public GameStatus GetBriefGamestatus(string GameID)
        {
            lock (sync)
            {
                if (!AllGames.ContainsKey(GameID))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                SetStatus(OK);
                if (AllGames[GameID].GameState != "pending")
                {
                    TimeSpan current = DateTime.Now.TimeOfDay;
                    double result = current.Subtract(AllGames[GameID].StartGameTime).TotalSeconds;
                    int times = Convert.ToInt32(result);

                    int TimeRemaining;


                    if (AllGames[GameID].GameState == "active" && int.TryParse(AllGames[GameID].TimeLeft, out TimeRemaining) && (TimeRemaining - times >= 0))
                    {
                        AllGames[GameID].TimeLeft = (TimeRemaining - times).ToString();

                    }

                    else
                    {
                        AllGames[GameID].TimeLeft = "0";
                    }

                    int.TryParse(AllGames[GameID].TimeLeft, out times);

                    if (times == 0)
                    {
                        AllGames[GameID].GameState = "completed";

                    }
                    if (AllGames[GameID].GameState == "pending")
                    {

                    }

                    GameStatus var = new GameStatus();
                    var.GameState = AllGames[GameID].GameState;
                    var.TimeLeft = AllGames[GameID].TimeLeft;
                    var.Player1 = AllGames[GameID].Player1;
                    var.Player2 = AllGames[GameID].Player2;
                    return var;
                }
                return AllGames[GameID];
            }
        }

        public GameStatus GetFullGameStatus(string GameID)
        {
            lock (sync)
            {
                if (!AllGames.ContainsKey(GameID))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                SetStatus(OK);
                if (AllGames[GameID].GameState != "pending")
                {
                    TimeSpan current = DateTime.Now.TimeOfDay;
                    double result = current.Subtract(AllGames[GameID].StartGameTime).TotalSeconds;
                    int times = Convert.ToInt32(result);
                    int TimeRemaining;

                    if (AllGames[GameID].GameState == "active" && int.TryParse(AllGames[GameID].TimeLeft, out TimeRemaining) && (TimeRemaining - times >= 0))
                    {
                        AllGames[GameID].TimeLeft = (TimeRemaining - times).ToString();
                    }

                    else
                    {
                        AllGames[GameID].TimeLeft = "0";
                    }

                    int.TryParse(AllGames[GameID].TimeLeft, out times);

                    if (times == 0)
                    {
                        AllGames[GameID].GameState = "completed";

                    }

                    if (AllGames[GameID].GameState == "completed")
                    {
                        AllGames[GameID].Player1.WordsPlayed = AllGames[GameID].Player1.personalList;
                        AllGames[GameID].Player2.WordsPlayed = AllGames[GameID].Player2.personalList;
                        if (AllGames[GameID].Player1.WordsPlayed == null)
                        {
                            AllGames[GameID].Player1.WordsPlayed = new List<WordScore>();
                        }

                        if (AllGames[GameID].Player2.WordsPlayed == null)
                        {
                            AllGames[GameID].Player2.WordsPlayed = new List<WordScore>();
                        }
                        return AllGames[GameID];
                    }
                }

                return AllGames[GameID];
            }
        }

        public TokenScoreGameIDReturn JoinGame(GameJoin info)
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
                TokenScoreGameIDReturn var = new TokenScoreGameIDReturn();
                foreach (KeyValuePair<string, GameStatus> game in AllGames)
                {
                    if (game.Value.GameState == "pending")
                    {
                        if (game.Value.Player1 != null)
                        {
                            game.Value.Player2 = AllPlayers[info.UserToken];
                            SetStatus(Created);
                            setupGame(info.TimeLimit, game.Key);

                            var.GameID = game.Key;

                            return var;
                        }
                        else
                        {
                            game.Value.Player1 = AllPlayers[info.UserToken];
                            SetStatus(Accepted);
                            var.GameID = game.Key;
                            return var;
                        }
                    }
                }

                
                gameID += 1;
                SetStatus(Accepted);
                AllGames.Add(gameID.ToString(), new GameStatus());
                SetStatus(Accepted);
                AllGames[gameID.ToString()].Player1 = AllPlayers[info.UserToken];
                AllGames[gameID.ToString()].GameState = "pending";
                AllGames[gameID.ToString()].TimeLimit = info.TimeLimit;
                var.GameID = gameID.ToString();

                return var;
            }
        }

        private void setupGame(string timeLimit, string gameID)
        {
            int time1;
            int time2;
            int.TryParse(AllGames[gameID].TimeLimit, out time1);
            int.TryParse(timeLimit, out time2);

            if(time1 < time2)
            {
                time2 = ((time2 - time1)/2);
            }
            else
            {
                time1 = ((time1 - time2) / 2);
            }
            
            AllGames[gameID].TimeLimit = (time1 + time2).ToString();
            AllGames[gameID].TimeLeft = AllGames[gameID].TimeLimit;
            AllGames[gameID].GameState = "active";

            AllGames[gameID].Board = board.ToString();
            AllGames[gameID].StartGameTime = DateTime.Now.TimeOfDay;
        }

        public TokenScoreGameIDReturn playWord(UserGame words, string GameID)
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


                if (AllPlayers[words.UserToken].personalList == null)
                {
                    AllPlayers[words.UserToken].personalList = new List<WordScore>();
                }
            
                int userScore;
                int.TryParse(AllPlayers[words.UserToken].Score, out userScore);
                int WordScoreResult = ScoreWord(words.Word, words.UserToken);
                WordScore totalResult = new WordScore();
                totalResult.Word = words.Word;
                totalResult.Score = WordScoreResult;
                AllPlayers[words.UserToken].personalList.Add(totalResult);
                AllPlayers[words.UserToken].Score = (userScore + WordScoreResult).ToString();
                TokenScoreGameIDReturn var = new TokenScoreGameIDReturn();
                var.Score = WordScoreResult.ToString();
                SetStatus(OK);
                return var;

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

        public TokenScoreGameIDReturn RegisterUser(UserInfo user)
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
                    TokenScoreGameIDReturn result = new TokenScoreGameIDReturn();
                    result.UserToken = userID;
                    return result;
                }
            }
        }
    }
}
