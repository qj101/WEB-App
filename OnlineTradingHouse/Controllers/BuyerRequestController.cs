using OnlineTradingHouse.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OnlineTradingHouse.Controllers
{
    public class BuyerRequestController : Controller
    {
        private OnlineTradingHouseEntities db = new OnlineTradingHouseEntities();
        public ActionResult BuyerRequests()
        {
            List<BuyerRequest> buyerRequestList = new List<BuyerRequest>();
            int currentUserId = UserInfo.GetUserID(User.Identity.Name);
            if (User.IsInRole("Buyer"))
            {
                buyerRequestList = db.BuyerRequests.Where(u => u.BuyerId == currentUserId).ToList();
            }
            else
            {
                buyerRequestList = db.BuyerRequests.ToList();
            }

            return View(buyerRequestList);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BuyerRequest buyerRequest = db.BuyerRequests.Find(id);
            if (buyerRequest == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Title", buyerRequest.ProductId);
            return View(buyerRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BuyerRequest buyerRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(buyerRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Title", buyerRequest.ProductId);
            return View(buyerRequest);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BuyerRequest request = db.BuyerRequests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.BuyerRequests.Remove(request);
                db.SaveChanges();
                return RedirectToAction("BuyerRequests");
            }
        }


    }
}