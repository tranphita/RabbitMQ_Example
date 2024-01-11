using Application.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Application.Services
{
    /// <summary>
    /// Sử dụng Worker Service (dịch vụ chạy nền) để chạy liên tục khi ứng dụng hoạt động.
    /// Cung cấp một service nhận tin nhắn và xử lý thông báo
    /// </summary>
    public class NoticeService : BackgroundService
    {
        private readonly IRabbitMQConsumer _rabbitMQConsumer;

        public NoticeService(IRabbitMQConsumer rabbitMQConsumer)
        {
            _rabbitMQConsumer = rabbitMQConsumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Duyệt qua danh sách các tenant
            List<string> tenants = new List<string> { "tenant1", "tenant2", "tenant3", "tenant4", "tenant5" };

            foreach (var tenantId in tenants)
            {
                string queueName = $"{tenantId}_queue";

                _rabbitMQConsumer.ConsumeMessages(queueName, message =>
                {
                    // Xử lý message ở đây
                    Console.WriteLine($"{tenantId} da nhan duoc tin nhan: {message}");
                });
            }

            return Task.CompletedTask;
        }
    }
}
