using System.ComponentModel.DataAnnotations;

namespace Sahityak.Models
{
    public class BlogComment
    {
        public int Id { get; set; }
        [Required]
        public int BlogId { get; set; }    
        public int UserId { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime InsertedAt { get; set; }
    }
}
