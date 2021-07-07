using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Web.Security;
using BuildingManagement.Models;

namespace BuildingManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Registration reg)
        {
            if (ModelState.IsValid)
            {
                using (OurDbContext db = new OurDbContext())
                {
                    db.Registrations.Add(reg);
                    db.SaveChanges();
                }
                ModelState.Clear();
                ViewBag.Message = reg.Name + " " + "Successfully registered";
            }
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Registration reg)
        {
            using (OurDbContext db = new OurDbContext())
            {
          
                var user = db.Registrations.FirstOrDefault(u => u.UserName == reg.UserName && u.Password == reg.Password);
                if (user == null)
                {
                    ViewBag.Message = "Wrong details";
                }
                else
                {   
                    Session["userId"] = user.Id.ToString();
                    Session["userName"] = user.UserName.ToString();
                    if (user.IsAdmin == false)
                    {
                        return RedirectToAction("ListRequest", new { id = user.Id });
                    }
                    else
                    {
                        return RedirectToAction("AdminModule", new { id = user.Id });
                    }
                    
                }
            }

            return View();
        }

        public ActionResult LoggedIn()
        {
            if (Session["userId"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        public ActionResult CreateRequest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRequest(MaintenanceRequest obj)
        {
            if (ModelState.IsValid)
            {
              
                string filename = Path.GetFileNameWithoutExtension(obj.UploadImage.FileName);
                string extension = Path.GetExtension(obj.UploadImage.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                obj.ImagePath = "~/images/" + filename;
                filename = Path.Combine(Server.MapPath("../images/"), filename);
                if (obj.UploadImage.ContentLength <= 20000000)
                {
                    obj.UploadImage.SaveAs(filename);
                    obj.UserId = Convert.ToInt32(Session["userId"]);
                    obj.CreateDate = DateTime.UtcNow.Date;
                    obj.Status = "Pending";

                    using (OurDbContext db = new OurDbContext())
                    {
                        db.maintenanceRequests.Add(obj);
                        db.SaveChanges();
                        ViewBag.Message = "Request Submitted Successfully";
                    }
                }
                else
                {
                    ViewBag.Message = "File Size High";
                }
            }
            ModelState.Clear();
            return View();
        }
        public ActionResult ListRequest(int? id)
        {
            using (OurDbContext db = new OurDbContext())
            {
                return View(db.maintenanceRequests.Where(x => x.UserId == id).ToList<MaintenanceRequest>());
            }
        }
        public ActionResult EditRequest(int? id)
        {
            using (OurDbContext db = new OurDbContext())
            {
                var item = db.maintenanceRequests.Find(id);
                Session["imgpath"] = item.ImagePath;
                return View(db.maintenanceRequests.Where(x => x.Id == id).FirstOrDefault());
            }

        }
        [HttpPost]
        public ActionResult EditRequest(int id, MaintenanceRequest obj)
        {

            try
            {
                using (OurDbContext db = new OurDbContext())
                {
                   
                    if (obj.UploadImage != null)
                    {
                        string filename = Path.GetFileNameWithoutExtension(obj.UploadImage.FileName);
                        string extension = Path.GetExtension(obj.UploadImage.FileName);
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                        obj.ImagePath = "~/images/" + filename;
                        filename = Path.Combine(Server.MapPath("~/images/"), filename);
                        obj.UploadImage.SaveAs(filename);
                    }
                    db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("ListRequest", new { id = obj.UserId });
            }
            catch (Exception e)
            {
                return View();
            }
        }


        public ActionResult DeleteRequest(int id)
        {
            using (OurDbContext db = new OurDbContext())
            {
                var item = db.maintenanceRequests.Find(id);
                db.maintenanceRequests.Remove(item);
                db.SaveChanges();
                return RedirectToAction("ListRequest", new { id = item.UserId });
            }
        }
        public ActionResult DetailsOfRequest(int id)
        {
            using (OurDbContext db = new OurDbContext())
            {

                return View(db.maintenanceRequests.Where(x => x.Id == id).FirstOrDefault());
            }
        }
        public ActionResult AdminModule()
        {
            using (OurDbContext db = new OurDbContext())
            {

                return View(db.maintenanceRequests.Include(m => m.UserDetails).ToList());
            }
        }
        public ActionResult Approve(int? id)
        {
            using (OurDbContext db = new OurDbContext())
            {
                var item = db.maintenanceRequests.Find(id);
               
                return View(db.maintenanceRequests.Where(x => x.Id == id).FirstOrDefault());
            }                                
        }
        [HttpPost]
        public ActionResult Approve(int id, MaintenanceRequest obj)
        {
            using (OurDbContext db = new OurDbContext())
            {                            
                db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("AdminModule");
        }
        public ActionResult ViewReport(DateTime? start, DateTime? end)
        {
            using (OurDbContext db = new OurDbContext())
            {
                List<MaintenanceRequest> reg = db.maintenanceRequests.ToList();
                return View(db.maintenanceRequests.Where(x=>x.CreateDate>=start && x.CreateDate<=end).Include(m => m.UserDetails).ToList());
            }
        }
    }
}