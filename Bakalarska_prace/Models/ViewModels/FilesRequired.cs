using Bakalarska_prace.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Bakalarska_prace.Models.ViewModels
{
    public class FilesRequired : Files
    {
        [Required]
        public override IFormFile _File { get => base._File; set => base._File = value; }
    }
}
