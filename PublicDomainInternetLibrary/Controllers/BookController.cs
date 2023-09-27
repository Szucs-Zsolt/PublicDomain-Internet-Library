using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicDomainInternetLibrary.Models;
using PublicDomainInternetLibrary.Data;
using PublicDomainInternetLibrary.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace PublicDomainInternetLibrary.Controllers
{

    public class BookController : Controller
    {
        // Metaadatok adatbázisba írásához
        private readonly ApplicationDbContext _db;
        // Szerveren lévő wwwroot könyvtár eléréséhez
        private IWebHostEnvironment _environment { get; set; }
        public BookController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Upsert(int? bookId)
        {
            BookUpsertViewModel viewModel = new BookUpsertViewModel();
            // új könyv
            if (bookId == null)
            {
                viewModel.IsNewBook = true;
                viewModel.Book = new Book();
            }
            else
            {
                // már létező könyv (sikerült az adatbáziból az adatait betölteni?)
                var bookFromDb = _db.Books.FirstOrDefault(x => x.Id == bookId);
                if (bookFromDb != null)
                {
                    viewModel.IsNewBook = false;
                    viewModel.Book = bookFromDb;
                }
                else
                {
                    viewModel.IsNewBook = true;
                    viewModel.Book = new Book();
                }
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Upsert(BookUpsertViewModel model, IFormFile? fileToUpload)
        {
            if (model.IsNewBook && fileToUpload == null)
            {
                ModelState.AddModelError("Book.DownloadLink", "Hiányzik a feltöltendő könyv.");
            }

            if (ModelState.IsValid)
            {
                if (model.IsNewBook)
                {
                    _db.Books.Add(model.Book);
                    _db.SaveChanges();          // frissíti az Id-t a modelben is!
                }

                // Újonnan feltöltött, vagy erre lecserélendő file
                if (fileToUpload != null)
                {
                    string directoryPath = Path.Combine(_environment.WebRootPath, "books");

                    // Törölni kell a régit, mivel a file kiterjesztése lehet, hogy nem ugyanaz
                    if (!string.IsNullOrEmpty(model.Book.DownloadLink))
                    {
                        string oldFilePath = Path.Combine(directoryPath, model.Book.DownloadLink);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Új név = könyv azonosítója az adatbázisban + eredeti kiterjesztés
                    string newFileName = model.Book.Id.ToString() + Path.GetExtension(fileToUpload.FileName);
                    string newFilePath = Path.Combine(directoryPath, newFileName);
                    // Feltöltés
                    using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                    {
                        fileToUpload.CopyTo(fileStream);
                    }
                    // Filenév módosítása a metaadatokban
                    model.Book.DownloadLink = newFileName;

                    Console.WriteLine("********************************************");
                    Console.WriteLine($"directoryPath: {directoryPath}");
                    Console.WriteLine($"newFileName: {newFileName}");
                    Console.WriteLine($"newFilePath: {newFilePath}");
                    Console.WriteLine("********************************************");
                }

                // Metaadatok módosítása az adatbázisban is                
                _db.Books.Update(model.Book);
                _db.SaveChanges();

                TempData["Success"] = model.IsNewBook ? "Könyv feltöltve" : "Könyv módosítva";
                TempData["Success"] += $" { model.Book.Title} ({ model.Book.Author})";
                return RedirectToAction("Index", "Book", new {
                    findAuthor=model.Book.Author,
                    findTitle=model.Book.Title
                });
            }

            return View(model);
        }

        public IActionResult Index(string? findAuthor, string? findTitle)
        {
            BookIndexViewModel viewModel = new BookIndexViewModel
            {
                FindAuthor = findAuthor,
                FindTitle = findTitle
            };
            // query összeállítása
            var query = from Books in _db.Books select Books;
            // kisbetű/nagybetű nem számít
            if (!string.IsNullOrEmpty(findAuthor))
                query = query.Where(x => x.Author.Contains(findAuthor));
            if (!string.IsNullOrEmpty(findTitle))
                query = query.Where(x => x.Title.Contains(findTitle));

            viewModel.Books = query
                .OrderBy(x => x.Title)
                .OrderBy(x => x.Author)
                .Take(100)
                .ToList();
            //ViewBag.SuccessMessage = "EZ az üzenet átmegy?";
            //TempData["Success"] = "TEMPDATA üzenet";
            return View(viewModel);
        }

        public IActionResult Download(int id)
        {
            var book = _db.Books.FirstOrDefault(x => x.Id == id);
            if (book == null)
            {
                return RedirectToAction("Book", "Index");
            }

            string directoryPath = Path.Combine(_environment.WebRootPath, "books");
            string fullPath = Path.Combine(directoryPath, book.DownloadLink);
            // Tartalom, típus, néven
            return File(System.IO.File.ReadAllBytes(fullPath), "application/octet-stream",
              book.DownloadLink);
        }

        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Delete(int bookId, string findAuthor, string findTitle)
        {
            var book = _db.Books.FirstOrDefault(x => x.Id == bookId);
            if (book != null)
            {
                // File törlése
                string directoryPath = Path.Combine(_environment.WebRootPath, "books");
                string fullPath = Path.Combine(directoryPath, book.DownloadLink);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);

                }
                // Metaadatok törlése
                _db.Books.Remove(book);
                _db.SaveChanges();
                TempData["Success"] = $"Könyv törölve: {book.Title} ({book.Author})";
            }

            return RedirectToAction("Index", "Book", new { findAuthor = findAuthor, findTitle = findTitle });
        }
    }
}
