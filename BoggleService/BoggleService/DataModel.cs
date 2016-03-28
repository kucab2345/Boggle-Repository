using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
{
    public class DataModel 
    {
        public class GameStatus
        {
            public string GameState { get; set; }
            public string Board { get; set; }
            public string TimeLimit { get; set; }
            public string TimeLeft { get; set; }
        }
        public class UserInfo
        {
            public string Nickname { get; set; }
            public string Score { get; set; }
            public string userToken { get; set; }
        }
        public class GameJoin
        {
            public string TimeLimit { get; set; }
            public string userToken { get; set; }
        }
        public class UserGame
        {
            public string currentWord { get; set; }
        }
    }
}