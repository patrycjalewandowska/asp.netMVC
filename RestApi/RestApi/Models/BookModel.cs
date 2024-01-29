using System.ComponentModel.DataAnnotations;

namespace RestApi.Models
{
    public class BookModel
    {
        [Key]
        public Guid BookId { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? User { get; set; }


        //public string? Category {get; set; }

    }
}
