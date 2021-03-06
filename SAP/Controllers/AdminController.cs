﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using SAP.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SAP.Attributes;
using Webdiyer.WebControls.Mvc;
namespace SAP.Controllers
{
    [AccessDeniedAuthorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private SAPEntities context = new SAPEntities();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin
        public ActionResult Index(String search, int id = 1)
        {
            var users = db.Users.AsQueryable();
            if (!String.IsNullOrWhiteSpace(search))
            {
                users = users.Where(x => x.FirstName.Contains(search) || x.Email.Contains(search) || x.UserName.Contains(search));
            }
            var model = users.OrderByDescending(x => x.Id).ToPagedList(id, 4);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPartial", model);
            }
            return View(model);
        }
        [Route("{id_clicked}")]
        public ActionResult Statistics(string id_clicked) {

            var interviewerName = context.AspNetUsers.Where(x => x.Id.Equals(id_clicked)).Select(x => x.Email).FirstOrDefault();
            var query = (from paperSurvey in context.PaperSurvey
                         join aspNetUsers in context.AspNetUsers on paperSurvey.id_interviewer equals aspNetUsers.Id
                         join offSurvey in context.OfflineSurvey on paperSurvey.id_offlinesurvey equals offSurvey.Id
                         join survey in context.Survey on offSurvey.Id equals survey.Id
                         where aspNetUsers.Id.Equals(id_clicked)
                         group offSurvey by offSurvey.Id into g
                         select new
                         {
                             g                        
                         }

                          );
            var surveys = new List<InterviewerStatistics>();
            foreach (var t in query) {

                var tmp = t.g.Key;
                var name = context.Survey.Where(x => x.Id.Equals(tmp)).ToList().FirstOrDefault().Name;
              
                var pom = t.g.ToList().Count();
                surveys.Add(new InterviewerStatistics()
                {
                    surveyName = name,                  
                    count = pom,                
                });
            }
            ViewBag.userName = interviewerName;
            return View(surveys);
        }

        // GET: Admin/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.Role = UserManager.GetRoles(applicationUser.Id).FirstOrDefault();
            return View(applicationUser);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existingUser = db.Users.Where(x => x.Email == model.Email).FirstOrDefault();
            if(existingUser != null)
            {
                ModelState.AddModelError("", "This user already exists");
                var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
                return View(model);
            }
            var user = new ApplicationUser
            {
               UserName = model.UserName,
               Email = model.Email
            };
            var id = Guid.Parse(user.Id);
            if (context.UserTokens.Where(x => x.UserID == id).FirstOrDefault() != null){
                ModelState.AddModelError("", "This user already exists");
                var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
                return View(model);
            }
            
            var password = GetPassword();
            var hashedpass = Crypto.HashPassword(password);
            user.PasswordHash = hashedpass;
            user.SecurityStamp = Guid.NewGuid().ToString();
            var token = Guid.NewGuid().ToString();
            db.Users.Add(user);
            db.SaveChanges();

            if (!UserManager.IsInRole(user.Id, model.UserRole))
            {
                UserManager.AddToRole(user.Id, model.UserRole);
            }

