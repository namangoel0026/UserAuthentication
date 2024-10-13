using UserManagement.Models;

namespace UserManagement.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleModel>> GetAll();
        Task<RoleModel?> GetById(int id);
        Task<RoleModel?> AddAndUpdateRole(RoleModel RoleObj);
    }
}
