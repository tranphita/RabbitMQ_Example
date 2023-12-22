using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

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
            string message = $"New user created: {userName}";

            // 2. Tên queue để xuất bản tin nhắn
            string queueName = "user_created_queue";

            // 3. Gọi phương thức để xuất bản tin nhắn
            _rabbitMQPublisher.PublishMessage("", queueName, message);

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
                string message = $"Update user name: {newUserName}";

                // 2. Tên queue để xuất bản tin nhắn
                string queueName = "user_update_queue";

                // 3. Gọi phương thức để xuất bản tin nhắn
                _rabbitMQPublisher.PublishMessage("", queueName, message);
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
                string message = $"Delete user name: {userToRemove.Name}";

                // 2. Tên queue để xuất bản tin nhắn
                string queueName = "user_delete_queue";

                // 3. Gọi phương thức để xuất bản tin nhắn
                _rabbitMQPublisher.PublishMessage("", queueName, message);
            }

            return 0;
        }
    }
}
