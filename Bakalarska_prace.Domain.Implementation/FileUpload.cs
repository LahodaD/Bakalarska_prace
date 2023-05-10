using Bakalarska_prace.Domain.Abstraction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bakalarska_prace.Domain.Implementation
{
    public class FileUpload : IFileUpload
    {
        public string RootPath { get; set; }

        public FileUpload(string rootPath)
        {
            RootPath = rootPath;
        }

        public async Task<string> FileUploadAsync(IFormFile iFormFile, string folderNameOnServer)
        {
            string filePathOutput = String.Empty;
           
            var fileName = Path.GetFileNameWithoutExtension(iFormFile.FileName);
            var uniqueValue = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            fileName = fileName + "_" +uniqueValue;
            var fileExtension = Path.GetExtension(iFormFile.FileName);
            
            var fileRelative = Path.Combine(folderNameOnServer, fileName + fileExtension);
            var filePath = Path.Combine(this.RootPath, fileRelative);

            Directory.CreateDirectory(Path.Combine(this.RootPath, folderNameOnServer));
            using (Stream stream = new FileStream(filePath, FileMode.Create))
            {
                await iFormFile.CopyToAsync(stream);
            }

            filePathOutput = RootPath + $"/{fileRelative}";

            return filePathOutput;
        }

        public string CopyFileForExport(string folderNameOnServer, string fileName,string pathFile)
        {
            
            folderNameOnServer = Path.Combine(this.RootPath, folderNameOnServer);
            var fileRelative = Path.Combine(folderNameOnServer, fileName);
            Directory.CreateDirectory(folderNameOnServer);

            using (FileStream sourceFileStream = new FileStream(pathFile, FileMode.Open))
            {
                // Zkopírujte soubor na nové místo

                using (FileStream destinationFileStream = new FileStream(fileRelative, FileMode.Create))
                {
                    sourceFileStream.CopyTo(destinationFileStream);
                }
            }
            return fileRelative;
        }
    }
}
