using Application.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace Application.RabbitMQ
{
    /// <summary>
    /// Xử lý tạo hàng đợi và xuất bản message vào hàng đợi đó
    /// </summary>
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private IModel _channel;
        private IRabbitMQConnectionManager _connectionManager;

        public RabbitMQPublisher(IRabbitMQConnectionManager connectionManager)
        {
            _channel = connectionManager.Channel;
            _connectionManager = connectionManager;
        }

        public void PublishMessage(string exchange, string routingKey, string message)
        {
            try
            {
                // Kiểm tra kết nối trước khi gửi tin nhắn
                if (!_connectionManager.IsConnected())
                {
                    _connectionManager.TryReconnect();
                    _channel = _connectionManager.Channel;
                }

                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(
                   exchange: exchange,
                   routingKey: routingKey,
                   basicProperties: null,
                   body: body
               );

               Console.WriteLine($"gui tin nhan thanh cong:");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Co loi trong qua trinh gui tin nhan: {ex.Message}");
            }
        }
    }
}
