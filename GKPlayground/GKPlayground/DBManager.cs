using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;

namespace GKPlayground
{
    public class DBManager : Singleton<DBManager>
    {
        private string connectionString;
        private SqliteConnection connection;

        public void Init() 
        {
            connectionString = $"Data Source=database.db";
            connection = new SqliteConnection(connectionString);
            connection.Open();

            Dispatcher.Instance.actionMap["Register"] = Register;
            Dispatcher.Instance.actionMap["Login"] = Login;
        }

        public async void Register(CommandData commandData)
        {
            using (var dbCommand = connection.CreateCommand())
            {
                    dbCommand.CommandText = "INSERT INTO Accounts (Name, Password, Image) VALUES (@Name, @Password, @Image)";
                    dbCommand.Parameters.AddWithValue("@Name", commandData.name);
                    dbCommand.Parameters.AddWithValue("@Password", commandData.password);
                    dbCommand.Parameters.AddWithValue("@Image", commandData.image);

                try
                {
                    dbCommand.ExecuteNonQuery();
                    LoginSucceed(commandData);
                    Console.WriteLine(commandData.name + "가입함");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(commandData.name + "가입실패");
                    Console.WriteLine(ex);
                }
            }
        }

        public async void Login(CommandData commandData)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Image FROM Accounts WHERE Name = @Name AND Password = @Password";
                command.Parameters.AddWithValue("@Name", commandData.name);
                command.Parameters.AddWithValue("@Password", commandData.password);

                try
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();

                    commandData.image = (byte[])reader["Image"];
                    
                    LoginSucceed(commandData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(commandData.name + "로그인실패");
                    Console.WriteLine(ex);
                }
            }
        }

        private async void LoginSucceed(CommandData commandData)
        {
            Server.clientMap[commandData.name]=(new Client(commandData.sender, commandData.image));

            CommandData reply = new()
            {
                command = "Login",
                name = commandData.name,
                image = commandData.image
            };
            await Server.SendMessageTcp(commandData.sender, JsonSerializer.Serialize(reply));
            await ClientManager.Instance.SendWholeClientInfo(commandData.sender);

            reply.command = "Join";
            await Server.BroadcastMessageTcp(JsonSerializer.Serialize(reply));
        }
    }
}
