using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineTradingHouse.Models;

namespace OnlineTradingHouse.Controllers
{
    public class OrdersController : Controller
    {
        private OnlineTradingHouseEntities db = new OnlineTradingHouseEntities();

        public ActionResult Index()
        {
            List<Order> orderList = new List<Order>();
            int currentUserId = UserInfo.GetUserID(User.Identity.Name);
            if (User.IsInRole("Supplier"))
            {
                orderList = db.Orders.Where(s => s.SupplierId == currentUserId).Include(a => a.BuyerRequest.Product).ToList();
            }
            else if (User.IsInRole("Buyer"))
            {
                orderList = db.Orders.Where(s => s.BuyerRequest.BuyerId == currentUserId).Include(a => a.BuyerRequest.Product).ToList();
            }
            else
            {
                orderList = db.Orders.Include(a=> a.BuyerRequest.Product).ToList();
            }
            //var orders = db.Orders.Include(o => o.BuyerRequest).Include(o => o.User);
            return View(orderList);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }


        [HttpPost]
        public ActionResult Create(int Id)
        {
            var buyerRequest = db.BuyerRequests.Find(Id);
            int? supplierId = buyerRequest.Product.SupplierId;

            if (ModelState.IsValid)
            {
                Order order = new Order();
                order.Status = "Pending";
                order.Date = DateTime.Now;
                order.SupplierId = supplierId;
                order.BuyerRequestId = buyerRequest.Id;

                db.Orders.Add(order);
                buyerRequest.Status = "Proceed";
                db.Entry(buyerRequest).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("BuyerRequests", "BuyerRequest");
            }
            return RedirectToAction("BuyerRequests", "BuyerRequest");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerId = new SelectList(db.Users, "Id", "Name", order.BuyerRequest.BuyerId);
            //ViewBag.SupplierId = new SelectList(db.Users, "Id", "Name", order.SupplierId);
            return View(order);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                order.Date = DateTime.Now;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BuyerRequestId = order.BuyerRequestId;
            ViewBag.SupplierId = new SelectList(db.Users, "Id", "Name", order.SupplierId);
            return View(order);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.Orders.Remove(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
