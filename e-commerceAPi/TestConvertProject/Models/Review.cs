using TestConvertProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestConvertProject.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string Body { get; set; }
        public int Rating { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
