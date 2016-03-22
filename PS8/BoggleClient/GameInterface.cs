using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    public interface GameInterface
    {
        event Action<string, string, string> CreateGameEvent;
        event Action CancelGameEvent;

        string Player1Name { get; set; }
        string Player2Name { get; set; }
        string Player1Score { get; set; }
        string Player2Score { get; set; }

        string Message { set; }
        bool cancelbutton { get; set; }
    }
}
