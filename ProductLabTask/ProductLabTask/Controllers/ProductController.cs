using ProductLabTask.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductLabTask.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult AllProducts()
        {
            var db = new ProductEntities();
            var data = db.Products;
            return View(data);
        }

        [HttpGet]
        public ActionResult createProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult createProduct(Product product)
        {
            var db = new ProductEntities();
            db.Products.Add(product);
            db.SaveChanges();
            return RedirectToAction("AllProducts");
        }

        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            var db = new ProductEntities();
            var ex = (from d in db.Products where d.id == id select d).SingleOrDefault();
            return View(ex);
        }

        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            var db = new ProductEntities();
            var exData = db.Products.Find(product.id);
            exData.Name = product.Name;
            exData.Price = product.Price;
            exData.CatId = product.CatId;
            db.SaveChanges();
            return RedirectToAction("AllProducts");
        }

        public ActionResult DeleteProduct(int id)
        {
            var db = new ProductEntities();
            var exData = db.Products.Find(id);
            db.Products.Remove(exData);
            db.SaveChanges();
            return RedirectToAction("AllProducts");
        }

        public ActionResult AllCategories()
        {
            var db = new ProductEntities();
            var data = db.Categories;
            return View(data);
        }

        [HttpGet]
        public ActionResult createCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult createCategory(Category category)
        {
            var db = new ProductEntities();
            db.Categories.Add(category);
            db.SaveChanges();
            return RedirectToAction("AllCategories");
        }
    }
}