using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using newapi.Context;
using newapi.Controllers;
using newapi.models;
using System.Data.SqlTypes;
using System.Security.Cryptography;

namespace newapi.services
{

    public interface IAuthService
    {
        Task<User?> Login(string email, string password);
        Task<User> Register(RegisterModel registerModel);
    };
    public class AuthService : IAuthService
    {
        private readonly UserContext _context;
        public AuthService(UserContext context) 
        { 
            _context  = context;
        }
        private static string HashPassword(string password,string salts)
        {
            byte[] salt = Convert.FromBase64String(salts);
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        public async Task<User?> Login(string email, string password)
        {
           User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
           
            if (user == null){
                return null;
            } 

           String hashedPassword = HashPassword(password, user.Salt);
           if(user.Password == hashedPassword)
            {
                return user;
            }
            return null;
                       
        }
      
        public async Task<User> Register(RegisterModel registerModel)
        {

            byte[] bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            var salt = Convert.ToBase64String(bytes);

            var user = new User
            {
                Email = registerModel.Email,
                Password =  HashPassword(registerModel.Password,salt),
                Salt = salt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
