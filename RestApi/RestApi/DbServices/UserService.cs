using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestApi.DbServices
{
    public class UserService : IUserService
    {/*
        private readonly AppDbContext db;

        public UserService(AppDbContext db)
        {
            this.db = db;
        }
*/
        private readonly AppDbContext db;
        private readonly IConfiguration configuration;

        public UserService(AppDbContext db, IConfiguration configuration)
        {
            this.db = db;
            this.configuration = configuration;
        }



        public async Task<UserRegisterModel> AddUser(UserRegisterModel user)
        {
            if (await db.Users.AnyAsync(u => u.Username == user.Username))
            {
                throw new Exception("Username is already taken.");
            }

            if (await db.Users.AnyAsync(u => u.Email == user.Email))
            {
                throw new Exception("Email is already taken.");
            }

            var newUser = new UserRegisterModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            };

            await db.Users.AddAsync(newUser);
            await db.SaveChangesAsync();

            return newUser;
        }


        public async Task<ActionResult<string>> LoginUser(UserLoginModel user)
        {
            var userDb = await db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);

            if (userDb == null) {

                throw new Exception("User does not exist");

            }
            else if (BCrypt.Net.BCrypt.Verify(user.Password, userDb.Password))
            {
                string token = CreateToken(user);
                Console.WriteLine(token);
                return token;
            }
            else
            {
                throw new Exception("Password or username is not correct.");
            }

            
        }

        public string CreateToken(UserLoginModel user)
        {
            List<Claim> claims = new List<Claim>
             {
                new Claim(ClaimTypes.Name, user.Username)
             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(

                configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }
    }
}