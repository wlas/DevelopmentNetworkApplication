using DevelopmentNetworkApplication.Abstracts;
using DevelopmentNetworkApplication.Models;
using System.Net;


namespace DevelopmentNetworkApplication.Services
{
    public class Server
    {
        Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
        private readonly IMessageSource _messageSource;
        private IPEndPoint ep;
        private bool work = true;
        public Server()
        {
            _messageSource = new UdpMessageSource(12345);
            ep = new IPEndPoint(IPAddress.Any, 0);
        }
        async Task ConfirmMessageReceived(int? id)
        {
            Console.WriteLine($"Message confirmation id={id}");
            using(var context = new ChatContext())
            {
                var msg = context.Messages.FirstOrDefault(x => x.MessageId == id);
                if (msg != null)
                {
                    msg.IsSent = true;
                    await context.SaveChangesAsync();
                }
            }
        }
        private async Task Register(NetMessage message)
        {
            Console.WriteLine($"Message Register name = {message.NickNameFrom}");

            if (clients.TryAdd(message.NickNameFrom, message.EndPoint))
            {
                using(var context = new ChatContext())
                {
                    context.Users.Add(new User() { FullName = message.NickNameFrom});
                    await context.SaveChangesAsync();
                }
            }
        }
        private async Task RelyMessage(NetMessage message)
        {
            if (clients.TryGetValue(message.NickNameTo, out IPEndPoint iPEand))
            {
                int? id = 0;
                using(var context = new ChatContext())
                {
                    var fromUser = context.Users.First(x => x.FullName == message.NickNameFrom);
                    var toUser = context.Users.First(x => x.FullName == message.NickNameTo);
                    var msg = new Message { UserFrom = fromUser, UserTo = toUser, IsSent = false, Text = message.Text };
                    context.Messages.Add(msg);
                    context.SaveChanges();
                    id = msg.MessageId;
                }
                message.Id = id;
                await _messageSource.SendAsync(message, ep);

                Console.WriteLine($"Message Relied, from = {message.NickNameFrom} to = {message.NickNameTo}");               
            }
            else
            {
                Console.WriteLine("Пользователь не найден");
            }
        }
        async Task ProcessMessage(NetMessage message)
        {
            switch(message.Command)
            {
                case Command.Register: await Register(message); break;
                case Command.Message: await RelyMessage(message); break;
                case Command.Confirmation: await ConfirmMessageReceived(message.Id); break;
            }
        }
        public async Task Start()
        {
            Console.WriteLine("Сервер ожидает сообщения");

            while (work)
            {
                try
                {
                    var message = _messageSource.Receive(ref ep);
                    Console.WriteLine(message.ToString());
                    await ProcessMessage(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        public void Stop()
        {
            work = false;
        }
    }
}
