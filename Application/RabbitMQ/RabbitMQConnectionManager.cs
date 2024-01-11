using Application.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using RabbitMQ.Client;
using System.Security.Cryptography;
using System.Threading.Channels;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.RabbitMQ
{
    public class RabbitMQConnectionManager : IRabbitMQConnectionManager
    {
        private IConnection _connection;
        private IModel _channel;

        private void Connection(ConnectionFactory factory)
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Tạo Exchange

            // Direct: khi muốn chuyển đến hàng đợi dựa trên một routing key cụ thể
            // Fanout: khi muốn chuyển đến tất cả các hàng đợi mà nó biết đến, không quan tâm đến routing key.
            // Topic: khi muốn định tuyến dựa trên mẫu của routing key
            // Headers: khi muốn định tuyến tin nhắn dựa trên các thuộc tính (headers) của tin nhắn
            _channel.ExchangeDeclare(exchange: "fanout_exchange", type: ExchangeType.Fanout);
            _channel.ExchangeDeclare(exchange: "gps_exchange", type: ExchangeType.Fanout);
            _channel.ExchangeDeclare(exchange: "direct_exchange", type: ExchangeType.Direct);



            // Duyệt qua danh sách các tenant
            List<string> tenants = new List<string> { "tenant1", "tenant2", "tenant3", "tenant4", "tenant5" };

            for (int i = 0; i < tenants.Count; i++)
            {
                string queueName = $"{tenants[i]}_queue";

                // Tạo queue (hàng đợi) - Nếu queue đã tồn tại thì không tạo mới queue đó

                // durable (bền vững) - giúp giữ queue tồn tại ngay cả khi RabbitMQ server bị tắt và khởi động lại 
                // exclusive - Chỉ được sử dụng trong một kết nối duy nhất, khi kết nối này đóng thì queue sẽ bị xóa. Sử dụng exclusive khi muốn đảm bảo 1 comsumer đọc được message từ queue đó
                // autoDelete - tự động xóa nếu không có consumer nào kết nối đến queue đó, giúp giải phóng tài nguyên khi hàng đợi không còn sử dụng
                // arguments - tham số tùy chỉnh như Time-to-Live, Maximum Length của queue hoặc message, 
                _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                _channel.QueueBind(queue: queueName, exchange: "fanout_exchange", routingKey: "");

                if (i < 3)
                    _channel.QueueBind(queue: queueName, exchange: "direct_exchange", routingKey: "premium"); // bind queue này vào fanout_exchange    
                else
                    _channel.QueueBind(queue: queueName, exchange: "direct_exchange", routingKey: "free");
            }

            // tạo queue không thuộc exchange để xử lý 1 : 1
            _channel.QueueDeclare(queue: "user_create_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: "user_update_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: "user_delete_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            //
            _channel.QueueDeclare(queue: "redis_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: "redis_queue", exchange: "gps_exchange", routingKey: "");
            _channel.QueueDeclare(queue: "gps3_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: "gps3_queue", exchange: "gps_exchange", routingKey: "");
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

            Connection(factory);
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
            const int maxReconnectAttempts = 10;

            for (int attempt = 0; attempt < maxReconnectAttempts; attempt++)
            {
                // Thử tái kết nối nếu kết nối đang đóng
                if (_connection == null || !_connection.IsOpen)
                {
                    Console.WriteLine("Dang co gang ket noi lai...");

                    try
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

                        Connection(factory);

                        Console.WriteLine("ket noi thanh cong!");
                        return;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Ket noi khong thanh cong, dang thu lai:..." + attempt);
                    }
                }
                else
                {
                    Console.WriteLine("Ket noi khong thanh cong, dang thu lai..." + attempt);
                }

                // Đợi 5 giây trước khi thử lại
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
    }
}
