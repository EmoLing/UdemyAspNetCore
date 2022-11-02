using Microsoft.AspNetCore.Mvc;
using UdemyAspNetCore.Data;
using UdemyAspNetCore.Models;

namespace UdemyAspNetCore.Controllers
{
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
            if (applicationType is null)
                return BadRequest();

            _db.ApplicationType.Add(applicationType);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
