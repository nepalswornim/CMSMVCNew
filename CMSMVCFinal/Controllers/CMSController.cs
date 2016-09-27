using CMSMVCFinal.Models;
using CMSMVCFinal.Models.Models;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSMVCFinal.Controllers
{
    public class CMSController : Controller
    {
        //
        // GET: /CMS/
        private CMSDBFinalEntities _db = new CMSDBFinalEntities();

        public ActionResult LoginIndex(int? page)
        {
            List<PostViewModel> pv = new List<PostViewModel>();

            List<tbl_Post> tlp = _db.tbl_Post.OrderByDescending(u => u.PostId).ToList();
            foreach (tbl_Post item in tlp)
            {
                PostViewModel pvm = new PostViewModel();
                pvm.PostId = item.PostId;
                pvm.Post = item.Post;
                pvm.PostTitle = item.PostTitle;
                pvm.FeaturedPhoto = item.FeaturedPhoto;
                pvm.PostCreator = item.PostCreator;
                pv.Add(pvm);
            }

            return View(pv.ToList().ToPagedList(page ?? 1, 3));
        }

        [Authorize]
        public ActionResult AdminPanel()
        {
            string username = Session["Username"].ToString();
            if (username != null)
            {
                List<PostViewModel> pv = new List<PostViewModel>();
                List<tbl_Post> tlp = _db.tbl_Post.OrderByDescending(u => u.PostId).Where(u => u.PostCreator == username).ToList();

                foreach (tbl_Post item in tlp)
                {
                    PostViewModel pvm = new PostViewModel();
                    pvm.PostId = item.PostId;
                    pvm.Post = item.Post;
                    pvm.PostTitle = item.PostTitle;
                    pvm.FeaturedPhoto = item.FeaturedPhoto;
                    pvm.PostCreator = item.PostCreator;
                    pv.Add(pvm);
                }
                return View(pv);
            }
            return View();
        }

        ////public int IdByUsername()
        ////{
        ////    string username = Session["Username"].ToString();
        ////    tbl_Post tb = _db.tbl_Post.Where(u => u.PostCreator == username).FirstOrDefault();

        ////    int i = tb.PostId;
        ////    return i;
        ////}

        public ActionResult Index(int? page)
        {
            List<PostViewModel> pv = new List<PostViewModel>();

            List<tbl_Post> tlp = _db.tbl_Post.OrderByDescending(u => u.PostId).ToList();
            foreach (tbl_Post item in tlp)
            {
                PostViewModel pvm = new PostViewModel();
                pvm.PostId = item.PostId;
                pvm.Post = item.Post;
                pvm.PostTitle = item.PostTitle;
                pvm.FeaturedPhoto = item.FeaturedPhoto;
                pvm.PostCreator = item.PostCreator;
                pv.Add(pvm);
            }

            return View(pv.ToList().ToPagedList(page ?? 1, 3));
        }

        [Authorize]
        public ActionResult CreatePost()
        {
            return View();
        }

        [ActionName("Delete")]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            tbl_Post tb = _db.tbl_Post.Single(u => u.PostId == id);
            PostViewModel pvm = new PostViewModel();
            pvm.PostId = tb.PostId;
            pvm.PostTitle = tb.PostTitle;
            return View(pvm);
        }

        [ActionName("Delete")]
        [HttpPost]
        public ActionResult Delet(int id)
        {
            tbl_Post t = _db.tbl_Post.Single(u => u.PostId == id);
            _db.tbl_Post.Remove(t);
            _db.SaveChanges();
            return RedirectToAction("AdminPanel", "CMS");
        }

        public ActionResult Edit(int id)
        {
            tbl_Post tb = _db.tbl_Post.Single(u => u.PostId == id);
            PostViewModel pvm = new PostViewModel();
            pvm.PostId = tb.PostId;
            pvm.PostCreator = tb.PostCreator;
            pvm.Post = tb.Post;
            pvm.PostTitle = tb.PostTitle;
            pvm.FeaturedPhoto = tb.FeaturedPhoto;

            return View(pvm);
        }

        [HttpPost]
        public ActionResult Edit(PostViewModel pvm)
        {
            tbl_Post tb = _db.tbl_Post.Single(u => u.PostId == pvm.PostId);
            tb.Post = pvm.Post;

            tb.PostTitle = pvm.PostTitle;
            HttpPostedFileBase photo = Request.Files["FeaturedPhoto"];
            if (photo != null)
            {
                tb.FeaturedPhoto = photo.FileName;
                photo.SaveAs(Server.MapPath("~/Photo/" + photo.FileName));
            }

            tb.PostCreator = Session["Username"].ToString();
            _db.SaveChanges();

            return RedirectToAction("AdminPanel", "CMS");
        }

        [HttpPost]
        public ActionResult CreatePost(PostViewModel pvm)
        {
            try
            {
                LoginDetails l = new LoginDetails();
                tbl_Post tp = new tbl_Post();
                tp.PostTitle = pvm.PostTitle;
                tp.Post = pvm.Post;
                HttpPostedFileBase photo = Request.Files["FeaturedPhoto"];
                if (photo != null)
                {
                    tp.FeaturedPhoto = photo.FileName;
                    photo.SaveAs(Server.MapPath("~/Photo/" + photo.FileName));
                }

                tp.PostCreator = Session["Username"].ToString();

                _db.tbl_Post.Add(tp);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Couldnt add Post");
            }
            ViewBag.Message = "Post Created Successfully";
            return RedirectToAction("AdminPanel");
        }
    }
}