using UserManagement.Models;

namespace UserManagement.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetAll();
        Task<UserModel?> GetById(int id);
        Task<UserModel?> AddAndUpdateUser(UserModel userObj);
    }
}
