using Application.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Application.Services
{
    /// <summary>
    /// Sử dụng Worker Service (dịch vụ chạy nền) để chạy liên tục khi ứng dụng hoạt động.
    /// Cung cấp một service nhận tin nhắn và xử lý thông báo
    /// </summary>
    public class Notice2Service : BackgroundService
    {
        private readonly IRabbitMQConsumer _rabbitMQConsumer;

        public Notice2Service(IRabbitMQConsumer rabbitMQConsumer)
        {
            _rabbitMQConsumer = rabbitMQConsumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMQConsumer.ConsumeMessages("user_create_queue", message =>
            {
                Console.WriteLine($"Service Notice 2 da nhan duoc tin nhan: {message}");
            });

            _rabbitMQConsumer.ConsumeMessages("user_update_queue", message =>
            {
                Console.WriteLine($"Service Notice 2 da nhan duoc tin nhan: {message}");
            });

            _rabbitMQConsumer.ConsumeMessages("user_delete_queue", message =>
            {
                Console.WriteLine($"Service Notice 2 da nhan duoc tin nhan: {message}");
            });

            return Task.CompletedTask;
        }
    }
}
