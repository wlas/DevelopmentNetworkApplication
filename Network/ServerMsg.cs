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
        public ServerMsg(string from, string to) 
        {
            _from = from;
            _to = to;
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5555);
            server.Start();

            LoopClients();
        }       

        void LoopClients()
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
            }
        }

        void HandleClient(TcpClient client)
        {
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.UTF8);
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.UTF8);
            
            Console.WriteLine("Сервер ждет сообщение от клиента: ");

            while (true)
            {
                SenderMsg.Receive(sReader);

                SenderMsg.Send(_from, _to, sWriter);
            }
        }
    }
}
