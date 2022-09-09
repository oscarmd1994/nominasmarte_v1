﻿using Payroll.Models.Beans;
using Payroll.Models.Daos;
using Payroll.Models.Utilerias;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Payroll.Controllers
{
    public class CatalogosController : Controller
    {
        // GET: PETICIONES
        public PartialViewResult Encriptamiento()
        {
            return PartialView();
        }
        public PartialViewResult Empleados()
        {
            return PartialView();
        }
        public PartialViewResult CEmpleados()
        {
            return PartialView();
        }
        public PartialViewResult Localidades()
        {
            return PartialView();
        }
        public PartialViewResult CLocalidades()
        {
            return PartialView();
        }
        public PartialViewResult Puestos()
        {
            return PartialView();
        }
        public PartialViewResult CPuestos()
        {
            return PartialView();
        }
        public PartialViewResult Regionales()
        {
            return PartialView();
        }
        public PartialViewResult CRegionales()
        {
            return PartialView();
        }
        public PartialViewResult Sucursales()
        {
            return PartialView();
        }
        public PartialViewResult CSucursales()
        {
            return PartialView();
        }
        public PartialViewResult CentrosCostos()
        {
            return PartialView();
        }
        public PartialViewResult CCentrosCostos()
        {
            return PartialView();
        }
        public PartialViewResult PoliticasVacaciones()
        {
            return PartialView();
        }
        public PartialViewResult CPoliticasVacaciones()
        {
            return PartialView();
        }
        public PartialViewResult RegistroPatronal()
        {
            return PartialView();
        }
        public PartialViewResult CRegistroPatronal()
        {
            return PartialView();
        }
        public PartialViewResult FechasPeriodos()
        {
            return PartialView();
        }
        public PartialViewResult CFechasPeriodos()
        {
            return PartialView();
        }
        public PartialViewResult GruposEmpresas()
        {
            return PartialView();
        }
        public PartialViewResult CGruposEmpresas()
        {
            return PartialView();
        }
        public PartialViewResult VistaRenglones()
        {
            return PartialView();
        }
        public PartialViewResult CBancos()
        {
            return PartialView();
        }
        public PartialViewResult CUsuarios()
        {
            return PartialView();
        }
        public PartialViewResult CPagoRecibo2()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult LoadFechasPeriodos()
        {
            List<InicioFechasPeriodoBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_Retrieve_CInicio_Fechas_Periodo();
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadFechasPeriodosDetalle(int Empresa_id)
        {
            List<InicioFechasPeriodoBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_Retrieve_CInicio_Fechas_Periodo_Detalle(Empresa_id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult SaveNewPeriodo(int inEmpresa_id, int inano, int inperiodo, string infinicio, string inffinal, string infproceso, string infpago, int indiaspago, int intipoperiodoid, int inespecial)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            string dia = DateTime.Today.ToString("dd");
            string mes = DateTime.Today.ToString("MM");
            string año = DateTime.Today.ToString("yyyy");
            string hora = DateTime.Now.ToString("HH");
            string minuto = DateTime.Now.ToString("mm");
            string Referencia = dia + "/" + mes + "/" + año + " " + hora + ":" + minuto + " " + Session["sUsuario"].ToString();
            Lista = Dao.sp_CInicio_Fechas_Periodo_Insert_Fecha_Periodo(inEmpresa_id, inano, inperiodo, infinicio, inffinal, infproceso, infpago, indiaspago, intipoperiodoid, inespecial, Referencia);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult SaveNewEffdt(int Empresa_id, string Effdt)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();

            Lista = Dao.sp_CPoliticasVacaciones_Insert_Effdt_Futura(Empresa_id, Effdt);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult DeletePeriodo(int Empresa_id, int Id)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();

            Lista = Dao.sp_CInicio_Fechas_Periodo_Delete_Fecha_Periodo(Empresa_id, Id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult DeletePolitica(int Empresa_id, string Effdt, int Anios)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();

            Lista = Dao.sp_CPoliticasVacaciones_Delete_Politica(Empresa_id, Effdt, Anios);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult SaveNewPolitica(string inEmpresa_id, string inEffdt, string inano, string indias, string inprimav, string indiasa)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();

            Lista = Dao.sp_CPoliticasVacaciones_Insert_Politica(inEmpresa_id, inEffdt, inano, indias, inprimav, indiasa);
            return Json(Lista);
        }


        [HttpPost]
        public JsonResult LoadPoliticasVacaciones()
        {
            List<TabPoliticasVacacionesBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_Retrieve_CPoliticasVacaciones();
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadPoliticasVacacionesFuturas()
        {
            List<TabPoliticasVacacionesBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_Retrieve_CPoliticasVacaciones_Futuras();
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadPoliticasVacacionesDetalle(int Empresa_id)
        {
            List<TabPoliticasVacacionesBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_Retrieve_CPoliticasVacaciones_Detalle(Empresa_id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadPoliticasVacaciones_Futuras_Detalle(int Empresa_id, string Effdt)
        {
            List<TabPoliticasVacacionesBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_Retrieve_CPoliticasVacaciones_Futuras_Detalle(Empresa_id, Effdt);
            return Json(Lista);
        }

        [HttpPost]
        public JsonResult LoadPuestos()
        {
            List<TabPoliticasVacacionesBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_Retrieve_CPoliticasVacaciones();
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadEmpresasNEmpleados()
        {
            List<EmpleadosxEmpresaBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_CEmpresas_Retrieve_NoEmpleados();
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadPuestosxEmpresa(int Empresa_id)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_TPuestos_Retrieve_Puestos_Empresa(Empresa_id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadPuestosEmpresas()
        {
            List<List<string>> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_TPuestos_Retrieve_Empresas();
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult SearchPuesto(int Empresa_id, string Search)
        {
            List<DataPuestosBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_Tpuestos_Search_Puesto(Empresa_id, Search);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadPuesto(int Empresa_id, string Puesto_id)
        {
            List<DataPuestosBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_TPuestos_Retrieve_Puesto(Empresa_id, Puesto_id);
            return Json(Lista);
        }
        public JsonResult datRenglones(int IdEmpresa, int iElemntoNOm)
        {
            List<CRenglonesBean> LR = new List<CRenglonesBean>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            LR = Dao.sp_CRenglones_Retrieve_CRenglones(IdEmpresa, iElemntoNOm);

            return Json(LR);
        }

        [HttpPost]
        public JsonResult LoadGruposEmpresas()
        {
            List<List<string>> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_CGruposEmpresas_Retrieve_Grupos();
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadEmpresasGrupo(int Grupo_id)
        {
            List<List<string>> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_CGruposEmpresas_Retrieve_EmpresasGrupo(Grupo_id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadAllPuestos()
        {
            List<DataPuestosBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_TPuestos_Retrieve_AllPuestos();
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadPolitica(int Empresa_id, string Effdt, string Anio)
        {
            List<TabPoliticasVacacionesBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_CPoliticasVacaciones_Retrieve_Politica(Empresa_id, Effdt, Anio);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult UpdatePolitica(int Empresa_id, string Effdt, int Anio, int Dias, int Diasa, int Prima, int Anion)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_CPoliticasVacaciones_Update_Politica(Empresa_id, Effdt, Anio, Dias, Diasa, Prima, Anion);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadPeriodo(int Empresa_id, int Id)
        {
            List<InicioFechasPeriodoBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_CInicio_Fechas_Periodo_Retrieve_Periodo(Empresa_id, Id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult UpdatePeriodo(int Empresa_id, int editid, int editano, int editperiodo, string editfinicio, string editffinal, string editfproceso, string editfpago, int editdiaspago, int edespecial)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_CInicio_Fechas_Periodo_Update_Periodo(Empresa_id, editid, editano, editperiodo, editfinicio, editffinal, editfproceso, editfpago, editdiaspago, edespecial);
            return Json(Lista);
        }

        [HttpPost]
        public JsonResult LoadBancosEmpresa(int Empresa_id)
        {
            List<TabBancosEmpresas> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_BancosEmpresas_Retrieve_Bancos(Empresa_id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadTipoBanco()
        {
            List<InfDomicilioBean> Lista;
            InfDomicilioDao Dao = new InfDomicilioDao();
            Lista = Dao.sp_CatalogoGeneral_Retrieve_CatalogoGeneral(31);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult SaveNewBanco(int Empresa_id, int Banco_id, int TipoBanco, int Cliente, int Plaza, string CuentaEmp, string Clabe)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_BancosEmpresas_Insert_Banco(Empresa_id, Banco_id, TipoBanco, Cliente, Plaza, CuentaEmp, Clabe);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult UpdateBancoEmpresa(int Banco_id, int TipoBanco, int Id, int Cliente, int Plaza, string CuentaEmp, string Clabe)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_BancosEmpresas_updatebanco_Banco(Banco_id, TipoBanco, Id, Cliente, Plaza, CuentaEmp, Clabe);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult UpdateBanco(int key, int Id)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_BancosEmpresas_updatestatus_Banco(key, Id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadUsers()
        {
            List<DataUsersBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_CUsuarios_Retrieve_Users();
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadProfiles()
        {
            List<DataProfilesBean> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_CPerfiles_Retrieve_Perfiles();
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult Loadmainmenus()
        {
            List<MainMenuBean> Lista;
            MainMenuDao Dao = new MainMenuDao();
            Lista = Dao.sp_Retrieve_Menu_Paths(3);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult Loadonemenu(int Id)
        {
            List<MainMenuBean> Lista;
            MainMenuDao Dao = new MainMenuDao();
            Lista = Dao.Bring_Main_Menus(1, Id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult LoadCentrosCostoDetalle(int Empresa_id)
        {
            List<DataCentrosCosto> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_TCentrosCostos_Retrieve_CentrosCostoxEmpresa(Empresa_id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult SaveNewCentrosCostos(int Empresa_id, string nombre, string descripcion)
        {
            List<string> Lista = new List<string>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            int Usuario_id = int.Parse(Session["iIdUsuario"].ToString());
            Lista = Dao.sp_Catalogos_Insert_Centro_Costo(Empresa_id, nombre.Trim().ToUpper(), descripcion.Trim().ToUpper(), Usuario_id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult SaveNewRegionales(string ClaveRegion, string Descripcion)
        {
            List<string> Lista = new List<string>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            int Usuario_id = int.Parse(Session["iIdUsuario"].ToString());
            int Empresa_id = int.Parse(Session["IdEmpresa"].ToString());
            Lista = Dao.sp_CRegionales_Insert_Regional(Empresa_id, ClaveRegion.Trim().ToUpper(), Descripcion.Trim().ToUpper(), Usuario_id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult SaveNewRegionalesCatalogo(int Empresa_id, string ClaveRegion, string Descripcion)
        {
            List<string> Lista = new List<string>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            int Usuario_id = int.Parse(Session["iIdUsuario"].ToString());
            Lista = Dao.sp_CRegionales_Insert_Regional(Empresa_id, ClaveRegion.Trim().ToUpper(), Descripcion.Trim().ToUpper(), Usuario_id);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult ChangestatusRegistroPatronal(int Empresa_id, int RegPat_id, int Status)
        {
            List<string> Lista = new List<string>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_TRegistroPatronal_update_Status(Empresa_id, RegPat_id, Status);
            return Json(Lista);
        }
        [HttpPost]
        public JsonResult SaveNewRegistroPatronal(int Empresa_id, string Afiliacion_IMSS, string NombreAfiliacion, string RiesgoTrabajo, int Clase)
        {
            List<string> Lista = new List<string>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            Lista = Dao.sp_TRegistroPatronal_insert_RegistroPatronal(Empresa_id, Afiliacion_IMSS, NombreAfiliacion, RiesgoTrabajo, Clase);
            return Json(Lista);
        }
        // llena  listado de Tipo Renglon
        [HttpPost]
        public JsonResult ListTipoRenglon()
        {
            List<TipoRenglonBean> LTR = new List<TipoRenglonBean>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            LTR = Dao.sp_TipoRenglon_Retrieve_TipoRenlgon();
            return Json(LTR);
        }

        // llena  listado Elemento Nom
        [HttpPost]
        public JsonResult ListEleNom()
        {
            List<ElementoNominaBean> LTEle = new List<ElementoNominaBean>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            LTEle = Dao.sp_CElemntoNomina_Retrieve_Cgeneral();
            return Json(LTEle);
        }

        // llena  listado calculo
        [HttpPost]
        public JsonResult ListCalcu()
        {
            List<ListaCalculoBean> LTcal = new List<ListaCalculoBean>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            LTcal = Dao.sp_ListCalculo_Retrieve_ClistaCalculo();
            return Json(LTcal);
        }

        // llena  listado Acumulados
        [HttpPost]
        public JsonResult LisAcumu(int iIdEmpresa, int iElementoNom)
        {
            List<CRenglonesBean> LTcal = new List<CRenglonesBean>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            LTcal = Dao.sp_Acumulados_Retrieve_CRenglones(iIdEmpresa, iElementoNom);
            return Json(LTcal);
        }

        // Lista Sat
        [HttpPost]
        public JsonResult ListSat()
        {
            List<ListSatBean> LTcal = new List<ListSatBean>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            LTcal = Dao.sp_ListSat_Retrieve_CSatRenglones();
            return Json(LTcal);
        }

        //Guarda los datos en la tabla Crenglones
        [HttpPost]
        public JsonResult SaveRenglon(int iIdEmpresa, int iIdRenglon, string sNombreRenglon, int iElementoNom, int iIdReporte
           , int IdAcumulado, int icancel, int iTipodeRenglon, int iEspejo, int idCalculo, string sCuentaCont, string sDespCuenta,
            string sCargaCuenta, int iIdSat, int PenAlin)
        {

            CRenglonesBean bean = new CRenglonesBean();
            ModCatalogosDao dao = new ModCatalogosDao();
            bean = dao.ps_Renglon_Insert_CRenglones(iIdEmpresa, iIdRenglon, sNombreRenglon, iElementoNom, iIdReporte, IdAcumulado
            , icancel, iTipodeRenglon, iEspejo, idCalculo, sCuentaCont, sDespCuenta, sCargaCuenta, iIdSat, PenAlin);
            return Json(bean);
        }


        // Lista reporte
        [HttpPost]
        public JsonResult ListReporte()
        {
            List<SeccionReporte> LTcal = new List<SeccionReporte>();
            ModCatalogosDao Dao = new ModCatalogosDao();
            LTcal = Dao.sp_SeccionReporte_Retrieve_Cgeneral();
            return Json(LTcal);
        }

        //Actualiza los datos en la tabla Crenglones
        [HttpPost]
        public JsonResult UpdateRenglon(int iIdEmpresa, int iIdRenglon, string sNombreRenglon,
     int iEspejo, int iIdSat, int PenAlin)
        {

            CRenglonesBean bean = new CRenglonesBean();
            ModCatalogosDao dao = new ModCatalogosDao();
            bean = dao.ps_Renglon_Update_CRenglones(iIdEmpresa, iIdRenglon, sNombreRenglon,
            iEspejo, iIdSat, PenAlin);
            return Json(bean);
        }

        [HttpPost]
    public JsonResult LoadTipoRecuperaAusentismo()
    {
      List<List<string>> Lista;
      PruebaEmpresaDao Dao = new PruebaEmpresaDao();
      Lista = Dao.sp_CatalogoGeneral_Retrieve_RecuperaAusentismos();
      return Json(Lista);
    }
    [HttpPost]
    public JsonResult SaveNewPerfil(List<int> maincheck, List<int> subcheck)
    {
      List<List<string>> Lista;
      PruebaEmpresaDao Dao = new PruebaEmpresaDao();
      Lista = Dao.sp_CatalogoGeneral_Retrieve_RecuperaAusentismos();
      return Json(Lista);
    }
    [HttpPost]
    public JsonResult LoadFilePeriodos(HttpPostedFileBase fileUpload)
    {
      List<object> list = new List<object>();

            string RutaSitio = Server.MapPath("~/");
            string dia = DateTime.Today.ToString("dd");
            string mes = DateTime.Today.ToString("MM");
            string año = DateTime.Today.ToString("yyyy");
            string hora = DateTime.Now.ToString("HH");
            string minuto = DateTime.Now.ToString("mm");

            string Referencia = dia + "/" + mes + "/" + año + " " + hora + ":" + minuto + " " + Session["sUsuario"].ToString();

            if (!Directory.Exists(RutaSitio + "/Content/FilesCargaMasivaIncidencias/LogsCarga/"))
            {
                Directory.CreateDirectory(RutaSitio + "/Content/FilesCargaMasivaIncidencias/LogsCarga/");
            }

            string pathGuardado = Path.Combine(RutaSitio + "/Content/FilesCargaMasivaIncidencias/" + dia + "_" + mes + "_" + año + "_" + hora + "_" + minuto + "_" + "Periodos" + Path.GetExtension(fileUpload.FileName));
            string pathLogs = Path.Combine(RutaSitio + "/Content/FilesCargaMasivaIncidencias/LogsCarga/Notas_de_carga.txt");
            fileUpload.SaveAs(pathGuardado);
            CargaMasivaDao Dao = new CargaMasivaDao();
            DataTable table = Dao.ValidaArchivo(pathGuardado);

            List<string> ResutLog = new List<string>();
            int i;
            string errorh = "Error en la linea: ";

            for (i = 0; i < table.Rows.Count; i++)
            {

                var resultvEmpresa = Dao.ValidaEmpresa(table.Rows[i]["Empresa_id"].ToString());
                if (resultvEmpresa == 0) { ResutLog.Add(errorh + (i + 1) + ", La empresa " + table.Rows[i]["Empresa_id"].ToString() + " no existe"); }

                var resulTipoPeriodo = Dao.ValidaEmpresaTipoPeriodo(table.Rows[i]["Empresa_id"].ToString(), table.Rows[i]["TipoPeriodo"].ToString());
                if (resulTipoPeriodo == 0) { ResutLog.Add(errorh + (i + 1) + ", La empresa " + table.Rows[i]["Empresa_id"].ToString() + " no tiene el tipo de periodo " + table.Rows[i]["TipoPeriodo"].ToString()); }

                var isValidDate1 = Dao.Valida_FormatoFechas(table.Rows[i]["Fecha_inicio"].ToString());
                if (isValidDate1 == 0) { ResutLog.Add(errorh + (i + 1) + ", El formato de fecha para Fecha Inicio es incorrecto a dd/mm/aaaa"); }

                var isValidDate2 = Dao.Valida_FormatoFechas(table.Rows[i]["Fecha_fin"].ToString());
                if (isValidDate2 == 0) { ResutLog.Add(errorh + (i + 1) + ", El formato de fecha para Fecha Fin es incorrecto a dd/mm/aaaa"); }

                var isValidDate3 = Dao.Valida_FormatoFechas(table.Rows[i]["Fecha_pago"].ToString());
                if (isValidDate3 == 0) { ResutLog.Add(errorh + (i + 1) + ", El formato de fecha para Fecha Pago es incorrecto a dd/mm/aaaa"); }

                var isValidDate4 = Dao.Valida_FormatoFechas(table.Rows[i]["Fecha_proceso"].ToString());
                if (isValidDate4 == 0) { ResutLog.Add(errorh + (i + 1) + ", El formato de fecha para Fecha Proceso es incorrecto a dd/mm/aaaa"); }

                var resultExistePeriodos = Dao.Valida_PeriodoExistente(table.Rows[i]["Empresa_id"].ToString(), table.Rows[i]["Anio"].ToString(), table.Rows[i]["TipoPeriodo"].ToString(), table.Rows[i]["Periodo"].ToString());
                if (resultExistePeriodos > 0 && resultExistePeriodos < 100) { ResutLog.Add(errorh + (i + 1) + ", La empresa " + table.Rows[i]["Empresa_id"].ToString() + " tipo periodo " + table.Rows[i]["TipoPeriodo"].ToString() + " en el año " + table.Rows[i]["Anio"].ToString() + " ya cuenta con un periodo " + table.Rows[i]["Periodo"].ToString()); }
                else if (resultExistePeriodos == 100) { ResutLog.Add(errorh + (i + 1) + ", El año " + table.Rows[i]["Anio"].ToString() + " se encuentra fuera del rango valido para insertar periodos."); }


            }

            if (ResutLog.Count == 0)
            {
                for (int k = 0; k < table.Rows.Count; k++)
                {
                    Dao.InsertaCargaMasivaPeriodos(table.Rows[k], Referencia);
                }
                list.Add("1");
                list.Add("Carga de Periodos correcta, se cargaron " + i + " registos.");
            }
            else
            {
                StreamWriter txtfile = new StreamWriter(pathLogs);
                foreach (var error in ResutLog)
                {
                    txtfile.WriteLine(error);
                }
                txtfile.Close();
                list.Clear();
                list.Add("0");
                list.Add("/Content/FilesCargaMasivaIncidencias/LogsCarga/Notas_de_carga.txt");
            }

            return Json(list);
        }
        [HttpPost]
        public JsonResult UpdateFechaPagoPeriodo(int Empresa_id, int editid, string editfpago)
        {
            List<string> Lista;
            ModCatalogosDao Dao = new ModCatalogosDao();

            Lista = Dao.sp_CInicio_Fechas_Periodo_Update_Fecha_Pago(Empresa_id, editid, editfpago);
            return Json(Lista);
        }
        ////////////////////////////////////////////////////////
        ////////////// PRUEBAS DE ENCRIPTAMIENTO  //////////////
        ////////////////////////////////////////////////////////
        //[HttpPost]
        //public JsonResult Encrypt(string txt)
        //{
        //    string txt_encrypted;
        //    Encriptamiento crypter = new Encriptamiento();
        //    return Json(txt_encrypted);
        //}

        //[HttpPost]
        //public JsonResult Decrypt(string txt)
        //{
        //    string txt_decrypted;
        //    Encriptamiento crypter = new Encriptamiento();
        //    txt_decrypted = crypter.AES256Decrypt(txt);
        //    return Json(txt_decrypted);
        //}

        [HttpPost]
        public JsonResult ChangeUserStatus(int status, int iduser)
        {
            ReturnBean respuesta = new ReturnBean();
            ModCatalogosDao dao = new ModCatalogosDao();
            respuesta = dao.sp_Retrieve_ChangeUserStatus(status, iduser);
            return Json(respuesta);
        }

        [HttpPost]
        public JsonResult CSeparacionEmpresas_getEmpresas()
        {
            List<InicioFechasPeriodoBean> respuesta = new List<InicioFechasPeriodoBean>();
            ModCatalogosDao dao = new ModCatalogosDao();
            respuesta = dao.sp_CSeparacion_Empresas_Retrieve_Empresas();
            return Json(respuesta);
        }

        [HttpPost]
        public JsonResult CSeparacion_Empresas_DetallePeriodos(int Empresa_id)
        {
            List<InfoPagosRecibo2> respuesta = new List<InfoPagosRecibo2>();
            ModCatalogosDao dao = new ModCatalogosDao();
            respuesta = dao.sp_CSeparacion_Empresas_DetallePeriodos(Empresa_id);
            return Json(respuesta);
        }
        
        [HttpPost]
        public JsonResult CSeparacion_Empresas_getEmpresasPagadoras()
        {
            List<EmpresasBean> respuesta = new List<EmpresasBean>();
            ModCatalogosDao dao = new ModCatalogosDao();
            respuesta = dao.sp_CSeparacion_Empresas_getEmpresasPagadoras();
            return Json(respuesta);
        }
        [HttpPost]
        public JsonResult sp_CSeparacion_Empresas_insertNew(string EmpresaDestino,string Periodo)
        {
            ReturnBean respuesta = new ReturnBean();
            ModCatalogosDao dao = new ModCatalogosDao();
            respuesta = dao.sp_CSeparacion_Empresas_insertNew(EmpresaDestino,Periodo);
            return Json(respuesta);
        }
    }
}