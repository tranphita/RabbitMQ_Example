namespace Application.Interfaces
{
    public interface IRabbitMQPublisher
    {
        void PublishMessage(string exchange, string routingKey, string message);
    }
}
