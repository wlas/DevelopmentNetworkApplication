using System.Net;
using System.Net.Sockets;
using System.Text;


namespace DevelopmentNetworkApplication
{
    internal class UDPServer
    {
        class Server
        {
            Dictionary<String, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
            UdpClient udpClient;

            void Register(NetMessage message, IPEndPoint fromep)
            {
                Console.WriteLine("Message Register, name = " + message.NickNameFrom);
                clients.Add(message.NickNameFrom, fromep);


                using (var ctx = new ChatContext())
                {
                    if (ctx.Users.FirstOrDefault(x => x.FullName == message.NickNameFrom) != null) return;

                    ctx.Add(new User { FullName = message.NickNameFrom });

                    ctx.SaveChanges();
                }
            }

            void ConfirmMessageReceived(int? id)
            {
                Console.WriteLine("Message confirmation id=" + id);

                using (var ctx = new ChatContext())
                {
                    var msg = ctx.Messages.FirstOrDefault(x => x.MessageId == id);

                    if (msg != null)
                    {
                        msg.IsSent = true;
                        ctx.SaveChanges();
                    }
                }
            }

            void RelyMessage(NetMessage message)
            {
                int? id = null;
                if (clients.TryGetValue(message.NickNameTo, out IPEndPoint ep))
                {
                    using (var ctx = new ChatContext())
                    {
                        var fromUser = ctx.Users.First(x => x.FullName == message.NickNameFrom);
                        var toUser = ctx.Users.First(x => x.FullName == message.NickNameTo);
                        var msg = new Message() { UserFrom = fromUser, UserTo = toUser, IsSent = false, Text = message.Text };
                        ctx.Messages.Add(msg);

                        ctx.SaveChanges();

                        id = msg.MessageId;
                    }


                    var forwardMessageJson = new NetMessage()
                    {
                        Id = id,
                        Command = Command.Message,
                        NickNameTo = message.NickNameTo,
                        NickNameFrom = message.NickNameFrom,
                        Text = message.Text
                    }.SerialazeMessageToJSON();

                    byte[] forwardBytes = Encoding.ASCII.GetBytes(forwardMessageJson);

                    udpClient.Send(forwardBytes, forwardBytes.Length, ep);
                    Console.WriteLine($"Message Relied, from = {message.NickNameFrom} to = {message.NickNameTo}");
                }
                else
                {
                    Console.WriteLine("Пользователь не найден.");
                }
            }

            void ProcessMessage(NetMessage message, IPEndPoint fromep)
            {
                Console.WriteLine($"Получено сообщение от {message.NickNameFrom} для {message.NickNameTo} с командой {message.Command}:");
                Console.WriteLine(message.Text);


                if (message.Command == Command.Register)
                {
                    Register(message, new IPEndPoint(fromep.Address, fromep.Port));

                }
                if (message.Command == Command.Confirmation)
                {
                    Console.WriteLine("Confirmation receiver");
                    ConfirmMessageReceived(message.Id);
                }
                if (message.Command == Command.Message)
                {
                    RelyMessage(message);
                }
            }


            public void Work()
            {

                IPEndPoint remoteEndPoint;

                udpClient = new UdpClient(12345);
                remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                Console.WriteLine("UDP Клиент ожидает сообщений...");

                while (true)
                {
                    byte[] receiveBytes = udpClient.Receive(ref remoteEndPoint);
                    string receivedData = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine(receivedData);

                    try
                    {
                        var message = NetMessage.DeserializeMessgeFromJSON(receivedData);

                        ProcessMessage(message, remoteEndPoint);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                    }
                }

            }
        }
    }
}
