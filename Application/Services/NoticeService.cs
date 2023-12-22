using Application.Interfaces;
using Application.RabbitMQ;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NoticeService : BackgroundService
    {
        private readonly IRabbitMQConsumer _rabbitMQConsumer;

        public NoticeService(IRabbitMQConsumer rabbitMQConsumer)
        {
            _rabbitMQConsumer = rabbitMQConsumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_rabbitMQConsumer.ConsumeMessages("user_created_queue", message =>
            //{
            //    // Xử lý message ở đây
            //    Console.WriteLine($"Service received message: {message}");
            //});

            return Task.CompletedTask;
        }
    }
}