            var entry = new UserTokens
            {
                UserID = Guid.Parse(user.Id),
                Token = Guid.Parse(token)
            };
            context.UserTokens.Add(entry);
            context.SaveChanges();
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, protocol: Request.Url.Scheme);
            SendEmail(model.Email, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return RedirectToAction("Index");
        }

        public void SendEmail(string toEmailAddress, string emailSubject, string emailMessage)
        {
            var message = new MailMessage();
            message.To.Add(toEmailAddress);
            message.IsBodyHtml = true;
            message.Subject = emailSubject;
            message.Body = emailMessage;

            using (var smtpClient = new SmtpClient())
            {
                System.Net.NetworkCredential networkCred = new System.Net.NetworkCredential();
                networkCred.UserName = "sap.mail.info@gmail.com";
                networkCred.Password = "Admin123!";
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = networkCred;
                smtpClient.Send(message);
            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            string userRole = db.Roles.Where(x => x.Users.Any(y => y.UserId == applicationUser.Id)).Select(x => x.Name).FirstOrDefault();
            ViewBag.UserRole = new SelectList(list,"Value","Text", userRole);
            var model = new EditUserViewModel
            {
                Age = applicationUser.Age,
                Email = applicationUser.Email,
                FirstName = applicationUser.FirstName,
                EmailConfirmed = applicationUser.EmailConfirmed,
                PhoneNumber = applicationUser.PhoneNumber,
                UserName = applicationUser.UserName,
                UserRole = userRole,
                Id = applicationUser.Id
            };
            return View(model);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var appuser = db.Users.Where(x => x.Id == user.Id).FirstOrDefault();
                    if (appuser == null)
                    {
                        return HttpNotFound();
                    }
                    appuser.Age = user.Age;
                    appuser.FirstName = user.FirstName;
                    appuser.PhoneNumber = user.PhoneNumber;
                    appuser.UserName = user.UserName;

                    db.Entry(appuser).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    if (UserManager.IsInRole(user.Id, "Admin"))
                    {
                        if (user.UserRole != "Admin")
                        {
                            UserManager.RemoveFromRole(user.Id, "Admin");
                            UserManager.AddToRole(user.Id, "Interviewer");
                        }

                    }
                    else
                    {
                        if (user.UserRole != "Interviewer")
                        {
                            UserManager.RemoveFromRole(user.Id, "Interviewer");
                            UserManager.AddToRole(user.Id, "Admin");
                        }
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                    string userRole = db.Roles.Where(x => x.Users.Any(y => y.UserId == user.Id)).Select(x => x.Name).FirstOrDefault();
                    ViewBag.UserRole = new SelectList(list, "Value", "Text", userRole);
                    ModelState.AddModelError("", "An error occured during update of user. Please try again!");
                    return View(user);
                }
            }
            return View(user);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Where(x => x.Id == id).FirstOrDefault();
            var addToSurvey = context.AddToSurvey.Where(x => x.Id_interviewer == id).ToList();
            var paperSurvey = context.PaperSurvey.Where(x => x.id_interviewer == id).ToList();
            var sendsSurvey = context.SendsSurvey.Where(x => x.id_interviewer == id).ToList();
            if(addToSurvey.Count() != 0 || paperSurvey.Count() != 0 || sendsSurvey.Count() != 0)
            {
                ViewBag.canDelete = false;
                return View(applicationUser);
            }
            //foreach (var a in addToSurvey)
            //{
            //    context.AddToSurvey.Remove(a);
            //}
            //foreach (var a in paperSurvey)
            //{
            //    foreach(var ans in a.OfflineAnswer)
            //    {
            //        context.PaperSurvey.Remove(a);
            //    }
            //    context.PaperSurvey.Remove(a);

            //}
            //foreach (var a in sendsSurvey)
            //{
            //    context.SendsSurvey.Remove(a);
            //}
            //context.SaveChanges();
            db.Users.Remove(applicationUser);
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

        public static string GetPassword()
        {
            Random random = new Random();
            const string numbers = "0123456789";
            const string lowerLetters = "abcdefghijklmnopqrstuvwxyz";
            const string upperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string specialLetters = ",.!*&$@#";
            int length = random.Next(1, 3);
            string numberString = new string(Enumerable.Repeat(numbers, length).Select(s => s[random.Next(s.Length)]).ToArray());
            length = random.Next(3, 4);
            string lowerString = new string(Enumerable.Repeat(lowerLetters, length).Select(s => s[random.Next(s.Length)]).ToArray());
            length = random.Next(3, 4);
            string upperString = new string(Enumerable.Repeat(upperLetters, length).Select(s => s[random.Next(s.Length)]).ToArray());
            length = random.Next(1, 2);
            string specialString = new string(Enumerable.Repeat(specialLetters, length).Select(s => s[random.Next(s.Length)]).ToArray());
            string finalString = numberString + lowerString + upperString + specialString;
            char[] array = finalString.ToCharArray();
            Random rng = new Random();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);

        }
    }
}
