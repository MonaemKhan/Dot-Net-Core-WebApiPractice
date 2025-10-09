using TestConvertProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestConvertProject.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
