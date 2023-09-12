using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApplication.DTO;
using TodoApplication.Interfaces;
using TodoApplication.Models;
using TodoApplication.Repository;

namespace TodoApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult CreateTodo([FromBody] User usercreate)
        {
            if (usercreate == null)
                return BadRequest(ModelState);

            var user = _userRepository.GetAllUsers().
                Where(c => c.Email.Trim().ToUpper() == usercreate.Email.Trim().ToUpper()).FirstOrDefault();

            if (user != null)
            {
                ModelState.AddModelError("", "User Already Exists");
                return StatusCode(442, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_userRepository.CreateUsers(usercreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Created");
        }

        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(LoginDTO))]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            var user = _userRepository.GetAllUsers().
               Where(c => c.Email.Trim().ToUpper() == loginDTO.Email.Trim().ToUpper()).FirstOrDefault();

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Credentials ");
                return StatusCode(442, ModelState);
            }
            var users = _userRepository.loginUser(loginDTO);
            var usersdata =
           new
           {
               id = users.Id,
               name = users.Name,
               email = users.Email
           };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            TimeSpan time = new TimeSpan(0, 0, 30);
            var token = _userRepository.GetToken(user, time);
            TimeSpan tokenExpiration = new TimeSpan(10, 10, 0);
            var refreshtoken = _userRepository.RefreshToken(token, tokenExpiration);
            _userRepository.StoreToken(users.Id, refreshtoken);
            var data = new {
                user = usersdata,
                status = "Login Sucess",
                AccessToken = token,
            };

            return Ok(data);
        }

        [HttpGet("refreshtokencheck/{id}")]
        [ProducesResponseType(200)]
        public IActionResult refreshtoken(int id)
        {
            var token = _userRepository.viewRefreshToken(id);
            return Ok(token);
        }

        [HttpGet("logout")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult logout([FromHeader] string Authorization)
        {
            var authorization = Authorization.Split(' ');
            var id = String.Join(" ", authorization.Skip(1));
            var token = _userRepository.logout(id);
            return Ok("Logout");
        }
    }
}
