using System.Net;
using DevelopmentNetworkApplication.Models;

namespace DevelopmentNetworkApplication.Abstracts
{
    public interface IMessageSource
    {
        Task SendAsync(NetMessage message, IPEndPoint ep);

        NetMessage Receive(ref IPEndPoint ep);

    }
}
