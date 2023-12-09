using System.Net.Sockets;
using System.Text;

namespace Network
{
    public class ClientMsg
    {
        public ClientMsg(string from, string to)
        {
            var client = new TcpClient("127.0.0.1", 5555);
            var sWriter = new StreamWriter(client.GetStream(), Encoding.UTF8);
            var sReader = new StreamReader(client.GetStream(), Encoding.UTF8);

            while (true)
            {
                SenderMsg.Send(from, to, sWriter);
                SenderMsg.Receive(sReader);
            }            
        }       
    }
}
