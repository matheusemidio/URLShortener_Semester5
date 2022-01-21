using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalExam_1931358.Controllers
{
    public class HomeController : Controller
    {
        private dbFinalExam_1931358Entities db = new dbFinalExam_1931358Entities();
        public ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies["ConectedCookie"];
            //User IS logged in
            if (cookie != null)
            {
                log log = db.logs.Find(cookie.Value);
                ViewBag.message = "Welcome " +  log.username + "!!\nYour last visit was on " + log.lastVisit + ".\nThank you for returning.\n";
                return View();
            }
            //User IS NOT logged in
            else
            {
                return View();
            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}