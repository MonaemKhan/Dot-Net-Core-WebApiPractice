using System.ComponentModel.DataAnnotations;

namespace TestConvertProject.DataTransferObjects
{
    public class UserForAuthenticationDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Password { get; set; }
    }
}
