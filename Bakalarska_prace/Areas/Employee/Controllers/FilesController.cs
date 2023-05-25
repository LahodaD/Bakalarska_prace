using Bakalarska_prace.Domain.Abstraction;
using Bakalarska_prace.Domain.Implementation;
using Bakalarska_prace.Models.ApplicationServices.Abstract;
using Bakalarska_prace.Models.Database;
using Bakalarska_prace.Models.Entities;
using Bakalarska_prace.Models.Identity;
using Bakalarska_prace.Models.ViewModels;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bakalarska_prace.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = nameof(Roles.Employee))]
    public class FilesController : Controller
    {
        private readonly AutosalonDbContext _dbContext;
        private IFileUpload _fileUpload;
        private ISecurityApplicationService iSecure;

        public FilesController(AutosalonDbContext dbContext, FileUpload fileUpload, ISecurityApplicationService iSecure)
        {
            _dbContext = dbContext;
            _fileUpload = fileUpload;
            this.iSecure = iSecure;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                User currentUser = await iSecure.GetCurrentUser(User);
                if (currentUser != null)
                {
                    IList<Files> files = await this._dbContext.Files
                                                        .Where(or => or.UserId == currentUser.Id)
                                                        .Include(o => o.User)
                                                        .ToListAsync();
                    return View(files);
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> Select(string objectController, string objectArea, int objectId)
        {
            if (User.Identity.IsAuthenticated)
            {
                User currentUser = await iSecure.GetCurrentUser(User);
                if (currentUser != null)
                {
                    IList<Files> files = await this._dbContext.Files
                                                        .Where(or => or.UserId == currentUser.Id)
                                                        .Include(o => o.User)
                                                        .ToListAsync();

                    
                    ViewData["objectController"] = objectController;
                    ViewData["objectArea"] = objectArea;
                    ViewData["objectId"] = objectId;
                    return View(files);
                }
            }

            return NotFound();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilesRequired filesRequired)
        {
            User currentUser = await iSecure.GetCurrentUser(User);
            filesRequired.UserId = currentUser.Id;
            filesRequired.User = currentUser;

            ModelState.Remove(nameof(Files.Path));
            ModelState.Remove(nameof(Files.UserId));
            ModelState.Remove(nameof(Files.User));
            if (ModelState.IsValid)
            {
                filesRequired.Path = await _fileUpload.FileUploadAsync(filesRequired._File, Path.Combine("files", "savedFiles"));

                ModelState.Clear();
                if (TryValidateModel(filesRequired))
                {
                    _dbContext.Files.Add(filesRequired);
                    _dbContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                
            }

            return View(filesRequired);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(FilesRequired filesRequired)
        {
            User currentUser = await iSecure.GetCurrentUser(User);
            filesRequired.UserId = currentUser.Id;
            filesRequired.User = currentUser;

            ModelState.Remove(nameof(Files.Path));
            ModelState.Remove(nameof(Files.UserId));
            ModelState.Remove(nameof(Files.User));
            if (ModelState.IsValid)
            {
                filesRequired.Path = await _fileUpload.FileUploadAsync(filesRequired._File, Path.Combine("files", "savedFiles"));

                ModelState.Clear();
                if (TryValidateModel(filesRequired))
                {
                    _dbContext.Files.Add(filesRequired);
                    _dbContext.SaveChanges();
                    return RedirectToAction(nameof(Select));
                }

            }

            return View(filesRequired);
        }

        public async Task<IActionResult> Delete(int ID)
        {
            Files file = _dbContext.Files.FirstOrDefault(f => f.Id == ID);
            User currentUser = await iSecure.GetCurrentUser(User);

            if (file != null && currentUser.Id == file.UserId)
            {
                _dbContext.Files.Remove(file);
                _dbContext.SaveChanges();

                if (System.IO.File.Exists(file.Path))
                {
                    System.IO.File.Delete(file.Path);
                }

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        public async Task<string> GetPathAsync(int id)
        {
            Files file = _dbContext.Files.FirstOrDefault(f => f.Id == id);
            User currentUser = await iSecure.GetCurrentUser(User);

            if (file != null && currentUser.Id == file.UserId)
            {
                return file.Path;
            }
            return null;
        }
    }
}
