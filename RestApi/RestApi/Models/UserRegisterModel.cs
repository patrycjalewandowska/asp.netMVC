using System.ComponentModel.DataAnnotations;

namespace RestApi.Models
{
    public class UserRegisterModel
    {

        [Key]
        public Guid UserId { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
