using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class UDPServer
{
    static List<IPEndPoint> connectedClients = new List<IPEndPoint>(); // 연결된 클라이언트의 IP 주소를 저장하는 리스트
    static UdpClient udpServer;

    static async Task Main()
    {
        int serverPort = 9999; // 서버 포트

        udpServer = new UdpClient(serverPort);
        Console.WriteLine("서버가 {0} 포트에서 대기 중...", serverPort);
        await ReceiveMessagesAsync();
    }

    static async Task ReceiveMessagesAsync()
    {
        while (true)
        {
            UdpReceiveResult result = await udpServer.ReceiveAsync();
            IPEndPoint clientEndPoint = result.RemoteEndPoint;

            // 연결된 클라이언트 IP 주소를 리스트에 저장
            if (!connectedClients.Contains(clientEndPoint))
            {
                connectedClients.Add(clientEndPoint);
                Console.WriteLine($"새로운 클라이언트 연결: {clientEndPoint}");
            }

            string message = Encoding.ASCII.GetString(result.Buffer);
            await BroadcastDataToClientsAsync(message);
        }
    }

    static async Task BroadcastDataToClientsAsync(String message)
    {
        DateTime currentTime = DateTime.Now;
        Byte[] data = Encoding.ASCII.GetBytes(message);
        foreach (var endPoint in connectedClients)
        {
            udpServer.Send(data, data.Length, endPoint);
        }
    }
}
