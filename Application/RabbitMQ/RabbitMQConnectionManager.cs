using Application.Interfaces;
using RabbitMQ.Client;

namespace Application.RabbitMQ
{
    public class RabbitMQConnectionManager : IRabbitMQConnectionManager
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private void Reconnect()
        {
            // Tạo kết nối mới
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            _connection.Dispose(); // Giải phóng tài nguyên của kết nối cũ
            _connection.Close();   // Đóng kết nối cũ

            //_connection = factory.CreateConnection();
            //_channel = _connection.CreateModel();

            Console.WriteLine("Reconnected successfully.");
        }

        public RabbitMQConnectionManager(string hostName, int port, string userName, string password)
        {
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password,
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public IModel Channel => _channel;

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }

        public bool IsConnected()
        {
            // Kiểm tra trạng thái kết nối
            return _connection != null && _connection.IsOpen;
        }

        public void TryReconnect()
        {
            // Thử tái kết nối nếu kết nối đang đóng
            if (_connection == null || !_connection.IsOpen)
            {
                Console.WriteLine("Attempting to reconnect...");
                Reconnect();
            }
        }
    }
}
