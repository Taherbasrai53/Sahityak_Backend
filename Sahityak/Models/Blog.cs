using System.ComponentModel.DataAnnotations;

namespace Sahityak.Models
{
    public class Blog
    {
        public int Id { get; set; }        
        public int UserId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        public string Thumbnail { get; set; }
        
    }
}
