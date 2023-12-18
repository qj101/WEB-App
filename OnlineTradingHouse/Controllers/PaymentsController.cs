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
    public class PaymentsController : Controller
    {
        private OnlineTradingHouseEntities db = new OnlineTradingHouseEntities();

        // GET: Payments
        public ActionResult Index()
        {
            List<Payment> paymentList = new List<Payment>();
            int currentUserId = UserInfo.GetUserID(User.Identity.Name);
            if (User.IsInRole("Supplier"))
            {
                paymentList = db.Payments.Where(s => s.SupplierId == currentUserId).ToList();
            }
            else if (User.IsInRole("Buyer"))
            {
                paymentList = db.Payments.Where(s => s.Order.BuyerRequest.BuyerId == currentUserId).ToList();
            }
            else
            {
                paymentList = db.Payments.ToList();
            }
           // var payments = db.Payments.Include(p => p.Order).Include(p => p.User);
            return View(paymentList);
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        [HttpPost]
        public ActionResult Create(int Id)
        {
            var order = db.Orders.Find(Id);
            Payment payment= new Payment();
            if (order != null)
            {
            int? supplierId = order.BuyerRequest.Product.SupplierId;

                if (ModelState.IsValid)
                {
                    payment.SupplierId = supplierId;
                    payment.Date = DateTime.Now;
                    payment.OrderId = order.Id;
                    payment.Amount = order.BuyerRequest.Product.Price;
                    payment.Status = "Done";
                    db.Payments.Add(payment);
                    order.Status = "Paid";
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                }            
            }
            return RedirectToAction("Index", "Orders");
        }

        //// GET: Payments/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Payment payment = db.Payments.Find(id);
        //    if (payment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.OrderId = new SelectList(db.Orders, "Id", "Status", payment.OrderId);
        //    ViewBag.SupplierId = new SelectList(db.Users, "Id", "Name", payment.SupplierId);
        //    return View(payment);
        //}

        //// POST: Payments/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,OrderId,Amount,SupplierId,Status,Date")] Payment payment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(payment).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.OrderId = new SelectList(db.Orders, "Id", "Status", payment.OrderId);
        //    ViewBag.SupplierId = new SelectList(db.Users, "Id", "Name", payment.SupplierId);
        //    return View(payment);
        //}


        //// GET: Payments/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Payment payment = db.Payments.Find(id);
        //    if (payment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(payment);
        //}

        //// POST: Payments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Payment payment = db.Payments.Find(id);
        //    db.Payments.Remove(payment);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
