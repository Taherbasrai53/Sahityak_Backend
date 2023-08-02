using System.ComponentModel.DataAnnotations;

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
        public string Mob_Num { get; set; }
        public string? Whatsapp_Num { get; set; }
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
}
