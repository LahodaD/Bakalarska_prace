using System.ComponentModel.DataAnnotations;

namespace Bakalarska_prace.Models.Entities
{
    public class Customers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }
        

    }
}
