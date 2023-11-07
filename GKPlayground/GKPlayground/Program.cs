using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GKPlayground
{
    class Server
    {
        static int port = 9999;
        static UdpClient udpServer = new UdpClient(port);
        static TcpListener tcpServer = new TcpListener(IPAddress.Any,port);
        public static Dictionary<string, Client> clientMap = new();
        public static List<IPEndPoint> ipEndPoints = new();

        static async Task Main()
        {
            InitAllClasses();

            Task handleTcpTask = HandleTcp();
            Task receiveMessageUdpTask = ReceiveMessagesUdp();
            Console.WriteLine("Tcp Udp \"가능\"");
            await Task.WhenAll(handleTcpTask, receiveMessageUdpTask);

        }

        static async Task HandleTcp()
        {
            tcpServer.Start();
            while (true)
            {
                TcpClient client = await tcpServer.AcceptTcpClientAsync();
                NetworkStream stream = client.GetStream();
                Console.WriteLine("TCP 클라이언트가 연결되었습니다.");

                ReceiveMessagesTcp(stream);
            }
        }

        static async void ReceiveMessagesTcp(NetworkStream stream)
        {
            byte[] buffer = new byte[8000];
            StringBuilder jsonBuffer = new StringBuilder();
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    break;
                }

                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                jsonBuffer.Append(data, 0, bytesRead);

                if (IsJsonComplete(jsonBuffer.ToString()))
                {
                    string message = jsonBuffer.ToString();
                    jsonBuffer.Clear();
                    Console.WriteLine("TCP 클라이언트로부터 메시지 받음: " + message);

                    CommandData commandData = JsonSerializer.Deserialize<CommandData>(message);
                    commandData.sender = stream;

                    Dispatcher.Instance.DispatchCommand(commandData);
                }
            }

            Console.WriteLine("come");
        }

        public static async Task SendMessageTcp(NetworkStream stream,string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
            Console.WriteLine("Tcp 메세지 보냄 " + data.Length);
        }

        public static async Task BroadcastMessageTcp(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach(Client client in clientMap.Values)
            {
                await client.stream.WriteAsync(data,0,data.Length);
            }
        }

        static async Task ReceiveMessagesUdp()
        {
            while (true)
            {
                
                UdpReceiveResult result = await udpServer.ReceiveAsync();
                IPEndPoint clientEndPoint = result.RemoteEndPoint;

                string message = Encoding.UTF8.GetString(result.Buffer);
                //Console.WriteLine("UDP 클라이언트로부터 메시지 받음: " + message);

                CommandData commandData = JsonSerializer.Deserialize<CommandData>(message);

                if (clientMap[commandData.name].endPoint == null)
                {
                    clientMap[commandData.name].endPoint = clientEndPoint;
                }

                Dispatcher.Instance.DispatchCommand(commandData);
            }
        }

        public static async Task BroadcastMessageUdp(String message)
        {
            Byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (Client client in clientMap.Values)
            {
                if (client.endPoint == null)
                    continue;

                await udpServer.SendAsync(data, data.Length,client.endPoint);
            }
        }

        static void InitAllClasses()
        {
            DBManager.Instance.Init();
            ClientManager.Instance.Init();
        }

        static bool IsJsonComplete(string data)
        {
            return data.TrimStart().StartsWith("{") && data.TrimEnd().EndsWith("}");
        }
    }
}
