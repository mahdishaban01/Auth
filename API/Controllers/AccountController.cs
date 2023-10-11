using API.Helper;
using Common.DTOs;
using Common.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly Common.Context.AppContext _context;
        private readonly APISetting _aPISetting;
        public AccountController(Common.Context.AppContext context, IOptions<APISetting> options)
        {
            _context = context;
            _aPISetting = options.Value;

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] AuthenticationDTO authenticationDTO)
        {
            var result = _context.User.Any(x => x.UserName == authenticationDTO.UserName && x.Password == authenticationDTO.Password);
            if (result)
            {
                var user = _context.User.SingleOrDefault(x => x.UserName == authenticationDTO.UserName);

                var signinCredentials = GetSigningCredentials();
                var claims = GetClaims(user);

                var tokenOptions = new JwtSecurityToken(
                        issuer: _aPISetting.ValidIssuer,
                        audience: _aPISetting.ValidAudience,
                        claims: claims,
                        expires: DateTime.Now.AddDays(30),
                        signingCredentials: signinCredentials);

                var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return Ok(new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = true,
                    Token = token,
                    User = new User
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                    }
                });
            }
            else
            {
                return Unauthorized(new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Invalid Authentication"
                });
            }
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_aPISetting.SecretKey));
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim("Id",user.Id.ToString()),
            };

            return claims;
        }
    }
}
