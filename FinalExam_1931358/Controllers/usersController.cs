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
    public class usersController : Controller
    {
        private dbFinalExam_1931358Entities db = new dbFinalExam_1931358Entities();

        // GET: users
        public ActionResult Index()
        {
            return View(db.users.ToList());
        }

        // GET: users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "username,password")] user user)
        {
            if (ModelState.IsValid)
            {
                db.users.Add(user);
                db.SaveChanges();
                //When the user is created, he still needs to log, so I'm redirecting him to the correct screen.
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "username,password")] user user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            user user = db.users.Find(id);
            db.users.Remove(user);
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            //Check if the user inputted something on the form text field
            if (fc["username"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Search on the database for the inputted username
            user user = db.users.Find(fc["username"]);

            //Check if the database already has that username
            if (user == null)
            {
                ViewBag.message = "User and password are not not a match. Please try again.";
            }

            //The username was found on the database
            else
            {
                //Now it's necessary to check if the password is correct
                if(user.password.Equals(fc["password"]))
                {
                    //Password matches
                    //Create the cookie
                    HttpCookie cookie = new HttpCookie("ConectedCookie");
                    cookie.Value = fc["username"];
                    cookie.Path = Request.ApplicationPath;

                    //Check if the user clicked on the remember me
                        //With the debugger, I was able to check the variety of content for the fc["rememberMe"]
                            //1)Clicked -> fc["rememberMe"] = "true,false"
                            //2)Not Clicked -> fc["rememberMe"] = "false"
                    List<string> rememberMeAnswer = fc["rememberMe"].Split(',').ToList<string>();
                    if (rememberMeAnswer[0].Equals("true"))
                    {
                        //If the remember me was clicked, the cookie will be valid for 7 days (a week)
                        cookie.Expires = DateTime.Now.AddDays(7);
                    }

                    //If the log was never created, I will create one at this moment, but updated it when the user logs out
                    log log = db.logs.Find(cookie.Value);
                    if (log == null)
                    {
                        //Log was never created, so I need to create a new one
                        if (ModelState.IsValid)
                        {
                            log = new log();
                            log.username = cookie.Value;
                            log.lastVisit = DateTime.Now;
                            db.logs.Add(log);
                            db.SaveChanges();
                        }
                    }

                    //Add the cookie
                    Response.Cookies.Add(cookie);

                    //Redirect the user
                    return Redirect("/Home/Index");
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            //Request the cookie
            HttpCookie cookie = Request.Cookies["ConectedCookie"];
            //If the cookie does not exist, means the person is not logged in
            if (cookie != null)
            {
                //Before deleting the cookie, I'm going to update the log with the time of this visit.
                //Create the log
                log log = db.logs.Find(cookie.Value);
                if (log == null)
                {
                    //Just in case of something bad happens, but the code should never reach this part since the log is created at log time
                    //when the user logs for the first time.
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    //Log already existed. The last visit needs to be updated.
                    if (ModelState.IsValid)
                    {
                        log.lastVisit = DateTime.Now;
                        db.Entry(log).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                cookie.Expires = DateTime.Now.AddDays(-1);
                cookie.Path = Request.ApplicationPath;
                Response.Cookies.Add(cookie);
            }
            return Redirect("/Home/Index");
        }
    }
}
