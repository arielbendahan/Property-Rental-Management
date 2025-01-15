using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FInal_Web_Project.Models;
using FInal_Web_Project.Controllers;

namespace FInal_Web_Project.Controllers
{
    public class AppointmentsController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Appointments
        public ActionResult Index()
        {
            // Get the logged-in manager's ID
            int managerId = int.Parse(User.Identity.Name);

            // Filter appointments by the logged-in manager's ID
            var appointments = db.Appointments
                                 .Include(a => a.Apartment)
                                 .Include(a => a.User)
                                 .Include(a => a.User1)
                                 .Where(a => a.ManagerID == managerId);

            return View(appointments.ToList());
        }

        // GET: Appointments/MakeAppointment
        public ActionResult MakeAppointment()
        {
            ViewBag.ApartmentID = new SelectList(db.Apartments, "ApartmentID", "ApartmentNumber");
            ViewBag.TenantID = User.Identity.Name;

            var apartments = db.Apartments.ToList();
            ViewBag.Apartments = apartments;


            return View();
        }

        public JsonResult GetManagerByApartment(int apartmentId)
        {
            var manager = db.Apartments
                            .Where(a => a.ApartmentID == apartmentId)
                            .Select(a => new { a.User.UserID, FullName = a.User.FirstName + " " + a.User.LastName })
                            .FirstOrDefault();

            return Json(manager, JsonRequestBehavior.AllowGet);
        }


        // POST: Appointments/MakeAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeAppointment([Bind(Include = "ApartmentID,ManagerID,AppointmentDate,TenantID")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.Status = "Scheduled";

                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Index", "Tenant");
            }

            // Filter managers to show only property managers
            var managers = db.Users.Where(u => u.Role == "Property Manager").ToList();
            ViewBag.Managers = new SelectList(managers, "UserID", "Username");

            ViewBag.ApartmentID = new SelectList(db.Apartments, "ApartmentID", "ApartmentNumber", appointment.ApartmentID);

            return View(appointment);
        }


        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/Create
        public ActionResult Create()
        {
            // Get the logged-in manager's ID
            int managerId = int.Parse(User.Identity.Name);

            // Get tenants with their full names
            var tenants = db.Users
                            .Where(u => u.Role == "Tenant")
                            .Select(u => new { u.UserID, FullName = u.FirstName + " " + u.LastName })
                            .ToList();

            ViewBag.ApartmentID = new SelectList(db.Apartments, "ApartmentID", "ApartmentNumber");
            ViewBag.TenantID = new SelectList(tenants, "UserID", "FullName");
            ViewBag.ManagerID = managerId; // Set the manager ID to the logged-in manager

            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppointmentID,TenantID,ApartmentID,ManagerID,AppointmentDate")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // Set the manager ID to the logged-in manager
                appointment.ManagerID = int.Parse(User.Identity.Name);
                // Set the status to "Scheduled" by default
                appointment.Status = "Scheduled";

                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                }
            }

            // Get tenants with their full names
            var tenants = db.Users
                            .Where(u => u.Role == "Tenant")
                            .Select(u => new { u.UserID, FullName = u.FirstName + " " + u.LastName })
                            .ToList();

            ViewBag.ApartmentID = new SelectList(db.Apartments, "ApartmentID", "ApartmentNumber", appointment.ApartmentID);
            ViewBag.TenantID = new SelectList(tenants, "UserID", "FullName", appointment.TenantID);
            ViewBag.ManagerID = appointment.ManagerID; // Keep the manager ID

            return View(appointment);
        }



        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApartmentID = new SelectList(db.Apartments, "ApartmentID", "ApartmentNumber", appointment.ApartmentID);
            ViewBag.ManagerID = new SelectList(db.Users, "UserID", "Username", appointment.ManagerID);
            ViewBag.TenantID = new SelectList(db.Users, "UserID", "Username", appointment.TenantID);
            ViewBag.Status = new SelectList(new List<string> { "Scheduled", "Completed", "Cancelled" }, appointment.Status);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentID,TenantID,ApartmentID,ManagerID,AppointmentDate,Status")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApartmentID = new SelectList(db.Apartments, "ApartmentID", "ApartmentNumber", appointment.ApartmentID);
            ViewBag.ManagerID = new SelectList(db.Users, "UserID", "Username", appointment.ManagerID);
            ViewBag.TenantID = new SelectList(db.Users, "UserID", "Username", appointment.TenantID);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
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
