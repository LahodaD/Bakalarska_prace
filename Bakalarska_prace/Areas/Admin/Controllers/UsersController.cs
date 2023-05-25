using Bakalarska_prace.Domain.Abstraction;
using Bakalarska_prace.Domain.Implementation;
using Bakalarska_prace.Models.ApplicationServices.Abstract;
using Bakalarska_prace.Models.Database;
using Bakalarska_prace.Models.Entities;
using Bakalarska_prace.Models.Identity;
using Bakalarska_prace.Models.ViewModels;
using Bakalarska_prace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bakalarska_prace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Manager))]
    public class UsersController : Controller
    {
        private readonly AutosalonDbContext _context;
        ISecurityApplicationService _securityService;
        private readonly WordService _templateSerivce;
        private readonly ExcelService _excelService;
        private readonly PDFService _pdfService;
        private IFileUpload _fileUpload;

        public UsersController(AutosalonDbContext context, ISecurityApplicationService securityService, FileUpload fileUpload)
        {
            _context = context;
            _securityService = securityService;
            _templateSerivce = new WordService();
            _fileUpload = fileUpload;
            _excelService = new ExcelService();
            _pdfService = new PDFService();
        }

        public IActionResult Index()
        {
            return View(_context.Users.ToList());
        }

        public IActionResult Create()
        {
            ViewData["Role_id"] = new SelectList(_context.Roles, "Name", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel registerVM)
        {
            if (ModelState.IsValid == true)
            {
                string[] errors = await _securityService.Register(registerVM, registerVM.Role);
            }
            ViewData["Role_id"] = new SelectList(_context.Roles, "Name", "Name", registerVM.Role);
            return View(registerVM);
        }

        private IActionResult ExportPDF(string filePath, User user)
        {
            string fileName = "User_PDF.pdf";

            string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "user", "pdf"), fileName, filePath);

            string contentType = "application/pdf";
            byte[] fileBytes = _pdfService.exporter(newPath, fileName, user);

            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
            Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
            Response.ContentType = contentType;
            return File(fileBytes, contentType, fileName);
        }

        private IActionResult ExportExcel(string filePath, User user)
        {
            //var formFile = Request.Form.Files.GetFile("FilePathPDF");
            string fileName = "User_Excel.xlsx";

            string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "user", "excel"), fileName, filePath);

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            byte[] fileBytes = _excelService.Exporter(newPath, user);

            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
            Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
            Response.ContentType = contentType;
            return File(fileBytes, contentType, fileName);
        }

        private IActionResult ExportWord(string filePath, User user)
        {
            //var formFile = Request.Form.Files.GetFile("FilePathPDF");
            string fileName = "Customer_Word.docx";

            string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "user", "excel"), fileName, filePath);

            string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            byte[] fileBytes = _templateSerivce.Exporter(newPath, user);

            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
            Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
            Response.ContentType = contentType;
            return File(fileBytes, contentType, fileName);
        }

        public IActionResult Export(int fileId, int objectId)
        {
            User user = _context.Users.FirstOrDefault(c => c.Id == objectId);
            Files file = _context.Files.FirstOrDefault(f => f.Id == fileId);

            if (file == null || user == null)
            {
                return NotFound();
            }

            if (file.Path.EndsWith(".pdf"))
            {
                return ExportPDF(file.Path, user);
            }
            else if (file.Path.EndsWith(".docx"))
            {
                return ExportWord(file.Path, user);
            }
            else
            {
                return ExportExcel(file.Path, user);
            }

        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(m => m.Id == id);


            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult Delete(int ID)
        {
            var user = _context.Users.FirstOrDefault(f => f.Id == ID);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirstName,LastName,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.Id == id))
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
            return View(user);
        }
    }
}
