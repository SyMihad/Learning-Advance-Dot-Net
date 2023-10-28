using ProductLabTask.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ProductLabTask.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCustomer(Customer customer)
        {
            var db = new ProductEntities();
            db.Customers.Add(customer);
            db.SaveChanges();
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var db = new ProductEntities();
            var allCustomers = db.Customers.ToList();
            var data = new Customer();
            bool sucess = false;
            foreach (var customer in allCustomers)
            {
                if(customer.Email == email && customer.Password == password)
                {
                    data = customer;
                    sucess = true;
                    Session["id"] = customer.id;
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

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult ShowProducts()
        {
            var db = new ProductEntities();
            var data = db.Products.ToList();
            return View(data);
        }

        public ActionResult AddToCart(int id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            HttpCookie ExistingCookie = Request.Cookies["CartCookie"];
            if (ExistingCookie != null)
            {
                string cookieValue = ExistingCookie.Value;
                List<int> oldValues = serializer.Deserialize<List<int>>(cookieValue);
                oldValues.Add(id);
                string sendList = serializer.Serialize(oldValues);
                ExistingCookie.Value = sendList;
                Response.Cookies.Add(ExistingCookie);
            }
            else
            {
                List<int> values = new List<int>();
                values.Add(id);
                string sendList = serializer.Serialize(values);
                HttpCookie CartCookie = new HttpCookie("CartCookie");
                CartCookie.Value = sendList;Response.Cookies.Add(CartCookie);
            }
            return RedirectToAction("ShowProducts");
        }

        public ActionResult ShowCart()
        {
            var db = new ProductEntities();
            List<Product> allProducts = db.Products.ToList();
            List<Product> cartProducts = new List<Product>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            HttpCookie ExistingCookie = Request.Cookies["CartCookie"];
            if (ExistingCookie != null)
            {
                string cookieValue = ExistingCookie.Value;
                List<int> oldValues = serializer.Deserialize<List<int>>(cookieValue);
                foreach(var product in allProducts)
                {
                    foreach(var id in oldValues)
                    {
                        if(product.id == id)
                        {
                            cartProducts.Add(product);
                        }
                    }
                }
                return View(cartProducts);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ConfirmOrders()
        {
            var db = new ProductEntities();
            List<CustomerOrder> allOrders = new List<CustomerOrder>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            HttpCookie ExistingCookie = Request.Cookies["CartCookie"];
            if (ExistingCookie != null)
            {
                string cookieValue = ExistingCookie.Value;
                List<int> oldValues = serializer.Deserialize<List<int>>(cookieValue);
                foreach (int id in oldValues)
                {
                    CustomerOrder customerOrder = new CustomerOrder();
                    customerOrder.PId = id;
                    customerOrder.CId = (int)Session["id"];
                    allOrders.Add(customerOrder);
                }
                foreach (CustomerOrder customerOrder in allOrders)
                {
                    db.CustomerOrders.Add(customerOrder);
                }
                db.SaveChanges();
                return RedirectToAction("AllOrders");
            }
            else
            {
                return RedirectToAction("ShowProducts");
            }
        }

        public ActionResult AllOrders()
        {
            var db = new ProductEntities();
            List<CustomerOrder> customerOrders = new List<CustomerOrder>();
            List<CustomerOrder> allOrders = db.CustomerOrders.ToList();
            int customerId = (int)Session["id"];
            foreach (var order in allOrders)
            {
                if(order.CId == customerId)
                {
                    customerOrders.Add(order);
                }
            }
            return View(customerOrders);
        }
    }
}