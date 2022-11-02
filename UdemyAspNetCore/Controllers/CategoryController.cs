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
            if (!ModelState.IsValid) //Валидация на стороне сервера
                return View(category);

            _db.Category.Add(category);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Get - Edit
        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0)
                return NotFound();

            var category = _db.Category.Find(id);

            if (category is null)
                return NotFound();

            return View(category);
        }

        //Post - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid) //Валидация на стороне сервера
                return View(category);

            _db.Category.Update(category);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Get - Delete
        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
                return NotFound();

            var category = _db.Category.Find(id);

            if (category is null)
                return NotFound();

            return View(category);
        }

        //Post - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var category = _db.Category.Find(id);

            if (category is null) //Валидация на стороне сервера
                return NotFound();

            _db.Category.Remove(category);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
