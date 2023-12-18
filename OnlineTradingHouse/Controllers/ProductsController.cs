using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineTradingHouse.Models;

namespace OnlineTradingHouse.Controllers
{
    public class ProductsController : Controller
    {
        private OnlineTradingHouseEntities db = new OnlineTradingHouseEntities();

        public ActionResult Index()
        {
            List<Product> productList = new List<Product>();
            int currentUserId = UserInfo.GetUserID(User.Identity.Name);
            if (User.IsInRole("Supplier"))
            {
                productList = db.Products.Where(s => s.SupplierId == currentUserId).ToList();
            }
            else
            {
                productList = db.Products.ToList();
            }
            return View(productList);
        }

        public ActionResult RequestNow(int id)
        {
            int currentUserId = UserInfo.GetUserID(User.Identity.Name);
            try
            {                
                    BuyerRequest request = new BuyerRequest();
                    request.ProductId = id;
                    request.BuyerId = UserInfo.GetUserID(User.Identity.Name);
                    request.CreatedDate = DateTime.Now.Date;
                    request.Status = "Pending";
                    db.BuyerRequests.Add(request);
                    db.SaveChanges();
                    return RedirectToAction("BuyerRequests", "BuyerRequest");
                                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase image)
        {
            product.Date = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength >0)
                {
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(image.FileName));
                        product.Image = image.FileName;
                        image.SaveAs(path);
                        product.SupplierId = UserInfo.GetUserID(User.Identity.Name);
                        db.Products.Add(product);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else
                {
                    ViewBag.Message = "File is not specified.";
                }
            }

            return View(product);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(image.FileName));
                        product.Image = image.FileName;
                        image.SaveAs(path);
                        product.Date = DateTime.Now.Date;
                        product.SupplierId = UserInfo.GetUserID(User.Identity.Name);
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                else
                {
                    ViewBag.Message = "File is not specified.";
                }
            }
            return View(product);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.Products.Remove(product);
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
