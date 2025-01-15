using FInal_Web_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FInal_Web_Project.Controllers
{
    public class AccountsController : Controller
    {
        // GET: Accounts
        public ActionResult Index()
        {
            return View();
        }

        // GET: Accounts/Register
        public ActionResult Login()
        {
            return View();
        }

        // POST: Accounts/Register
        [HttpPost]
        public ActionResult Login(User model)
        {
            using (PropertyRentalManagementDBEntities context = new PropertyRentalManagementDBEntities())
            {
                bool isValidUser = context.Users.Any(user => user.Username == model.Username && user.Password == model.Password);
                if (isValidUser)
                {
                    var user = context.Users.FirstOrDefault(u => u.Username == model.Username);
                    FormsAuthentication.SetAuthCookie(user.UserID.ToString(), false); // Sets User.Identity.Name to the user's ID

                    switch (user.Role)
                    {
                        case "Property Admin":
                            HttpCookie cookie = new HttpCookie("Type", "PropertyAdmin");
                            Response.Cookies.Add(cookie);
                            return RedirectToAction("Index", "Admin");
                        case "Property Manager":
                            HttpCookie cookie2 = new HttpCookie("Type", "PropertyManager");
                            Response.Cookies.Add(cookie2);
                            return RedirectToAction("Index", "Manager");
                        case "Tenant":
                            HttpCookie cookie3 = new HttpCookie("Type", "Tenant");
                            Response.Cookies.Add(cookie3);
                            return RedirectToAction("Index", "Tenant");
                        default:
                            ModelState.AddModelError("", "User is not an Property Admin, Property Manager or Tenant");
                            return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password is incorrect.");
                    return View();
                }
            }

        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Response.Cookies["UserType"].Value = null;
            Response.Cookies["UserType"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Login");
        }
    }
}