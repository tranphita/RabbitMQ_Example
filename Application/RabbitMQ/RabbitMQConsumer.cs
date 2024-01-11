using Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace Application.RabbitMQ
{
    public class RabbitMQConsumer : IRabbitMQConsumer
    {
        private readonly IRabbitMQConnectionManager _connectionManager;
        private readonly IModel _channel;

        #region Check Queue Exists
        private bool QueueExists(string queueName)
        {
            try
            {
                // Kiểm tra xem queue có tồn tại hay không
                _channel.QueueDeclarePassive(queue: queueName);
                return true;
            }
            catch (OperationInterruptedException)
            {
                return false;
            }
        }
        #endregion

        public RabbitMQConsumer(IRabbitMQConnectionManager connectionManager)
        {
            _channel = connectionManager.Channel;
            _connectionManager = connectionManager;
        }

        /// <summary>
        /// Xử lý nhận tin nhắn
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="messageHandler"></param>
        public void ConsumeMessages(string queueName, Action<string> messageHandler)
        {
            // Kiểm tra xem queue có tồn tại hay không
            if (QueueExists(queueName))
            {
                try
                {
                    // Kiểm tra kết nối trước khi tiếp tục
                    if (!_connectionManager.IsConnected())
                    {
                        _connectionManager.TryReconnect();
                    }

                    var consumer = new EventingBasicConsumer(_channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        messageHandler?.Invoke(message); // Khi một message được nhận, nội dung của nó sẽ được chuyển tới hàm xử lý message được chỉ định thông qua messageHandler.
                    };

                    // Quá trình tiêu thụ message từ queue
                    _channel.BasicConsume(
                        queue: queueName,
                        autoAck: true,
                        consumer: consumer
                    );
                }
                catch (Exception ex)
                {                 
                    Console.WriteLine($"co loi trong qua trinh nhan message: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Queue '{queueName}' khong ton tai, khong the lang nghe tu queue nay!");
            }
        }
    }
}
