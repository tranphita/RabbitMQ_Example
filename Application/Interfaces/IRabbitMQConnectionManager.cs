using RabbitMQ.Client;

namespace Application.Interfaces
{
    public interface IRabbitMQConnectionManager : IDisposable
    {
        IModel Channel { get; }
        bool IsConnected();
        void TryReconnect();
    }
}
