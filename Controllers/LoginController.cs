using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using newapi.Context;
using newapi.models;
using newapi.services;
using newapi.Services;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace newapi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : Controller
    {
        private readonly UserContext _context;
        // private readonly IConfiguration _config;
        private readonly IAuthService _iAuthService;
        private readonly IConfiguration _config;
        private readonly IJwtService _jwtService;
        public LoginController(UserContext context, 
                               IAuthService iAuthService, 
                               IConfiguration config,
                               IJwtService jwtService) 
        {
            _context = context;
            _iAuthService = iAuthService;
            _config = config;
            _jwtService = jwtService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<User>>> Users()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>?> login(LoginModel loginModel)
        {   
            var user = await _iAuthService.Login(loginModel.Email, loginModel.Password);
            if (user == null)
            {
                return BadRequest("[ Wrong login data ]");
            }
            var token = _jwtService.GenerateToken(user);
            return Ok(new {token = token});
        }


        [HttpPost("register")]
        public async Task<ActionResult<User>> register(RegisterModel registerModel)
        {
            var user = await _iAuthService.Register(registerModel);
            return Ok(user);
        }
    }

    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
    public class RegisterModel
    {

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
