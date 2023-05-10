using Bakalarska_prace.Domain.Abstraction;
using Bakalarska_prace.Domain.Implementation;
using Bakalarska_prace.Models.Database;
using Bakalarska_prace.Models.Entities;
using Bakalarska_prace.Models.Identity;
using Bakalarska_prace.Models.ViewModels;
using Bakalarska_prace.Services;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bakalarska_prace.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = nameof(Roles.Customer))]
    public class CarsController : Controller
    {
        private readonly AutosalonDbContext _context;
        private readonly TemplateSerivce _templateSerivce;
        private readonly ExcelService _excelService;
        private readonly PDFService _pdfService;
        private IFileUpload _fileUpload;

        public CarsController(AutosalonDbContext context, FileUpload fileUpload)
        {
            _context = context;
            _templateSerivce = new TemplateSerivce(context);
            _fileUpload = fileUpload;
            _excelService = new ExcelService(context);
            _pdfService = new PDFService();
        }

        public IActionResult Index()
        {
            return View(_context.Cars.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreateYear,Price,VehicleBrand,Model,Description")] Cars cars)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cars);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(cars);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreateYear,Price,VehicleBrand,Model,Description")] Cars car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Cars.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(car);
        }

        //[HttpPost]
        //public async Task<IActionResult> Export()
        //{

        //    var formFile = Request.Form.Files.GetFile("FilePath");
        //    string fileName = "Cars_Template.docx";
        //    //ModelState.Remove(nameof(file));
        //    if (formFile == null)
        //    {
        //        TempData["Warning"] = "Please select a file.";
        //        return RedirectToAction(nameof(Index));
        //    }


        //    string filePath = await _fileUpload.FileUploadAsync(formFile, Path.Combine("files", "cars"));

        //    string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; ; // Zde nastavte odpovídající MIME typ

        //    byte[] fileBytes = _templateSerivce.Exporter(filePath, _context.Cars.ToList()[0]); //TODO: try catch a finally smazat soubor

        //    Response.Headers.Clear();
        //    Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
        //    Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
        //    Response.ContentType = contentType;

        //    return File(fileBytes, contentType, fileName);
        //}


        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = _context.Cars.FirstOrDefault(m => m.Id == id);

            var fileViewModel = new FileViewModel();

            if (car == null)
            {
                return NotFound();
            }

            return View(Tuple.Create(car, fileViewModel));
        }

        //[HttpPost]
        //public async Task<IActionResult> ExportById(int? id, FileViewModel fileViewModel)
        //{

        //    var formFile = fileViewModel.UploadFile;//Request.Form.Files.GetFile("FilePath");
        //    string fileName = "Cars_Word.docx";

        //    if (ModelState.IsValid)
        //    {

        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var car = _context.Cars.FirstOrDefault(m => m.Id == id);

        //        if (car == null)
        //        {
        //            return NotFound();
        //        }

        //        string filePath = await _fileUpload.FileUploadAsync(formFile, Path.Combine("files", "cars"));

        //        string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; // Zde nastavte odpovídající MIME typ

        //        byte[] fileBytes = _templateSerivce.Exporter(filePath, car);

        //        Response.Headers.Clear();
        //        Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
        //        Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
        //        Response.ContentType = contentType;

        //        return File(fileBytes, contentType, fileName);
        //    }
        //    return RedirectToAction(nameof(Details), new { Id = id, FileViewModel = fileViewModel });

        //}

        //[HttpPost]
        //public async Task<IActionResult> ExportExcelById(int? id)
        //{

        //    var formFile = Request.Form.Files.GetFile("FilePathExcel");
        //    string fileName = "Cars_Excel.xlsx";
        //    //ModelState.Remove(nameof(file));
        //    if (formFile == null)
        //    {
        //        TempData["Warning"] = "Please select a file.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var car = _context.Cars.FirstOrDefault(m => m.Id == id);

        //    if (car == null)
        //    {
        //        return NotFound();
        //    }

        //    string filePath = await _fileUpload.FileUploadAsync(formFile, Path.Combine("files", "cars", "excel"));

        //    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // nastaví MIME typ pro Excel
        //    byte[] fileBytes = _excelService.Exporter(filePath, car);

        //    Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
        //    Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
        //    Response.ContentType = contentType;
        //    return File(fileBytes, contentType, fileName);
        //}

        //[HttpPost]
        //public async Task<IActionResult> ExportPDFById(int? id)
        //{

        //    var formFile = Request.Form.Files.GetFile("FilePathPDF");
        //    string fileName = "Cars_PDF.pdf";
        //    //ModelState.Remove(nameof(file));
        //    if (formFile == null)
        //    {
        //        TempData["Warning"] = "Please select a file.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var car = _context.Cars.FirstOrDefault(m => m.Id == id);

        //    if (car == null)
        //    {
        //        return NotFound();
        //    }

        //    string filePath = await _fileUpload.FileUploadAsync(formFile, Path.Combine("files", "cars", "pdf"));

        //    string contentType = "application/pdf";
        //    byte[] fileBytes = _pdfService.exporter(filePath, fileName, car);

        //    Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
        //    Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
        //    Response.ContentType = contentType;
        //    return File(fileBytes, contentType, fileName);
        //}

        private IActionResult ExportPDF(string filePath, Cars car)
        {
            //var formFile = Request.Form.Files.GetFile("FilePathPDF");
            string fileName = "Cars_PDF.pdf";

            string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "cars", "pdf"), fileName, filePath);

            string contentType = "application/pdf";
            byte[] fileBytes = _pdfService.exporter(newPath, fileName, car);

            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
            Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
            Response.ContentType = contentType;
            return File(fileBytes, contentType, fileName);
        }

        private IActionResult ExportExcel(string filePath, Cars car)
        {
            //var formFile = Request.Form.Files.GetFile("FilePathPDF");
            string fileName = "Cars_Excel.xlsx";

            string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "cars", "excel"), fileName, filePath);

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            byte[] fileBytes = _excelService.Exporter(newPath, car);

            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
            Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
            Response.ContentType = contentType;
            return File(fileBytes, contentType, fileName);
        }

        private IActionResult ExportWord(string filePath, Cars car)
        {
            //var formFile = Request.Form.Files.GetFile("FilePathPDF");
            string fileName = "Cars_Word.docx";

            string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "cars", "excel"), fileName, filePath);

            string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            byte[] fileBytes = _templateSerivce.Exporter(newPath, car);

            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
            Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
            Response.ContentType = contentType;
            return File(fileBytes, contentType, fileName);
        }

        public IActionResult Export(int fileId, int objectId)
        {
            Cars car = _context.Cars.FirstOrDefault(c => c.Id == objectId);
            Files file = _context.Files.FirstOrDefault(f => f.Id == fileId);

            if (file == null || car == null)
            {
                return NotFound();
            }

            if (file.Path.EndsWith(".pdf"))
            {
                return ExportPDF(file.Path, car);
            }
            else if (file.Path.EndsWith(".docx"))
            {
                return ExportWord(file.Path, car);
            }
            else
            {
                return ExportExcel(file.Path, car);
            }

        }
    }
}
