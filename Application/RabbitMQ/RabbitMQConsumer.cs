using Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Application.RabbitMQ
{
    public class RabbitMQConsumer : IRabbitMQConsumer
    {
        private readonly IModel _channel;

        // Hàm kiểm tra sự tồn tại của queue
        private static bool QueueExists(IModel channel, string queueName)
        {
            try
            {
                // DeclarePassive sẽ ném ra một ngoại lệ nếu queue không tồn tại
                channel.QueueDeclarePassive(queueName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public RabbitMQConsumer(IRabbitMQConnectionManager connectionManager)
        {
            _channel = connectionManager.Channel;
        }

        public void ConsumeMessages(string queueName, Action<string> messageHandler)
        {
            try
            {
                // Kiểm tra xem queue có tồn tại không
                if (QueueExists(_channel, queueName))
                {

                    var consumer = new EventingBasicConsumer(_channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        messageHandler?.Invoke(message);
                    };

                    _channel.BasicConsume(
                        queue: queueName,
                        autoAck: true,
                        consumer: consumer
                    );
                }
                else
                {
                    Console.WriteLine($"Queue '{queueName}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ConsumeMessages: {ex.Message}");
            }
        }
    }
}
