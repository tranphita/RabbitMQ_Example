namespace Application.Interfaces
{
    public interface IRabbitMQConsumer
    {
        void ConsumeMessages(string queueName, Action<string> messageHandler);
    }
}
