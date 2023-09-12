using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;
using TodoApplication.Data;
using TodoApplication.DTO;
using TodoApplication.Interfaces;
using TodoApplication.Models;

namespace TodoApplication.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _datacontext;
        IConfiguration _configuration;
        public UserRepository(DataContext datacontext, IConfiguration configuration)
        {
            _datacontext = datacontext;
            _configuration = configuration;
        }
        public bool CreateUsers(User user)
        {
            _datacontext.Add(user);
            return Save();
        }

        public ICollection<User> GetAllUsers()
        {
            return _datacontext.Users.OrderBy(p => p.Id).AsNoTracking().ToList();
        }

        public string GetToken(User user, TimeSpan tokenExpiration)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserName", user.Name),
                new Claim("UserEmail", user.Email),

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.Add(tokenExpiration),
                signingCredentials:signIn
                );
            string Token = new JwtSecurityTokenHandler().WriteToken(token);
            return Token;
        }

        public User loginUser(LoginDTO login)
        {
            return _datacontext.Users.Where(p => (p.Email == login.Email) && (p.Password == login.Password)).FirstOrDefault();
        }

        public string logout(string idtoken)
        {
            var token = new JwtSecurityToken(jwtEncodedString: idtoken);
            string id = token.Claims.First(c => c.Type == "UserId").Value;
            _datacontext.Users.Where(u => u.Id == int.Parse(id)).ExecuteUpdate(b => b.SetProperty(u => u.Refreshtoken, ""));
            return "logoutSucessfully";
        }

        public string RefreshToken(string refreshToken, TimeSpan tokenExpiration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key =Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out validatedToken);

            if (validatedToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var username = principal.Identity.Name;

            var user=GetAllUsers().Where(c => c.Email.Trim().ToUpper() ==username.Trim().ToUpper()).FirstOrDefault();
            return GetToken(user, tokenExpiration);
        }

        public bool Save()
        {
           var saved= _datacontext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool StoreToken(int id,string refreshToken)
        {
            _datacontext.Users.Where(u=>u.Id==id).ExecuteUpdate(b=>b.SetProperty(u=>u.Refreshtoken, refreshToken));
            return Save();
        }

        public string viewRefreshToken(int id)
        {
           var users= _datacontext.Users.Where(u => u.Id == id).FirstOrDefault();
            return users.Refreshtoken;
        }
    }
}
