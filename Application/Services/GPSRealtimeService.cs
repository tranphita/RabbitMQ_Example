using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Application.Services
{
    /// <summary>
    /// Sử dụng Worker Service (dịch vụ chạy nền) để chạy liên tục khi ứng dụng hoạt động.
    /// Cung cấp một service nhận tin nhắn và xử lý thông báo
    /// </summary>
    public class GPSRealtimeService : BackgroundService
    {
        private readonly IRabbitMQConsumer _rabbitMQConsumer;

        private async Task ProcessAndSaveToRedisAsync(string message)
        {
            // Process the message (e.g., deserialize JSON)
            var data = JsonConvert.DeserializeObject<GPS3Dto>(message);

            // Save to Redis
            await SaveToRedisAsync(data);
        }

        private async Task SaveToRedisAsync(GPS3Dto data)
        {
            // Đọc cấu hình từ appsettings.json
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Lấy thông tin kết nối Redis từ cấu hình
            string redisConnectionString = configuration.GetSection("Redis:ConnectionString").Value;
            string redisPassword = configuration.GetSection("Redis:Password").Value;

            // Tạo đối tượng ConnectionMultiplexer để quản lý kết nối
            ConfigurationOptions options = ConfigurationOptions.Parse(redisConnectionString);
            options.Password = redisPassword;

            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(options);

            var database = connectionMultiplexer.GetDatabase();

            // Assuming data is a string; you may need to serialize it accordingly
            await database.StringSetAsync("redis-key-sample", JsonConvert.SerializeObject(data));

            // Check log data
            var keys = connectionMultiplexer.GetServer("localhost:6379").Keys();
            foreach (var key in keys)
            {
                var type = database.KeyType(key);
                if (type == RedisType.String)
                {
                    Console.WriteLine($"Redis data: {database.StringGet(key)}");
                }
            }
        }

        public GPSRealtimeService(IRabbitMQConsumer rabbitMQConsumer)
        {
            _rabbitMQConsumer = rabbitMQConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _rabbitMQConsumer.ConsumeMessages("redis_queue", async message =>
                {
                    await ProcessAndSaveToRedisAsync(message);
                });

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
