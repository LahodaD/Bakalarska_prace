using Bakalarska_prace.Models.Database;
using Bakalarska_prace.Models.Entities;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bakalarska_prace.Services
{
    public class TemplateSerivce
    {
        public readonly AutosalonDbContext _dbContext; //TODO: neni potreba

        public TemplateSerivce(AutosalonDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public string getValueByFieldName(string fieldName, Object obj)
        {
            FieldInfo[] fieldsInfo = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            string hodnota = "NOT FOUND";

            if (fieldName == null)
            {
                return hodnota;
            }

            foreach (var item in fieldsInfo)
            {
                string s = item.Name;
                if (item.Name.ToLower().Contains(fieldName.ToLower()))
                {
                    hodnota = item.GetValue(obj).ToString();
                }
            }
            return hodnota;
        }

        public string LibreOffice(WordprocessingDocument document)
        {
            var fileProps = document.ExtendedFilePropertiesPart.Properties;
            return fileProps.Application.Text;   
        }


        public byte[] WordLibreOfficeExporter(WordprocessingDocument document, object obj, string filePath)
        {
            byte[] documentBytes = null;

            MainDocumentPart mainDocPart = document.MainDocumentPart;
            var mergeFields = mainDocPart.Document.Descendants<FieldCode>().Where(fc => fc.InnerText.StartsWith(" MERGEFIELD "));

            //pro simpleFieldy(neupravené mergeFieldy jen ve Wordu)
            var body = mainDocPart.Document.Body;
            var simpleFields = body.Descendants<SimpleField>();
            for (int i = 0; i < simpleFields.Count(); i++)
            {
                var fieldCode = simpleFields.ElementAt(i).Instruction.Value;
                if (fieldCode.Contains("MERGEFIELD"))
                {
                    string fieldName = simpleFields.ElementAt(i).InnerText.Trim().Substring(1, simpleFields.ElementAt(i).InnerText.Trim().Length - 2);
                    string fieldValue = getValueByFieldName(fieldName, obj);

                    Run fieldRun = simpleFields.ElementAt(i).Descendants<Run>().FirstOrDefault();
                    if (fieldRun != null)
                    {
                        var newRun = new Run();
                        var newText = new Text(fieldValue);
                        newRun.AppendChild(newText);
                        simpleFields.ElementAt(i).Parent.ReplaceChild(newRun, simpleFields.ElementAt(i));
                        i--;
                    }
                }
            }
            
            foreach (var mergeField in mergeFields)
            {
                string fieldName;

                if (LibreOffice(document).ToLower().Contains("LibreOffice".ToLower()))
                {
                    fieldName = mergeField.InnerText.Replace(" MERGEFIELD ", "").Trim();
                }
                else
                {
                    fieldName = Regex.Replace(mergeField.InnerText.Trim(), @"\s+", " ");
                    fieldName = fieldName.Split(" ")[1];
                }

                var paragraph = mergeField.Ancestors<Paragraph>().FirstOrDefault();
                var runsToRemove = paragraph?.Descendants<Run>().ToList();

                List<Tuple<int, int>> tuplesList = GetIndex(runsToRemove);

                foreach (var tuple in tuplesList)
                {
                    var rPrElement = runsToRemove[tuple.Item1].GetFirstChild<RunProperties>()?.CloneNode(true);

                    runsToRemove.RemoveRange(tuple.Item1, (tuple.Item2 - tuple.Item1) + 1);

                    var newText = new Text(getValueByFieldName(fieldName, obj));
                    var newRun = new Run(rPrElement, newText);

                    runsToRemove.Insert(tuple.Item1, newRun);
                }

                paragraph?.RemoveAllChildren<Run>();
                paragraph?.Append(runsToRemove);
            }

            document.MainDocumentPart?.Document.Save();
            document.Close();

            documentBytes = File.ReadAllBytes(filePath);
            Console.WriteLine(documentBytes.Length);
            return documentBytes;
        }

        private List<Tuple<int, int>> GetIndex(List<Run> runsList)
        {
            List<Tuple<int, int>> tuplesList = new List<Tuple<int, int>>();

            int indexBegin = 0;
            int indexEnd = 0;
            int indexMergeField = -1;

            int counter = 0;
            foreach (var run in runsList)
            {
                FieldChar fieldChar = run.Descendants<FieldChar>().FirstOrDefault();
                if (fieldChar != null && fieldChar.FieldCharType == FieldCharValues.Begin)
                {
                    indexBegin = counter;
                }
                if (fieldChar != null && fieldChar.FieldCharType == FieldCharValues.End)
                {
                    indexEnd = counter;
                }
                if (run.InnerText.Contains(" MERGEFIELD "))
                {
                    indexMergeField = counter;
                }

                counter++;
            }

            if ((indexBegin < indexMergeField) && (indexEnd > indexMergeField))
            {
                tuplesList.Add(Tuple.Create(indexBegin, indexEnd));
            }

            return tuplesList;
        }


        public byte[] Exporter(string templatePath, object obj)
        {
            byte[] documentBytes = null;

            if (templatePath.EndsWith(".docx"))
            {
                try
                {
                    using (WordprocessingDocument document = WordprocessingDocument.Open(templatePath, true))
                    {
                        documentBytes = WordLibreOfficeExporter(document, obj, templatePath);
                    }
                }
                catch(Exception ex)
                {
                    throw;
                }
            }

            if (File.Exists(templatePath))
            {
                File.Delete(templatePath);
            }
            return documentBytes;
        }
    }
}
