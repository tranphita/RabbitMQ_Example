using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.Services
{
    /// <summary>
    /// Sử dụng Worker Service (dịch vụ chạy nền) để chạy liên tục khi ứng dụng hoạt động.
    /// Cung cấp một service nhận tin nhắn và xử lý thông báo
    /// </summary>
    public class GPSCollectorsService : BackgroundService
    {
        private readonly IRabbitMQPublisher _rabbitMQPublisher;
        private int _total = 1;
        private double _lat = 12.7126732;
        private double _lng = 108.056328;

        public GPSCollectorsService(IRabbitMQPublisher rabbitMQPublisher)
        {
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Tạo một mảng JObject[]
                List<GpsPositionQueueModel> jsonDataArray = new List<GpsPositionQueueModel>();
                GpsPositionQueueModel jsonData = new GpsPositionQueueModel();

                // Thêm các thuộc tính vào đối tượng JObject
                jsonData.Id = "12396";
                jsonData.Plate = "60H12396";
                jsonData.Lat = _lat;
                jsonData.Lng = _lng;
                jsonData.Speed = 43.0;
                jsonData.Km = 57.784;
                jsonData.TimeUpdate = DateTime.Now;

                jsonDataArray.Add(jsonData);

                // 1. Chuẩn bị thông điệp
                var gpsInfo = new GPS3Dto
                {
                    VehicleCode = "60H12396",
                    Date = 2023122712,
                    CreatedTime = DateTime.Now,
                    Total = _total,
                    Detail = jsonDataArray,
                };

                string message = JsonConvert.SerializeObject(gpsInfo);
                string routingKey = "";

                // Gọi phương thức để xuất bản tin nhắn
                _rabbitMQPublisher.PublishMessage("gps_exchange", routingKey, message);

                _lat += 0.0009;
                _lng += 0.0009;
                _total = _total + 1;

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
