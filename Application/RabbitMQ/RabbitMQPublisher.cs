using Application.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace Application.RabbitMQ
{
    /// <summary>
    /// Xử lý tạo hàng đợi và xuất bản message vào hàng đợi đó
    /// </summary>
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IModel _channel;

        /// <summary>
        /// Function to check if a queue exists
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        private static bool QueueExists(IModel channel, string queueName)
        {
            try
            {
                // DeclarePassive sẽ đưa ra một ngoại lệ nếu queue không tồn tại
                channel.QueueDeclarePassive(queueName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public RabbitMQPublisher(IRabbitMQConnectionManager connectionManager)
        {
            _channel = connectionManager.Channel;
        }

        public void PublishMessage(string exchange, string routingKey, string message)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message);

                // Tạo queue (hàng đợi) - Nếu queue đã tồn tại thì không tạo mới queue đó

                // durable (bền vững) - giúp giữ queue tồn tại ngay cả khi RabbitMQ server bị tắt và khởi động lại 
                // exclusive - Chỉ được sử dụng trong một kết nối duy nhất, khi kết nối này đóng thì queue sẽ bị xóa. Sử dụng exclusive khi muốn đảm bảo 1 comsumer đọc được message từ queue đó
                // autoDelete - tự động xóa nếu không có consumer nào kết nối đến queue đó, giúp giải phóng tài nguyên khi hàng đợi không còn sử dụng
                // arguments - tham số tùy chỉnh như Time-to-Live, Maximum Length của queue hoặc message, 

                if (!QueueExists(_channel, routingKey))
                {
                    _channel.QueueDeclare(queue: routingKey, durable: false, exclusive: false, autoDelete: false, arguments: null);
                }

                _channel.BasicPublish(
                    exchange: exchange,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body
                );

                Console.WriteLine($"Sent message: {message}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PublishMessage: {ex.Message}");
            }
        }
    }
}
