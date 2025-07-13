using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper method to get genres
        private static List<SelectListItem> GetGenres()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Fiction", Text = "Fiction" },
                new SelectListItem { Value = "Non-Fiction", Text = "Non-Fiction" },
                new SelectListItem { Value = "Science", Text = "Science" },
                new SelectListItem { Value = "History", Text = "History" },
                new SelectListItem { Value = "Romance", Text = "Romance" },
                new SelectListItem { Value = "Comedy", Text = "Comedy" },
                new SelectListItem { Value = "Biography", Text = "Biography" }
            };
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Book.ToListAsync());
        }

        // GET
        public async Task<IActionResult> Search()
        {
            return View(await _context.Book.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchterm)
        {
            return View("Index", await _context.Book.Where(x => x.Title.Contains(searchterm)).ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _context.Book.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        [Authorize]
        // GET: Books/Create
        public IActionResult Create()
        {
            ViewBag.Genres = GetGenres();
            return View();
        }

        [Authorize]
        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Description,Price,Genre")] Book book, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    book.ImageUrl = "/images/" + fileName;
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Genres = GetGenres();
            return View(book);
        }

        [Authorize]
        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _context.Book.FindAsync(id);
            if (book == null)
                return NotFound();

            ViewBag.Genres = GetGenres();
            return View(book);
        }

        [Authorize]
        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Description,Price,Genre,ImageUrl")] Book book, IFormFile? ImageFile)
        {
            if (id != book.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        book.ImageUrl = "/images/" + fileName;
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Genres = GetGenres();
            return View(book);
        }

        [Authorize]
        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _context.Book.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        [Authorize]
        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
