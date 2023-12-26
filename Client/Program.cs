using DevelopmentNetworkApplication.Services;

Client client = new Client("Sandr", "127.0.0.1", 12345);
client.Start().Wait();
Console.ReadLine();

