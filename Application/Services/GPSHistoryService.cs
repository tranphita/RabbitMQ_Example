using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;

namespace Application.Services
{
    /// <summary>
    /// Sử dụng Worker Service (dịch vụ chạy nền) để chạy liên tục khi ứng dụng hoạt động.
    /// Cung cấp một service nhận tin nhắn và xử lý thông báo
    /// </summary>
    public class GPSHistoryService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRabbitMQConsumer _rabbitMQConsumer;

        public GPSHistoryService(IServiceScopeFactory serviceScopeFactory, IRabbitMQConsumer rabbitMQConsumer)
        {
            _rabbitMQConsumer = rabbitMQConsumer;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _rabbitMQConsumer.ConsumeMessages("gps3_queue", async message =>
                {
                    // Xử lý message thành đối tượng GPS3
                    GPS3Dto data = JsonConvert.DeserializeObject<GPS3Dto>(message);

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _dbContext = scope.ServiceProvider.GetRequiredService<SampleDbContext>();

                        var existingGPS3 = await _dbContext.GPS3s.OrderByDescending(c => c.CreatedTime).FirstOrDefaultAsync(u => u.VehicleCode == data.VehicleCode);

                        if (existingGPS3 != null)
                        {
                            TimeSpan difference = data.CreatedTime - existingGPS3.CreatedTime;

                            if (difference.TotalMinutes <= 2)
                            {
                                existingGPS3.Total = data.Total;

                                // Deserialize chuỗi JSON thành một đối tượng List<dynamic>
                                var details = JsonConvert.DeserializeObject<List<dynamic>>(existingGPS3.Detail);

                                // Thêm chuỗi json mới từ queue vào danh sách
                                details.Add(data.Detail.FirstOrDefault());

                                // Serialize lại danh sách thành chuỗi JSON và cập nhật trường Detail
                                existingGPS3.Detail = JsonConvert.SerializeObject(details);
                            }
                            else
                            {
                                var entity = new GPS3();
                                entity.Date = data.Date;
                                entity.VehicleCode = data.VehicleCode;
                                entity.Total = 1;
                                entity.Detail = JsonConvert.SerializeObject(data.Detail);
                                entity.CreatedTime = data.CreatedTime;
                                _dbContext.GPS3s.Add(entity);
                            }
                        }
                        else
                        {
                            var entity = new GPS3();
                            entity.Date = data.Date;
                            entity.VehicleCode = data.VehicleCode;
                            entity.Total = 1;
                            entity.Detail = JsonConvert.SerializeObject(data.Detail);
                            entity.CreatedTime = data.CreatedTime;
                            _dbContext.GPS3s.Add(entity);
                        }      
                        await _dbContext.SaveChangesAsync();
                    }
                });

                // Đợi một khoảng thời gian trước khi lặp lại công việc
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
