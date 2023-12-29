using DevelopmentNetworkApplication.Models;
using DevelopmentNetworkApplication.Services;

namespace ServerTest
{
    public class ServerTest
    {
        [SetUp]
        public void Setup()
        {
            using (var chatContext = new ChatContext()) 
            {
                chatContext.Messages.RemoveRange(chatContext.Messages);
                chatContext.Users.RemoveRange(chatContext.Users);
                chatContext.SaveChanges();
            }
        }

        [TearDown]
        public void TearDown() 
        {
            using(ChatContext chatContext = new ChatContext())
            {
                chatContext.Messages.RemoveRange(chatContext.Messages);
                chatContext.Users.RemoveRange(chatContext.Users);
                chatContext.SaveChanges();
            }
        }

        [Test]
        public async Task Test()
        {
            var mock = new MockMessageSource();
            var server = new Server();
            mock.AddServer(server);
            await server.Start();

            using (ChatContext chatContext = new ChatContext())
            {
                var user1 = chatContext.Users.FirstOrDefault(x => x.FullName == "Вася");
                var user2 = chatContext.Users.FirstOrDefault(x => x.FullName == "Юля");

                Assert.IsNotNull(user1, "Пользователь не создан");
                Assert.IsNotNull(user2, "Пользователь не создан");

                Assert.IsTrue(user1.MessagesFrom.Count == 1);
                Assert.IsTrue(user2.MessagesFrom.Count == 1);

                Assert.IsTrue(user1.MessagesTo.Count == 1);
                Assert.IsTrue(user2.MessagesTo.Count == 1);

                var msg1 = chatContext.Messages.FirstOrDefault(x => x.UserFrom == user1 && x.UserTo == user2);
                var msg2 = chatContext.Messages.FirstOrDefault(x => x.UserFrom == user2 && x.UserTo == user1);

                Assert.AreEqual("От Юли", msg2.Text);
                Assert.AreEqual("От Васи", msg1.Text);

            }

        }
    }
}
