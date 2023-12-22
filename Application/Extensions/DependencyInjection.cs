using Application.Interfaces;
using Application.RabbitMQ;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            // Đăng ký các service user
            services.AddScoped<IUserService, UserService>();

            // đăng ký các service của rabbitmq
            services.AddSingleton<IRabbitMQConnectionManager>(sp =>
            {
                string hostName = "localhost";
                int port = 5672;
                string userName = "guest";
                string password = "guest";

                return new RabbitMQConnectionManager(hostName, port, userName, password);
            });

            services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();
            services.AddSingleton<IRabbitMQConsumer, RabbitMQConsumer>();

            // Đăng ký Service Background notice
            services.AddHostedService<NoticeService>();

            return services;
        }
    }
}
