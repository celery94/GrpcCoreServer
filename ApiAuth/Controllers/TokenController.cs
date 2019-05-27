using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiAuth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public TokenController(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] UserModel userModel)
        {
            var user = await _userManager.FindByEmailAsync(userModel.Email);
            if (user == null) return NotFound();

            if (!await _userManager.CheckPasswordAsync(user, userModel.Password))
                return BadRequest(new {message = "Email or password is incorrect"});

            var role = _db.UserRoles.Where(q => q.UserId == user.Id)
                .SelectMany(q => _db.Roles.Where(x => x.Id == q.RoleId))
                .FirstOrDefault();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("41B71F9E-4204-4E88-8E91-64B1981F1B82");
            var expires = DateTime.UtcNow.AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    //new Claim(ClaimTypes.Role, role.Name)
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                AccessToken = tokenString,
                ExpiresIn = expires
            });
        }
    }

    public class UserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}