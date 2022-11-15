using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using UdemyAspNetCore.Data;
using UdemyAspNetCore.Models;
using UdemyAspNetCore.Models.ViewModels;
using UdemyAspNetCore.Utility;

namespace UdemyAspNetCore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ProductUserViewModel ProductUserViewModel { get; set; }

        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var shoppingCartList = new List<ShoppingCart>();
            var sessionShoppingCart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);

            if (sessionShoppingCart is not null && sessionShoppingCart.Any())
                shoppingCartList = sessionShoppingCart.ToList();

            var prodInCart = shoppingCartList.Select(sc => sc.ProductId).ToList();
            var prodList = _db.Product.Where(p => prodInCart.Contains(p.Id));

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {

            return RedirectToAction(nameof(Summary));
        }

/*
 * ВАРИАНТЫ ПОЛУЧЕНИЯ ИМЕНИ ПОЛЬЗОВАТЕЛЯ:
 * 1.
 * var claimsIdentity = (ClaimsIdentity)User.Identity;
 * var claim = claimsIdentity.FindFirst(ClaimTypes.Name);
 * 
 * -------------------------------------------------------
 * 
 * 2.
 * 
 * var userId = User.FindFirstValue(ClaimTypes.Name);
 * 
 * -------------------------------------------------------
 * 
 * 3.
 * 
 * User.Identity.Name
 */

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCartList = new List<ShoppingCart>();
            var sessionShoppingCart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);

            if (sessionShoppingCart is not null && sessionShoppingCart.Any())
                shoppingCartList = sessionShoppingCart.ToList();

            var prodInCart = shoppingCartList.Select(sc => sc.ProductId).ToList();
            var prodList = _db.Product.Where(p => prodInCart.Contains(p.Id));

            ProductUserViewModel = new ProductUserViewModel()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                ProductList= prodList.ToList(),
            };

            return View(ProductUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserViewModel productUserViewModel)
        {
            var pathToTemplate = _webHostEnvironment.WebRootPath
                + Path.DirectorySeparatorChar.ToString()
                + "templates"
                + Path.DirectorySeparatorChar.ToString()
                + "Inquiry.html";

            string subject = "New Inquiry";
            string htmlBody = String.Empty;

            using var sr = System.IO.File.OpenText(pathToTemplate);
            htmlBody = sr.ReadToEnd();

            /*
             * Name: {0}
             * Email: {1}
             * Phone: {2}
             * Products: {3}
             */

            var productListSB = new StringBuilder();

            foreach (var product in productUserViewModel.ProductList)
                productListSB.Append($" - Name: {product.Name} <span style='font-size:14px;'> (ID: {product.Id})</span><br />");
            
            string messageBody = String.Format(htmlBody,
                productUserViewModel.ApplicationUser.FullName,
                productUserViewModel.ApplicationUser.Email,
                productUserViewModel.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult Remove(int id)
        {
            var shoppingCartList = new List<ShoppingCart>();
            var sessionShoppingCart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);

            if (sessionShoppingCart is not null && sessionShoppingCart.Any())
                shoppingCartList = sessionShoppingCart.ToList();

            var removedProduct = shoppingCartList.FirstOrDefault(p => p.ProductId == id);
            if (removedProduct is null)
                return BadRequest();

            shoppingCartList.Remove(removedProduct);
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }
    }
}
