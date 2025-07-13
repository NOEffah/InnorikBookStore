using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Helpers;

using BookStore.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Cart
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        // POST: /Cart/AddToCart/5
        public async Task<IActionResult> AddToCart(int id)
        {
            var book = await _context.Book.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();

            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(x => x.Book.Id == id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem { Book = book, Quantity = 1 });
            }

            HttpContext.Session.SetObject("Cart", cart);
            return RedirectToAction("Index");
        }

        // GET: /Cart/RemoveFromCart/5
        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.Book.Id == id);
            if (item != null)
            {
                cart.Remove(item);
            }

            HttpContext.Session.SetObject("Cart", cart);
            return RedirectToAction("Index");
        }

        // GET: /Cart/Checkout

        public IActionResult Checkout()
        {
            // 🧠 Retrieve cart from session
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Count == 0)
            {
                ViewBag.Message = "Your cart is empty.";
                return View("Cart", cart); // ✅ optionally reuse the cart view
            }

            return View(cart); // ✅ Pass cart model to the Checkout view
        }
        [HttpPost]
        public IActionResult PlaceOrder()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Checkout");
            }

            // 🧾 In real app: Save order to database here

            // ✅ Clear the cart
            HttpContext.Session.Remove("Cart");

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("Index", "Books");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int bookId, int quantity)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            var item = cart.FirstOrDefault(c => c.Book.Id == bookId);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                HttpContext.Session.SetObject("Cart", cart);
            }

            return RedirectToAction("Index");
        }


    }
}
