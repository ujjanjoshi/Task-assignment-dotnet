using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TodoApplication.Interfaces;
using TodoApplication.Models;

namespace TodoApplication.Controllers
{
    
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TodoController: Controller
    {
        private readonly ITodoRepository _todoRepository;

        public TodoController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Todo>))]
        public IActionResult GetCategory()
        {
            var todos = _todoRepository.GetAllTodos();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(todos);
        }
        [HttpGet("getbyId/{id}")]
        [ProducesResponseType(200, Type = typeof(Todo))]
        public IActionResult GetCategory(int id)
        {
            if (!_todoRepository.TodoExists(id))
                return NotFound();
            var todo = _todoRepository.GetTodoById(id);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(todo);
        }
        [HttpGet("getbyName/{title}")]
        [ProducesResponseType(200, Type = typeof(Todo))]
        public IActionResult GetCategory(string title)
        {
            var todo = _todoRepository.GetTodoByTitle(title);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(todo);
        }
        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult CreateTodo([FromBody] Todo todocreate, [FromHeader] string Authorization)
        {
            if (todocreate == null)
                return BadRequest(ModelState);

            var todo = _todoRepository.GetAllTodos().
                Where(c => c.Title.Trim().ToUpper() == todocreate.Title.Trim().ToUpper()).FirstOrDefault();

            if (todo != null)
            {
                ModelState.AddModelError("", "Todo Already Exists");
                return StatusCode(442, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var authorization = Authorization.Split(' ');
            var token= String.Join(" ", authorization.Skip(1));
            var tokens = new JwtSecurityToken(jwtEncodedString: token);
            string id = tokens.Claims.First(c => c.Type == "UserId").Value;
            todocreate.CreatedBy = int.Parse(id);
            if (!_todoRepository.CreateTodos(todocreate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Sucessfully Created");
        }

        [HttpPut("{todoId}")]
        [ProducesResponseType(200)]
        public IActionResult UpdateTodo(int todoId,[FromBody] Todo todoupdate)
        {
            if (todoupdate == null)
                return BadRequest(ModelState);

           if(todoId !=todoupdate.Id)
                return BadRequest(ModelState);

            if (!_todoRepository.TodoExists(todoId))
                return NotFound();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_todoRepository.UpdateTodos(todoupdate))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Updated");
        }

        [HttpPut]
        [ProducesResponseType(200)]
        public IActionResult UpdateTodoStatus([FromQuery]int todoId, [FromQuery] string status)
        {

            if (!_todoRepository.TodoExists(todoId))
                return NotFound();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_todoRepository.updateStatus(todoId,status))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Updated status");
        }

        [HttpDelete("{todoId}")]
        [ProducesResponseType(200)]
        public IActionResult DeleteTodo(int todoId)
        {

            if (!_todoRepository.TodoExists(todoId))
                return NotFound();

            var todoDelete = _todoRepository.GetTodoById(todoId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_todoRepository.DeleteTodos(todoDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Deleted");
        }
    }
}
