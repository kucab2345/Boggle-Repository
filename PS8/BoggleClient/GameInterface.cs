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

        string Message { set; }
    }
}
