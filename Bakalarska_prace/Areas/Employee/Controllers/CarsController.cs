using Bakalarska_prace.Domain.Abstraction;
using Bakalarska_prace.Domain.Implementation;
using Bakalarska_prace.Models.Database;
using Bakalarska_prace.Models.Entities;
using Bakalarska_prace.Models.Identity;
using Bakalarska_prace.Models.ViewModels;
using Bakalarska_prace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bakalarska_prace.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = nameof(Roles.Employee))]
    public class CarsController : Controller
    {
        private readonly AutosalonDbContext _context;
        private readonly WordService _templateSerivce;
        private readonly ExcelService _excelService;
        private readonly PDFService _pdfService;
        private IFileUpload _fileUpload;

        public CarsController(AutosalonDbContext context, FileUpload fileUpload)
        {
            _context = context;
            _templateSerivce = new WordService();
            _fileUpload = fileUpload;
            _excelService = new ExcelService();
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

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = _context.Cars.FirstOrDefault(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        private IActionResult ExportPDF(string filePath, Cars car)
        {
            string fileName = "Cars_PDF.pdf";
            try
            {
                string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "cars", "pdf"), fileName, filePath);

                string contentType = "application/pdf";
                byte[] fileBytes = _pdfService.exporter(newPath, fileName, car);

                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
                Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
                Response.ContentType = contentType;
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                var errorMessage = "Došlo k chybě: soubor používá jiný proces";

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(Details), new { Id = car.Id });
            }
        }

        private IActionResult ExportExcel(string filePath, Cars car)
        {
            string fileName = "Cars_Excel.xlsx";
            try
            {
                string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "cars", "excel"), fileName, filePath);

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                byte[] fileBytes = _excelService.Exporter(newPath, car);

                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
                Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
                Response.ContentType = contentType;
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                var errorMessage = "Došlo k chybě: soubor používá jiný proces";

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(Details), new { Id = car.Id });
            }

        }

        private IActionResult ExportWord(string filePath, Cars car)
        {
            string fileName = "Cars_Word.docx";
            try
            {
                string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "cars", "excel"), fileName, filePath);

                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                byte[] fileBytes = _templateSerivce.Exporter(newPath, car);
                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
                Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
                Response.ContentType = contentType;
                return File(fileBytes, contentType, fileName);
            }
            catch(Exception ex)
            {
                var errorMessage = "Došlo k chybě: soubor používá jiný proces";

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(Details), new { Id = car.Id });
            }
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

        public IActionResult Delete(int ID)
        {
            var car = _context.Cars.FirstOrDefault(f => f.Id == ID);

            if (car != null)
            {
                _context.Cars.Remove(car);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}
