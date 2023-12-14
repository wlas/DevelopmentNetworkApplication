using System.Net.Sockets;
using System.Net;

namespace Network
{
    public class ServerMsg
    {
        string _from;
        string _to;
        bool _work = true;
        UdpClient udpClient;

        public ServerMsg(string from, string to) 
        {
            try
            {
                _from = from;
                _to = to;
                udpClient = new UdpClient(12345);
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

                LoopClients(iPEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }            
        }       

        void LoopClients(IPEndPoint iPEndPoint)
        {
            Console.WriteLine("Сервер ждет сообщение от клиента: ");
            while (_work)
            {
                
                ThreadPool.QueueUserWorkItem(obj =>
                {
                    _work = SenderMsg.Receive(udpClient, ref iPEndPoint);

                    if (_work)
                    {
                        SenderMsg.Send(_from, _to, udpClient, ref iPEndPoint);
                    }
                    else
                    {
                        Console.WriteLine("Завершение работы сервера.");
                    }
                });
            }
        }        
    }
}
