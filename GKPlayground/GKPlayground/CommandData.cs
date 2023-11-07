using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GKPlayground
{
    public class CommandData
    {
        public string command { get; set; }
        public string type { get; set; }
        public byte[] image { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public NetworkStream sender;
    }
}
