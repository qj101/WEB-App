using OnlineTradingHouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineTradingHouse.Controllers
{
    public class HomeController : Controller
    {

        private OnlineTradingHouseEntities db = new OnlineTradingHouseEntities();
        // GET: Home
        public ActionResult Index()
        {
            var list = db.Products;
            var list2 = list.Where(x => x.IsHomePageDisplay == true).ToList();
            
            return View(list2);
        }
    }
}