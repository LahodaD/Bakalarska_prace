using Bakalarska_prace.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bakalarska_prace.Models.Entities
{
    public class Sales
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey(nameof(User))]
        public int User_id { get; set; }
        
        [ForeignKey(nameof(Customers))]
        public int Customers_id { get; set; }

        [ForeignKey(nameof(Cars))]
        public int Cars_id { get; set; }
        [DataType(DataType.Date)]
        public DateOnly Sale_date { get; set; }
        public decimal Price { get; set; }

    }
}
