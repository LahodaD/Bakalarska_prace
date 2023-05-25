using Bakalarska_prace.Models.Database;
using OfficeOpenXml;
using System.Reflection;

namespace Bakalarska_prace.Services
{
    public class ExcelService
    {
        public object getValueByFieldName(string cellName, Object obj)
        {
            FieldInfo[] fieldsInfo = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var hodnota = "NOT FOUND";

            if (cellName == null)
            {
                return hodnota;
            }

            foreach (var item in fieldsInfo)
            {
                string s = item.Name;
                if (item.Name.ToLower().Contains(cellName.ToLower()))
                {
                    return item.GetValue(obj);
                }
            }
            return hodnota;
        }

        public byte[] Exporter(string excelPath, object obj)
        {

            byte[] documentBytes = null;
            
            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(excelPath)))
                {

                    foreach (var sheetName in GetSheetNames(package))
                    {
                        foreach (var workbook in package.Workbook.Names)
                        {
                            if (sheetName.Equals(workbook.Worksheet.Name))
                            {
                                Console.WriteLine(workbook);
                                Console.WriteLine(" (list " + workbook.Worksheet.Name + ")");

                                workbook.Worksheet.Cells[workbook.Start.Row, workbook.Start.Column].Value = getValueByFieldName(workbook.ToString(), obj);

                            }
                        }
                    }
                    package.Save();
                    package.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            documentBytes = File.ReadAllBytes(excelPath);
            
            if (File.Exists(excelPath))
            {
                File.Delete(excelPath);
                // Soubor byl úspěšně smazán
            }

            Console.WriteLine(documentBytes.Length);
            return documentBytes;

        }

        public List<string> GetSheetNames(ExcelPackage package)
        {
            ExcelWorkbook workBook = package.Workbook;
            ExcelWorksheets worksheets = workBook.Worksheets;

            // Získat počet listů
            int numberOfSheets = worksheets.Count;

            // Vytvořit kolekci pro uložení jmen listů
            List<string> sheetNames = new List<string>();

            // Projít každý list a uložit jeho název
            foreach (var sheet in worksheets)
            {
                sheetNames.Add(sheet.Name);
            }
            return sheetNames;
        }

    }
}
