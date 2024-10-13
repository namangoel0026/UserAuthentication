using UserManagement.DTO;
using UserManagement.Models;

namespace UserManagement.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAll();
        Task<UserDTO?> GetById(int id);
        public Task<UserDTO> CreateUserWithRolesAsync(UserModel user);
        public Task<UserDTO> UpdateUserAsync(UserRequest user);
        Task<bool> DeleteUserAsync(int id);
        public Task<UserModel> Login(string email);
    }
}
