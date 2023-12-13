using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Network
{
    public class ServerMsg
    {
        TcpListener server;
        string _from;
        string _to;
        bool _work = true;

        public ServerMsg(string from, string to) 
        {
            try
            {
                _from = from;
                _to = to;
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5555);
                server.Start();

                LoopClients();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
            
        }       

        void LoopClients()
        {           
                TcpClient client = server.AcceptTcpClient();
                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
        }

        void HandleClient(TcpClient client)
        {
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.UTF8);
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.UTF8);
            
            Console.WriteLine("Сервер ждет сообщение от клиента: ");

            while (_work)
            {
                _work = SenderMsg.Receive(sReader);

                if(_work)
                {
                    SenderMsg.Send(_from, _to, sWriter);
                }
                else
                {
                    Console.WriteLine("Завершение работы сервера.");
                }
            }
        }
    }
}
