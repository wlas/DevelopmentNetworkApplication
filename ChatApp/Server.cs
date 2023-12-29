using ChatCommon.Abstracts;
using ChatCommon.Model;
using ChatDB;
using System.Net;


namespace ChatApp
{
    public class Server<T>
    {
        Dictionary<string, T> clients = new Dictionary<string, T>();
        private readonly IMessageSourceServer<T> _messageSource;
        private T ep;
        private bool work = true;
        public Server(IMessageSourceServer<T> messageSourceServer)
        {
            _messageSource = messageSourceServer;
            ep = _messageSource.CreateEndpoint();
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

            if (clients.TryAdd(message.NickNameFrom, _messageSource.CopyEndpoint(message.EndPoint)))
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
            if (clients.TryGetValue(message.NickNameTo, out T iPEand))
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
