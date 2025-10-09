using TestConvertProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestConvertProject.Models
{
    public class ProductOrder
    {
        public int ProductOrderId { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}