using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BikeShop.Models;

namespace BikeShop.Controllers
{
    public class HomeController : Controller
    {
        ShopDBEntities db = new ShopDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MakeNewUser(User u)
        {
            db.Users.Add(u);
            db.SaveChanges();

            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            List<User> users = db.Users.ToList();

            var output = users.Where(u => 
                u.UserName == userName && 
                u.Password == password);
            Session["LoggedInUser"] = output;

            if (output.Count() != 0)
            {
                return RedirectToAction("Shop");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Shop()
        {
            return View();
        }
    }
}