using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalExam_1931358;

namespace FinalExam_1931358.Controllers
{
    public class urlsController : Controller
    {
        private dbFinalExam_1931358Entities db = new dbFinalExam_1931358Entities();

        // GET: urls
        public ActionResult Index()
        {
            //Display the list of shorteners for the logged user.
            HttpCookie cookie = Request.Cookies["ConectedCookie"];
            var loggedUserShorteners = from url in db.urls
                                       where url.username == cookie.Value
                                       select url;
            return View(loggedUserShorteners);
        }

        // GET: urls/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            url url = db.urls.Find(id);
            if (url == null)
            {
                return HttpNotFound();
            }
            return View(url);
        }

        // GET: urls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: urls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "short_url,long_url,username")] url url)
        {
            //The shortener will be created for the logged user
            HttpCookie cookie = Request.Cookies["ConectedCookie"];
            url.username = cookie.Value;

            //If the short url is not passed, let's generate one for the user
            if (url.short_url == null || url.short_url == "")
            {
                url.short_url = generateShortener();
            }
            //Check if the beginning of the long url has the correct format
            if (!isValidLongUrl(url.long_url))
            {
                url.long_url = makeValidUrl(url.long_url);
            }
            if (ModelState.IsValid)
            {
                db.urls.Add(url);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(url);
        }

        // GET: urls/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            url url = db.urls.Find(id);
            if (url == null)
            {
                return HttpNotFound();
            }
            return View(url);
        }

        // POST: urls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "short_url,long_url,username")] url url)
        {
            //The shortener will be edited for the logged user
            HttpCookie cookie = Request.Cookies["ConectedCookie"];
            url.username = cookie.Value;
            //Check if the beginning of the long url has the correct format
            if (!isValidLongUrl(url.long_url))
            {
                url.long_url = makeValidUrl(url.long_url);
            }
            if (ModelState.IsValid)
            {
                db.Entry(url).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(url);
        }

        // GET: urls/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            url url = db.urls.Find(id);
            if (url == null)
            {
                return HttpNotFound();
            }
            return View(url);

        }

        // POST: urls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            url url = db.urls.Find(id);
            db.urls.Remove(url);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Go(string shortUrlParameter)
        {
            //This function was made to check the redirect and perform it. It's called from the route config and from the home form.
            //It will check if the parameter was passed correctly and redirect the user to the requested long_url.

            //Check if the parameter passed was null
            if (shortUrlParameter == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Search the url on the database
            url url = db.urls.Find(shortUrlParameter);

            //If the url does not exist, send bad request
            if (url == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Shortener exists
            else
            {
                //Redirect the user to the long url
                return Redirect(url.long_url);
            }
        }
        public string generateShortener()
        {
            //This function was made to generate an automatic shortner in case the user does not input it
            //It will check the last number after the root pattern implemented and increase it by 1
            //By doing this, I can make sure that the short_url generated will always be unique.
 
            string newComplement = "";
            foreach (var item in db.urls.ToList())
            {
                string shortUrl = item.short_url.ToString();
                if (shortUrl.Length >= 7)
                {
                    string root = shortUrl.Substring(0, 9);
                    if (root.Equals("shortener"))
                    {
                        //This code will check the last number of the shortener and add 1, maintaining unique shorteners
                        newComplement = shortUrl.Substring(9);
                        newComplement = "shortener" + (Convert.ToInt32(newComplement) + 1).ToString();
                    }
                    else
                    {
                        newComplement = "shortener1";
                    }
                }
            }
            //If the list does not contain anything, add a new shortener
            if (db.urls.ToList().Count() == 0)
            {
                newComplement = "shortener1";
            }
            return newComplement;
        }
        public bool isValidLongUrl(string long_url)
        {
            //This function was made to check if the long url has the correct pattern.

            //Return the url with the corret pattern
            string pattern = "https://";
            string pattern2 = "http://";
            if (long_url.Substring(0, 8).Equals(pattern) || long_url.Substring(0, 7).Equals(pattern2))
            {
                return true;
            }
            else
            {
                return false;
            }
            //https://www.google.com/
        }
        public string makeValidUrl(string long_url)
        {
            //This function was made to change the invalid long url and put the pattern correctly.
            string pattern = "https://";
            return (pattern + long_url);
        }
        public ActionResult RedirectUrl()
        {
            return Redirect("/Home/Index");
        }
        [HttpPost]
        public ActionResult RedirectUrl(FormCollection fc)
        {
            //This function is used on the form from Home Page
            //When the user enters the shortener and press the button, he is redirect to here where I will validate the input and send him to the
            //Go function, where the redirect call is made
            if (fc["short_url"] == null || fc["short_url"] == "")
            {
                //ViewBag.messageError = "Please, enter a shortener to be redirected.";
                //return Redirect("/Home/Index");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else 
            {
                return Go(fc["short_url"]);
            }
        }
        public ActionResult Generator(string longUrlParameter)
        {
            //This was the attempt of generating the shortener automatically if the user entered a long url directly as GET
            //The problem in this attempt is that I got a dangerous request path error.
            //The idea would be for the user to paste the encoded long_url, on the route config I redirect the user to here and send him to create

            //string long_url = Request.QueryString["long_url"];
            url url = new url();
            url.long_url = longUrlParameter;
            return Create(url);
        }
    }
}
