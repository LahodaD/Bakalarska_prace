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
        public User User { get; set; }

        [ForeignKey(nameof(Customers))]
        public int Customers_id { get; set; }
        public Customers Customers { get; set; }

        [ForeignKey(nameof(Cars))]
        public int Cars_id { get; set; }
        public Cars Cars { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Sale_date { get; protected set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

    }
}
