using System.ComponentModel.DataAnnotations;
using TestConvertProject.Models;

namespace TestConvertProject.DataTransferObjects
{
    public class UserForRegistrationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        public User getDto()
        {
            return new User
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                UserName = this.UserName,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber
            };
        }
    }
}
