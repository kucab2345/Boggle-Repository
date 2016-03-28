using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
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
            public string UserToken { get; set; }
        }
        public class GameJoin
        {
            public string TimeLimit { get; set; }
            public string UserToken { get; set; }
        }
        public class UserGame
        {
            public string Word { get; set; }
            public string UserToken { get; set; }
        }
    
}