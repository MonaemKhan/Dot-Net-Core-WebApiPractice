using TestConvertProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestConvertProject.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [ForeignKey("Seller")]
        public string UserId { get; set; }
        public User Seller { get; set; }
    }
}
