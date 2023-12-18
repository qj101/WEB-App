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
    public class BuyerFeedbacksController : Controller
    {
        private OnlineTradingHouseEntities db = new OnlineTradingHouseEntities();

        // GET: BuyerFeedbacks
        public ActionResult Index()
        {
            List<BuyerFeedback> feedbackList = new List<BuyerFeedback>();
            int currentUserId = UserInfo.GetUserID(User.Identity.Name);
            if (User.IsInRole("Buyer"))
            {
                feedbackList = db.BuyerFeedbacks.Where(s => s.Order.BuyerRequest.BuyerId == currentUserId).Include(b => b.User).ToList();
            }
            else if (User.IsInRole("Supplier"))
            {
                feedbackList = db.BuyerFeedbacks.Where(s => s.Order.BuyerRequest.Product.SupplierId == currentUserId).Include(b => b.User).ToList();
            }
            else
            {
                feedbackList= db.BuyerFeedbacks.Include(b => b.Order).Include(b => b.User).ToList(); 
            }
            return View(feedbackList);
        }

        // GET: BuyerFeedbacks/Create
        public ActionResult Create(int Id)
        {
            TempData["Id"] = Id;
            
            return View();
        }

        [HttpPost]
        public ActionResult Create(string Feekback)
        {
            BuyerFeedback buyerFeedback = new BuyerFeedback();

            int Id = Convert.ToInt32(TempData["Id"]);
            var order = db.Orders.Find(Id);
            if (order != null)
            {
                buyerFeedback.OrderId = order.Id;
                buyerFeedback.BuyerId = order.User.Id;
                buyerFeedback.Date = DateTime.Now;
                buyerFeedback.Feekback = Feekback;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                db.BuyerFeedbacks.Add(buyerFeedback);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(buyerFeedback);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BuyerFeedback feedback = db.BuyerFeedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerId = feedback.BuyerId;
            ViewBag.OrderId = feedback.OrderId;
            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BuyerFeedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.Date = DateTime.Now;
                db.Entry(feedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BuyerId = feedback.BuyerId;
            ViewBag.OrderId = feedback.OrderId;
            return View(feedback);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BuyerFeedback feedback = db.BuyerFeedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.BuyerFeedbacks.Remove(feedback);
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
