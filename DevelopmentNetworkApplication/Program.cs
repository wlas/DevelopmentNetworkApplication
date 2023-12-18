using Network;

Console.WriteLine("Клиет - Сервер 1.3");

var t = Task.Run(() => new ServerMsg("Kira", "Sandr"));
t.Wait();