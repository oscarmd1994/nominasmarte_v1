using Payroll.Models.Beans;
using Payroll.Models.Daos;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Payroll.Controllers
{
    public class ControlPayrollController : Controller
    {
        // GET: ControlPayroll

        public ActionResult Home()
        {
            Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            if (Session["iIdUsuario"] == null)
            {
                return Redirect("../Home/Index");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public JsonResult MenuInit()
        {
            List<MainMenuBean> MmenuBean;
            MainMenuDao MmenuDao = new MainMenuDao();
            int Sesion_IdUser = int.Parse(Session["iIdUsuario"].ToString());
            MmenuBean = MmenuDao.sp_Retrieve_Menu_Paths(Sesion_IdUser);
            string li = "";
            string sidebar = "";
            string collapse = "";
            //string collapseinit = "<div class='collapsible-body'><ul class='list-unstyled'>";
            //string collapseend = "</ul></div>";
            foreach (var item in MmenuBean)
            {
                collapse = "";
                if (item.iParent == 0)
                {
                    li = "";
                    li += "<li><a class='collapsible-header waves-effect arrow-r' data-toggle='collapse' href='#collapse" + item.iIdItem + "' aria-expanded='false' aria-controls='collapse" + item.iIdItem + "'><i class='fas fa-circle small'></i> " + item.sNombre + "<i class='fas fa-angle-down rotate-icon'></i></a>";
                    //li += "<li><a class='collapsible-header waves-effect arrow-r'><i class='fas fa-chevron-right'></i>" + item.sNombre + "<i class='fas fa-angle-down rotate-icon'></i></a>";
                    List<MainMenuBean> submenus;
                    submenus = MmenuDao.Bring_Main_Menus(int.Parse(Session["Profile"].ToString()), item.iIdItem);
                    foreach (var subitem in submenus)
                    {
                        if (subitem.iParent == item.iIdItem)
                        {
                            //collapse += "<li><a href='#' class='waves-effect pl-4' onclick='seeview(" + '"' + subitem.sUrl + '"' + ")'><i class='" + subitem.sIcono + "'></i> " + subitem.sNombre + "</a></li>";
                            collapse += "<li><a href='#' class='waves-effect ml-4' onclick='seeview(" + '"' + subitem.sUrl + '"' + ")'>" + subitem.sNombre + "</a></li>";
                        }
                    }
                    li += "<div class='collapse' id='collapse" + item.iIdItem + "' data-parent='#sidenavMenu'><ul class='list-unstyled'>" + collapse + "</ul></div>" + "</li>";
                    //li += collapseinit + collapse + collapseend + "</li>";
                }
                sidebar += li;
            }
            return Json(sidebar);

        }
    }
}