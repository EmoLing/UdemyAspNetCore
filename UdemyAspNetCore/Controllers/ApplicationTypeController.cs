using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UdemyAspNetCore.Data;
using UdemyAspNetCore.Models;

namespace UdemyAspNetCore.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ApplicationTypeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.ApplicationType);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType applicationType)
        {
            if (!ModelState.IsValid)
                return View(applicationType);

            _db.ApplicationType.Add(applicationType);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0)
                return NotFound();

            var applicationType = _db.ApplicationType.Find(id);

            if (applicationType is null)
                return NotFound();

            return View(applicationType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType applicationType)
        {
            if (!ModelState.IsValid)
                return View(applicationType);

            _db.ApplicationType.Update(applicationType);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
                return NotFound();

            var applicationType = _db.ApplicationType.Find(id);

            if (applicationType is null)
                return NotFound();

            return View(applicationType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id is null || id == 0)
                return NotFound();

            var applicationType = _db.ApplicationType.Find(id);

            if (applicationType is null)
                return NotFound();

            _db.ApplicationType.Remove(applicationType);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
