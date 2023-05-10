using System.ComponentModel.DataAnnotations;

namespace Bakalarska_prace.Models.ViewModels
{
    public class FileViewModel
    {
        [Required]
        public IFormFile UploadFile { get; set; }
    }
}
