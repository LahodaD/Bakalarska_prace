using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Reflection;

namespace Bakalarska_prace.Services
{
    public class PDFService
    {

        public string getValueByFieldName(string cellName, Object obj)
        {
            FieldInfo[] fieldsInfo = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            string hodnota = "NOT FOUND";

            if (cellName == null)
            {
                return hodnota;
            }

            foreach (var item in fieldsInfo)
            {
                string s = item.Name;
                if (item.Name.ToLower().Contains(cellName.ToLower()))
                {
                    hodnota = item.GetValue(obj).ToString();
                }
                
            }
            return hodnota;
        }


        public byte[] exporter(string filePath, string outputPath, object obj)
        {
            byte[] documentBytes = null;

            using (PdfReader reader = new PdfReader(filePath))
            {
                using (PdfStamper stamper = new PdfStamper(reader, new FileStream(outputPath, FileMode.Create)))
                {
                    AcroFields fields = stamper.AcroFields;

                    foreach (var field in fields.Fields)
                    {
                        fields.SetField(field.Key, getValueByFieldName(field.Key, obj));

                        Console.WriteLine("Název pole: " + field.Key);
                        Console.WriteLine("Typ pole: " + fields.GetFieldType(field.Key));
                        Console.WriteLine("Popis pole: " + fields.GetField(field.Key));

                    }
                    stamper.Writer.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.ALLOW_PRINTING);

                    stamper.Close();

                }
                reader.Close();
            }

            documentBytes = File.ReadAllBytes(outputPath);

            if (File.Exists(filePath) && File.Exists(outputPath))
            {
                File.Delete(filePath);
                File.Delete(outputPath);

                // Soubor byl úspěšně smazán
            }

            Console.WriteLine(documentBytes.Length);
            return documentBytes;
        }
    }
}
