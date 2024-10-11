using UserManagement.Models;

namespace UserManagement.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAll();
        Task<Role?> GetById(int id);
        Task<Role?> AddAndUpdateRole(Role RoleObj);
    }
}
