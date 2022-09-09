﻿using System;
using System.Web.Mvc;

namespace Payroll.Controllers
{
    public class HomeController : Controller
    {

        [HttpPost]
        public JsonResult test()
        {

            return Json(new { });
        }

        [HttpPost]
        public JsonResult ClearValues()
        {
            Boolean flag = true;
            Session.Remove("iIdUsuario");
            Session.Remove("sUsuario");
            Session.Remove("Administrador");
            Session.Remove("Profile");
            Session.Remove("sEmpresa");
            Session.Remove("IdEmpresa");
            Session.Contents.RemoveAll();
            Session.Clear();
            Session.Abandon();
            return Json(new { Bandera = flag });
        }

        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult About()
        {
            return PartialView();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}