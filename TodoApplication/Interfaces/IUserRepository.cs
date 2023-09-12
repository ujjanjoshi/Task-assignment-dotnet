using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TodoApplication.DTO;
using TodoApplication.Models;

namespace TodoApplication.Interfaces
{
    public interface IUserRepository
    {
        bool CreateUsers(User user);
        bool Save();
        ICollection<User> GetAllUsers();
        User loginUser(LoginDTO login);
        public string GetToken(User user, TimeSpan tokenExpiration);
        public string RefreshToken(string refreshToken, TimeSpan tokenExpiration);
        public bool StoreToken(int id,string refreshToken);
        public string viewRefreshToken(int id);
        public string logout(string idtoken);


    }
}
