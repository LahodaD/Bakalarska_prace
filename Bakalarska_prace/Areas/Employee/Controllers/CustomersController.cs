using Bakalarska_prace.Domain.Abstraction;
using Bakalarska_prace.Domain.Implementation;
using Bakalarska_prace.Models.Database;
using Bakalarska_prace.Models.Entities;
using Bakalarska_prace.Models.Identity;
using Bakalarska_prace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bakalarska_prace.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = nameof(Roles.Employee))]
    public class CustomersController : Controller
    {
        private readonly AutosalonDbContext _context;
        private readonly WordService _templateSerivce;
        private readonly ExcelService _excelService;
        private readonly PDFService _pdfService;
        private IFileUpload _fileUpload;

        public CustomersController(AutosalonDbContext context, FileUpload fileUpload)
        {
            _context = context;
            _templateSerivce = new WordService();
            _fileUpload = fileUpload;
            _excelService = new ExcelService();
            _pdfService = new PDFService();
        }

        public IActionResult Index()
        {

            return View(_context.Customers.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Adress,PhoneNumber")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(customers);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Adress,PhoneNumber")] Customers customers)
        {
            if (id != customers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Customers.Any(e => e.Id == id))
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

            return View(customers);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _context.Customers.FirstOrDefault(m => m.Id == id);


            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        public IActionResult Delete(int ID)
        {
            var customer = _context.Customers.FirstOrDefault(f => f.Id == ID);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        private IActionResult ExportPDF(string filePath, Customers customer)
        {
            string fileName = "Customer_PDF.pdf";

            try
            {
                string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "customer", "pdf"), fileName, filePath);

                string contentType = "application/pdf";
                byte[] fileBytes = _pdfService.exporter(newPath, fileName, customer);

                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
                Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
                Response.ContentType = contentType;
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                var errorMessage = "Došlo k chybě: soubor používá jiný proces";

                // Zde můžete přesměrovat na pohled s chybovou zprávou
                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(Details), new { Id = customer.Id });
            }
        }

        private IActionResult ExportExcel(string filePath, Customers customer)
        {
            //var formFile = Request.Form.Files.GetFile("FilePathPDF");
            string fileName = "Customer_Excel.xlsx";

            try
            {
                string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "customer", "excel"), fileName, filePath);

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                byte[] fileBytes = _excelService.Exporter(newPath, customer);

                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
                Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
                Response.ContentType = contentType;
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                var errorMessage = "Došlo k chybě: soubor používá jiný proces";

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(Details), new { Id = customer.Id });
            }
        }

        private IActionResult ExportWord(string filePath, Customers customer)
        {
            string fileName = "Customer_Word.docx";

            try
            {
                string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "customer", "excel"), fileName, filePath);

                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                byte[] fileBytes = _templateSerivce.Exporter(newPath, customer);

                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
                Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
                Response.ContentType = contentType;
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                var errorMessage = "Došlo k chybě: soubor používá jiný proces";

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(Details), new { Id = customer.Id });
            }
        }

        public IActionResult Export(int fileId, int objectId)
        {
            Customers customer = _context.Customers.FirstOrDefault(c => c.Id == objectId);
            Files file = _context.Files.FirstOrDefault(f => f.Id == fileId);

            if (file == null || customer == null)
            {
                return NotFound();
            }

            if (file.Path.EndsWith(".pdf"))
            {
                return ExportPDF(file.Path, customer);
            }
            else if (file.Path.EndsWith(".docx"))
            {
                return ExportWord(file.Path, customer);
            }
            else
            {
                return ExportExcel(file.Path, customer);
            }

        }
    }
}
