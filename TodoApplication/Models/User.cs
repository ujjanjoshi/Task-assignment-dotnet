namespace TodoApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Refreshtoken { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }


    }
}
