namespace Network
{
    public class SenderMsg
    {
        public static void Send(string from, string to, StreamWriter sWriter)
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

                sWriter.WriteLine(json);
                sWriter.Flush();                
            
        }
        public static void Receive(StreamReader sReader)
        {
            string answerServer = sReader.ReadLine();
            Message messageIn = Message.DeserializeFromJson(answerServer);
            messageIn.Print();
        }
    }
}
