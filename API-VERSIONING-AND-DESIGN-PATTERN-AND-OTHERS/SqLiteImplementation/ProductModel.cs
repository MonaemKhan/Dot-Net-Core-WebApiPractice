using System.ComponentModel.DataAnnotations.Schema;

namespace SqLiteImplementation
{
    [Table("Products")]
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
    }
}
