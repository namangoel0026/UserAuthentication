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
    private readonly IUserService _userService;

    public AuthenticationController(UserContext context, IConfiguration configuration, IUserService userService)
    {
        _context = context;
        _configuration = configuration;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthenticateRequest login)
    {
        var UserModel = await _userService.Login(login.Email);

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
        foreach (string Role in UserModel.UserRoles.Split(','))
        {
            claims.Add(new Claim(ClaimTypes.Role, Role));
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
