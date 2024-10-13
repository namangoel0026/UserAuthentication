using UserManagement.Models;

namespace UserManagement.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleModel>> GetAll();
        Task<RoleModel?> GetById(int id);
        Task<RoleModel> CreateRoleAsync(RoleModel role);
        Task<RoleModel> UpdateRoleAsync(RoleModel role);
        Task<bool> DeleteRoleAsync(int roleid);
    }
}
