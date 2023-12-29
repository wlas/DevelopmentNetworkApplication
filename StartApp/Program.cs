using ChatApp;
using System.Net;

class Program
{
	static async Task Main(string[] args)
	{
		if (args.Length == 0)
		{
			var s = new Server<IPEndPoint>(new UdpMessageSourceServer());
			await s.Start();
		}
		else
		if (args.Length == 1)
		{
			var c = new Client<IPEndPoint>(new UdpMessageSourceClient(), args[0]);
			await c.Start();
		}
		else
		{
			Console.WriteLine("Для запуска сервера введите ник-нейм как параметр запуска приложения");
			Console.WriteLine("Для запуска клиента введите ник-нейм и IP сервера как параметры запуска приложения");
		}
		Console.ReadLine();
	}
}
