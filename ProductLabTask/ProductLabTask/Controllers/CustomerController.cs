using ProductLabTask.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ProductLabTask.Auth;
using ProductLabTask.DTO;

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

        [Logged]
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
            AllOrderClass allOrderClass = new AllOrderClass();
            List<Product> allProducts = db.Products.ToList();
            List<ProductOrderDTO> cartProducts = new List<ProductOrderDTO>();
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
                            ProductOrderDTO productOrderDTO = new ProductOrderDTO();
                            productOrderDTO.Pid = product.id;
                            productOrderDTO.Name = product.Name;
                            productOrderDTO.Price = product.Price;
                            productOrderDTO.Quantity = 1;
                            cartProducts.Add(productOrderDTO);
                        }
                    }
                }
                allOrderClass.AllOrders = cartProducts;
                return View(allOrderClass);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ConfirmOrders(AllOrderClass allOrderClass)
        {
            /*var db = new ProductEntities();
            CustomerOrder customerOrder = new CustomerOrder();
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            HttpCookie ExistingCookie = Request.Cookies["CartCookie"];
            if (ExistingCookie != null)
            {

                string cookieValue = ExistingCookie.Value;
                List<int> oldValues = serializer.Deserialize<List<int>>(cookieValue);
                foreach (int id in oldValues)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OId = customerOrder.id;
                    
                }
                foreach (OrderDetail orderDetail in orderDetails)
                {
                    db.OrderDetails.Add(orderDetail);
                }
                db.SaveChanges();
                return RedirectToAction("AllOrders");
            }
            else
            {
                return RedirectToAction("ShowProducts");
            }*/
            var db = new ProductEntities();
            CustomerOrder customerOrder = new CustomerOrder();
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            customerOrder.CId = (int)Session["id"];
            customerOrder.Status = "Ordered";
            customerOrder.Date = DateTime.Now;
            db.CustomerOrders.Add(customerOrder);
            db.SaveChanges();
            int orderId = customerOrder.id;

            foreach(var order in allOrderClass.AllOrders)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OId = orderId;
                orderDetail.PId = order.Pid;
                orderDetail.Price = order.Price;
                orderDetail.Quantity = order.Quantity;
                db.OrderDetails.Add(orderDetail);
            }
            db.SaveChanges();
            return RedirectToAction("Home");
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