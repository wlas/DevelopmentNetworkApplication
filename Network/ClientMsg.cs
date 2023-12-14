using System.Net;
using System.Net.Sockets;

namespace Network
{
    public class ClientMsg
    {
        public ClientMsg(string from, string to)
        {
            try 
            {
                var client = new UdpClient();
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);


                while (true)
                {
                    SenderMsg.Send(from, to, client, ref iPEndPoint);
                    SenderMsg.Receive(client, ref iPEndPoint);
                }
            } 
            catch (Exception e) 
            { 
                Console.WriteLine(e.Message);
                Console.ReadLine();
            } 
                      
        }       
    }
}
