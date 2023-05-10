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
        public readonly AutosalonDbContext _dbContext;

        public TemplateSerivce(AutosalonDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        //public List<string> GetMergeFieldWord(WordprocessingDocument documet)
        //{
        //    List<string> mergeFiels = new List<string>();
        //    var body = documet.MainDocumentPart.Document.Body;
        //    foreach (SimpleField field in body.Descendants<SimpleField>())
        //    {
        //        string fieldName = field.InnerText.Trim().Substring(1, field.InnerText.Trim().Length-2);
        //        Console.WriteLine(fieldName);
        //        mergeFiels.Add(fieldName);
        //    }
        //    return mergeFiels;
        //}

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

        //public byte[] WordExporter(WordprocessingDocument document, Cars car, string filePath)
        //{
        //    byte[] documentBytes;
        //    var body = document.MainDocumentPart.Document.Body;
        //    foreach (SimpleField field in body.Descendants<SimpleField>())
        //    {
        //        var fieldCode = field.Instruction.Value;
        //        if (fieldCode.Contains("MERGEFIELD"))
        //        {
        //            //var fieldName = fieldCode.Split(new string[] { "MERGEFIELD", " ", "\\" }, StringSplitOptions.RemoveEmptyEntries)[1];
        //            string fieldName = field.InnerText.Trim().Substring(1, field.InnerText.Trim().Length - 2);
        //            string fieldValue = getValueByFieldName(fieldName, car);
                        
        //            Run fieldRun = field.Descendants<Run>().FirstOrDefault();
        //            if (fieldRun != null)
        //            {
        //                var newRun = new Run();
        //                var newText = new Text(fieldValue);
        //                newRun.AppendChild(newText);
        //                field.Parent.ReplaceChild(newRun, field);
        //            }
        //        }
        //    }
        //    document.Save();
        //    document.Close();

        //    documentBytes = File.ReadAllBytes(filePath);
        //    Console.WriteLine(documentBytes.Length);
        //    return documentBytes;

        //}

        //public int GetNumberOfPages(string filePath)
        //{
        //    int pageCount = 0;

        //    using (WordprocessingDocument wordDocument = WordprocessingDocument.Open("C:\\Users\\dlahoda\\Documents\\UTB\\BAKALARKA\\Template.docx", false))
        //    {
        //        ExtendedFilePropertiesPart extendedFilePropertiesPart = wordDocument.ExtendedFilePropertiesPart;
        //        if (extendedFilePropertiesPart != null)
        //        {
        //            var prop = extendedFilePropertiesPart.Properties.Pages;
        //            if (prop != null)
        //            {
        //                pageCount = (int)prop.Value;
        //            }
        //        }
        //    }

        //    return pageCount;
        //}

        public void CopyDocument()
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open("C:\\Users\\dlahoda\\Documents\\UTB\\BAKALARKA\\Template.docx", false))
            {
                // Vytvoření nového dokumentu
                using (WordprocessingDocument newDoc = WordprocessingDocument.Create("C:\\Users\\dlahoda\\Documents\\UTB\\BAKALARKA\\TemplateEdit.docx", WordprocessingDocumentType.Document))
                {
                    // Vytvoření hlavní části nového dokumentu
                    MainDocumentPart mainPart = newDoc.AddMainDocumentPart();

                    Document documentRoot = new Document();

                    // Vytvoření těla dokumentu
                    Body body = new Body();

                    // Přidání těla do kořenového elementu dokumentu
                    documentRoot.Append(body);

                    // Nastavení kořenového elementu jako obsahu hlavní části dokumentu
                    mainPart.Document = documentRoot;

                    // Nakopírování obsahu hlavní části ze zdrojového dokumentu do nového
                    //mainPart.Document = new Document(doc.MainDocumentPart.Document.OuterXml);

                    foreach (StyleDefinitionsPart sourceStylesPart in doc.MainDocumentPart.GetPartsOfType<StyleDefinitionsPart>())
                    {
                        StyleDefinitionsPart targetStylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
                        targetStylesPart.Styles = (Styles)sourceStylesPart.Styles.CloneNode(true);
                        targetStylesPart.Styles.Save();
                    }

                    foreach (var el in doc.MainDocumentPart.Document.Body.Elements())
                    {
                        mainPart.Document.Body.AppendChild(el.CloneNode(true));
                    }

                    // Zkopírování obrázků do nového dokumentu
                    foreach (ImagePart oldImagePart in doc.MainDocumentPart.ImageParts)
                    {
                        // Získání ID starého obrázku
                        string oldImageId = doc.MainDocumentPart.GetIdOfPart(oldImagePart);

                        // Vytvoření nového obrázku
                        ImagePart newImagePart = newDoc.MainDocumentPart.AddImagePart(oldImagePart.ContentType, oldImageId);

                        // Zkopírování dat ze starého obrázku do nového
                        using (Stream oldStream = oldImagePart.GetStream())
                        using (Stream newStream = newImagePart.GetStream(FileMode.Create, FileAccess.Write))
                        {
                            oldStream.CopyTo(newStream);
                        }
                    }

                    var body_two = mainPart.Document.Body;
                    foreach (SimpleField field in body_two.Descendants<SimpleField>())
                    {
                        var fieldCode = field.Instruction.Value;
                        if (fieldCode.Contains("MERGEFIELD"))
                        {
                            var fieldName = fieldCode.Split(new string[] { "MERGEFIELD", " ", "\\" }, StringSplitOptions.RemoveEmptyEntries)[1];

                            string fieldValue = "Auto";
                            Run fieldRun = field.Descendants<Run>().FirstOrDefault();
                            if (fieldRun != null)
                            {
                                var newRun = new Run();
                                var newText = new Text(fieldValue);
                                newRun.AppendChild(newText);
                                field.Parent.ReplaceChild(newRun, field);
                            }
                        }
                    }
                    // Uložení nového dokumentu
                    mainPart.Document.Save();
                }
            }
        }


        //public void CopyDocument(string sourceFilePath, string targetFilePath)
        //{
        //    // Otevření zdrojového dokumentu pro čtení
        //    using (WordprocessingDocument sourceDoc = WordprocessingDocument.Open(sourceFilePath, false))
        //    {

        //        //MainDocumentPart mainPart = CreateNewDocument(targetFilePath);
        //        // Vytvoření nového dokumentu
        //        using (WordprocessingDocument targetDoc = WordprocessingDocument.Create(targetFilePath, WordprocessingDocumentType.Document, false))
        //        {
        //            // Vytvoření hlavní části nového dokumentu
        //            MainDocumentPart mainPart = targetDoc.AddMainDocumentPart();

        //            Document documentRoot = new Document();

        //            // Vytvoření těla dokumentu
        //            Body body = new Body();

        //            // Přidání prvku do těla dokumentu
        //            //Paragraph paragraph = new Paragraph();
        //            //Run run = new Run(new Text("Hello World!"));
        //            //paragraph.Append(run);
        //            //body.Append(paragraph);

        //            // Přidání těla do kořenového elementu dokumentu
        //            documentRoot.Append(body);

        //            // Nastavení kořenového elementu jako obsahu hlavní části dokumentu
        //            mainPart.Document = documentRoot;


        //            foreach (StyleDefinitionsPart sourceStylesPart in sourceDoc.MainDocumentPart.GetPartsOfType<StyleDefinitionsPart>())
        //            {
        //                StyleDefinitionsPart targetStylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
        //                targetStylesPart.Styles = (Styles)sourceStylesPart.Styles.CloneNode(true);
        //                targetStylesPart.Styles.Save();
        //            }

        //            foreach (var el in sourceDoc.MainDocumentPart.Document.Body.Elements())
        //            {
        //                mainPart.Document.Body.AppendChild(el.CloneNode(true));
        //            }

        //            // Zkopírování obsahu ze zdrojového dokumentu do nového dokumentu
        //            //sourceDoc.MainDocumentPart.Document.Body.Elements().ToList().ForEach(el => mainPart.Document.Body.AppendChild(el.CloneNode(true)));
        //            mainPart.Document.Save();
        //        }
        //    }
        //}

        //public MainDocumentPart CreateNewDocument(string filePath)
        //{
        //    // Vytvoření nového dokumentu
        //    using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
        //    {
        //        // Vytvoření hlavní části dokumentu
        //        MainDocumentPart mainPart = document.AddMainDocumentPart();

        //        // Vytvoření kořenového elementu pro obsah dokumentu
        //        Document documentRoot = new Document();

        //        // Vytvoření těla dokumentu
        //        Body body = new Body();

        //        // Přidání prvku do těla dokumentu
        //        Paragraph paragraph = new Paragraph();
        //        Run run = new Run(new Text("Hello World!"));
        //        paragraph.Append(run);
        //        body.Append(paragraph);

        //        // Přidání těla do kořenového elementu dokumentu
        //        documentRoot.Append(body);

        //        // Nastavení kořenového elementu jako obsahu hlavní části dokumentu
        //        mainPart.Document = documentRoot;

        //        return mainPart;
        //    }
        //}

        //public string ComputeFileHash(string filePath)
        //{
        //    using (FileStream stream = File.OpenRead(filePath))
        //    {
        //        SHA256Managed sha = new SHA256Managed();
        //        byte[] hash = sha.ComputeHash(stream);
        //        return BitConverter.ToString(hash).Replace("-", "").ToLower();
        //    }
        //}

        //public void Controla()
        //{
        //    string sourceFilePath = "C:\\Users\\dlahoda\\Documents\\UTB\\BAKALARKA\\Template.docx";
        //    string targetFilePath = "C:\\Users\\dlahoda\\Documents\\UTB\\BAKALARKA\\TemplateEdit.docx";

        //    string sourceHash = ComputeFileHash(sourceFilePath);
        //    string targetHash = ComputeFileHash(targetFilePath);

        //    if (sourceHash == targetHash)
        //    {
        //        Console.WriteLine("Dokumenty jsou totožné.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Dokumenty nejsou totožné.");
        //    }
        //}

        public string LibreOffice(WordprocessingDocument document)
        {
            var fileProps = document.ExtendedFilePropertiesPart.Properties;
            return fileProps.Application.Text;   
        }

        //public byte[] LibreOfficeExporter(WordprocessingDocument document, Cars car, string filePath)
        //{
        //    byte[] documentBytes;
            

        //    //TODO: asi hodit do Try/Catch
        //    MainDocumentPart mainDocPart = document.MainDocumentPart;
        //    var mergeFields = mainDocPart.Document.Descendants<FieldCode>().Where(fc => fc.InnerText.StartsWith(" MERGEFIELD "));

        //    for (int i = 0; i < mergeFields.Count(); i++)
        //    {
        //        string fieldName = mergeFields.ElementAt(i).InnerText.Replace(" MERGEFIELD ", "").Trim();
        //        var paragraph = mergeFields.ElementAt(i).Ancestors<Paragraph>().FirstOrDefault();
        //        var text = mergeFields.ElementAt(i).Descendants<Text>().FirstOrDefault();

        //        if (paragraph != null)
        //        {
        //            var textElements = paragraph.Descendants<Text>();

        //            foreach (var textElement in textElements)
        //            {
        //                var rElement = textElement.Ancestors<Run>().FirstOrDefault();

        //                if (rElement != null)
        //                {
        //                    var rPrElement = rElement.GetFirstChild<RunProperties>().CloneNode(true);
        //                    var newText = new Text(getValueByFieldName(fieldName, car));
        //                    var newRun = new Run(rPrElement, newText);
        //                    var paraProps = paragraph.GetFirstChild<ParagraphProperties>().CloneNode(true);
        //                    var newPara = new Paragraph(paraProps);

        //                    newPara.AppendChild(newRun);

        //                    var parent = paragraph.Parent;
        //                    parent.ReplaceChild(newPara, paragraph);
        //                }
        //            }
        //        }
        //        Console.WriteLine($"MergeField: {fieldName}");
        //        i--;
        //    }
        //    document.MainDocumentPart.Document.Save();
        //    document.Close();

        //    documentBytes = File.ReadAllBytes(filePath);
        //    Console.WriteLine(documentBytes.Length);
        //    return documentBytes;
        //}

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
                    //var fieldName = fieldCode.Split(new string[] { "MERGEFIELD", " ", "\\" }, StringSplitOptions.RemoveEmptyEntries)[1];
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
            //foreach (SimpleField field in simpleFields)
            //{
            //    var fieldCode = field.Instruction.Value;
            //    if (fieldCode.Contains("MERGEFIELD"))
            //    {
            //        //var fieldName = fieldCode.Split(new string[] { "MERGEFIELD", " ", "\\" }, StringSplitOptions.RemoveEmptyEntries)[1];
            //        string fieldName = field.InnerText.Trim().Substring(1, field.InnerText.Trim().Length - 2);
            //        string fieldValue = getValueByFieldName(fieldName, obj);

            //        Run fieldRun = field.Descendants<Run>().FirstOrDefault();
            //        if (fieldRun != null)
            //        {
            //            var newRun = new Run();
            //            var newText = new Text(fieldValue);
            //            newRun.AppendChild(newText);
            //            field.Parent.ReplaceChild(newRun, field);
            //        }
            //    }
            //}

            foreach (var mergeField in mergeFields)
            {
                //string fieldName = mergeField.InnerText.Replace(" MERGEFIELD ", "").Trim();
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
                using (WordprocessingDocument document = WordprocessingDocument.Open(templatePath, true))
                {
                    documentBytes = WordLibreOfficeExporter(document, obj, templatePath);
                }
            }

            if (File.Exists(templatePath))
            {
                File.Delete(templatePath);
                // Soubor byl úspěšně smazán
            }

            return documentBytes;
        }


    }
}
