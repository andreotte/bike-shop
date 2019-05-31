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
                u.Password == password);

            if(output.Count() > 0)
            {
                var loginMatch = output.ToList().First();
                Session["LoggedInUser"] = loginMatch;
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
            return View(items);
        }

        public ActionResult Buy(int ItemID)
        {
            Item item = db.Items.Find(ItemID);

            User loggedInUser = (User)Session["LoggedInUser"];
            if(item.Price < loggedInUser.Money)
            {
                TempData["ErrorMessage"] = "You don't have enough money for that purchase.";
                return RedirectToAction("ErrorPage");
            }
            else if(item.Quantity <= 0)
            {
                TempData["ErrorMessage"] = "Sorry, out of stock.";
                return RedirectToAction("ErrorPage");
            }
            else
            {
                loggedInUser.Money -= item.Price;
                item.Quantity -= 1;

                UserItem ui = new UserItem(); 

                ui.UserID = loggedInUser.ID;
                ui.ItemID = item.ID;
                db.UserItems.Add(ui);
                db.Users.AddOrUpdate(loggedInUser);
                db.SaveChanges();
            }

            return RedirectToActionPermanent("Index", "UserItems");
        }

        public ActionResult ErrorPage()
        {
            return View();
        }
    }
}