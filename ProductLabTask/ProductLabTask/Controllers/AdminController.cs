using ProductLabTask.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductLabTask.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var db = new ProductEntities();
            var allAdmins = db.Admins.ToList();
            var data = new Admin();
            bool sucess = false;
            foreach (var admin in allAdmins)
            {
                if (admin.Email == email && admin.Password == password)
                {
                    data = admin;
                    sucess = true;
                    Session["id"] = admin.id;
                }
            }

            if (sucess)
            {
                return RedirectToAction("Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        // GET: Admin
        public ActionResult Home()
        {
            return View();
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

        public ActionResult AllProducts()
        {
            var db = new ProductEntities();
            var data = db.Products;
            return View(data);
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

        [HttpGet]
        public ActionResult allOrders()
        {
            var db = new ProductEntities();
            var data = db.CustomerOrders;
            return View(data);
        }

        [HttpGet]
        public ActionResult confirmOrder(int id)
        {
            var db = new ProductEntities();
            var order = db.CustomerOrders.Find(id);
            order.Status = "Confirmed";
            db.SaveChanges();
            return RedirectToAction("allOrders");
        }
    }
}