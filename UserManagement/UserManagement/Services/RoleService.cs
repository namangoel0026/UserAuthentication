using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.Models;
namespace UserManagement.Services
{
    public class RoleService : IRoleService
    {
        private readonly IConfiguration _configuration;
        private readonly UserContext db;

        public RoleService(IConfiguration configuration, UserContext _db)
        {
            _configuration = configuration;
            db = _db;
        }

       public async Task<IEnumerable<Role>> GetAll()
        {
            return await db.Roles.Where(x => x.isActive == true).ToListAsync();
        }

        public async Task<Role?> GetById(int id)
        {
            return await db.Roles.FirstOrDefaultAsync(x => x.RoleId == id);
        }

        public async Task<Role?> AddAndUpdateRole(Role RoleObj)
        {
            bool isSuccess = false;
            if (RoleObj.RoleId > 0)
            {
                var obj = await db.Roles.FirstOrDefaultAsync(c => c.RoleId == RoleObj.RoleId);
                if (obj != null)
                {
                    // obj.Address = RoleObj.Address;
                    obj.Name = RoleObj.Name;
                    db.Roles.Update(obj);
                    isSuccess = await db.SaveChangesAsync() > 0;
                }
            }
            else
            {
                await db.Roles.AddAsync(RoleObj);
                isSuccess = await db.SaveChangesAsync() > 0;
            }

            return isSuccess ? RoleObj : null;
        }
    }
}
