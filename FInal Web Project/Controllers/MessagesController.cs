using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FInal_Web_Project.Models;
using Microsoft.AspNet.Identity;

namespace FInal_Web_Project.Controllers
{
    public class MessagesController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Messages
        public ActionResult Index()
        {
            var userId = int.Parse(User.Identity.Name); // Get the logged-in user's ID

            var messages = db.Messages
                             .Include(m => m.User)
                             .Include(m => m.User1)
                             .Where(m => m.ReceiverID == userId); // Filter messages by ReceiverID

            return View(messages.ToList());
        }

        // GET: Messages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // GET: Messages/Create
        public ActionResult Create()
        {
            var propertyManagers = db.Users
                .Where(u => u.Role == "Property Manager")
                .Select(u => new { u.UserID, FullName = u.FirstName + " " + u.LastName })
                .ToList();
            ViewBag.ReceiverID = new SelectList(propertyManagers, "UserID", "Fullname");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MessageID,SenderID,ReceiverID,Content")] Message message)
        {
            if (ModelState.IsValid)
            {
                message.SenderID = int.Parse(User.Identity.Name); // Set the SenderID to the logged-in user's ID
                db.Messages.Add(message);
                db.SaveChanges();
                string userType = Request.Cookies["Type"].Value;
                if (userType == "PropertyManager")
                {
                    return RedirectToAction("Index", "Manager");
                }
                else if (userType == "Tenant")
                {
                    return RedirectToAction("Index", "Tenant");
                }
                else if (userType == "PropertyAdmin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index");
            }

            ViewBag.ReceiverID = new SelectList(db.Users, "UserID", "Username", message.ReceiverID);
            return View(message);
        }

        // GET: Messages/MessageAdmin
        public ActionResult MessageAdmin()
        {
            var propertyAdmins = db.Users
                                   .Where(u => u.Role == "Property Admin")
                                   .Select(u => new { u.UserID, FullName = u.FirstName + " " + u.LastName })
                                   .ToList();
            ViewBag.ReceiverID = new SelectList(propertyAdmins, "UserID", "FullName");
            return View();
        }

        // POST: Messages/MessageAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MessageAdmin([Bind(Include = "MessageID,SenderID,ReceiverID,Content")] Message message)
        {
            if (ModelState.IsValid)
            {
                message.SenderID = int.Parse(User.Identity.Name); // Set the SenderID to the logged-in user's ID
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var propertyAdmins = db.Users
                                   .Where(u => u.Role == "Property Admin")
                                   .Select(u => new { u.UserID, FullName = u.FirstName + " " + u.LastName })
                                   .ToList();
            ViewBag.ReceiverID = new SelectList(propertyAdmins, "UserID", "FullName", message.ReceiverID);
            return View(message);
        }

        // GET: Messages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            var propertyManagers = db.Users.Where(u => u.Role == "Property Manager").ToList();
            ViewBag.ReceiverID = new SelectList(propertyManagers, "UserID", "Username", message.ReceiverID);
            ViewBag.SenderID = new SelectList(db.Users, "UserID", "Username", message.SenderID);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MessageID,SenderID,ReceiverID,Content")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(message).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReceiverID = new SelectList(db.Users, "UserID", "Username", message.ReceiverID);
            ViewBag.SenderID = new SelectList(db.Users, "UserID", "Username", message.SenderID);
            return View(message);
        }

        // GET: Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
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
    }
}
