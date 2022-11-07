using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UdemyAspNetCore.Data;
using UdemyAspNetCore.Models;
using UdemyAspNetCore.Models.ViewModels;

namespace UdemyAspNetCore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var objList = _db.Product
                .Include(p => p.Category)
                .Include(p => p.ApplicationType);

            return View(objList);
        }

        //Get - Upsert
        public IActionResult Upsert(int? id)
        {
            var productVM = new ProductViewModel()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                ApplicationTypeSelectList = _db.ApplicationType.Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
            };

            if (id is null)
                return View(productVM);

            productVM.Product = _db.Product.Find(id);

            if (productVM.Product is null)
                return NotFound();

            return View(productVM);
        }

        //POST - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productVM)
        {
            if (!ModelState.IsValid) //Валидация на стороне сервера
            {
                productVM.CategorySelectList = _db.Category.Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

                productVM.ApplicationTypeSelectList = _db.ApplicationType.Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

                return View(productVM);
            }

            var files = HttpContext.Request.Form.Files.ToList();
            string webRootpath = _webHostEnvironment.WebRootPath;

            if (productVM.Product.Id == 0)
            {
                UploadNewImage(productVM, String.Empty, false);

                _db.Product.Add(productVM.Product);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(p => p.Id == productVM.Product.Id);

            if (files.Count > 0)
                UploadNewImage(productVM, objFromDb.Image, true);
            else
                productVM.Product.Image = objFromDb.Image;

            _db.Product.Update(productVM.Product);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Get - Delete
        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
                return NotFound();

            var product = _db.Product
                .Include(p => p.Category)
                .Include(p => p.ApplicationType)
                .FirstOrDefault(p => p.Id == id);

            if (product is null)
                return NotFound();

            return View(product);
        }

        //Post - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var product = _db.Product.Find(id);

            if (product is null) //Валидация на стороне сервера
                return NotFound();

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            string pathToImage = Path.Combine(upload, product.Image);

            if (System.IO.File.Exists(pathToImage))
                System.IO.File.Delete(pathToImage);

            _db.Product.Remove(product);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        private void UploadNewImage(ProductViewModel productVM, string oldImage, bool needDeleteOldImage = false)
        {
            var files = HttpContext.Request.Form.Files.ToList();

            string webRootpath = _webHostEnvironment.WebRootPath;
            string upload = webRootpath + WC.ImagePath;
            string fileName = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(files.FirstOrDefault().FileName);

            var oldFile = Path.Combine(upload, oldImage);

            if (needDeleteOldImage && System.IO.File.Exists(oldFile))
                System.IO.File.Delete(oldFile);

            using var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create);
            files.FirstOrDefault().CopyTo(fileStream);

            productVM.Product.Image = fileName + extension;
        }
    }
}
