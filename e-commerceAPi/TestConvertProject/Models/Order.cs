using TestConvertProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestConvertProject.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
    }
}

