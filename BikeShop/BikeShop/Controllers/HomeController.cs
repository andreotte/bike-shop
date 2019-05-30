using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

        public ActionResult LogOut()
        {
            if(Session["LoggedInUser"] != null)
            {
                Session["LoggedInUser"] = null;
                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("Login");
            }
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
                u.Password == password).ToList().First();
            //this isn't a linq cluster, so now I need to make some changes...
            Session["LoggedInUser"] = output;
            
            if (output != null)
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
            var items = db.Items.ToArray();
            //Session["inStock"] = items;
            return View(items);
        }

        public ActionResult Buy(int ItemID)
        {
            Item item = db.Items.Find(ItemID);

            //IEnumerable<User> user = (IEnumerable<User>)Session["LoggedInUser"];
            //List<User> users = user.ToList();
            //User loggedInUser = users[0];

            //This cast breaks it, but I'm not totally sure why
            User loggedInUser = (User)Session["LoggedInUser"];

            if (item.Price < loggedInUser.Money && item.Quantity > 0)
            {
                loggedInUser.Money -= item.Price;
                item.Quantity -= 1;
                db.Users.AddOrUpdate(loggedInUser);
                db.SaveChanges();
            }
            else
            {
                TempData["ErrorMessage"] = "You don't have enough money for that purchase.";
                return RedirectToAction("ErrorPage");
            }
            return RedirectToAction("Shop");
        }

        public ActionResult ErrorPage()
        {
            return View();
        }
    }
}