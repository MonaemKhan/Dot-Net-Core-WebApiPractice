using Microsoft.AspNetCore.Identity;

namespace TestConvertProject.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
