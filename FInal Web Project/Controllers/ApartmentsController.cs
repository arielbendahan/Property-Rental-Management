using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FInal_Web_Project.Models;

namespace FInal_Web_Project.Controllers
{
    public class ApartmentsController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Apartments
        public ActionResult Index()
        {
            var apartments = db.Apartments.Include(a => a.Building).Include(a => a.User);
            return View(apartments.ToList());
        }
        public ActionResult ViewApartments()
        {
            var apartments = db.Apartments.Include(a => a.Building).Include(a => a.User);
            return View(apartments.ToList());
        }

        // GET: Apartments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartment apartment = db.Apartments.Find(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // GET: Apartments/Create
        public ActionResult Create()
        {
            ViewBag.BuildingID = new SelectList(db.Buildings, "BuildingID", "Name");
            var managers = db.Users
                             .Where(u => u.Role == "Property Manager")
                             .Select(u => new { u.UserID, FullName = u.FirstName + " " + u.LastName })
                             .ToList();
            ViewBag.ManagerID = new SelectList(managers, "UserID", "FullName");
            ViewBag.Status = new SelectList(new List<string> { "Available" });
            return View();
        }


        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ApartmentID,BuildingID,ManagerID,ApartmentNumber,Status,RentAmount,Description")] Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                db.Apartments.Add(apartment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BuildingID = new SelectList(db.Buildings, "BuildingID", "Name", apartment.BuildingID);
            ViewBag.ManagerID = new SelectList(db.Users, "UserID", "Username", apartment.ManagerID);
            return View(apartment);
        }

        // GET: Apartments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartment apartment = db.Apartments.Find(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuildingID = new SelectList(db.Buildings, "BuildingID", "Name", apartment.BuildingID);
            var managers = db.Users
                             .Where(u => u.Role == "Property Manager")
                             .Select(u => new { u.UserID, FullName = u.FirstName + " " + u.LastName })
                             .ToList();
            ViewBag.ManagerID = new SelectList(managers, "UserID", "FullName", apartment.ManagerID);
            ViewBag.Status = new SelectList(new List<string> { "Available", "Occupied" }, apartment.Status);
            return View(apartment);
        }


        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ApartmentID,BuildingID,ManagerID,ApartmentNumber,Status,RentAmount,Description")] Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(apartment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "The record you attempted to edit was modified by another user after you got the original value. The edit operation was canceled. If you still want to edit this record, click the Save button again.");
                }
            }
            ViewBag.BuildingID = new SelectList(db.Buildings, "BuildingID", "Name", apartment.BuildingID);
            var managers = db.Users
                             .Where(u => u.Role == "Property Manager")
                             .Select(u => new { u.UserID, FullName = u.FirstName + " " + u.LastName })
                             .ToList();
            ViewBag.ManagerID = new SelectList(managers, "UserID", "FullName", apartment.ManagerID);
            ViewBag.Status = new SelectList(new List<string> { "Available", "Occupied" }, apartment.Status);
            return View(apartment);
        }


        // GET: Apartments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartment apartment = db.Apartments.Find(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Apartment apartment = db.Apartments.Find(id);
            db.Apartments.Remove(apartment);
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
