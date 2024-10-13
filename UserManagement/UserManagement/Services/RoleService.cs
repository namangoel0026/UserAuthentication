using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.DTO;
using UserManagement.Models;
namespace UserManagement.Services
{
    public class RoleService : IRoleService
    {
        private readonly IConfiguration _configuration;
        private readonly UserContext db;
        private readonly IDbConnection _dbConnection;

        public RoleService(IConfiguration configuration, UserContext _db,IDbConnection dbConnection)
        {
            _configuration = configuration;
            db = _db;
            _dbConnection = dbConnection;
        }

       public async Task<IEnumerable<RoleModel>> GetAll()
        {
            const string sql = @"        
            SELECT *
            FROM Roles r 
            where r.IsActive=1";

            return await _dbConnection.QueryAsync<RoleModel>(sql);
        }
        public async Task<RoleModel?> GetById(int id)
        {
            const string sql = @"        
            SELECT *
            FROM Roles r 
            where r.Id=@id and r.IsActive=1";
            return _dbConnection.QueryFirstOrDefault<RoleModel>(sql, new { id = id });
        }
        public async Task<RoleModel> CreateRoleAsync(RoleModel role)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open(); // Open the connection if it's not already open
            }
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var insertUserSql = @"
                    INSERT INTO Roles (Name, Description, isActive)
                    VALUES (@Name, @Description, 1);
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                    var roleId = await _dbConnection.ExecuteScalarAsync<int>(insertUserSql,
                        new { Name = role.Name, Description = role.Description }, transaction);
                    transaction.Commit();
                    return new RoleModel
                    {
                        Id = roleId,
                        Name = role.Name,
                        Description = role.Description,
                        IsActive = role.IsActive
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<RoleModel> UpdateRoleAsync(RoleModel role)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var updateUserSql = @"
                    UPDATE Roles
                    SET Name = @Name, Description = @Description
                    WHERE Id = @Id;";
                    await _dbConnection.ExecuteAsync(updateUserSql,
                        new { Name = role.Name, Description = role.Description, Id = role.Id }, transaction);
                    transaction.Commit();
                    return new RoleModel
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Description = role.Description,
                        IsActive =role.IsActive
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<bool> DeleteRoleAsync(int roleid)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var deleteUserSql = @"
                    UPDATE Roles
                    SET IsActive=0 
                    WHERE Id = @Id;";
                    var rowsAffected = await _dbConnection.ExecuteAsync(deleteUserSql,
                        new { Id = roleid }, transaction);
                    if (rowsAffected == 0)
                    {
                        throw new Exception("role not found.");
                    }
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
