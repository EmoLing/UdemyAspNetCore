using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UdemyAspNetCore.Data;
using UdemyAspNetCore.Models;
using UdemyAspNetCore.Models.ViewModels;
using UdemyAspNetCore.Utility;

namespace UdemyAspNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var homeVm = new HomeViewModel()
            {
                Products = _db.Product.Include(p => p.Category).Include(p => p.ApplicationType),
                Categories = _db.Category
            };

            return View(homeVm);
        }

        public IActionResult Details(int id)
        {
            var shoppingCardList = new List<ShoppingCard>();
            var sessionCard = HttpContext.Session.Get<IEnumerable<ShoppingCard>>(WC.SessionCard);

            if (sessionCard is not null && sessionCard.Any())
                shoppingCardList = sessionCard.ToList();

            var detailsViewModel = new DetailsViewModel()
            {
                Product = _db.Product.Include(p => p.Category).Include(p => p.ApplicationType)
                    .FirstOrDefault(p => p.Id == id),
                ExistsInCart = false,
            };

            foreach (var itemShoppingCard in shoppingCardList)
            {
                if (itemShoppingCard.ProductId == id)
                    detailsViewModel.ExistsInCart = true;
            }

            return View(detailsViewModel);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            var shoppingCardList = new List<ShoppingCard>();
            var sessionCard = HttpContext.Session.Get<IEnumerable<ShoppingCard>>(WC.SessionCard);

            if (sessionCard is not null && sessionCard.Any())
                shoppingCardList = sessionCard.ToList();

            shoppingCardList.Add(new ShoppingCard() { ProductId = id });
            HttpContext.Session.Set(WC.SessionCard, shoppingCardList);

            return RedirectToAction(nameof(Index));
        }

        //RemoveFromCard
        public IActionResult RemoveFromCard(int id)
        {
            var shoppingCardList = new List<ShoppingCard>();
            var sessionCard = HttpContext.Session.Get<IEnumerable<ShoppingCard>>(WC.SessionCard);

            if (sessionCard is not null && sessionCard.Any())
                shoppingCardList = sessionCard.ToList();

            var itemToRemove = shoppingCardList.SingleOrDefault(item => item.ProductId == id);

            if (itemToRemove is not null)
                shoppingCardList.Remove(itemToRemove);

            HttpContext.Session.Set(WC.SessionCard, shoppingCardList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}