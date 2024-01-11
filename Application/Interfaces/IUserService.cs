using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserService
    {
        User GetUserById(int userId);
        int AddUser(string userName);
        User UpdateUser(int userId, string newUserName);
        int DeleteUser(int userId);
        int SendNoticeAll();
        int SendNoticeForGroup();
    }
}
