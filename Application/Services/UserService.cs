using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly SampleDbContext _dbContext;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;

        public UserService(SampleDbContext dbContext, IRabbitMQPublisher rabbitMQPublisher)
        {
            _dbContext = dbContext;
            _rabbitMQPublisher = rabbitMQPublisher;
        }


        public User GetUserById(int userId)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        }

        public int AddUser(string userName)
        {
            // Thêm mới người dùng
            var newUser = new User { Name = userName };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            // Xử lý gửi Message vào queue

            // 1. Chuẩn bị thông điệp
            string message = $"User moi duoc tao: {userName}";

            // 2. Tên queue để xuất bản tin nhắn
            string routingKey = "user_create_queue";

            // 3. Gọi phương thức để xuất bản tin nhắn
            _rabbitMQPublisher.PublishMessage("", routingKey, message);

            return newUser.Id;
        }

        public User UpdateUser(int userId, string newUserName)
        {
            var existingUser = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            if (existingUser != null)
            {
                existingUser.Name = newUserName;
                _dbContext.SaveChanges();

                // Xử lý gửi Message vào queue

                // 1. Chuẩn bị thông điệp
                string message = $"Cap nhat user: {newUserName}";

                // 2. Tên queue để xuất bản tin nhắn
                string routingKey = "user_update_queue";

                // 3. Gọi phương thức để xuất bản tin nhắn
                _rabbitMQPublisher.PublishMessage("", routingKey, message);
            }

            return existingUser;
        }

        public int DeleteUser(int userId)
        {
            var userToRemove = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            if (userToRemove != null)
            {
                _dbContext.Users.Remove(userToRemove);
                _dbContext.SaveChanges();

                // Xử lý gửi Message vào queue

                // 1. Chuẩn bị thông điệp
                string message = $"Xoa user: {userToRemove.Name}";

                // 2. Tên queue để xuất bản tin nhắn
                string routingKey = "user_delete_queue";

                // 3. Gọi phương thức để xuất bản tin nhắn
                _rabbitMQPublisher.PublishMessage("", routingKey, message);
            }

            return 0;
        }


        public int SendNoticeAll()
        {
            // Xử lý gửi message vào queue

            // 1. Chuẩn bị thông điệp
            string message = "Thong bao bao tri he thong! vào ngày 2023-12-28";
            string routingKey = "";

            // 2. Gọi phương thức để xuất bản tin nhắn
            _rabbitMQPublisher.PublishMessage("fanout_exchange", routingKey, message);

            return 0;
        }

        public int SendNoticeForGroup()
        {
            // Xử lý gửi message vào queue

            // 1. Chuẩn bị thông điệp
            string message = "Thong bao het han su dung ban Pro!";
            string routingKey = "premium";

            // 2. Gọi phương thức để xuất bản tin nhắn
            _rabbitMQPublisher.PublishMessage("direct_exchange", routingKey, message);

            return 0;
        }
    }
}
