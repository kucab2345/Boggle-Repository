using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    public interface GameInterface
    {
        void EndGame(List<string> Player1List, List<string> Player2List);
        event Action<string, string, string> CreateGameEvent;
        event Action CancelGameEvent;
        event Action<string> WordEnteredEvent;
        string Player1Name { get; set; }
        string Player2Name { get; set; }
        string Player1Score { get; set; }
        string Player2Score { get; set; }

        string Message { set; }
        bool cancelbutton { get; set; }

        char[] Board { set; }

        string Timer { set; }
    }
}
