using DevelopmentNetworkApplication.Abstracts;
using DevelopmentNetworkApplication.Models;
using System.Net;

namespace DevelopmentNetworkApplication.Services
{
    public class Client
    {
        private readonly string _name;
        private readonly int _port;
        private readonly int _localPort = 12346;
        private readonly IMessageSource _messageSource;
        IPEndPoint remoteEndPoint;
        public Client(string name, string address, int port) 
        { 
            _name = name;
            _port = port;
            _messageSource = new UdpMessageSource(_localPort);
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(address), _port);
        }
        
        public async Task Start()
        {
            await ClientSender();
            await ClientListener();
            
        }

        private async Task ClientSender()
        {
            Register(remoteEndPoint);

            while (true)
            {
                try 
                {
                    Console.Write("Введите имя получателя:");
                    var nameTo = Console.ReadLine();

                    Console.Write("Введите сообщение и нажмите Enter: ");
                    var messageText = Console.ReadLine();

                    var message = new NetMessage { Command = Command.Message, NickNameFrom = _name, NickNameTo = nameTo, Text = messageText };
                    await _messageSource.SendAsync(message, remoteEndPoint);
                    Console.WriteLine("Сообщение отправлено.");
                } 
                catch (Exception ex)
                { 
                    Console.WriteLine($"Ошибка при обработке сообщения: {ex.Message}");
                }
            }
        }

        private void Register(IPEndPoint remoteEndPoint)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, _localPort);
            var message = new NetMessage() { NickNameFrom = _name, NickNameTo = null, Text = null, Command = Command.Register, EndPoint = ep };
            _messageSource.SendAsync(message, remoteEndPoint);
        }
        
        private async Task ClientListener()
        {
            while (true)
            {
                try 
                {                
                    var messageReceived = _messageSource.Receive(ref remoteEndPoint);
                    Console.WriteLine($"Получено сообщение от {messageReceived.NickNameFrom}");
                    Console.WriteLine(messageReceived.Text);

                    await Confirm(messageReceived, remoteEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при получение сообщения " + ex.Message);
                }
            }
        }

        private async Task Confirm(NetMessage message, IPEndPoint remoteEndPoint)
        {
            message.Command = Command.Confirmation;
            await _messageSource.SendAsync(message, remoteEndPoint);
        }
    }
}
