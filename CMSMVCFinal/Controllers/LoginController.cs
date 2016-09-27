using CMSMVCFinal.Models;
using CMSMVCFinal.Models.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CMSMVCFinal.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        private CMSDBFinalEntities _db = new CMSDBFinalEntities();

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(tbl_User t)
        {
            try
            {
                tbl_User tu = _db.tbl_User.Single(u => u.UserId == t.UserId);
                tu.Username = t.Username;
                tu.Email = t.Email;
                tu.Password = t.Password;
                _db.SaveChanges();
                return RedirectToAction("Login", "Login");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Couldnt Succeed in changing password");
            }

            return View();
        }

        public ActionResult iRetrieve()
        {
            return View();
        }

        [HttpPost]
        public ActionResult iRetrieve(tbl_Retrieve tb)
        {
            try
            {
                tbl_Retrieve tr = _db.tbl_Retrieve.Single(u => u.Code == tb.Code);
                if (tr != null)
                {
                    tbl_User tu = _db.tbl_User.Single(u => u.Email == tr.Email);

                    return RedirectToAction("ChangePassword", "Login", tu);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult iForgot()
        {
            return View();
        }

        public static class RandomGen2
        {
            private static Random _global = new Random();

            [ThreadStatic]
            private static Random _local;

            public static int Next()
            {
                Random inst = _local;
                if (inst == null)
                {
                    int seed;
                    lock (_global) seed = _global.Next();
                    _local = inst = new Random(seed);
                }
                return inst.Next();
            }
        }

        [HttpPost]
        public ActionResult iForgot(MailModel objModelMail, HttpPostedFileBase fileUploader)
        {
            try
            {
                tbl_User tu = _db.tbl_User.Single(u => u.Email == objModelMail.To);
                if (tu != null)
                {
                    var rand = RandomGen2.Next();
                    tbl_Retrieve t = new tbl_Retrieve();
                    t.Code = rand.ToString();
                    t.Email = objModelMail.To;
                    _db.tbl_Retrieve.Add(t);
                    _db.SaveChanges();
                    if (ModelState.IsValid)
                    {
                        objModelMail.Body = "Hello, The retrieval Code is" + " " + rand;
                        objModelMail.Subject = "Regarding Forgot password of Swornim CMS";
                        string from = "testSwornim@gmail.com";
                        using (MailMessage mail = new MailMessage(from, objModelMail.To))
                        {
                            try
                            {
                                mail.Subject = objModelMail.Subject;
                                mail.Body = objModelMail.Body;
                                if (fileUploader != null)
                                {
                                    string fileName = Path.GetFileName(fileUploader.FileName);
                                    mail.Attachments.Add(new Attachment(fileUploader.InputStream, fileName));
                                }
                                mail.IsBodyHtml = false;
                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = "smtp.gmail.com";
                                smtp.EnableSsl = true;
                                NetworkCredential networkCredential = new NetworkCredential(from, "swornim@12345");
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = networkCredential;
                                smtp.Port = 587;
                                smtp.Send(mail);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                ViewBag.Message = "The Mail With The retrieval Code has been sent to your email";
                            }
                            return View("iRetrieve");
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email doesnot exists");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Email doesnot exists");
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginDetails l, string ReturnUrl)
        {
            tbl_User t = _db.tbl_User.Single(u => u.Username == l.Username);
            if (t != null)
            {
                tbl_User tb = _db.tbl_User.Single(u => u.Username == l.Username && u.Password == l.Password);
                if (tb != null)
                {
                    FormsAuthentication.SetAuthCookie(l.Username, true);
                    Session.Add("Username", l.Username);
                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("AdminPanel", "CMS");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or Password");
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();

            return RedirectToAction("Login", "Login");
        }

        [AllowAnonymous]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(SignUpViewModel svm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tbl_User t = new tbl_User();
                    t.Username = svm.Username;
                    t.Password = svm.Password;
                    t.Email = svm.Email;
                    HttpPostedFileBase photo = Request.Files["Photo"];
                    if (photo != null)
                    {
                        t.Photo = photo.FileName;
                        photo.SaveAs(Server.MapPath("~/UserPhoto/" + photo.FileName));
                    }

                    _db.tbl_User.Add(t);
                    _db.SaveChanges();
                    ViewBag.UserCreated = "User Created Successfully";
                    return RedirectToAction("Login", "Login");
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("", "Could'nt Sign Up");
                    throw ex;
                }
            }
            return View();
        }
    }
}