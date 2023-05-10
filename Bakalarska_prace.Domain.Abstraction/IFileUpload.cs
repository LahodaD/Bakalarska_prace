using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bakalarska_prace.Domain.Abstraction
{
    public interface IFileUpload
    {
        Task<string> FileUploadAsync(IFormFile iFormFile, string folderNameOnServer);
        string CopyFileForExport(string folderNameOnServer, string fileName, string pathFile);
    }
}
