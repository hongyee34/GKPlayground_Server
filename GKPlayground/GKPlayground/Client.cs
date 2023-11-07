using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GKPlayground
{
    public class Client
    {
        public IPEndPoint endPoint;
        public NetworkStream stream;
        public byte[] profileImage;

        public (float x, float y) pos;

        public Client(NetworkStream stream, byte[] profileImage)
        {
            this.stream = stream;
            this.profileImage = profileImage;
        }
    }
}
