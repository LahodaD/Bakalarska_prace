﻿using Bakalarska_prace.Domain.Abstraction;
using Bakalarska_prace.Domain.Implementation;
using Bakalarska_prace.Models.Database;
using Bakalarska_prace.Models.Entities;
using Bakalarska_prace.Models.Identity;
using Bakalarska_prace.Services;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bakalarska_prace.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = nameof(Roles.Employee))]
    public class SalesController : Controller
    {
        private readonly AutosalonDbContext _context;
        private readonly WordService _templateSerivce;
        private readonly ExcelService _excelService;
        private readonly PDFService _pdfService;
        private IFileUpload _fileUpload;

        public SalesController(AutosalonDbContext context, FileUpload fileUpload)
        {
            _context = context;
            _templateSerivce = new WordService();
            _fileUpload = fileUpload;
            _excelService = new ExcelService();
            _pdfService = new PDFService();
        }

        public IActionResult Index()
        {
            return View(_context.Sales.ToList());
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = _context.Sales.FirstOrDefault(m => m.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        public IActionResult Delete(int ID)
        {
            var sale = _context.Sales.FirstOrDefault(f => f.Id == ID);

            if (sale != null)
            {
                _context.Sales.Remove(sale);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        public IActionResult Create()
        {
            ViewData["User_id"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["Customers_id"] = new SelectList(_context.Customers, "Id", "Id");
            ViewData["Cars_id"] = new SelectList(_context.Cars, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,User_id,Customers_id,Cars_id,Price")] Sales sale)
        {

            ModelState.Remove(nameof(Sales.User));
            ModelState.Remove(nameof(Sales.Customers));
            ModelState.Remove(nameof(Sales.Cars));
            if (ModelState.IsValid)
            {
                _context.Add(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["User_id"] = new SelectList(_context.Users, "Id", "Id", sale.User_id);
            ViewData["Customers_id"] = new SelectList(_context.Users, "Id", "Id", sale.Customers_id);
            ViewData["Cars_id"] = new SelectList(_context.Users, "Id", "Id", sale.Cars_id);
            return View(sale);
        }

        private IActionResult ExportPDF(string filePath, Sales sale)
        {
            string fileName = "Customer_PDF.pdf";

            try
            {
                string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "sale", "pdf"), fileName, filePath);

                string contentType = "application/pdf";
                byte[] fileBytes = _pdfService.exporter(newPath, fileName, sale);

                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
                Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
                Response.ContentType = contentType;
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                var errorMessage = "Došlo k chybě: soubor používá jiný proces";

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(Details), new { Id = sale.Id });
            }
        }

        private IActionResult ExportExcel(string filePath, Sales sale)
        {
            string fileName = "Customer_Excel.xlsx";

            try
            {
                string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "sale", "excel"), fileName, filePath);

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                byte[] fileBytes = _excelService.Exporter(newPath, sale);

                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
                Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
                Response.ContentType = contentType;
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                var errorMessage = "Došlo k chybě: soubor používá jiný proces";

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(Details), new { Id = sale.Id });
            }
        }

        private IActionResult ExportWord(string filePath, Sales sale)
        {
            string fileName = "Customer_Word.docx";

            try
            {
                string newPath = _fileUpload.CopyFileForExport(Path.Combine("files", "sale", "excel"), fileName, filePath);

                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                byte[] fileBytes = _templateSerivce.Exporter(newPath, sale);

                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName); // Content-Disposition hlavička s nastavením přílohy
                Response.Headers.Add("Content-Length", fileBytes.Length.ToString()); // Content-Length hlavička s délkou souboru v bajtech
                Response.ContentType = contentType;
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                var errorMessage = "Došlo k chybě: soubor používá jiný proces";

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(Details), new { Id = sale.Id });
            }
        }

        public IActionResult Export(int fileId, int objectId)
        {
            Sales sale = _context.Sales.FirstOrDefault(c => c.Id == objectId);
            Files file = _context.Files.FirstOrDefault(f => f.Id == fileId);

            if (file == null || sale == null)
            {
                return NotFound();
            }

            if (file.Path.EndsWith(".pdf"))
            {
                return ExportPDF(file.Path, sale);
            }
            else if (file.Path.EndsWith(".docx"))
            {
                return ExportWord(file.Path, sale);
            }
            else
            {
                return ExportExcel(file.Path, sale);
            }

        }
    }
}
