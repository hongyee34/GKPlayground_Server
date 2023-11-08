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
    public class ClientManager : Singleton<ClientManager>
    {
        public void Init() 
        {
            Dispatcher.Instance.actionMap["SetPos"] = SetPos;
            Dispatcher.Instance.actionMap["Chat"] = Chat;
        }

        async void SetPos(CommandData commandData)
        {
            Server.clientMap[commandData.name].pos = (commandData.posX, commandData.posY);

            await Server.BroadcastMessageUdp(JsonSerializer.Serialize(commandData));
        }

        async void Chat(CommandData commandData)
        {
            await Server.BroadcastMessageTcp(JsonSerializer.Serialize(commandData));
        }

        public async Task SendWholeClientInfo(NetworkStream stream)
        {
            foreach(KeyValuePair<string,Client> pair in Server.clientMap)
            {
                CommandData commandData = new()
                {
                    command = "Join",
                    name = pair.Key,
                    image = pair.Value.profileImage
                };
                await Server.SendMessageTcp(stream,JsonSerializer.Serialize(commandData));
            }
        }
    }
}
