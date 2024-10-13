using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.DTO;
using UserManagement.Models;
namespace UserManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly UserContext db;
        private readonly IDbConnection _dbConnection;

        public UserService(IConfiguration configuration, UserContext _db, IDbConnection dbConnection)
        {
            _configuration = configuration;
            db = _db;
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            const string sql = @"        
            SELECT u.Id, u.UserName,u.Email, STRING_AGG(r.Name, ', ') UserRoles
            FROM Users u
            JOIN UserRoleModels ur ON u.Id = ur.UserId
            JOIN Roles r ON ur.RoleId = r.Id
            where u.IsActive = 1 and r.IsActive=1
            GROUP BY u.Id, u.UserName, u.Email";
           
                return await _dbConnection.QueryAsync<UserDTO>(sql);
        }        
        public async Task<UserDTO> GetById(int id)
        {
            const string sql = @"        
            SELECT u.Id, u.UserName,u.Email, STRING_AGG(r.Name, ', ') UserRoles
            FROM Users u
            JOIN UserRoleModels ur ON u.Id = ur.UserId
            JOIN Roles r ON ur.RoleId = r.Id
            where u.IsActive = 1 and u.Id=@id and r.IsActive=1
            GROUP BY u.Id, u.UserName, u.Email";            
            return _dbConnection.QueryFirstOrDefault<UserDTO>(sql, new { id = id });            
        }
        public async Task<UserDTO> CreateUserWithRolesAsync(UserModel user)
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
                    INSERT INTO Users (UserName, Email,Password, isActive)
                    VALUES (@UserName, @Email,@Password, 1);
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                        var userId = await _dbConnection.ExecuteScalarAsync<int>(insertUserSql,
                            new { UserName = user.Username, Password=user.Password,Email = user.Email }, transaction);
                        var roleIds = user.UserRoles.Split(',').Select(int.Parse).ToList();
                        var selectActiveRolesSql = @"
                    SELECT Id FROM Roles
                    WHERE Id IN @RoleIds AND isActive = 1";
                        var activeRoleIds = (await _dbConnection.QueryAsync<int>(selectActiveRolesSql,
                            new { RoleIds = roleIds }, transaction)).ToList();

                        if (!activeRoleIds.Any())
                        {
                            throw new Exception("No active roles found for the provided Role IDs.");
                        }
                        var insertUserRolesSql = @"
                    INSERT INTO UserRoleModels (UserId, RoleId)
                    VALUES (@UserId, @RoleId);";

                        foreach (var roleId in activeRoleIds)
                        {
                            await _dbConnection.ExecuteAsync(insertUserRolesSql,
                                new { UserId = userId, RoleId = roleId }, transaction);
                        }
                        transaction.Commit();
                        return new UserDTO
                        {
                            Id = userId,
                            Username = user.Username,
                            Email = user.Email,
                            UserRoles = string.Join(",", activeRoleIds) 
                        };
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
            }
        }
        public async Task<UserDTO> UpdateUserAsync(UserRequest user)
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
                    UPDATE Users
                    SET UserName = @UserName, Email = @Email
                    WHERE Id = @UserId;";
                    await _dbConnection.ExecuteAsync(updateUserSql,
                        new { UserName = user.Username, Email = user.Email, UserId = user.Id }, transaction);
                    if (!string.IsNullOrEmpty(user.UserRoles))
                    {
                        var roleIds = user.UserRoles.Split(',').Select(int.Parse).ToList();
                        var selectActiveRolesSql = @"
                        SELECT Id FROM Roles
                        WHERE Id IN @RoleIds AND isActive = 1";
                        var activeRoleIds = (await _dbConnection.QueryAsync<int>(selectActiveRolesSql,
                            new { RoleIds = roleIds }, transaction)).ToList();
                        var currentRolesSql = @"
                        SELECT RoleId FROM UserRoleModels
                        WHERE UserId = @UserId";
                        var currentRoleIds = (await _dbConnection.QueryAsync<int>(currentRolesSql,
                            new { UserId = user.Id }, transaction)).ToList();
                        var rolesToRemove = currentRoleIds.Except(activeRoleIds).ToList();
                        if (rolesToRemove.Any())
                        {
                            var deleteRolesSql = @"
                            DELETE FROM UserRoleModels
                            WHERE UserId = @UserId AND RoleId IN @RoleIds";

                            await _dbConnection.ExecuteAsync(deleteRolesSql,
                                new { UserId = user.Id, RoleIds = rolesToRemove }, transaction);
                        }
                        var rolesToAdd = activeRoleIds.Except(currentRoleIds).ToList();
                        foreach (var roleId in rolesToAdd)
                        {
                            var insertUserRoleSql = @"
                            INSERT INTO UserRoleModels (UserId, RoleId)
                            VALUES (@UserId, @RoleId);";
                            await _dbConnection.ExecuteAsync(insertUserRoleSql,
                                new { UserId = user.Id, RoleId = roleId }, transaction);
                        }
                    }
                    transaction.Commit();
                    return new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        UserRoles = user.UserRoles
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<bool> DeleteUserAsync(int userid)
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
                    UPDATE Users
                    SET IsActive=0 
                    WHERE Id = @UserId;";
                    var rowsAffected = await _dbConnection.ExecuteAsync(deleteUserSql,
                        new { UserId = userid }, transaction);
                    if (rowsAffected == 0)
                    {
                        throw new Exception("User not found.");
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
        public async Task<UserModel> Login(string email)
        {
            const string sql = @"        
            SELECT u.Id, u.UserName,u.Email,u.Password, STRING_AGG(r.Id, ', ') UserRoles
            FROM Users u
            JOIN UserRoleModels ur ON u.Id = ur.UserId
            JOIN Roles r ON ur.RoleId = r.Id
            where u.IsActive = 1 and u.email=@email and r.IsActive=1
            GROUP BY u.Id, u.UserName, u.Email,u.Password";

            return _dbConnection.QueryFirstOrDefault<UserModel>(sql, new { email = email });

        }
    }
}
