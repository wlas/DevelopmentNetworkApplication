using DevelopmentNetworkApplication.Abstracts;
using DevelopmentNetworkApplication.Models;
using DevelopmentNetworkApplication.Services;
using System.Net;

namespace ServerTest
{
    public class MockMessageSource : IMessageSource
    {
        private Queue<NetMessage> messages = new();
        private Server server;
        private IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        public MockMessageSource() 
        {
            messages.Enqueue( new NetMessage { Command = Command.Register, NickNameFrom = "Вася" });
            messages.Enqueue( new NetMessage { Command = Command.Register, NickNameFrom = "Юля" });
            messages.Enqueue( new NetMessage { Command = Command.Message, NickNameTo = "Вася", NickNameFrom = "Юля" });
            messages.Enqueue( new NetMessage { Command = Command.Message, NickNameTo = "Юля", NickNameFrom = "Вася" });
        }
        public NetMessage Receive(ref IPEndPoint ep)
        {
            ep = endPoint;
            if(messages.Count == 0)
            {
                server.Stop();
                return null;
            }
            return messages.Dequeue();

        }

        public Task SendAsync(NetMessage message, IPEndPoint ep)
        {
            throw new NotImplementedException();
        }
        public void AddServer(Server server)
        {
            this.server = server;
        }


    }
}
