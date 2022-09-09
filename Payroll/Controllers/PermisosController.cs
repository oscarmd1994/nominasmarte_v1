using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Payroll.Models.Beans;
using Payroll.Models.Daos;

namespace Payroll.Controllers
{
    public class PermisosController : Controller
    {
        
        [HttpPost]
        public JsonResult UsuarioPermisoConsulta()
        {
            bool bandera = false;
            UsuariosDao usuariosDao = new UsuariosDao();
            try {
                int usuario = Convert.ToInt32(Session["iIdUsuario"]);
                bandera = usuariosDao.sp_Consulta_Permiso_Consulta(usuario);
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            }
            return Json(new { Consulta = bandera });
        }

    }
}