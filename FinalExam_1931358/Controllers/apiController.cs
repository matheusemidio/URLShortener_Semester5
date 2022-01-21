using FinalExam_1931358.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalExam_1931358.Controllers
{
    public class apiController : Controller
    {
        // GET: api
        private dbFinalExam_1931358Entities db = new dbFinalExam_1931358Entities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create(string username, string password, string short_url, string long_url)
        {
            //Since it was not mandatory to choose POST and it's a little bit more complicated to test it, since I would need to 
            //go to POSTMAN to verify, I decided to receive the parameters via GET because I feel it's easier to test.

            //Test Link
            //https://localhost:44354/api/Create?username=matheus&password=12345&short_url=bbc&long_url=https%3A%2F%2Fwww.bbc.com%2F

            username = Request.QueryString["username"];
            password = Request.QueryString["password"];
            short_url = Request.QueryString["short_url"];
            long_url = Request.QueryString["long_url"];
            Response response = new Response();
            if (hasOtherErrors(username, password, short_url, long_url))
            {
                response.status_code = "4";
                response.status_description = "Unknown error";
            }
            else if (shortenerAlreadyExists(short_url))
            {
                response.status_code = "2";
                response.status_description = "Short URL already exists.";
            }
            else if (!isValidAuthentication(username, password))
            {
                response.status_code = "3";
                response.status_description = "Authentication failed.";
            }
            else
            {
                response.status_code = "1";
                response.status_description = "OK";

                //Now that everything is okay, perform the operations
                //Create the cookie 
                HttpCookie cookie = new HttpCookie("ConectedCookie");
                cookie.Value = username;
                cookie.Path = Request.ApplicationPath;
                Response.Cookies.Add(cookie);
                //Create the url 
                url url = new url();
                url.username = username;
                url.short_url = short_url;
                url.long_url = long_url;
                //Make valid long url
                if (!isValidLongUrl(url.long_url))
                {
                    url.long_url = makeValidUrl(url.long_url);
                }
                if (ModelState.IsValid)
                {
                    db.urls.Add(url);
                    db.SaveChanges();
                }
            }
            //Return the JSON expression with the response.
            return Json(new
            {
                status_code = response.status_code,
                status_description = response.status_description
            }, JsonRequestBehavior.AllowGet);
        }

        public bool shortenerAlreadyExists(string short_url)
        {
            url url = db.urls.Find(short_url);
            //Shortener does not exist
            if (url == null)
            {
                return false;
            }
            //Shortener already exists
            else
            {
                return true;
            }

        }
        public bool isValidAuthentication(string username, string password)
        { 
            user user = db.users.Find(username);

            //User does not exist
            if (user == null)
            {
                return false;
            }
            //User exists
            else
            {
                //Check password
                if (user.password.Equals(password))
                {
                    //Valid authentication
                    return true;
                }
                //Password was wrong
                else
                {
                    return false;
                }
            }
        }
        public bool hasOtherErrors(string username, string passowrd, string short_url, string long_url)
        {
            //Another possible errors that still need to be checked is if the fields are empty or null
            if (username == null || username == "")
            {
                return true;
            }
            else if(passowrd == null || passowrd == "")
            {
                return true;
            }
            else if (short_url == null || short_url == "")
            {
                return true;
            }
            else if (long_url == null || long_url == "")
            {
                return true;
            }
            return false;
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
    }
}