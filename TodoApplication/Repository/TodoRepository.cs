using TodoApplication.Data;
using TodoApplication.Interfaces;
using TodoApplication.Models;

namespace TodoApplication.Repository
{
    public class TodoRepository : ITodoRepository
    {
        private readonly DataContext _datacontext;

        public TodoRepository(DataContext datacontext)
        {
            _datacontext = datacontext;
        }
        public bool CreateTodos(Todo todo)
        {
           _datacontext.Add(todo);
            return Save();
        }

        public bool DeleteTodos(Todo todo)
        {
            _datacontext.Remove(todo);
            return Save();
        }

        public ICollection<Todo> GetAllTodos()
        {
           return _datacontext.Todos.OrderBy(p=>p.Id).ToList();
        }

        public Todo GetTodoById(int Id)
        {
           return _datacontext.Todos.Where(p=>p.Id==Id).FirstOrDefault();
        }

        public Todo GetTodoByTitle(string Title)
        {
            return _datacontext.Todos.Where(p=>p.Title==Title).FirstOrDefault();
        }

        public bool Save()
        {
            var saved= _datacontext.SaveChanges();
            return saved>0 ? true:false;
        }

        public bool TodoExists(int Id)
        {
           return _datacontext.Todos.Any(P=>P.Id==Id);
        }

        public bool updateStatus(int Id, string Status)
        {
           var todo= new Todo()
           {
               Id=Id,
               Status=Status,
           };
            _datacontext.Attach(todo).Property(p=>p.Status).IsModified=true;
            return Save();
        }

        public bool UpdateTodos(Todo todo)
        {
            
           _datacontext.Update(todo);
            return Save();
        }


    }
}
