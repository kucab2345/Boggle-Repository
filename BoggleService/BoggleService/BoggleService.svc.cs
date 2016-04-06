using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Web;
using System.ServiceModel.Web;
using System.Configuration;
using static System.Net.HttpStatusCode;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {
        /// <summary>
        /// object used to sync the methods, ensuring that all happen at the right rate.
        /// </summary>

        private static HashSet<string> dictionaryContents = new HashSet<string>(File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\dictionary.txt"));

        private static string BoggleDB;

        static BoggleService()
        {
            BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;
        }


        /// <summary>
        /// The most recent call to SetStatus determines the response code used when
        /// an http response is sent.
        /// </summary>
        /// <param name="status"></param>
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

        /// <summary>
        /// If the requesting player is in a pending game, removes them from the pending game.
        /// </summary>
        /// <param name="endUser"></param>
        public void CancelGame(UserGame endUser)
        {
            if (endUser == null || endUser.UserToken == null || endUser.UserToken.Trim().Length == 0)
            {
                SetStatus(Forbidden);

            }
            else
            {
                   using (SqlConnection conn = new SqlConnection(BoggleDB))
                   {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        // Here, the SqlCommand is a select query.  We are interested in whether item.UserID exists in
                        // the Users table.
                        using (SqlCommand command = new SqlCommand("Delete from Games where Player1 = @Player AND Player2 is Null", conn, trans))
                        {
                            command.Parameters.AddWithValue("@Player", endUser.UserToken);

                            int id = command.ExecuteNonQuery();

                            if(id <= 0)
                            {
                                SetStatus(Forbidden);
                            

                            }
                            else
                            {
                                SetStatus(OK);
                            }
                            trans.Commit();
                        }


                    }
                }
            }
                       

        }

        /// <summary>
        /// Return the brief status of the game
        /// </summary>
        /// <param name="GameID">ID of the game that the client is requesting from the server</param>
        /// <returns></returns>
        public GameStatus GetBriefGamestatus(string GameID)
        {
            if (GameID == null || GameID.Trim().Length == 0)
            {
                SetStatus(Forbidden);
                return null;
            }
            GameStatus game = new GameStatus();
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Here, the SqlCommand is a select query.  We are interested in whether item.UserID exists in
                    // the Users table.
                    using (SqlCommand command = new SqlCommand("Select TimeLimit, Player1, Player2 from Games where GameID = @Game", conn, trans))
                    {
                        command.Parameters.AddWithValue("@Game", GameID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                SetStatus(Forbidden);
                                trans.Commit();
                                return null;
                            }
                            reader.Read();
                            game.TimeLimit = reader["TimeLimit"].ToString();
                            int TimeRemaining;
                            int.TryParse(game.TimeLimit, out TimeRemaining);
                            DateTime startTime = (DateTime)reader["StartTime"];

                            double result = (DateTime.Now - startTime).TotalSeconds;
                            int times = Convert.ToInt32(result);
                            game.GameState = "active";
                            if (DBNull.Value.Equals(reader["Player2"]))
                            {
                                game.GameState = "pending";
                            }
                            else {
                                game.Player1 = new UserInfo();
                                game.Player2 = new UserInfo();

                                game.Player1.UserToken = reader["Player1"].ToString();
                                game.Player1.UserToken = reader["Player2"].ToString();

                                if (game.GameState == "active" && (TimeRemaining - times > 0))
                                {
                                    int.TryParse(game.TimeLimit, out TimeRemaining);
                                    game.TimeLeft = (TimeRemaining - times).ToString();
                                    game.GameState = "active";
                                }

                                else
                                {
                                    game.TimeLeft = "0";
                                    game.GameState = "completed";
                                }


                            }

                            




                        }
                    }

                    using(SqlCommand command = new SqlCommand("Select Score from Words where GameID = @Game and Player = @Player"))
                    {
                        command.Parameters.AddWithValue("@Game", GameID);
                        command.Parameters.AddWithValue("@Player", game.Player1.UserToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                game.Player1.Score = "0";
                                
                            }

                            while (reader.Read())
                            {
                                game.Player1.Score += (int)reader["Score"];
                                
                            }
                        }

                    }

                    using (SqlCommand command = new SqlCommand("Select Score from Words where GameID = @Game and Player = @Player"))
                    {
                        command.Parameters.AddWithValue("@Game", GameID);
                        command.Parameters.AddWithValue("@Player", game.Player2.UserToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                
                                game.Player2.Score = "0";
                            }

                            while (reader.Read())
                            {
                                game.Player2.Score += (int)reader["Score"];

                            }
                        }

                    }

                    SetStatus(OK);
                    return game;
                }
            }
           
            
        }

            /*
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

                    double result = (DateTime.Now - AllGames[GameID].StartGameTime).TotalSeconds;
                    int times = Convert.ToInt32(result);

                    int TimeRemaining;
                    int.TryParse(AllGames[GameID].TimeLeft, out TimeRemaining);

                    if (AllGames[GameID].GameState == "active" && (TimeRemaining - times > 0))
                    {
                        int.TryParse(AllGames[GameID].TimeLimit, out TimeRemaining);
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

                    GameStatus var = new GameStatus();
                    var.GameState = AllGames[GameID].GameState;
                    var.TimeLeft = AllGames[GameID].TimeLeft;
                    var.Player1 = AllGames[GameID].Player1;
                    var.Player2 = AllGames[GameID].Player2;
                    return var;
                }
                
                return AllGames[GameID];
             
            }*/
        
        /// <summary>
        /// Gets the full game status from the server
        /// </summary>
        /// <param name="GameID">ID of the game from the server</param>
        /// <returns></returns>
        public GameStatus GetFullGameStatus(string GameID)
        {
            GameStatus game = new GameStatus();
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("select * from Games where GameID = @GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", GameID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                SetStatus(Forbidden);
                                return null;
                            }
                            SetStatus(OK);
                            reader.Read();
                            game.GameState = "active";
                            if (DBNull.Value.Equals(reader["Board"]))
                            {
                                SetStatus(Forbidden);
                                return null;
                            }
                            if (DBNull.Value.Equals(reader["TimeLimit"]))
                            {
                                SetStatus(Forbidden);
                                return null;
                            }
                            if (DBNull.Value.Equals(reader["StartTime"]))
                            {
                                SetStatus(Forbidden);
                                return null;
                            }
                            if (DBNull.Value.Equals(reader["Player2"]))
                            {
                                game.GameState = "pending";
                            }
                            //Get board
                            game.Board = (string)reader["Board"];
                            //Get TimeLimit
                            game.TimeLimit = (string)reader["TimeLimit"];
                            //Get StartTime
                            game.StartGameTime = (DateTime)reader["StartTime"];

                            double result = (DateTime.Now - game.StartGameTime).TotalSeconds;
                            int times = Convert.ToInt32(result);

                            int TimeRemaining;
                            int.TryParse(game.TimeLimit, out TimeRemaining);

                            //Get TimeLeft
                            if (game.GameState == "active" && (TimeRemaining - times > 0))
                            {
                                int.TryParse(game.TimeLimit, out TimeRemaining);
                                game.TimeLeft = (TimeRemaining - times).ToString();
                            }

                            else
                            {
                                game.TimeLeft = "0";
                            }

                            int.TryParse(game.TimeLeft, out times);

                            if (times == 0)
                            {
                                game.GameState = "completed";
                            }
                            /*
                            if (game.GameState == "completed")
                            {
                                game.Player1.WordsPlayed = game.Player1.personalList;
                                game.Player2.WordsPlayed = game.Player2.personalList;
                                if (game.Player1.WordsPlayed == null)
                                {
                                    game.Player1.WordsPlayed = new List<WordScore>();
                                }

                                if (game.Player2.WordsPlayed == null)
                                {
                                    game.Player2.WordsPlayed = new List<WordScore>();
                                }
                                //return game;
                            }*/
                        }
                    }
                    //Get Player1 Nickname
                    using (SqlCommand command = new SqlCommand("select Nickname from Users where UserID = @UserID)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", game.Player1);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            game.Player1.Nickname = (string)reader["Nickname"];
                        }
                    }
                    //Get Player2 Nickname
                    using (SqlCommand command = new SqlCommand("select Nickname from Users where UserID = @UserID)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", game.Player2);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            game.Player2.Nickname = (string)reader["Nickname"];
                        }
                    }
                    //Get Player1 Word List
                    using (SqlCommand command = new SqlCommand("select Word, Score from Words where UserID = @UserID and GameID = @GameID)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", game.Player1);
                        command.Parameters.AddWithValue("@GameID", GameID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                game.Player1.WordsPlayed.Add(new WordScore() { Word = reader["Word"].ToString(), Score = (int)reader["Score"] });
                            }
                        }
                    }
                    //Get Player2 Word List
                    using (SqlCommand command = new SqlCommand("select Word, Score from Words where UserID = @UserID and GameID = @GameID)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", game.Player2);
                        command.Parameters.AddWithValue("@GameID", GameID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                game.Player2.WordsPlayed.Add(new WordScore() { Word = reader["Word"].ToString(), Score = (int)reader["Score"] });
                            }
                        }
                    }
                }
                return game;
                /*
                lock (sync)
                {
                    if (AllGames[GameID].GameState != "pending")
                    {
                        double result = (DateTime.Now - AllGames[GameID].StartGameTime).TotalSeconds;
                        int times = Convert.ToInt32(result);

                        int TimeRemaining;
                        int.TryParse(AllGames[GameID].TimeLeft, out TimeRemaining);

                        if (AllGames[GameID].GameState == "active" && (TimeRemaining - times > 0))
                        {
                            int.TryParse(AllGames[GameID].TimeLimit, out TimeRemaining);
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
                }*/
            }
        }


        /// <summary>
        /// Private method that fully creates a game, called after two players enter a game.
        /// </summary>
        /// <param name="timeLimit">TimeLimit of second user</param>
        /// <param name="gameID">ID of game to be created</param>
   

        /// <summary>
        /// Takes the word submitted by the client and scores it for the client, returning it to them.
        /// </summary>
        /// <param name="words">class that containes the userToken and word that the client is submitting</param>
        /// <param name="GameID">ID of the game that the word is being submitted for</param>
        /// <returns></returns>
        public TokenScoreGameIDReturn playWord(UserGame words, string GameID)
        {
            if (words.UserToken == null || words.UserToken.Trim().Length == 0)
            {
                SetStatus(Forbidden);
                return null;
            }

            if (words.Word == null || words.Word.Trim().Length == 0)
            {
                SetStatus(Forbidden);
                return null;
            }
            string boardState = null;
            string currentPlayerToken = words.UserToken;
            string currentGameID = null;
            string gameState = null;
            List<string> playedWords = new List<string>();
            int WordScoreResult;

            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Command to retrieve boardState
                    using (SqlCommand command = new SqlCommand("select * from Games where GameID = @GameID and (Player1 = @Token or Player2 = @Token)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@Token", currentPlayerToken);
                        command.Parameters.AddWithValue("@GameID", GameID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                SetStatus(Forbidden);
                                trans.Commit();
                                return null;
                            }
                            reader.Read();
                            if (DBNull.Value.Equals(reader["Board"]))
                            {
                                SetStatus(Conflict);
                                trans.Commit();
                                return null;
                            }
                            if (DBNull.Value.Equals(reader["GameID"]))
                            {
                                SetStatus(Forbidden);
                                trans.Commit();
                                return null;
                            }
                            if ((string)reader["Player2"] == null)
                            {
                                gameState = "pending";
                            }
                            else
                            {
                                gameState = "active";
                            }
                            boardState = (string)reader["Board"];
                            currentGameID = (string)reader["GameID"];
                        }
                    }
                    //If word being played equals current word, 
                    using (SqlCommand command = new SqlCommand("select Word from Words where Word = @Word and GameID = @GameID and (Player1 = @Token or Player2 = @Token)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", GameID);
                        command.Parameters.AddWithValue("@Token", currentPlayerToken);
                        command.Parameters.AddWithValue("@Word", words.Word);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!DBNull.Value.Equals(reader["Word"]))
                            {
                                WordScoreResult = ScoreWord(boardState, words.Word, words.UserToken, GameID);
                                TokenScoreGameIDReturn zeroScore = new TokenScoreGameIDReturn();
                                zeroScore.Score = 0.ToString();
                                SetStatus(OK);
                                return zeroScore;
                            }
                        }
                    }
                }
                conn.Close();
            }
            if (gameState != "active")
            {
                SetStatus(Conflict);
                return null;
            }
            UserInfo currentUserInfo = new UserInfo() { UserToken = currentPlayerToken };

            int userScore;
            int.TryParse(currentUserInfo.Score, out userScore);

            WordScoreResult = ScoreWord(boardState, words.Word, words.UserToken, GameID);

            WordScore totalResult = new WordScore();
            totalResult.Word = words.Word;
            totalResult.Score = WordScoreResult;

            currentUserInfo.personalList.Add(totalResult);
            currentUserInfo.Score = (userScore + WordScoreResult).ToString();
            TokenScoreGameIDReturn var = new TokenScoreGameIDReturn();
            var.Score = WordScoreResult.ToString();
            SetStatus(OK);

            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Command to retrieve boardState
                    using (SqlCommand command = new SqlCommand("insert into Words (Word, GameID, Player, Score) values (@Word, @GameID, @Player, @Score)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@Word", words.Word);
                        command.Parameters.AddWithValue("@GameID", GameID);
                        command.Parameters.AddWithValue("@Player", currentPlayerToken);
                        command.Parameters.AddWithValue("@Score", WordScoreResult.ToString());
                    }
                    trans.Commit();
                }
            }
            return var;
        }

        /// <summary>
        /// Scores the word submitted, returning the approriate int value.
        /// </summary>
        /// <param name="word">word to be scored</param>
        /// <param name="userToken">userToken to check whether the user played the word before</param>
        /// <param name="GameID">ID of the game to be checked</param>
        /// <returns></returns>
        private int ScoreWord(string boardState, string word, string userToken, string GameID)
        {
            bool legalWord = searchDictionary(boardState, word.Trim().ToUpper());
            string currentWord = word.Trim();

            if (legalWord == true)
            {
                if (currentWord.Length < 3)
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

        /// <summary>
        /// Searches the dictionary and sees if it is a legal word.  Also checks to see if the word can be formed on the board.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        private bool searchDictionary(string boardState, string key)
        {
            BoggleBoard curBoard = new BoggleBoard(boardState);
            if (dictionaryContents.Contains(key) && curBoard.CanBeFormed(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Registers the client to a userToken.  Creates them in AllPlayer as well.
        /// </summary>
        /// <param name="user">Class that contains the nickname they want to be known as</param>
        /// <returns></returns>
        public TokenScoreGameIDReturn RegisterUser(UserInfo user)
        {
            if (user.Nickname == null || user.Nickname.Trim().Length == 0)
            {
                SetStatus(Forbidden);
                return null;
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(BoggleDB))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        using (SqlCommand command = new SqlCommand("insert into Users (UserID, Nickname) values(@UserID, @Nickname)", conn, trans))
                        {
                            string userID = Guid.NewGuid().ToString();

                            command.Parameters.AddWithValue("@UserID", userID);
                            command.Parameters.AddWithValue("@Nickname", user.Nickname.Trim());

                            command.ExecuteNonQuery();
                            SetStatus(Created);

                            trans.Commit();
                            TokenScoreGameIDReturn result = new TokenScoreGameIDReturn();
                            result.UserToken = userID;
                            return result;
                        }
                    }
                }

            }
        }


        /// <summary>
        /// Allows the client to join a game, or create one if needed
        /// </summary>
        /// <param name="info">Class that contains the UserToken and TimeLimit</param>
        /// <returns></returns>
        public TokenScoreGameIDReturn JoinGame(GameJoin info)
        {

            if (info.UserToken == null || info.UserToken.Trim().Length == 0)
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

            TokenScoreGameIDReturn result = null;
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Here, the SqlCommand is a select query.  We are interested in whether item.UserID exists in
                    // the Users table.
                    using (SqlCommand command = new SqlCommand("select UserID from Users where UserID = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", info.UserToken);

                        // This executes a query (i.e. a select statement).  The result is an
                        // SqlDataReader that you can use to iterate through the rows in the response.
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // In this we don't actually need to read any data; we only need
                            // to know whether a row was returned.
                            if (!reader.HasRows)
                            {
                                SetStatus(Forbidden);
                                trans.Commit();
                                return null;
                            }

                            
                        }
                    }



                    using (SqlCommand command = new SqlCommand("select Player1 from Games where Player1 = @Player AND Player2 IS NULL", conn, trans))
                    {
                        command.Parameters.AddWithValue("@Player", info.UserToken);

                        // This executes a query (i.e. a select statement).  The result is an
                        // SqlDataReader that you can use to iterate through the rows in the response.
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // In this we don't actually need to read any data; we only need
                            // to know whether a row was returned.
                            if (reader.HasRows)
                            {
                                SetStatus(Forbidden);
                                trans.Commit();
                                return null;

                            }
                        }
                    }



                    // Here we are executing an insert command, but notice the "output inserted.ItemID" portion.  
                    // We are asking the DB to send back the auto-generated ItemID.
                    using (SqlCommand command = new SqlCommand("Declare @id int Update Top (1) Games Set Player2 = @Player, TimeLimit = @Time, Board = @Board,  StartTime = @StartTime, @id=GameID  where Player2 is Null and Player1 is not null select @id", conn, trans))
                    {
                        ///test
                        ///This is another test
                        BoggleBoard board = new BoggleBoard();
                        command.Parameters.AddWithValue("@Player", info.UserToken);
                        command.Parameters.AddWithValue("@Time", DateTime.Now);
                        command.Parameters.AddWithValue("@Board", board.ToString());


                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                result = new TokenScoreGameIDReturn();
                                result.GameID = reader["GameID"].ToString();
                                SetStatus(Created);
                                trans.Commit();
                                return result;
                            }
                        }

                    }

                    using (SqlCommand command = new SqlCommand("insert into Games (Player1, TimeLimit) output inserted.GameID values(@Player,@Timelimit)"))
                    {

                        command.Parameters.AddWithValue("@Player", info.UserToken);
                        command.Parameters.AddWithValue("@Timelimit", test);


                        result = new TokenScoreGameIDReturn();
                        result.GameID = command.ExecuteScalar().ToString();
                        SetStatus(Created);
                        trans.Commit();
                        return result;

                    }
                }


            }

        }
    }
}
