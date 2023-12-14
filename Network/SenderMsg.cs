using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Network
{
    public class SenderMsg
    {
        public static void Send(string from, string to, UdpClient client, ref IPEndPoint iPEndPoint)
        {
            
                string messageText;
                do
                {
                    Console.Write("Введите сообщение: ");
                    messageText = Console.ReadLine();
                }

                while (string.IsNullOrEmpty(messageText));

                Message messageOut = new Message() { Text = messageText, NicknameFrom = from, NicknameTo = to, DateTime = DateTime.Now };
                string json = messageOut.SerializeMessageToJson();

                
                byte[] data = Encoding.UTF8.GetBytes(json);
                client.Send(data, data.Length, iPEndPoint);

        }
        public static bool Receive(UdpClient client, ref IPEndPoint iPEndPoint)
        {
            var buffer = client.Receive(ref iPEndPoint);
            var massage = Encoding.UTF8.GetString(buffer);
            Message messageIn = Message.DeserializeFromJson(massage);
            if (messageIn.Text.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Сервер получил ключевое слово shutdown \"Exit\" от клиента, сеанс чата будет закрыт.");
                return false;
            }
            messageIn.Print();
            return true;
        }
    }
}
