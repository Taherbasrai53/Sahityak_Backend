using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sahityak.Models
{
    public class User
    {
        public int UserId { get; set; }        
        public int? SahityakId { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public string Institution { get; set; }
        public int? CollegeId { get; set; }
        [Required]
        public string MobNo { get; set; }
        public string? WhatsappNo { get; set; }
        [Required]
        public string EmailId { get; set; }
        public EUserType UserType { get; set; } = EUserType.User;
        [Required]
        public string Password { get; set; }


        public enum EUserType
        {
            User=0,
            MedicapsStudent=1,
            SahityakMember=2,
            Admin=3,
        }

    }

    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public User user { get; set; }

        public LoginResponse(string token, User user)
        {
            this.Token = token;
            this.user = user;
        }
    }

    public class UserResponse
    {
        public int UserId { get; set; }
        public int? SahityakId { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public string Institution { get; set; }
        public int? CollegeId { get; set; }
        [Required]
        public string MobNo { get; set; }
        public string? WhatsappNo { get; set; }
        [Required]
        public string EmailId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int BlogsPosted { get; set; } = 0;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Blog>? Blogs { get; set; }

    }
}
