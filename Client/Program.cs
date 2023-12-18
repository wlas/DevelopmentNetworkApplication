using Network;

Console.WriteLine("Клиент 1.3");

var t = Task.Run(() => new ClientMsg("Sandr", "Kira"));
t.Wait();