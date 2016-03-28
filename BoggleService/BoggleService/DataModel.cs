using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Boggle
{
    [DataContract]
    public class GameStatus
    {
        [DataMember]
        public string GameState { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Board { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string TimeLimit { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string TimeLeft { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public UserInfo Player1 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public UserInfo Player2 { get; set; }

        public TimeSpan StartGameTime;
    }

    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public string Nickname { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public string Score { get; set; }

        [DataMember]
        public string UserToken { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> Words { get; set; }

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