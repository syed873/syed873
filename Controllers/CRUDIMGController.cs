using ImageCRUD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageCRUD.Models;
using System.Data.Entity;
using System.Net;

namespace ImageCRUD.Controllers
{
    public class CRUDIMGController : Controller
    {

        EREntities db = new EREntities();
        public ActionResult Index()
        {
            return View(db.tbl_img.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file, tbl_img emp)
        {
            string filename = Path.GetFileName(file.FileName);
            string _filename = DateTime.Now.ToString("yymmssfff") + filename;
            string extension = Path.GetExtension(file.FileName);
            string path = Path.Combine(Server.MapPath("~/images/"), _filename);
            emp.img = "~/images/" + _filename;

            if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
            {
                if (file.ContentLength <= 1000000)
                {
                    db.tbl_img.Add(emp);
                    if (db.SaveChanges() > 0)
                    {
                        file.SaveAs(path);
                        ViewBag.msg = "Record Added";
                        ModelState.Clear();
                    }
                }
                else
                {
                    ViewBag.msg = "Size is not valid";
                }
            }

            return View();
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_img tbl_img = db.tbl_img.Find(id);

            Session["imgPath"] = tbl_img.img;

            if (tbl_img == null)
            {
                return HttpNotFound();
            }
            return View(tbl_img);
        }
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, tbl_img emp)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                    string extension = Path.GetExtension(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                    emp.img = "~/images/" + _filename;

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (file.ContentLength <= 1000000)
                        {

                            db.Entry(emp).State = EntityState.Modified;
                            string oldImgPath = Request.MapPath(Session["imgPath"].ToString());
                            if (db.SaveChanges() > 0)
                            {
                                file.SaveAs(path);
                                if (System.IO.File.Exists(oldImgPath))
                                {
                                    System.IO.File.Delete(oldImgPath);


                                }

                                TempData["msg"] = "Record updated";
                                
                               // ViewBag.msg = "Record Added";
                                //ModelState.Clear();
                            }
                        }
                        else
                        {
                            ViewBag.msg = "Size is not valid";
                        }
                    }
                }
            }
            else
            {
                emp.img = Session["imgPath"].ToString();
                db.Entry(emp).State = EntityState.Modified;


                if (db.SaveChanges() > 0)
                {
                    TempData["msg"] = "Data Updated";
                    return RedirectToAction("Index");
                }
            }



            return View();

        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tbl_img = db.tbl_img.Find(id);
            //Session["imgPath"] = tbl_img.img;

            if (tbl_img == null)
            {
                return HttpNotFound();
            }
            return View(tbl_img);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tbl_img = db.tbl_img.Find(id);
            //Session["imgPath"] = tbl_img.img;

            if (tbl_img == null)
            {
                return HttpNotFound();
            }
            string currentimg = Request.MapPath(tbl_img.img);
            db.Entry(tbl_img).State = EntityState.Deleted;
            if (db.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(currentimg))
                {
                    System.IO.File.Delete(currentimg);
                }
                TempData["msg"] = "Data deleted!";
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}