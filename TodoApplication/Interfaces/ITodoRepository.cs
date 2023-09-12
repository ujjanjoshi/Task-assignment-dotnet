using TodoApplication.Models;

namespace TodoApplication.Interfaces
{
    public interface ITodoRepository
    {
        bool CreateTodos(Todo todo);
        bool UpdateTodos(Todo todo);
        bool Save();
        ICollection<Todo> GetAllTodos();
        Todo GetTodoById(int Id);
        Todo GetTodoByTitle(string Title);
        bool TodoExists(int Id);
        bool DeleteTodos(Todo todo);
        bool updateStatus(int Id, string Status);



    }
}
