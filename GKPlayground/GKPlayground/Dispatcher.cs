using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GKPlayground
{
    public class Dispatcher : Singleton<Dispatcher>
    {
        public Dictionary<string, Action<CommandData>> actionMap = new();

        public void DispatchCommand(CommandData commandData)
        {
            Task.Run(() => actionMap[commandData.command](commandData));
        }
    }
}
