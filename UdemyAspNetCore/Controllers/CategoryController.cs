using Microsoft.AspNetCore.Mvc;
using UdemyAspNetCore.Data;
using UdemyAspNetCore.Models;

namespace UdemyAspNetCore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var objList = _db.Category;

            return View(objList);
        }

        //Get - Create
        public IActionResult Create()
        {
            return View();
        }

        //POST - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (category is null)
                return BadRequest();

            _db.Category.Add(category);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
