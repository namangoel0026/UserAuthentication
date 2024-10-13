using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.Models;
using UserManagement.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserContext _context;
    private readonly IConfiguration _configuration;

    public AuthenticationController(UserContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthenticateRequest login)
    {
        var UserModel = await _context.Users.Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Email == login.Email);

        if (UserModel == null || !EncryptorDecryptor.VerifyPassword( UserModel.Password, login.Password))
        {
            return Unauthorized();
        }

        var token = GenerateJwtToken(UserModel);
        return Ok(new AuthenticateResponse(UserModel, await token));
    }

    private async Task<string> GenerateJwtToken(UserModel UserModel)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sid, UserModel.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        foreach (var RoleModel in UserModel.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, RoleModel.RoleId.ToString()));
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"])),
                signingCredentials: creds);
        
            return new JwtSecurityTokenHandler().WriteToken(token);
        
    }
}
