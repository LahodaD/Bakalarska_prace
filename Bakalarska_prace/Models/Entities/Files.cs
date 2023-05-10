using Bakalarska_prace.Domain.Implementation.Validation;
using Bakalarska_prace.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bakalarska_prace.Models.Entities
{
    [Table(nameof(Files))]
    public class Files
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string NameFile { get; set; }

        [Required]
        [StringLength(255)]
        public string Path { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        [NotMapped]
        [FileValidation("application/vnd.openxmlformats-officedocument.wordprocessingml.document" +
            ", application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" +
            ", application/pdf" +
            ", application/msword")]
        public virtual IFormFile _File { get; set; }

    }
}
