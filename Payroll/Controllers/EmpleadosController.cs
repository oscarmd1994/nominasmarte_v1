﻿using Payroll.Models.Beans;
using Payroll.Models.Daos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Text;
using MessagingToolkit.QRCode.Codec;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net.Mail;
using System.Globalization;

namespace Payroll.Controllers
{
    public class EmpleadosController : Controller
    {
        public PartialViewResult Datos_Generales()
        {
            return PartialView();
        }
        public PartialViewResult IMSS()
        {
            return PartialView();
        }
        public PartialViewResult Datos_Nomina()
        {
            return PartialView();
        }
        public PartialViewResult Estructura()
        {
            return PartialView();
        }
        public PartialViewResult RecibosNomina()
        {
            return PartialView();
        }

        public PartialViewResult XML()
        {
            return PartialView();
        }

        public ActionResult TimbradosXML()
        {
            return PartialView();
        }

        public ActionResult EmisionRecibos()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult LoadStates()
        {
            InfDomicilioDao infDomDao = new InfDomicilioDao();
            List<InfDomicilioBean> infDomBean = new List<InfDomicilioBean>();
            int type = 1;
            infDomBean = infDomDao.sp_CatalogoGeneral_Retrieve_CatalogoGeneral(type);
            var data = new { resp = "bien" };
            return Json(infDomBean);
        }
        [HttpPost]
        public JsonResult LoadInformationHome2(int codepost)
        {
            InfDomicilioDao infDomDao = new InfDomicilioDao();
            List<InfoDireccionByCPBean> listInfDomBean = new List<InfoDireccionByCPBean>();
            listInfDomBean = infDomDao.sp_Domicilio_Retrieve_Domicilio2(codepost);
            return Json(listInfDomBean);
        }
        [HttpPost]
        public JsonResult LoadInformationHome(int codepost, int state)
        {
            InfDomicilioDao infDomDao = new InfDomicilioDao();
            List<InfDomicilioBean> listInfDomBean = new List<InfDomicilioBean>();
            listInfDomBean = infDomDao.sp_Domicilio_Retrieve_Domicilio(codepost, state);
            return Json(listInfDomBean);
        }
        [HttpPost]
        public JsonResult LoadDataCatGen(int state, string type, int keycat, int keycam)
        {
            CatalogoGeneralDao catGenDao = new CatalogoGeneralDao();
            List<CatalogoGeneralBean> catGenBean = new List<CatalogoGeneralBean>();
            catGenBean = catGenDao.sp_CatalogoGeneral_Consulta_CatalogoGeneral(state, type, keycat, keycam);
            return Json(catGenBean);
        }

        [HttpPost]
        public JsonResult LoadNivelStudy(int state, string type, int keynivel)
        {
            NivelEstudiosDao nivEstDao = new NivelEstudiosDao();
            List<NivelEstudiosBean> nivEstBean = new List<NivelEstudiosBean>();
            nivEstBean = nivEstDao.sp_NivelEstudios_Retrieve_NivelEstudios(state, type, keynivel);
            return Json(nivEstBean);
        }

        [HttpPost]
        public JsonResult LoadTabs(int state, string type, int keytab)
        {
            TabuladoresDao tabDao = new TabuladoresDao();
            List<TabuladoresBean> tabBean = new List<TabuladoresBean>();
            tabBean = tabDao.sp_Tabuladores_Retrieve_Tabuladores(state, type, keytab);
            return Json(tabBean);
        }

        [HttpPost]
        public JsonResult LoadBanks(int keyban)
        {
            BancosDao banDao = new BancosDao();
            List<BancosBean> banBean = new List<BancosBean>();
            banBean = banDao.sp_Bancos_Retrieve_Bancos(keyban);
            return Json(banBean);
        }

        public JsonResult Submenus(int IdItem)
        {

            return Json("");
        }
        [HttpPost]
        public JsonResult SearchEmpleados(string txtSearch)
        {
            //List<DescEmpleadoVacacionesBean> empleados = new List<DescEmpleadoVacacionesBean>();
            //pruebaEmpleadosDao Dao = new pruebaEmpleadosDao();
            List<EmpleadoSearchBean> empleados = new List<EmpleadoSearchBean>();
            pruebaEmpleadosDao Dao = new pruebaEmpleadosDao();
            empleados = Dao.sp_Retrieve_liveSearchEmpleadoSinEmpresa(int.Parse(Session["IdEmpresa"].ToString()), txtSearch);

            return Json(empleados);
        }
        [HttpPost]
        public JsonResult SearchEmpleadosM(string txtSearch)
        {
            List<EmpleadoSearchBean> empleados = new List<EmpleadoSearchBean>();
            pruebaEmpleadosDao Dao = new pruebaEmpleadosDao();
            empleados = Dao.sp_Retrieve_liveSearchEmpleadoSinEmpresa(0, txtSearch);
            return Json(empleados);
        }
        [HttpPost]
        public JsonResult SearchEmpleado(int IdEmpleado)
        {
            Session["Empleado_id"] = IdEmpleado;
            List<DescEmpleadoVacacionesBean> empleados = new List<DescEmpleadoVacacionesBean>();
            pruebaEmpleadosDao Dao = new pruebaEmpleadosDao();
            empleados = Dao.sp_Retrieve_liveSearchEmpleado(int.Parse(Session["IdEmpresa"].ToString()), IdEmpleado.ToString());
            return Json(empleados);
        }

        [HttpPost]
        public JsonResult SearchEmpleadoInDown(int IdEmpleado)
        {
            Boolean flag = false;
            String messageError = "none";
            List<string> bean = new List<string>();
            FuncionesNomina dao = new FuncionesNomina();
            try
            {
                var Empresa_id = int.Parse(Session["IdEmpresa"].ToString());
                bean = dao.sp_TEmpleado_Nomina_Retrieve_DatosBaja(Empresa_id, IdEmpleado);
                bean.Add(Empresa_id.ToString());
            }
            catch (Exception exc)
            {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(bean);
        }

        [HttpPost]
        public JsonResult DataTabGenEmploye(int keyemploye)
        {
            Boolean flag = false;
            String messageError = "none";
            EmpleadosBean empleadoBean = new EmpleadosBean();
            ListEmpleadosDao listEmpleadoDao = new ListEmpleadosDao();
            try
            {
                int keyemp = int.Parse(Session["IdEmpresa"].ToString());
                empleadoBean = listEmpleadoDao.sp_Empleados_Retrieve_Empleado(keyemploye, keyemp);
                if (empleadoBean.sMensaje != "success")
                {
                    messageError = empleadoBean.sMensaje;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception exc)
            {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = empleadoBean });
        }

        [HttpPost]
        public JsonResult DataTabImssEmploye(int keyemploye)
        {
            Boolean flag = false;
            String messageError = "none";
            ImssBean imssBean = new ImssBean();
            ListEmpleadosDao listEmpleadoDao = new ListEmpleadosDao();
            try
            {
                int keyemp = int.Parse(Session["IdEmpresa"].ToString());
                imssBean = listEmpleadoDao.sp_Imss_Retrieve_ImssEmpleado(keyemploye, keyemp);
                if (imssBean.sMensaje != "success")
                {
                    messageError = imssBean.sMensaje.ToString();
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception exc)
            {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = imssBean });
        }

        [HttpPost]
        public JsonResult DataTabNominaEmploye(int keyemploye)
        {
            Boolean flag = false;
            String messageError = "none";
            DatosNominaBean datoNominaBean = new DatosNominaBean();
            ListEmpleadosDao listEmpleadoDao = new ListEmpleadosDao();
            try
            {
                int keyemp = int.Parse(Session["IdEmpresa"].ToString());
                datoNominaBean = listEmpleadoDao.sp_Nominas_Retrieve_NominaEmpleado(keyemploye, keyemp);
                if (datoNominaBean.sMensaje != "success")
                {
                    messageError = datoNominaBean.sMensaje;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception exc)
            {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = datoNominaBean });
        }

        [HttpPost]
        public JsonResult DataTabStructureEmploye(int keyemploye)
        {
            Boolean flag = false;
            String messageError = "none";
            DatosPosicionesBean datoPosicionBean = new DatosPosicionesBean();
            ListEmpleadosDao listEmpleadoDao = new ListEmpleadosDao();
            try
            {
                int keyemp = int.Parse(Session["IdEmpresa"].ToString());
                datoPosicionBean = listEmpleadoDao.sp_Posiciones_Retrieve_PosicionEmpleado(keyemploye, keyemp);
                if (datoPosicionBean.sMensaje != "success")
                {
                    messageError = datoPosicionBean.sMensaje;
                }
                if (datoPosicionBean.sMensaje == "success")
                {
                    flag = true;
                }
            }
            catch (Exception exc)
            {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = true, MensajeError = messageError, Datos = datoPosicionBean });
        }
        //Codigo nuevo
        [HttpPost]
        public JsonResult LoadTypePer()
        {
            List<TipoPeriodosBean> tipoPeriodoBean = new List<TipoPeriodosBean>();
            TipoPeriodosDao tipoPeriodoDao = new TipoPeriodosDao();
            tipoPeriodoBean = tipoPeriodoDao.sp_TipoPeriodos_Retrieve_TipoPeriodos();
            return Json(tipoPeriodoBean);
        }

        [HttpPost]
        public JsonResult LoadLocalitys()
        {
            List<LocalidadesBean2> localidadBean = new List<LocalidadesBean2>();
            LocalidadesDao localidadDao = new LocalidadesDao();
            //Reemplazar por la sesion de la empresa
            int keyemp = int.Parse(Session["IdEmpresa"].ToString());
            localidadBean = localidadDao.sp_TLocalicades_Retrieve_Localidades(keyemp);
            return Json(localidadBean);
        }

        [HttpPost]
        public JsonResult LoadPositiosRep()
        {
            List<DatosPosicionesBean> posicionBean = new List<DatosPosicionesBean>();
            DatosPosicionesDao posicionDao = new DatosPosicionesDao();
            // Reemplazar por la sesion de empresa
            int keyemp = int.Parse(Session["IdEmpresa"].ToString());
            string typefil = "AllPositions";
            posicionBean = posicionDao.sp_Posiciones_Retrieve_Posiciones(keyemp, typefil);
            return Json(posicionBean);
        }
        [HttpPost]
        public JsonResult LoadRegPatCla()
        {
            List<RegistroPatronalBean2> regPatronalBean = new List<RegistroPatronalBean2>();
            RegistroPatronalDao regPatronalDao = new RegistroPatronalDao();
            // Reemplazar por la sesion de la empresa
            int keyemp = int.Parse(Session["IdEmpresa"].ToString());
            regPatronalBean = regPatronalDao.sp_Registro_Patronal_Retrieve_Registros_Patronales(keyemp);
            return Json(regPatronalBean);
        }

        [HttpPost]
        public JsonResult LoadNacions()
        {
            List<NacionalidadesBean> nacionBean = new List<NacionalidadesBean>();
            NacionalidadesDao nacionDao = new NacionalidadesDao();
            nacionBean = nacionDao.sp_Nacionalidades_Retrieve_Nacionalidades();
            return Json(nacionBean);
        }
        [HttpPost]
        public JsonResult UpdatePosicionAct(int clvemp)
        {
            EmpleadosBean empleadoBean = new EmpleadosBean();
            EmpleadosDao empleadoDao = new EmpleadosDao();
            // Reemplazar por la sesion de la empresa
            int keyemp = int.Parse(Session["IdEmpresa"].ToString());
            empleadoBean = empleadoDao.sp_Empleado_Update_PosicionNomina(clvemp, keyemp);
            var data = new { empleado = clvemp, result = empleadoBean.sMensaje };
            return Json(data);
        }

        [HttpPost]
        public JsonResult DataListEmpleado(int iIdEmpresa, int TipoPeriodo, int periodo, int Anio)
        {
            List<EmpleadosEmpresaBean> ListEmple = new List<EmpleadosEmpresaBean>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            ListEmple = Dao.sp_EmpleadosDEmpresa_Retrieve_EmpleadosDEmpresa(iIdEmpresa, TipoPeriodo, periodo, Anio);
            return Json(ListEmple);
        }
        [HttpPost]
        public JsonResult EmisorEmpresa(int IdEmpresa, string sNombreComple)
        {

            string[] Nombre = sNombreComple.Split(' ');
            string Idempleado = Nombre[0].ToString();
            int id = int.Parse(Idempleado);
            List<EmisorReceptorBean> ListDatEmisor = new List<EmisorReceptorBean>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            ListDatEmisor = Dao.sp_EmisorReceptor_Retrieve_EmisorReceptor(IdEmpresa, id);

            return Json(ListDatEmisor);
        }

        //public JsonResult ListDatPeriodo(int iIdEmpresesas, int ianio, int iTipoPeriodo, int iPeriodo)
        //{
        //    List<CInicioFechasPeriodoBean> LPe = new List<CInicioFechasPeriodoBean>();
        //    ListEmpleadosDao dao = new ListEmpleadosDao();
        //    LPe = dao.sp_DatosPerido_Retrieve_DatosPerido(iIdEmpresesas, ianio, iTipoPeriodo, iPeriodo);
        //    return Json(LPe);

        //}

        [HttpPost]

        public JsonResult ReciboNomina(int iIdEmpresa, int iIdEmpleado, int ianio, int iTipodePerido, int iPeriodo, int iespejo)
        {
            int idRenglon = 0;
            double Row481 = 0;
            List<ReciboNominaBean> LCRecibo = new List<ReciboNominaBean>();
            List<TablaNominaBean> LsTabla = new List<TablaNominaBean>();
            List<string> LRenglin481 = new List<string>();
            FuncionesNomina dao = new FuncionesNomina();
            DataDispersionBusiness DaoDisp = new DataDispersionBusiness();
            LCRecibo = dao.sp_TpCalculoEmpleado_Retrieve_TpCalculoEmpleado(iIdEmpresa, iIdEmpleado, iPeriodo, iTipodePerido, ianio, iespejo);
            Row481 = DaoDisp.sp_Comprueba_Existencia_Renglon_Vales(iIdEmpresa, iIdEmpleado, iPeriodo, iTipodePerido, ianio);
            if (LCRecibo != null)
            {
                if (LCRecibo.Count > 0)
                {
                    for (int i = 0; i < LCRecibo.Count; i++)
                    {
                        if (LCRecibo[i].iIdRenglon == 1)
                        {
                            idRenglon = i;
                        }

                        TablaNominaBean ls = new TablaNominaBean();
                        {
                            ls.sConcepto = LCRecibo[i].iIdRenglon + " " + LCRecibo[i].sNombre_Renglon;

                            if (LCRecibo[i].sValor == "Percepciones")
                            {

                                ls.dPercepciones = LCRecibo[i].dSaldo.ToString("#.##");
                                if (LCRecibo[i].iIdRenglon == 481 && Row481 > 0)
                                {
                                    ls.dPercepciones = "0.00";
                                };

                                ls.dDeducciones = "0";
                            }
                            if (LCRecibo[i].sValor == "Deducciones")
                            {
                                if (LCRecibo[i].iIdRenglon == 1013)
                                {
                                    LsTabla[idRenglon].dDeducciones = "-" + LCRecibo[i].dSaldo.ToString();

                                }
                                ls.dPercepciones = "0";
                                ls.dDeducciones = LCRecibo[i].dSaldo.ToString();
                            }

                        }

                        ls.dExcento = LCRecibo[i].dExcento.ToString("#.##");
                        if (LCRecibo[i].dExcento < 1) { ls.dExcento = "0.00"; };
                        ls.dGravados = LCRecibo[i].dGravado.ToString("#.##");
                        if (LCRecibo[i].dGravado < 1) { ls.dGravados = "0.00"; };

                        ls.dSaldos = "0";
                        ls.dInformativos = "0";
                        LsTabla.Add(ls);

                    }

                }
            }
            return Json(LsTabla);

        }

        [HttpPost]

        public JsonResult XMLNomina(int IdEmpresa, string sNombreComple, int Periodo, int anios, int Tipodeperido, int Masivo, int PDF)
        {

            int Recibo = 1;

            String messageError = "none";
            List<EmisorReceptorBean> ListDatEmisor = new List<EmisorReceptorBean>();
            List<CInicioFechasPeriodoBean> LFechaPerido = new List<CInicioFechasPeriodoBean>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            ListEmpleadosDao Dao2 = new ListEmpleadosDao();
            FuncionesNomina DaoNomi = new FuncionesNomina();
            String pathLog = Server.MapPath("~/Content/");

            try
            {
                string path = Server.MapPath("Archivos\\XmlZip\\");
                path = path.Replace("\\Empleados", "");
                path = path + IdEmpresa + "\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                LFechaPerido = Dao2.sp_DatosPerido_Retrieve_DatosPerido(Periodo);

                ListDatEmisor = Dao.GXMLNOM(IdEmpresa, sNombreComple, path, Periodo, anios, Tipodeperido, Masivo, Recibo);


                if (Masivo == 1 && PDF == 1)
                {

                    GenPDF(anios, Tipodeperido, LFechaPerido[0].iPeriodo, Convert.ToString(IdEmpresa), 1);
                    if (ListDatEmisor[0].GrupoEmpresas == 1)
                    {
                        GenPDF(anios, Tipodeperido, LFechaPerido[0].iPeriodo, Convert.ToString(IdEmpresa), 3);

                    }
                }

                if (Directory.Exists(path))
                {
                    Directory.Delete(path, recursive: true);
                }
            }
            catch (Exception exc)
            {
                messageError = exc.Message.ToString();
                if (!Directory.Exists(pathLog + "LOGS"))
                {
                    Directory.CreateDirectory(pathLog + "LOGS");
                }
                using (StreamWriter file = new StreamWriter(pathLog + "LOGS" + @"\\" + "LogXMLNOMINA" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt"))
                {
                    file.Write(messageError + "\n");
                    file.Close();
                }
            }

            return Json(ListDatEmisor);
        }

        [HttpPost]
        public JsonResult TimbXML(int Anio, int TipoPeriodo, int Perido, int Version, string NomArchivo/*,int IdEmpresa, int Recibo*/)
        {
            string CadeSat, UUID, RfcEmi, RfcRep, SelloCF, RfcProv, Nomcer, fechatem, selloemisor;
            int NumEmpleado = 0, idempleado, anios = Anio, Tipodeperido = TipoPeriodo, idEmpresa = 0, ReciboAsi = 0, NoArchivos = 0, Dow = 0, ErrorXml = 0, Recibo = 0;
            List<EmisorReceptorBean> ListDatEmisor = new List<EmisorReceptorBean>();
            List<EmpresasBean> ListEmpresa = new List<EmpresasBean>();
            FuncionesNomina DaNom = new FuncionesNomina();

            var fileName = NomArchivo;
            string PathCarp = "C:\\Recibos\\";
            string PAthCarpPDf;
            string PathPDF = Server.MapPath("Archivos\\PDF\\");
            string PathZip = Server.MapPath("Archivos\\");
            PathPDF = PathPDF.Replace("\\Empleados", "");
            PathZip = PathZip.Replace("\\Empleados", "");
            PathZip = PathZip + NomArchivo;
            string path = Server.MapPath("Archivos\\XmlZip\\");
            path = path.Replace("\\Empleados", "");
            string DowPath = path;
            string ArchError = null;

            if (System.IO.Directory.Exists(PathPDF))
            {

                PathPDF = PathPDF + NomArchivo + "\\";
                PathPDF = PathPDF.Replace(".zip", "");
                Directory.CreateDirectory(PathPDF);
            }

            if (System.IO.Directory.Exists(DowPath))
            {
                string time = DateTime.Now.Date.ToString();
                time = time.Replace("/", "");
                time = time.Replace(":", "");
                DowPath = DowPath + time + "\\";
                Directory.CreateDirectory(DowPath);
            }

            System.IO.Compression.ZipFile.ExtractToDirectory(PathZip, DowPath);


            DirectoryInfo di = new DirectoryInfo(DowPath);
            foreach (var fi in di.GetFiles())
            {

                // nombre y ubicacion del archivo del xml que se va a crear

                string pathxml = DowPath + fi.Name;
                string SNombreArchivo = fi.Name;
                string[] words = SNombreArchivo.Split('_');
                string sIdEmpresa = words[2].ToString().Replace("E", "");
                string sIdEmpresaOrig = ""; // string sIdEmpresaOrig = words[5].ToString().Replace("E", "").Replace(".xml", "");
                string[] sIdEmreR2 = words[0].Split('N');
                if (sIdEmreR2.Length > 1)
                {
                    sIdEmpresaOrig = sIdEmreR2[0].Replace("E", "");
                    ListEmpresa = DaNom.sp_EmpresaDest_Retrieve_CSeparacion(int.Parse(sIdEmpresaOrig), Anio, Perido);
                    idEmpresa = int.Parse(sIdEmpresaOrig);
                    Recibo = 2;
                    if (Perido == 60)
                    {
                        Recibo = 3;
                    }

                }

                if (sIdEmreR2.Length < 2)
                {
                    Recibo = 1;
                    idEmpresa = Convert.ToInt32(sIdEmpresa);
                }

                //nombre y ubicacion del PDF 
                string Nombrearc = PathPDF + fi.Name;
                Nombrearc = Nombrearc.Replace(".xml", ".pdf");
                // crecion del archivo PDF
                FileStream Fs = new FileStream(Nombrearc, FileMode.Create);
                Document documento = new Document(iTextSharp.text.PageSize.LETTER, 5, 10, 10, 5);
                PdfWriter pw = PdfWriter.GetInstance(documento, Fs);
                documento.Open();
                NoArchivos = NoArchivos + 1;
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, BaseFont.EMBEDDED);
                iTextSharp.text.Font TTexNeg = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font TexNom = new iTextSharp.text.Font(bf, 7, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font TexNeg = new iTextSharp.text.Font(bf, 7, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font TTexNegCuerpo = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font TexNegCuerpo = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(pathxml);
                ErrorXml = 0;

                ////////Cabecera 
                XmlNode nodo = xmlDoc.GetElementsByTagName("cfdi:Emisor").Item(0);
                string Palabra = nodo.Attributes.GetNamedItem("Nombre").Value;
                Paragraph Empresa = new Paragraph(50, Palabra, TTexNeg);
                Empresa.IndentationLeft = 70;


                Paragraph Trfc = new Paragraph("R.F.C:", TexNeg);
                Trfc.IndentationLeft = 70;
                Palabra = nodo.Attributes.GetNamedItem("Rfc").Value;
                RfcEmi = Palabra;
                Paragraph Rfc = new Paragraph(-1, Palabra, TexNom);
                Rfc.IndentationLeft = 118;
                Paragraph Rfcpatron = new Paragraph(-1, Palabra, TexNom);
                Rfcpatron.IndentationLeft = 118;

                Paragraph TrfcPatron = new Paragraph("R.F.C. Patron:", TexNeg);
                TrfcPatron.IndentationLeft = 70;

                nodo = xmlDoc.GetElementsByTagName("nomina12:Emisor").Item(0);

                Palabra = " ";
                Paragraph TRegPat = new Paragraph(" ", TexNeg);
                TRegPat.IndentationLeft = 70;
                Paragraph RegPat = new Paragraph(-1, Palabra, TexNom);
                RegPat.IndentationLeft = 118;



                /// direccion Empresa

                ListEmpleadosDao Dao = new ListEmpleadosDao();
                ListDatEmisor = Dao.Sp_EmpresaDir_Retrieve(RfcEmi);


                Paragraph TrEmpraDir = new Paragraph("Dirección:", TexNeg);
                TrEmpraDir.IndentationLeft = 70;
                Palabra = ListDatEmisor[0].sDomiciolioEmple;
                Paragraph EmpraDir = new Paragraph(-1, Palabra, TexNom);
                EmpraDir.IndentationLeft = 118;


                Palabra = ListDatEmisor[0].sDomiciolioEmpre;
                Paragraph EmpraDir2 = new Paragraph(Palabra, TexNom);
                EmpraDir2.IndentationLeft = 118;


                Paragraph TfolioFis = new Paragraph(-50, "Folio Fiscal:", TexNeg);
                TfolioFis.IndentationLeft = 363;
                nodo = xmlDoc.GetElementsByTagName("tfd:TimbreFiscalDigital").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("UUID").Value;
                CadeSat = "||" + Palabra + "|";
                UUID = Palabra;
                Paragraph folioFis = new Paragraph(-1, Palabra, TexNom);
                folioFis.IndentationLeft = 450;

                nodo = xmlDoc.GetElementsByTagName("cfdi:Comprobante").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("NoCertificado").Value;

                Paragraph TNumCertEmi = new Paragraph("No. de serie del Emisor:", TexNeg);
                TNumCertEmi.IndentationLeft = 363;
                Paragraph NumCertEmi = new Paragraph(-1, Palabra, TexNom);
                NumCertEmi.IndentationLeft = 450;
                Paragraph TFechaEmisior = new Paragraph("Lugar de Emisión:", TexNeg);
                TFechaEmisior.IndentationLeft = 363;
                Palabra = "México, " + nodo.Attributes.GetNamedItem("Fecha").Value;
                Paragraph FechaEmisior = new Paragraph(-1, Palabra, TexNom);
                FechaEmisior.IndentationLeft = 450;
                Paragraph TFechaCertifi = new Paragraph("Fecha y hora de Certificación:", TexNeg);
                TFechaCertifi.IndentationLeft = 363;
                nodo = xmlDoc.GetElementsByTagName("tfd:TimbreFiscalDigital").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("FechaTimbrado").Value;
                CadeSat = CadeSat + Palabra + "|";
                Paragraph FechaCertifi = new Paragraph(-1, Palabra, TexNom);
                FechaCertifi.IndentationLeft = 450;
                Paragraph TRegimenFis = new Paragraph("Regimen fiscal:", TexNeg);
                TRegimenFis.IndentationLeft = 363;
                nodo = xmlDoc.GetElementsByTagName("cfdi:Emisor").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("RegimenFiscal").Value;
                if (Palabra == "601") { Palabra = Palabra + "-General De Ley Personas Morales"; }
                Paragraph RegimenFis = new Paragraph(-1, Palabra, TexNom);
                RegimenFis.IndentationLeft = 450;
                Paragraph TTipoCDFI = new Paragraph("Tipo de CDFI:", TexNeg);
                TTipoCDFI.IndentationLeft = 363;
                Paragraph TipoCDFI = new Paragraph(-1, "Recibo de Nomina", TexNom);
                TipoCDFI.IndentationLeft = 450;
                Paragraph TSerieFolio = new Paragraph("Serie y Folio:", TexNeg);
                TSerieFolio.IndentationLeft = 363;
                nodo = xmlDoc.GetElementsByTagName("cfdi:Comprobante").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("Folio").Value;
                Paragraph SerieFolio = new Paragraph(-1, Palabra, TexNom);
                SerieFolio.FirstLineIndent = 450;

                Paragraph TtipoNomina = new Paragraph("Tipo de Nómina:", TTexNegCuerpo);
                TtipoNomina.IndentationLeft = 363;
                nodo = xmlDoc.GetElementsByTagName("nomina12:Nomina").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("TipoNomina").Value;
                if (Palabra == "O")
                {
                    Palabra = "O (Ordinaria) ";
                }
                if (Palabra == "E")
                {
                    Palabra = "E (ExtraOrdinaria ) ";
                }

                Paragraph tipoNomina = new Paragraph(-1, Palabra, TexNegCuerpo);
                tipoNomina.IndentationLeft = 450;


                ////////// Info Personal  
                Paragraph table1 = new Paragraph();
                table1.IndentationLeft = 50;
                PdfPTable table = new PdfPTable(1);
                table.HorizontalAlignment = 0;
                table.PaddingTop = 10;
                table.TotalWidth = 200;
                table.LockedWidth = true;
                BaseFont bf2 = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font TexHatable = new iTextSharp.text.Font(bf2, 8, 1, BaseColor.WHITE);
                // Esta es la primera fila
                PdfPCell Cell = new PdfPCell();
                Cell.BackgroundColor = BaseColor.BLACK;
                Cell.AddElement(new Chunk("INFORMACION PERSONAL DEL TRABAJADOR", TexHatable));
                table.AddCell(Cell);
                table1.Add(table);

                nodo = xmlDoc.GetElementsByTagName("nomina12:Receptor").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("NumEmpleado").Value;
                NumEmpleado = int.Parse(Palabra);
                Paragraph TNoEmpleado = new Paragraph(10, "No.Empleado :", TTexNegCuerpo);
                TNoEmpleado.IndentationLeft = 50;
                Paragraph NoEmpleado = new Paragraph(-1, Palabra, TexNegCuerpo);
                NoEmpleado.IndentationLeft = 108;
                Paragraph TNommbre = new Paragraph("Nombre:", TTexNegCuerpo);
                TNommbre.IndentationLeft = 50;
                nodo = xmlDoc.GetElementsByTagName("cfdi:Receptor").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("Nombre").Value;
                Paragraph Nommbre = new Paragraph(-1, Palabra, TexNegCuerpo);
                Nommbre.IndentationLeft = 108;

                ReciboAsi = 0;
                List<EmisorReceptorBean> ListEmisor = new List<EmisorReceptorBean>();
                ListEmpleadosDao Dao22 = new ListEmpleadosDao();
                ListEmisor = Dao22.sp_EmisorReceptor_Retrieve_EmisorReceptor(idEmpresa, NumEmpleado);

                if (ListEmisor != null) { if (ListEmisor[0].iPagopor == 364) { ReciboAsi = 1; }; };

                if (ListEmisor != null)
                {

                    if (ListEmisor[0].iPagopor != 364 && Recibo == 0)
                    {

                        nodo = xmlDoc.GetElementsByTagName("nomina12:Emisor").Item(0);
                        Palabra = nodo.Attributes.GetNamedItem("RegistroPatronal").Value;
                        TRegPat = new Paragraph("Reg.Pat:", TexNeg);
                        TRegPat.IndentationLeft = 70;
                        RegPat = new Paragraph(-1, Palabra, TexNom);
                        RegPat.IndentationLeft = 118;
                    }
                }
                if (ListEmisor == null || Recibo == 2 || Recibo == 3) { ReciboAsi = 1; }

                Paragraph TCurp = new Paragraph("Curp:", TTexNegCuerpo);
                TCurp.IndentationLeft = 50;
                nodo = xmlDoc.GetElementsByTagName("nomina12:Receptor").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("Curp").Value;
                Paragraph Curp = new Paragraph(-1, Palabra, TexNegCuerpo);
                Curp.IndentationLeft = 108;

                Paragraph TrfcEmp = new Paragraph("R.F.C.:", TTexNegCuerpo);
                TrfcEmp.IndentationLeft = 50;
                nodo = xmlDoc.GetElementsByTagName("cfdi:Receptor").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("Rfc").Value;
                RfcRep = Palabra;
                Paragraph rfcEmp = new Paragraph(-1, Palabra, TexNegCuerpo);
                rfcEmp.IndentationLeft = 108;

                Paragraph TNSS = new Paragraph("", TTexNegCuerpo);
                Paragraph NSS = new Paragraph("", TexNegCuerpo);
                Paragraph TRegimen = new Paragraph("", TTexNegCuerpo);
                Paragraph Regimen = new Paragraph("", TexNegCuerpo);

                if (ReciboAsi != 1)
                {

                    TNSS = new Paragraph("NSS:", TTexNegCuerpo);
                    TNSS.IndentationLeft = 50;
                    nodo = xmlDoc.GetElementsByTagName("nomina12:Receptor").Item(0);
                    Palabra = nodo.Attributes.GetNamedItem("NumSeguridadSocial").Value;
                    NSS = new Paragraph(-1, Palabra, TexNegCuerpo);
                    NSS.IndentationLeft = 108;

                    TRegimen = new Paragraph("Regimen:", TTexNegCuerpo);
                    TRegimen.IndentationLeft = 50;
                    Palabra = nodo.Attributes.GetNamedItem("TipoRegimen").Value;
                    if (Palabra == "02") { Palabra = Palabra + "-Sueldos"; }
                    Regimen = new Paragraph(-1, Palabra, TexNegCuerpo);
                    Regimen.IndentationLeft = 108;

                }
                if (ReciboAsi == 1)
                {
                    TipoCDFI = new Paragraph(-1, "Recibo", TexNom);
                    TipoCDFI.IndentationLeft = 450;
                }
                ///// Informe


                Paragraph table3 = new Paragraph();
                table3.IndentationLeft = 350;

                PdfPTable table2 = new PdfPTable(1);
                table2.HorizontalAlignment = 0;
                table2.PaddingTop = 10;
                table2.TotalWidth = 200;
                table2.LockedWidth = true;
                ///////// Info Laboral

                PdfPCell Cell2 = new PdfPCell();
                Cell2.BackgroundColor = BaseColor.BLACK;
                Cell2.AddElement(new Chunk("INFORMACION LABORAL", TexHatable));
                table2.AddCell(Cell2);
                table3.Add(table2);

                Paragraph TPuesto = new Paragraph(10, "Puesto:", TTexNegCuerpo);
                TPuesto.IndentationLeft = 350;
                nodo = xmlDoc.GetElementsByTagName("nomina12:Receptor").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("Puesto").Value;
                Paragraph Puesto = new Paragraph(-1, Palabra, TexNegCuerpo);
                Puesto.IndentationLeft = 435;

                Paragraph TContrato = new Paragraph("Contrato:", TTexNegCuerpo);
                TContrato.IndentationLeft = 350;
                Palabra = nodo.Attributes.GetNamedItem("TipoContrato").Value;
                Paragraph Contrato = new Paragraph(-1, Palabra, TexNegCuerpo);
                Contrato.IndentationLeft = 435;

                Paragraph TSalarioB = new Paragraph("", TTexNegCuerpo);
                Paragraph SalarioB = new Paragraph("", TexNegCuerpo);
                Paragraph TSalarioInt = new Paragraph("", TTexNegCuerpo);
                Paragraph SalarioInt = new Paragraph("", TexNegCuerpo);
                Paragraph TAntiguedad = new Paragraph("", TTexNegCuerpo);
                Paragraph Antiguedad = new Paragraph("", TexNegCuerpo);
                Paragraph TJornada = new Paragraph("", TTexNegCuerpo);
                Paragraph Jornada = new Paragraph("", TexNegCuerpo);
                Paragraph TRiesgopu = new Paragraph("", TTexNegCuerpo);
                Paragraph Riesgopu = new Paragraph("", TexNegCuerpo);
                Paragraph TFechaInLab = new Paragraph("", TTexNegCuerpo);
                Paragraph FechaInLab = new Paragraph("", TexNegCuerpo);

                if (ReciboAsi != 1)
                {

                    TSalarioB = new Paragraph("Salario Base:", TTexNegCuerpo);
                    TSalarioB.IndentationLeft = 350;
                    Palabra = nodo.Attributes.GetNamedItem("SalarioBaseCotApor").Value;

                    decimal SalD = Convert.ToDecimal(Palabra);
                    SalD = Math.Round(SalD, 2);
                    Palabra = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", SalD);

                    SalarioB = new Paragraph(-1, Palabra, TexNegCuerpo);
                    SalarioB.IndentationLeft = 435;
                    TSalarioInt = new Paragraph("Salario Integrado:", TTexNegCuerpo);
                    TSalarioInt.IndentationLeft = 350;
                    decimal SalInt = Decimal.Round(Convert.ToDecimal(nodo.Attributes.GetNamedItem("SalarioDiarioIntegrado").Value), 2);

                    Palabra = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", SalInt);
                    SalarioInt = new Paragraph(-1, Palabra, TexNegCuerpo);
                    SalarioInt.IndentationLeft = 435;

                    TAntiguedad = new Paragraph("Antigüedad:", TTexNegCuerpo);
                    TAntiguedad.IndentationLeft = 350;

                    Palabra = nodo.Attributes.GetNamedItem("Antigüedad").Value;
                    Antiguedad = new Paragraph(-1, Palabra + "(Semanas)", TexNegCuerpo);
                    Antiguedad.IndentationLeft = 435;

                    TJornada = new Paragraph("Jornada:", TTexNegCuerpo);
                    TJornada.IndentationLeft = 350;

                    Palabra = nodo.Attributes.GetNamedItem("TipoJornada").Value;
                    if (ListEmisor != null)
                    {
                        Palabra = Palabra + "-" + ListEmisor[0].sTipoJornada;
                    }

                    Jornada = new Paragraph(-1, Palabra, TexNegCuerpo);
                    Jornada.IndentationLeft = 435;

                    TRiesgopu = new Paragraph("Riesgo Puesto:", TTexNegCuerpo);
                    TRiesgopu.IndentationLeft = 350;
                    Palabra = nodo.Attributes.GetNamedItem("RiesgoPuesto").Value;
                    Riesgopu = new Paragraph(-1, Palabra, TexNegCuerpo);
                    Riesgopu.IndentationLeft = 435;

                    TFechaInLab = new Paragraph("Fecha de Inicio Laboral :", TTexNegCuerpo);
                    TFechaInLab.IndentationLeft = 350;
                    Palabra = "";

                    if (ListEmisor != null)
                    {
                        Palabra = ListEmisor[0].sFechaIngreso;
                    }

                    FechaInLab = new Paragraph(-1, Palabra, TexNegCuerpo);
                    FechaInLab.IndentationLeft = 435;

                }

                /////////////// tipo de pago 

                Paragraph table6 = new Paragraph();
                table6.IndentationLeft = 50;
                PdfPTable table7 = new PdfPTable(1);
                table7.HorizontalAlignment = 0;
                table7.PaddingTop = 10;
                table7.TotalWidth = 500;
                table7.LockedWidth = true;

                PdfPCell Cell3 = new PdfPCell();
                Cell3.BackgroundColor = BaseColor.BLACK;
                Cell3.AddElement(new Chunk("INFORMACION DE PAGO", TexHatable));
                table7.AddCell(Cell3);
                table6.Add(table7);

                Paragraph TFecPago = new Paragraph("Fecha de Pago:", TTexNegCuerpo);
                TFecPago.IndentationLeft = 50;
                nodo = xmlDoc.GetElementsByTagName("nomina12:Nomina").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("FechaPago").Value;
                Paragraph FecPago = new Paragraph(-1, Palabra, TexNegCuerpo);
                FecPago.IndentationLeft = 110;

                Paragraph TClaveb = new Paragraph("Clave:", TTexNegCuerpo);
                TClaveb.IndentationLeft = 200;
                Palabra = "N/A";


                if (ListEmisor[0].iCgTipoPago != 218)
                {
                    ErrorXml = 1;
                    TClaveb.IndentationLeft = 200;
                    nodo = xmlDoc.GetElementsByTagName("nomina12:Receptor").Item(0);
                    XmlElement root = xmlDoc.DocumentElement;
                    XmlNodeList elemList = root.GetElementsByTagName("nomina12:Receptor");
                    Palabra = nodo.OuterXml;
                    string[] separatingStrings = { " ", "=" };
                    string[] subpalabra = Palabra.Split(separatingStrings, System.StringSplitOptions.None);
                    Palabra = "N/A";

                    for (int i = 0; i < subpalabra.Length; i++)
                    {

                        if (subpalabra[i].ToString() == "CuentaBancaria")
                        {
                            Palabra = nodo.Attributes.GetNamedItem("CuentaBancaria").Value;
                            ErrorXml = 0;
                            i = 100000 + 1;
                        }

                    }


                }
                Paragraph Claveb = new Paragraph(-1, Palabra, TexNegCuerpo);
                Claveb.IndentationLeft = 230;

                List<CInicioFechasPeriodoBean> LPe = new List<CInicioFechasPeriodoBean>();
                FuncionesNomina dao = new FuncionesNomina();
                LPe = dao.sp_Cperiodo_Retrieve_Cperiodo(idEmpresa, anios, TipoPeriodo);


                Paragraph TPeridoFec = new Paragraph("Perido:", TTexNegCuerpo);
                TPeridoFec.IndentationLeft = 200;

                for (int i = 0; i < LPe.Count; i++)
                {

                    if (Perido == LPe[i].iPeriodo)
                    {
                        Palabra = Perido + " (" + LPe[i].sFechaInicio + "-" + LPe[i].sFechaFinal + ")";

                    }
                }

                Paragraph PeridoFec = new Paragraph(-1, Palabra, TexNegCuerpo);
                PeridoFec.IndentationLeft = 230;

                Paragraph TMoneda = new Paragraph("Moneda:", TTexNegCuerpo);
                TMoneda.IndentationLeft = 200;
                Palabra = "MXP";
                Paragraph Moneda = new Paragraph(-1, Palabra, TexNegCuerpo);
                Moneda.IndentationLeft = 230;


                Palabra = "   ";
                Paragraph TBanco = new Paragraph("Banco:", TTexNegCuerpo);
                TBanco.IndentationLeft = 50;
                string sbanco = "";
                if (ListEmisor[0].iCgTipoPago != 218)
                {

                    TBanco = new Paragraph("Banco:", TTexNegCuerpo);
                    TBanco.IndentationLeft = 50;

                    if (Palabra.Length >= 7 && Palabra.Length < 18)
                    {
                        nodo = xmlDoc.GetElementsByTagName("nomina12:Receptor").Item(0);
                        Palabra = nodo.Attributes.GetNamedItem("Banco").Value;
                        sbanco = Palabra;
                    }
                    else
                    {
                        sbanco = Palabra.Substring(0, 3);
                    }


                }
                if (ListEmisor != null)
                {

                    sbanco = sbanco + " " + ListEmisor[0].sDescripcion;
                }
                Paragraph Banco = new Paragraph(-1, sbanco, TexNegCuerpo);
                Banco.IndentationLeft = 103;
                Paragraph TPeriodo = new Paragraph(-22, "Periodo:", TTexNegCuerpo);
                TPeriodo.IndentationLeft = 200;
                nodo = xmlDoc.GetElementsByTagName("nomina12:Receptor").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("PeriodicidadPago").Value;

                if (Palabra == "02")
                {
                    Palabra = "02 Semanal";
                }
                if (Palabra == "10")
                {
                    Palabra = "10 Docenal";
                }
                if (Palabra == "04")
                {
                    Palabra = "04 Quincenal";
                }
                if (Palabra == "03")
                {
                    Palabra = "03 Catorcenal";
                }
                if (Palabra == "05")
                {
                    Palabra = "05 Mensual";
                }
                if (Palabra == "06")
                {
                    Palabra = "06 Bimestral";
                }

                Paragraph Periodo = new Paragraph(-1, Palabra, TexNegCuerpo);
                Periodo.IndentationLeft = 230;
                Paragraph TLugarExp = new Paragraph(-30, "Lugar de Expedicion:", TTexNegCuerpo);
                TLugarExp.IndentationLeft = 380;
                nodo = xmlDoc.GetElementsByTagName("cfdi:Comprobante").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("LugarExpedicion").Value;
                Paragraph LugarExp = new Paragraph(-1, "Cp: " + Palabra, TexNegCuerpo);
                LugarExp.IndentationLeft = 455;

                Paragraph TTipopago = new Paragraph("Tipo de pago:", TTexNegCuerpo);
                TTipopago.IndentationLeft = 380;
                nodo = xmlDoc.GetElementsByTagName("cfdi:Comprobante").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("FormaPago").Value;
                Paragraph Tipopago = new Paragraph(-1, Palabra, TexNegCuerpo);
                Tipopago.IndentationLeft = 455;

                Paragraph TDiasPag = new Paragraph("Dias pagados:", TTexNegCuerpo);
                TDiasPag.IndentationLeft = 50;
                nodo = xmlDoc.GetElementsByTagName("nomina12:Nomina").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("NumDiasPagados").Value;
                Paragraph DiasPag = new Paragraph(-1, Palabra, TexNegCuerpo);
                DiasPag.IndentationLeft = 110;


                Paragraph table8 = new Paragraph();
                table8.IndentationLeft = 50;

                PdfPTable table9 = new PdfPTable(1);
                table9.HorizontalAlignment = 0;
                table9.PaddingTop = 10;
                table9.TotalWidth = 350;
                table9.LockedWidth = true;

                PdfPCell Cell4 = new PdfPCell();
                Cell4.BackgroundColor = BaseColor.BLACK;
                Cell4.AddElement(new Chunk("LEYENDA", TexHatable));
                table9.AddCell(Cell4);
                table8.Add(table9);

                Paragraph Espacio5 = new Paragraph(-16, " ");
                Paragraph table10 = new Paragraph();
                table10.IndentationLeft = 350;
                PdfPTable table11 = new PdfPTable(1);
                table11.HorizontalAlignment = 0;
                table11.PaddingTop = 10;
                table11.TotalWidth = 150;
                table11.LockedWidth = true;

                PdfPCell Cell5 = new PdfPCell();
                Cell5.BackgroundColor = BaseColor.BLACK;
                Cell5.AddElement(new Chunk("PERCEPCIONES", TexHatable));
                table11.AddCell(Cell5);
                table10.Add(table11);

                Paragraph Espacio6 = new Paragraph(-16, " ");
                Paragraph table12 = new Paragraph();
                table12.IndentationLeft = 450;
                PdfPTable table13 = new PdfPTable(1);
                table13.HorizontalAlignment = 0;
                table13.PaddingTop = 10;
                table13.TotalWidth = 100;
                table13.LockedWidth = true;

                PdfPCell Cell6 = new PdfPCell();
                Cell6.BackgroundColor = BaseColor.BLACK;
                Cell6.AddElement(new Chunk("DEDUCIONES", TexHatable));
                table13.AddCell(Cell6);
                table12.Add(table13);


                Paragraph Espacio = new Paragraph(10, " ");
                Paragraph Espacio2 = new Paragraph(-80, " ");
                Paragraph Espacio3 = new Paragraph(10, " ");
                Paragraph espacio4 = new Paragraph(25, " ", TexNegCuerpo);
                Paragraph Espacio9 = new Paragraph(5, " ");
                Paragraph Espacio10 = new Paragraph(-62, " ");
                Paragraph Espacio11 = new Paragraph(30, " ");

                /// imprime en documento
                documento.Add(Empresa);
                documento.Add(Trfc);
                documento.Add(Rfc);
                documento.Add(TrfcPatron);
                documento.Add(Rfcpatron);
                documento.Add(TrEmpraDir);
                documento.Add(EmpraDir);
                documento.Add(EmpraDir2);
                documento.Add(TRegPat);
                documento.Add(RegPat);


                documento.Add(TfolioFis);
                documento.Add(folioFis);
                documento.Add(TNumCertEmi);
                documento.Add(NumCertEmi);
                documento.Add(TFechaEmisior);
                documento.Add(FechaEmisior);
                documento.Add(TFechaCertifi);
                documento.Add(FechaCertifi);
                documento.Add(TRegimenFis);
                documento.Add(RegimenFis);
                documento.Add(TTipoCDFI);
                documento.Add(TipoCDFI);
                documento.Add(TSerieFolio);
                documento.Add(SerieFolio);
                documento.Add(TtipoNomina);
                documento.Add(tipoNomina);



                documento.Add(Espacio);
                documento.Add(table1);
                documento.Add(TNoEmpleado);
                documento.Add(NoEmpleado);
                documento.Add(TNommbre);
                documento.Add(Nommbre);
                documento.Add(TCurp);
                documento.Add(Curp);
                documento.Add(TrfcEmp);
                documento.Add(rfcEmp);

                if (ReciboAsi != 1)
                {
                    documento.Add(TNSS);
                    documento.Add(NSS);

                    documento.Add(TRegimen);
                    documento.Add(Regimen);
                }
                if (ReciboAsi == 1)
                {
                    documento.Add(Espacio9);
                }
                if (ReciboAsi == 1)
                {
                    documento.Add(Espacio10);
                }
                if (ReciboAsi != 1)
                {
                    documento.Add(Espacio2);
                }


                documento.Add(table3);

                //informe
                documento.Add(TPuesto);
                documento.Add(Puesto);
                documento.Add(TContrato);
                documento.Add(Contrato);

                if (ReciboAsi != 1)
                {
                    documento.Add(TSalarioB);
                    documento.Add(SalarioB);
                    documento.Add(TSalarioInt);
                    documento.Add(SalarioInt);
                    documento.Add(TAntiguedad);
                    documento.Add(Antiguedad);
                    documento.Add(TJornada);
                    documento.Add(Jornada);
                    documento.Add(TRiesgopu);
                    documento.Add(Riesgopu);
                    documento.Add(TFechaInLab);
                    documento.Add(FechaInLab);

                }
                if (ReciboAsi != 1)
                {
                    documento.Add(Espacio3);
                }
                if (ReciboAsi == 1)
                {
                    documento.Add(Espacio11);
                }

                documento.Add(table6);
                documento.Add(TFecPago);
                documento.Add(FecPago);
                documento.Add(TBanco);
                documento.Add(Banco);
                documento.Add(TDiasPag);
                documento.Add(DiasPag);
                documento.Add(TPeriodo);
                documento.Add(Periodo);
                documento.Add(TPeridoFec);
                documento.Add(PeridoFec);
                documento.Add(TClaveb);
                documento.Add(Claveb);
                documento.Add(TMoneda);
                documento.Add(Moneda);
                documento.Add(TLugarExp);
                documento.Add(LugarExp);
                documento.Add(TTipopago);
                documento.Add(Tipopago);

                documento.Add(espacio4);
                documento.Add(table8);
                documento.Add(Espacio5);
                documento.Add(table10);
                documento.Add(Espacio5);
                documento.Add(table12);

                int a = 0;
                string Palabra2, palabra3;
                decimal valor;
                decimal per = 0;
                decimal ded = 0;
                decimal total;
                for (int i = 0; i < 30;)
                {
                    nodo = xmlDoc.GetElementsByTagName("nomina12:Percepcion").Item(a);
                    if (nodo != null)
                    {
                        Palabra = nodo.Attributes.GetNamedItem("Concepto").Value;
                        Palabra2 = nodo.Attributes.GetNamedItem("ImporteGravado").Value;
                        palabra3 = nodo.Attributes.GetNamedItem("ImporteExento").Value;
                        valor = decimal.Parse(Palabra2) + decimal.Parse(palabra3);
                        string valor2 = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", valor);

                        per = per + valor;
                        Paragraph TLeyenda = new Paragraph(Palabra, TexNegCuerpo);
                        TLeyenda.IndentationLeft = 75;
                        Paragraph TPercep = new Paragraph(-1, valor2, TexNegCuerpo);
                        TPercep.IndentationLeft = 375;
                        documento.Add(TLeyenda);
                        documento.Add(TPercep);
                        a = a + 1;
                    }
                    else
                    {
                        i = 35;
                    }
                }
                a = 0;
                for (int i = 0; i < 30;)
                {
                    nodo = xmlDoc.GetElementsByTagName("nomina12:OtroPago").Item(a);
                    if (nodo != null)
                    {
                        Palabra = nodo.Attributes.GetNamedItem("Concepto").Value;
                        Palabra2 = nodo.Attributes.GetNamedItem("Importe").Value;
                        valor = decimal.Parse(Palabra2);
                        Palabra2 = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Palabra2);


                        per = per + valor;
                        Paragraph TLeyenda = new Paragraph(Palabra, TexNegCuerpo);
                        TLeyenda.IndentationLeft = 75;
                        Paragraph TPercep = new Paragraph(-1, Palabra2, TexNegCuerpo);
                        TPercep.IndentationLeft = 375;
                        documento.Add(TLeyenda);
                        documento.Add(TPercep);
                        a = a + 1;
                    }
                    else
                    {
                        i = 35;
                    }
                }
                a = 0;
                for (int i = 0; i < 30;)
                {
                    nodo = xmlDoc.GetElementsByTagName("nomina12:Deduccion").Item(a);

                    if (nodo != null)
                    {
                        Palabra = nodo.Attributes.GetNamedItem("Concepto").Value;
                        Palabra2 = nodo.Attributes.GetNamedItem("Importe").Value;
                        valor = decimal.Parse(Palabra2);
                        Palabra2 = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Palabra2);
                        ded = ded + valor;
                        Paragraph TLeyenda = new Paragraph(Palabra, TexNegCuerpo);
                        TLeyenda.IndentationLeft = 75;
                        Paragraph TDedu = new Paragraph(-1, Palabra2, TexNegCuerpo);
                        TDedu.IndentationLeft = 475;

                        documento.Add(TLeyenda);
                        documento.Add(TDedu);
                        a = a + 1;
                    }
                    else
                    {
                        i = 35;
                    }

                }

                a = 0;
                for (int i = 0; i < 30;)
                {
                    nodo = xmlDoc.GetElementsByTagName("nomina12:Empresa_id").Item(a);

                    if (nodo != null)
                    {
                        Palabra = nodo.Attributes.GetNamedItem("Id").Value;
                        Palabra2 = nodo.Attributes.GetNamedItem("Nombre").Value;
                        valor = decimal.Parse(Palabra2);
                        Palabra2 = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Palabra2);
                        ded = ded + valor;
                        Paragraph TLeyenda = new Paragraph(Palabra, TexNegCuerpo);
                        TLeyenda.IndentationLeft = 75;
                        Paragraph TDedu = new Paragraph(-1, Palabra2, TexNegCuerpo);
                        TDedu.IndentationLeft = 475;

                        documento.Add(TLeyenda);
                        documento.Add(TDedu);
                        a = a + 1;
                    }
                    else
                    {
                        i = 35;
                    }

                }

                Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 0.2f)));
                p.IndentationLeft = 50;
                p.IndentationRight = 50;
                documento.Add(p);
                Paragraph Ttotal = new Paragraph("Total:", TTexNegCuerpo);
                Ttotal.IndentationLeft = 350;
                string perp = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", per);

                Paragraph Tper = new Paragraph(-1, perp, TTexNegCuerpo);
                Tper.IndentationLeft = 375;

                Paragraph Tper2 = new Paragraph(-1, perp, TTexNegCuerpo);
                Tper2.IndentationLeft = 475;

                string deduc = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", ded);


                Paragraph Tdedu = new Paragraph(-1, deduc, TTexNegCuerpo);
                Tdedu.IndentationLeft = 475;

                Paragraph Tdedu2 = new Paragraph(-1, deduc, TTexNegCuerpo);
                Tdedu2.IndentationLeft = 475;

                total = per - ded;

                string Rtotal = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", total);

                Paragraph Ttotal2 = new Paragraph(-1, Rtotal, TTexNegCuerpo);
                Ttotal2.IndentationLeft = 475;


                Paragraph Espacio7 = new Paragraph(10, " ");
                Paragraph TSbtotal = new Paragraph("Subtotal:", TTexNegCuerpo);
                TSbtotal.IndentationLeft = 430;
                Paragraph TDes = new Paragraph("Descuento:", TTexNegCuerpo);
                TDes.IndentationLeft = 430;

                Paragraph TTotal2 = new Paragraph("Total:", TTexNegCuerpo);
                TTotal2.IndentationLeft = 430;

                Paragraph Espacio8 = new Paragraph(5, " ");
                Paragraph table14 = new Paragraph();
                table14.IndentationLeft = 50;

                PdfPTable table15 = new PdfPTable(1);
                table15.HorizontalAlignment = 0;
                table15.PaddingTop = 10;
                table15.TotalWidth = 250;
                table15.LockedWidth = true;

                PdfPCell Cell7 = new PdfPCell();
                Cell7.BackgroundColor = BaseColor.BLACK;
                Cell7.AddElement(new Chunk("SELLO DIGITAL EMISOR", TexHatable));
                table15.AddCell(Cell7);

                nodo = xmlDoc.GetElementsByTagName("cfdi:Comprobante").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("Sello").Value;
                string selloemi = Palabra;
                selloemisor = Palabra;
                Paragraph SelloEmi = new Paragraph(Palabra, TexNegCuerpo);

                Paragraph TSello = new Paragraph();
                table15.AddCell(SelloEmi);
                table14.Add(table15);



                Paragraph table16 = new Paragraph();
                table16.IndentationLeft = 50;

                PdfPTable table17 = new PdfPTable(1);
                table17.HorizontalAlignment = 0;
                table17.PaddingTop = 10;
                table17.TotalWidth = 250;
                table17.LockedWidth = true;

                PdfPCell Cell8 = new PdfPCell();
                Cell8.BackgroundColor = BaseColor.BLACK;
                Cell8.AddElement(new Chunk("SELLO DIGITAL DEL SAT", TexHatable));
                table17.AddCell(Cell8);

                nodo = xmlDoc.GetElementsByTagName("tfd:TimbreFiscalDigital").Item(0);
                Palabra = nodo.Attributes.GetNamedItem("SelloCFD").Value;
                SelloCF = Palabra;
                Palabra = nodo.Attributes.GetNamedItem("SelloSAT").Value;

                string SelloSat2 = Palabra;
                Paragraph SelloSAT = new Paragraph(Palabra, TexNegCuerpo);
                table17.AddCell(SelloSAT);
                table16.Add(table17);

                //--
                Paragraph table18 = new Paragraph();
                table18.IndentationLeft = 50;

                PdfPTable table19 = new PdfPTable(1);
                table19.HorizontalAlignment = 0;
                table19.PaddingTop = 10;
                table19.TotalWidth = 250;
                table19.LockedWidth = true;

                PdfPCell Cell9 = new PdfPCell();
                Cell9.BackgroundColor = BaseColor.BLACK;
                Cell9.AddElement(new Chunk("Cadena Original del complemento de certificación digital del SAT", TexHatable));
                table19.AddCell(Cell9);

                nodo = xmlDoc.GetElementsByTagName("tfd:TimbreFiscalDigital").Item(0);
                RfcProv = nodo.Attributes.GetNamedItem("RfcProvCertif").Value;
                Nomcer = nodo.Attributes.GetNamedItem("NoCertificadoSAT").Value;
                fechatem = nodo.Attributes.GetNamedItem("FechaTimbrado").Value;

                CadeSat = CadeSat + nodo.Attributes.GetNamedItem("RfcProvCertif").Value + "|" + selloemi + "|" + nodo.Attributes.GetNamedItem("NoCertificadoSAT").Value + "||";
                Paragraph CaSelloSAT = new Paragraph(CadeSat, TexNegCuerpo);
                table19.AddCell(CaSelloSAT);
                table18.Add(table19);

                documento.Add(Ttotal);
                documento.Add(Tper);
                documento.Add(Tdedu);
                documento.Add(Espacio7);
                documento.Add(TSbtotal);
                documento.Add(Tper2);
                documento.Add(TDes);
                documento.Add(Tdedu2);
                documento.Add(TTotal2);
                documento.Add(Ttotal2);
                documento.Add(Espacio8);
                documento.Add(table14);
                documento.Add(table16);
                documento.Add(table18);

                /// imagen Qr
                /// 


                string QrSat = "https://verificacfdi.facturaelectronica.sat.gob.mx/Defaul.aspx?id=" + UUID + "&re=" + RfcEmi + "&rr=" + RfcRep + "&tt=" + Rtotal + "&fe=" + selloemi.Substring(selloemi.Length - 8, 8);
                QRCodeEncoder encoder = new QRCodeEncoder();
                Bitmap img = encoder.Encode(QrSat);
                System.Drawing.Image QR = (System.Drawing.Image)img;

                using (MemoryStream ms = new MemoryStream())
                {
                    QR.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] imageBytes = ms.ToArray();

                    iTextSharp.text.Image imgQr = iTextSharp.text.Image.GetInstance(ms.ToArray());
                    imgQr.BorderWidth = 0;
                    imgQr.SetAbsolutePosition(400, 150);
                    float porcentaje = 0.0f;
                    porcentaje = 100 / imgQr.Width;
                    imgQr.ScalePercent(porcentaje * 100);
                    documento.Add(imgQr);

                }


                documento.Close();

                if (ErrorXml != 1)
                {
                    FuncionesNomina Daos = new FuncionesNomina();

                    if (Recibo == 2)
                    {
                        Daos.sp_Tsellos_InsertUPdate_TSellosSat(2, 0, ListEmpresa[0].iIdEmpresa, NumEmpleado, anios, Tipodeperido, Perido, "nominaR2", selloemisor, UUID, SelloCF, RfcProv, Nomcer, fechatem);

                    }

                    else if (Recibo == 3)
                    {
                        Daos.sp_Tsellos_InsertUPdate_TSellosSat(2, 0, idEmpresa, NumEmpleado, anios, Tipodeperido, Perido, "nomina", selloemisor, UUID, SelloCF, RfcProv, Nomcer, fechatem);


                    }

                    else
                    {

                        Daos.sp_Tsellos_InsertUPdate_TSellosSat(1, 0, idEmpresa, NumEmpleado, anios, Tipodeperido, Perido, "nomina", selloemisor, UUID, SelloCF, RfcProv, Nomcer, fechatem);

                    }

                }

                if (ErrorXml == 1)
                {

                    if (System.IO.File.Exists(Nombrearc))
                    {
                        System.IO.File.Delete(Nombrearc);

                        if (ArchError != null)
                        {
                            ArchError = ArchError + "," + NumEmpleado;
                            NoArchivos = NoArchivos - 1;
                        }

                        if (ArchError == null)
                        {
                            ArchError = NumEmpleado.ToString();
                            NoArchivos = NoArchivos - 1;
                        }


                    }


                }

            };



            string pathxm = path;
            path = path.Replace("\\XmlZip", "");
            path = path + "Pdfzio.zip";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            System.IO.Compression.ZipFile.CreateFromDirectory(PathPDF, path);
            string sourceDirPdf = PathPDF;
            string SourceDirXml = DowPath;
            try
            {

                string[] pdfList = Directory.GetFiles(sourceDirPdf, "*.pdf");
                string[] xmlList = Directory.GetFiles(SourceDirXml, "*.xml");
                foreach (string f in pdfList)
                {
                    string fName = f.Substring(sourceDirPdf.Length + 1);
                    string[] words = fName.Split('_');
                    // Remove path from the file name.
                    if (Recibo == 1)
                    {


                        string sIdEmpresa = words[2].ToString().Replace("E", "");
                        idEmpresa = int.Parse(sIdEmpresa);
                        NumEmpleado = int.Parse(words[4].ToString().Replace("N", "").Replace(".pdf", ""));

                    }

                    /// idEmpresa =Convert.ToInt32(sIdEmpresa);
                    /// int idempleado = Convert.ToInt32(sIdEmpleados);
                    PAthCarpPDf = "";
                    if (Recibo == 2)
                    {

                        string[] sIdEmreR2 = words[0].Split('N');
                        if (sIdEmreR2.Length > 1)
                        {
                            idEmpresa = int.Parse(sIdEmreR2[0].Replace("E", ""));
                            NumEmpleado = int.Parse(sIdEmreR2[1].ToString());

                        }

                        PAthCarpPDf = PathCarp + "Fiscal\\Empresa" + ListEmpresa[0].iIdEmpresa + "\\";


                    }
                    else if (Recibo == 3)
                    {

                        string[] sIdEmreR2 = words[0].Split('N');
                        if (sIdEmreR2.Length > 1)
                        {
                            idEmpresa = int.Parse(sIdEmreR2[0].Replace("E", ""));
                            NumEmpleado = int.Parse(sIdEmreR2[1].ToString());

                        }

                        PAthCarpPDf = PathCarp + "Fiscal\\Empresa" + idEmpresa + "\\";
                    }
                    else
                    {
                        PAthCarpPDf = PathCarp + "Fiscal\\Empresa" + idEmpresa + "\\";

                    }


                    if (System.IO.File.Exists(f))
                    {
                        if (System.IO.File.Exists(PAthCarpPDf))
                        {
                        }
                        else
                        {

                            DirectoryInfo did = Directory.CreateDirectory(PAthCarpPDf);

                        }

                        PAthCarpPDf = "";
                        if (Recibo == 2)
                        {

                            PAthCarpPDf = PathCarp + "Fiscal\\Empresa" + ListEmpresa[0].iIdEmpresa + "\\";

                        }

                        else if (Recibo == 3)
                        {

                            PAthCarpPDf = PathCarp + "Fiscal\\Empresa" + idEmpresa + "\\";

                        }

                        else
                        {
                            PAthCarpPDf = PathCarp + "Fiscal\\Empresa" + idEmpresa + "\\";

                        }



                        PAthCarpPDf = PAthCarpPDf + fName;
                        List<EmpleadosBean> Empleados = new List<EmpleadosBean>();
                        ListEmpleadosDao Dao2 = new ListEmpleadosDao();
                        if (Recibo == 2)
                        {
                            Dao2.sp_CCejecucionAndSen_update_TsellosSat(ListEmpresa[0].iIdEmpresa, NumEmpleado, anios, Tipodeperido, Perido, 5, PAthCarpPDf);

                        }

                        else
                        {

                            Dao2.sp_CCejecucionAndSen_update_TsellosSat(idEmpresa, NumEmpleado, anios, Tipodeperido, Perido, 0, PAthCarpPDf);

                        }

                        System.IO.File.Copy(f, PAthCarpPDf, true);
                        System.IO.File.Delete(f);
                    }


                }
                foreach (string f in xmlList)
                {
                    // Remove path from the file name.

                    string fName = "";
                    if (Recibo == 1)
                    {

                        fName = f.Substring(SourceDirXml.Length + 1);
                        string[] words = fName.Split('_');
                        string sIdEmpresa = words[2].ToString().Replace("E", "");
                        idEmpresa = int.Parse(sIdEmpresa);

                    }

                    if (System.IO.File.Exists(f))
                    {
                        PAthCarpPDf = "";
                        if (Recibo == 2)
                        {

                            PAthCarpPDf = PathCarp + "Fiscal\\Empresa" + ListEmpresa[0].iIdEmpresa + "\\";

                        }
                        else
                        {
                            PAthCarpPDf = PathCarp + "Fiscal\\Empresa" + idEmpresa + "\\";
                        }
                        PAthCarpPDf = PAthCarpPDf + fName;

                        System.IO.File.Copy(f, PAthCarpPDf, true);

                        System.IO.File.Copy(f, PAthCarpPDf, true);
                        System.IO.File.Delete(f);
                    }



                }




                if (System.IO.Directory.Exists(DowPath))
                {
                    System.IO.Directory.Delete(DowPath, recursive: true);
                }

            }
            catch (DirectoryNotFoundException dirNotFound)
            {
                Console.WriteLine(dirNotFound.Message);

            }

            List<EmisorReceptorBean> ListDatEmisor2 = new List<EmisorReceptorBean>();
            EmisorReceptorBean list = new EmisorReceptorBean();
            list.sMensaje = path;
            ListDatEmisor2.Add(list);
            ListDatEmisor[0].iNoEjecutados = NoArchivos;
            ListDatEmisor[0].archXmlErr = ArchError;
            return Json(ListDatEmisor);
        }




        [HttpPost]
        public ActionResult LoadFile(HttpPostedFileBase fileUpload)
        {
            try
            {
                string path = Server.MapPath("~/Archivos/");
                if (Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                fileUpload.SaveAs(path + Path.GetFileName(fileUpload.FileName));

            }
            catch (Exception e)
            {
                return Json(new { Value = false, Message = e.Message }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { Value = true, Message = "Archivo cargado se procedera el timbrado" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult TotalesRecibo(int iIdEmpresa, int iIdEmpleado, int iPeriodo, int iespejo, int ianio, int iTipodePerido)
        {
            decimal Ren481 = 0;
            List<ReciboNominaBean> ListTotales = new List<ReciboNominaBean>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            ListTotales = Dao.sp_SaldosTotales_Retrieve_TPlantillasCalculos(iIdEmpresa, iIdEmpleado, ianio, iTipodePerido, iPeriodo, iespejo);

            if (ListTotales != null)
            {
                for (int i = 0; i < ListTotales.Count; i++)
                {

                    if (ListTotales[i].iIdRenglon == 481)
                    {

                        Ren481 = ListTotales[i].dSaldo;

                    }

                    if (ListTotales[i].iIdRenglon == 990)
                    {

                        ListTotales[i].dSaldo = ListTotales[i].dSaldo - Ren481;
                    }
                    if (ListTotales[i].iIdRenglon == 9999)
                    {
                        ListTotales[i].dSaldo = ListTotales[i].dSaldo - Ren481;
                    }



                }


            }

            return Json(ListTotales);
        }

        [HttpPost]
        public JsonResult SearchDataEmpleado(int Empleado_id, int Empresa_id)
        {
            List<string> empleados = new List<string>();
            pruebaEmpleadosDao Dao = new pruebaEmpleadosDao();
            if (Empresa_id == 0)
            {
                Empresa_id = int.Parse(Session["IdEmpresa"].ToString());
            }
            Session["Empleado_id"] = Empleado_id;
            int Perfil_id = int.Parse(Session["Profile"].ToString());
            empleados = Dao.sp_Templeado_Retrieve_DatosEmpleado(Empresa_id, Empleado_id, Perfil_id);
            return Json(empleados);
        }

        /// generar Pdf en emision de Recibos
        [HttpPost]
        public JsonResult GenPDF(int Anio, int TipoPeriodo, int Perido, String sIdEmpresas, int iRecibo)
        {

            int Idusuario = Convert.ToInt32(Session["iIdUsuario"]), inactivo = 0, NoEjecuciones = 0, row1007 = 0, row1007Ac = 0;
            string sDEscripcion = " " + Idusuario;

            List<EmpresasBean> NoEmple = new List<EmpresasBean>();
            List<EmpleadosBean> Empleados = new List<EmpleadosBean>();
            List<EmisorReceptorBean> ListDatEmisor = new List<EmisorReceptorBean>();
            List<EmisorReceptorBean> url = new List<EmisorReceptorBean>();
            List<ControlEjecucionBean> LisIdcontrol = new List<ControlEjecucionBean>();
            List<EmisorReceptorBean> ListDirec = new List<EmisorReceptorBean>();
            FuncionesNomina Dao = new FuncionesNomina();
            ListEmpleadosDao Dao2 = new ListEmpleadosDao();


            string CadeSat, UUID, RfcEmi, RfcRep, SelloCF, RfcProv, Nomcer, fechatem, selloemisor;
            int NumEmpleado, anios = Anio, Tipodeperido = TipoPeriodo, Version = 12, Folio = 0;
            Folio = Anio * 100000 + TipoPeriodo * 10000 + Perido * 10;
            var fileName = "";
            string PathPDF = "";
            string PathCarp = "C:\\Recibos\\";
            PathPDF = PathPDF.Replace("\\Empleados", "");
            string pathCarp2 = "";
            string path = Server.MapPath("Archivos\\XmlZip\\");
            PathPDF = PathPDF.Replace("\\Empleados", "");
            string Nombrearc = PathPDF;
            int idEmpresa = 0, rows = 0;
            string[] valores = sIdEmpresas.Split(' ');

            rows = valores.Length - 1;


            if (rows == 0)
            {
                rows = valores.Length;
            }

            int idempleado = 0;
            string urlpdf;
            int defi = 0;
            for (int i = 0; i < rows; i++)
            {
                idEmpresa = Convert.ToInt32(valores[i]);
                int Op = 0;
                if (iRecibo == 2)
                {
                    Op = 3;
                }
                NoEmple = Dao.sp_NumeroEmple_Retrieve_TpCalculosLn(idEmpresa, TipoPeriodo, Perido, anios, Op);
                if (defi == 0)
                {
                    LisIdcontrol = Dao2.ps_ControlEje_Insert_CControlEjecEmpr(Idusuario, sDEscripcion, inactivo, idEmpresa, anios, Tipodeperido, Perido, iRecibo, 0);
                    defi = 1;
                }
                Dao2.sp_CControlEjeLn_insert_CControlEjeLn(LisIdcontrol[0].iIdContro, idEmpresa, 0, anios, Tipodeperido, Perido, iRecibo);
                Op = 1;
                if (iRecibo == 2) { Op = 4; };
                Empleados = Dao.sp_EmpleadosEmpresa_periodo(idEmpresa, TipoPeriodo, Perido, anios, Op);

                //NoEjecuciones = NoEjecuciones + NoEmple[0].iNoEmpleados;

                if (iRecibo == 1)
                {
                    pathCarp2 = PathCarp + "Simple\\Empresa" + idEmpresa + "\\";
                }
                if (iRecibo == 2)
                {
                    pathCarp2 = PathCarp + "Fiscal\\Empresa" + idEmpresa + "\\";
                }
                if (iRecibo == 3)
                {
                    pathCarp2 = PathCarp + "Recibo2\\Empresa" + idEmpresa + "\\";
                }
                if (System.IO.File.Exists(pathCarp2))
                {

                    PathPDF = pathCarp2;

                }
                else
                {

                    DirectoryInfo di = Directory.CreateDirectory(pathCarp2);
                    PathPDF = pathCarp2;
                }

                // con QR

                for (int a = 0; a < NoEmple[0].iNoEmpleados; a++)
                {
                    //nombre y unicacion del PDF
                    Nombrearc = PathPDF;
                    if (iRecibo == 1)
                    {
                        Nombrearc = Nombrearc + "Recibo_E" + idEmpresa + "_N" + Empleados[a].iIdEmpleado + "_F" + Folio + ".pdf";
                    }
                    if (iRecibo == 2)
                    {
                        Nombrearc = Nombrearc + "ReciboFiscal_E" + idEmpresa + "_N" + Empleados[a].iIdEmpleado + "_F" + Folio + ".pdf";
                    }

                    if (iRecibo == 3)
                    {
                        Nombrearc = Nombrearc + "Recibo2_E" + idEmpresa + "_N" + Empleados[a].iIdEmpleado + "_F" + Folio + ".pdf";
                    }

                    if (System.IO.File.Exists(Nombrearc))
                    {
                        System.IO.File.Delete(Nombrearc);
                    }


                    int valido = 0;
                    idempleado = Empleados[a].iIdEmpleado;
                    if (idempleado == 912826)
                    {

                        string stop = "";
                    }

                    ListDatEmisor = Dao2.sp_EmisorReceptor_Retrieve_EmisorReceptor(idEmpresa, Empleados[a].iIdEmpleado);
                    string sAntiguedad = "";
                    List<XMLBean> LisCer = new List<XMLBean>();

                    // con QR
                    List<SelloSatBean> LiTsat = new List<SelloSatBean>();
                    LiTsat = Dao2.sp_DatosSat_Retrieve_TSellosSat(idEmpresa, anios, Tipodeperido, Perido, Empleados[a].iIdEmpleado);
                    List<ReciboNominaBean> LisTRecibo = new List<ReciboNominaBean>();
                    int tipoRecibo = 0;
                    if (iRecibo == 3) { tipoRecibo = 1; };

                    LisTRecibo = Dao.sp_TpCalculoEmpleado_Retrieve_TpCalculoEmpleado(idEmpresa, ListDatEmisor[0].iIdEmpleado, Perido, TipoPeriodo, Anio, tipoRecibo);

                    if (ListDatEmisor != null)
                    {
                        if (iRecibo == 1 && ListDatEmisor[0].sRFC.Length > 3 && LisTRecibo != null)
                        {
                            Dao2.sp_CCejecucionAndSen_update_TsellosSat(idEmpresa, idempleado, anios, Tipodeperido, Perido, 2, Nombrearc);

                            NoEjecuciones = NoEjecuciones + 1;
                            valido = 1;
                        };
                        if (iRecibo == 2 && LiTsat != null && ListDatEmisor[0].sRFC.Length > 3 && LisTRecibo != null)
                        {
                            if (LiTsat[0].sUUID.Length > 3)
                            {
                                NoEjecuciones = NoEjecuciones + 1;
                                Dao2.sp_CCejecucionAndSen_update_TsellosSat(idEmpresa, idempleado, anios, Tipodeperido, Perido, 0, Nombrearc);
                                valido = 1;
                            };

                        };
                        if (iRecibo == 3 && ListDatEmisor[0].sRFC.Length > 3 && LisTRecibo != null)
                        {
                            Dao2.sp_CCejecucionAndSen_update_TsellosSat(idEmpresa, idempleado, anios, Tipodeperido, Perido, 4, Nombrearc);
                            NoEjecuciones = NoEjecuciones + 1;
                            valido = 1;
                        };


                    };
                    if (valido == 1)
                    {
                        string pathCert;

                        string s_certificadoKey = "";
                        string s_certificadoCer = "";
                        string s_transitorio = "";
                        if (iRecibo != 3)
                        {
                            ListDatEmisor[0].sUrl = PathPDF;
                            LisCer = Dao2.sp_FileCer_Retrieve_CCertificados(ListDatEmisor[0].sRFC);
                        }

                        pathCert = Server.MapPath("Archivos\\certificados\\");
                        pathCert = pathCert.Replace("\\Empleados", "");

                        if (iRecibo != 1 && iRecibo != 3)
                        {
                            s_certificadoKey = pathCert + LisCer[0].sfilekey;
                            s_certificadoCer = pathCert + LisCer[0].sfilecer;
                            s_transitorio = LisCer[0].stransitorio;
                        }


                        if ((System.IO.File.Exists(s_certificadoKey) && LisTRecibo != null) || iRecibo == 1 || iRecibo == 3)
                        {
                            byte[] bcert = null;
                            string CerNo = null;
                            byte[] CERT_SIS = null;
                            if (iRecibo != 1 && iRecibo != 3)
                            {
                                System.Security.Cryptography.X509Certificates.X509Certificate CerSAT;
                                CerSAT = System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromCertFile(s_certificadoCer);
                                bcert = CerSAT.GetSerialNumber();
                                CerNo = LibreriasFacturas.StrReverse((string)Encoding.UTF8.GetString(bcert));
                                CERT_SIS = CerSAT.GetRawCertData();
                            }



                            // crecion del archivo PDF
                            FileStream Fs = new FileStream(Nombrearc, FileMode.Create);
                            Document documento = new Document(iTextSharp.text.PageSize.LETTER, 5, 10, 10, 5);
                            PdfWriter pw = PdfWriter.GetInstance(documento, Fs);
                            documento.Open();

                            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, BaseFont.EMBEDDED);
                            iTextSharp.text.Font TTexNeg = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.BOLD);
                            iTextSharp.text.Font TexNom = new iTextSharp.text.Font(bf, 7, iTextSharp.text.Font.NORMAL);
                            iTextSharp.text.Font TexNeg = new iTextSharp.text.Font(bf, 7, iTextSharp.text.Font.BOLD);
                            iTextSharp.text.Font TTexNegCuerpo = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.BOLD);
                            iTextSharp.text.Font TexNegCuerpo = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL);

                            Paragraph Espacio = new Paragraph(10, " ");
                            Paragraph Espacio2 = new Paragraph(-80, " ");
                            Paragraph Espacio3 = new Paragraph(10, " ");
                            Paragraph espacio4 = new Paragraph(25, " ", TexNegCuerpo);
                            Paragraph Espacio9 = new Paragraph(5, " ");
                            Paragraph Espacio10 = new Paragraph(-62, " ");
                            Paragraph Espacio11 = new Paragraph(30, " ");
                            Paragraph Espacio12 = new Paragraph(-12, " ");
                            Paragraph Espacio13 = new Paragraph(-10, " ");


                            //////Cabecera  

                            string Palabra = ListDatEmisor[0].sNombreEmpresa;
                            Paragraph Empresa = new Paragraph(50, Palabra, TTexNeg);
                            Empresa.IndentationLeft = 70;


                            Paragraph Trfc = new Paragraph("R.F.C.:", TexNeg);
                            Trfc.IndentationLeft = 70;
                            Palabra = ListDatEmisor[0].sRFC;
                            RfcEmi = Palabra;
                            Paragraph Rfc = new Paragraph(-1, Palabra, TexNom);
                            Rfc.IndentationLeft = 118;

                            Paragraph Rfcpatron = new Paragraph(-1, Palabra, TexNom);
                            Rfcpatron.IndentationLeft = 118;


                            Paragraph TrfcPatron = new Paragraph("R.F.C. Patron:", TexNeg);
                            TrfcPatron.IndentationLeft = 70;

                            Palabra = ListDatEmisor[0].sAfiliacionIMSS;
                            Paragraph TRegPat = new Paragraph("Reg.Pat:", TexNeg);
                            TRegPat.IndentationLeft = 70;

                            Paragraph RegPat = new Paragraph(-1, Palabra, TexNom);
                            RegPat.IndentationLeft = 118;

                            ListDirec = Dao2.Sp_EmpresaDir_Retrieve(RfcEmi);


                            Paragraph TrEmpraDir = new Paragraph("Dirección:", TexNeg);
                            TrEmpraDir.IndentationLeft = 70;
                            Palabra = ListDirec[0].sDomiciolioEmple;
                            Paragraph EmpraDir = new Paragraph(-1, Palabra, TexNom);
                            EmpraDir.IndentationLeft = 118;


                            Palabra = ListDirec[0].sDomiciolioEmpre;
                            Paragraph EmpraDir2 = new Paragraph(Palabra, TexNom);
                            EmpraDir2.IndentationLeft = 118;


                            /// Emprime Cabecera
                            if (iRecibo != 3)
                            {
                                documento.Add(Empresa);
                            }
                            if (ListDatEmisor[0].GrupoEmpresas != 1)
                            {
                                documento.Add(Trfc);
                                documento.Add(Rfc);
                                documento.Add(TrfcPatron);
                                documento.Add(Rfcpatron);
                                documento.Add(TrEmpraDir);
                                documento.Add(EmpraDir);
                                documento.Add(EmpraDir2);
                                documento.Add(TRegPat);
                                documento.Add(RegPat);
                            }
                            if (ListDatEmisor[0].GrupoEmpresas == 1 && iRecibo == 2)
                            {
                                documento.Add(Trfc);
                                documento.Add(Rfc);
                                documento.Add(TrfcPatron);
                                documento.Add(Rfcpatron);
                                documento.Add(TrEmpraDir);
                                documento.Add(EmpraDir);
                                documento.Add(EmpraDir2);
                                documento.Add(TRegPat);
                                documento.Add(RegPat);
                            }


                            if (iRecibo == 2)
                            {

                                // con QR

                                Paragraph TfolioFis = new Paragraph(-50, "Folio Fiscal:", TexNeg);
                                TfolioFis.IndentationLeft = 412;
                                Palabra = LiTsat[0].sUUID;
                                CadeSat = "||" + Palabra + "|";
                                UUID = Palabra;

                                Paragraph folioFis = new Paragraph(-1, Palabra, TexNom);
                                folioFis.IndentationLeft = 450;
                                RegPat.IndentationLeft = 115;

                                Palabra = LiTsat[0].sNoCertificado;
                                Palabra = CerNo;
                                Paragraph TNumCertEmi = new Paragraph("No. de serie del Emisor:", TexNeg);
                                TNumCertEmi.IndentationLeft = 380;

                                Paragraph NumCertEmi = new Paragraph(-1, Palabra, TexNom);
                                NumCertEmi.IndentationLeft = 450;
                                Paragraph TFechaEmisior = new Paragraph("Lugar de Emisión:", TexNeg);
                                TFechaEmisior.IndentationLeft = 395;
                                Palabra = LiTsat[0].Fecha;
                                Paragraph FechaEmisior = new Paragraph(-1, Palabra, TexNom);
                                FechaEmisior.IndentationLeft = 450;

                                Paragraph TFechaCertifi = new Paragraph("Fecha y hora de Certificación:", TexNeg);
                                TFechaCertifi.IndentationLeft = 363;
                                Palabra = LiTsat[0].Fechatimbrado;
                                CadeSat = CadeSat + Palabra + "|";

                                Paragraph FechaCertifi = new Paragraph(-1, Palabra, TexNom);
                                FechaCertifi.IndentationLeft = 450;
                                Paragraph TRegimenFis = new Paragraph("Regimen fiscal:", TexNeg);
                                TRegimenFis.IndentationLeft = 403;
                                Palabra = Convert.ToString(ListDatEmisor[0].iRegimenFiscal);
                                if (Palabra == "601") { Palabra = Palabra + "-General De Ley Personas Morales"; }
                                Paragraph RegimenFis = new Paragraph(-1, Palabra, TexNom);
                                RegimenFis.IndentationLeft = 450;

                                Paragraph TTipoCDFI = new Paragraph("Tipo de CDFI:", TexNeg);
                                TTipoCDFI.IndentationLeft = 406;
                                Paragraph TipoCDFI = new Paragraph(-1, "Recibo de Nomina", TexNom);
                                TipoCDFI.IndentationLeft = 450;


                                if (ListDatEmisor[0].iCgTipoEmpleadoId == 156)
                                {
                                    TipoCDFI = new Paragraph(-1, "Recibo", TexNom);
                                    TipoCDFI.IndentationLeft = 450;

                                };

                                Paragraph TSerieFolio = new Paragraph("Serie y Folio:", TexNeg);
                                TSerieFolio.IndentationLeft = 409;

                                string folio = "";
                                List<XMLBean> LFolio = new List<XMLBean>();
                                LFolio = Dao2.sp_ObtenFolioCCertificados_RetrieveUpdate_Ccertificados(ListDatEmisor[0].sRFC);
                                if (LFolio != null) folio = LFolio[0].ifolio.ToString();
                                else ListDatEmisor[0].sMensaje = "Erro en Genera el folio Contacte a sistemas";
                                Palabra = folio;
                                Paragraph SerieFolio = new Paragraph(-1, Palabra, TexNom);
                                SerieFolio.FirstLineIndent = 450;


                                Paragraph TtipoNomina = new Paragraph("Tipo de Nómina:", TTexNegCuerpo);
                                TtipoNomina.IndentationLeft = 393;
                                Palabra = "0";
                                if (Palabra == "O")
                                {
                                    Palabra = "O (Ordinaria) ";
                                }
                                if (Palabra == "E")
                                {
                                    Palabra = "E (Ordinaria) ";
                                }
                                Paragraph tipoNomina = new Paragraph(-1, Palabra, TexNegCuerpo);
                                tipoNomina.IndentationLeft = 450;

                                /// imprime cabecera de cello
                                documento.Add(TfolioFis);
                                documento.Add(folioFis);
                                documento.Add(TNumCertEmi);
                                documento.Add(NumCertEmi);
                                documento.Add(TFechaEmisior);
                                documento.Add(FechaEmisior);
                                documento.Add(TFechaCertifi);
                                documento.Add(FechaCertifi);
                                documento.Add(TRegimenFis);
                                documento.Add(RegimenFis);
                                documento.Add(TTipoCDFI);
                                documento.Add(TipoCDFI);
                                documento.Add(TSerieFolio);
                                documento.Add(SerieFolio);
                                documento.Add(TtipoNomina);
                                documento.Add(tipoNomina);

                            }


                            ////////// Info Personal


                            Paragraph table1 = new Paragraph();
                            table1.IndentationLeft = 50;
                            PdfPTable table = new PdfPTable(1);
                            table.HorizontalAlignment = 0;
                            table.PaddingTop = 10;
                            table.TotalWidth = 200;
                            table.LockedWidth = true;
                            BaseFont bf2 = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
                            iTextSharp.text.Font TexHatable = new iTextSharp.text.Font(bf2, 8, 1, BaseColor.WHITE);
                            // Esta es la primera fila
                            PdfPCell Cell = new PdfPCell();
                            Cell.BackgroundColor = BaseColor.BLACK;
                            Cell.AddElement(new Chunk("INFORMACION PERSONAL DEL TRABAJADOR", TexHatable));
                            table.AddCell(Cell);
                            table1.Add(table);


                            Palabra = Convert.ToString(ListDatEmisor[0].iIdEmpleado);
                            NumEmpleado = int.Parse(Palabra);
                            Paragraph TNoEmpleado = new Paragraph(10, "No.Empleado :", TTexNegCuerpo);
                            TNoEmpleado.IndentationLeft = 50;
                            Paragraph NoEmpleado = new Paragraph(-1, Palabra, TexNegCuerpo);
                            NoEmpleado.IndentationLeft = 108;
                            Paragraph TNommbre = new Paragraph("Nombre:", TTexNegCuerpo);
                            TNommbre.IndentationLeft = 50;

                            Palabra = Empleados[a].sNombreEmpleado;
                            Paragraph Nommbre = new Paragraph(-1, Palabra, TexNegCuerpo);
                            Nommbre.IndentationLeft = 108;

                            Paragraph TCurp = new Paragraph("Curp:", TTexNegCuerpo);
                            TCurp.IndentationLeft = 50;
                            Palabra = ListDatEmisor[0].sCURP;
                            Paragraph Curp = new Paragraph(-1, Palabra, TexNegCuerpo);
                            Curp.IndentationLeft = 108;

                            Paragraph TrfcEmp = new Paragraph("R.F.C.:", TTexNegCuerpo);
                            TrfcEmp.IndentationLeft = 50;
                            Palabra = ListDatEmisor[0].sRFCEmpleado;
                            RfcRep = Palabra;
                            Paragraph rfcEmp = new Paragraph(-1, Palabra, TexNegCuerpo);
                            rfcEmp.IndentationLeft = 108;


                            Paragraph TNSS = new Paragraph("", TTexNegCuerpo);
                            TNSS.IndentationLeft = 50;
                            Paragraph NSS = new Paragraph(-1, Palabra, TexNegCuerpo);
                            NSS.IndentationLeft = 108;
                            Paragraph TRegimen = new Paragraph("", TTexNegCuerpo);
                            TRegimen.IndentationLeft = 50;
                            Paragraph Regimen = new Paragraph(-1, Palabra, TexNegCuerpo);
                            Regimen.IndentationLeft = 108;

                            if (ListDatEmisor[0].iCgTipoEmpleadoId != 156)
                            {

                                TNSS = new Paragraph("NSS:", TTexNegCuerpo);
                                TNSS.IndentationLeft = 50;
                                Palabra = ListDatEmisor[0].sRegistroImss;
                                NSS = new Paragraph(-1, Palabra, TexNegCuerpo);
                                NSS.IndentationLeft = 108;

                                TRegimen = new Paragraph("Regimen:", TTexNegCuerpo);
                                TRegimen.IndentationLeft = 50;
                                Palabra = "02";
                                if (Palabra == "02") { Palabra = Palabra + "-Sueldos"; }
                                Regimen = new Paragraph(-1, Palabra, TexNegCuerpo);
                                Regimen.IndentationLeft = 108;
                            };

                            Paragraph table3 = new Paragraph();
                            table3.IndentationLeft = 350;

                            PdfPTable table2 = new PdfPTable(1);
                            table2.HorizontalAlignment = 0;
                            table2.PaddingTop = 10;
                            table2.TotalWidth = 200;
                            table2.LockedWidth = true;


                            ///////// Info Laboral

                            PdfPCell Cell2 = new PdfPCell();
                            Cell2.BackgroundColor = BaseColor.BLACK;
                            Cell2.AddElement(new Chunk("INFORMACION LABORAL", TexHatable));
                            table2.AddCell(Cell2);
                            table3.Add(table2);

                            Paragraph TPuesto = new Paragraph(10, "Puesto:", TTexNegCuerpo);
                            TPuesto.IndentationLeft = 350;
                            Palabra = ListDatEmisor[0].sNombrePuesto;
                            if (Palabra.Length > 25)
                            {
                                Palabra = Palabra.Substring(0, 25);
                            }
                            Paragraph Puesto = new Paragraph(-1, Palabra, TexNegCuerpo);
                            Puesto.IndentationLeft = 435;


                            Paragraph TDepart = new Paragraph(10, "Departamento:", TTexNegCuerpo);
                            TDepart.IndentationLeft = 350;
                            Palabra = ListDatEmisor[0].sDescripcionDepartamento;
                            if (Palabra.Length > 25)
                            {
                                Palabra = Palabra.Substring(0, 25);
                            }
                            Paragraph Depart = new Paragraph(-1, Palabra, TexNegCuerpo);
                            Depart.IndentationLeft = 435;






                            Paragraph TContrato = new Paragraph("Contrato:", TTexNegCuerpo);
                            TContrato.IndentationLeft = 350;
                            Palabra = ListDatEmisor[0].sTipoContrato;
                            if (Palabra.Length > 25)
                            {
                                Palabra = Palabra.Substring(0, 25);
                            }



                            Paragraph Contrato = new Paragraph(-1, Palabra, TexNegCuerpo);
                            Contrato.IndentationLeft = 435;


                            List<ReciboNominaBean> ListTotales = new List<ReciboNominaBean>();
                            ListTotales = Dao2.sp_SaldosTotales_Retrieve_TPlantillasCalculos(idEmpresa, Empleados[a].iIdEmpleado, anios, TipoPeriodo, Perido, tipoRecibo);

                            Paragraph TSalarioB = new Paragraph("", TTexNegCuerpo);
                            Paragraph SalarioB = new Paragraph("", TexNegCuerpo);
                            Paragraph TSalarioInt = new Paragraph("", TTexNegCuerpo);
                            Paragraph SalarioInt = new Paragraph("", TexNegCuerpo);
                            Paragraph TAntiguedad = new Paragraph("", TTexNegCuerpo);
                            Paragraph Antiguedad = new Paragraph("", TexNegCuerpo);
                            Paragraph TJornada = new Paragraph("", TTexNegCuerpo);
                            Paragraph Jornada = new Paragraph("", TexNegCuerpo);
                            Paragraph TRiesgopu = new Paragraph("", TTexNegCuerpo);
                            Paragraph Riesgopu = new Paragraph("", TexNegCuerpo);
                            Paragraph TFechaInLab = new Paragraph("", TTexNegCuerpo);
                            Paragraph FechaInLab = new Paragraph("", TexNegCuerpo);
                            List<CInicioFechasPeriodoBean> LFechaPerido = new List<CInicioFechasPeriodoBean>();
                            LFechaPerido = Dao2.sp_DatPeridoEmpresa(idEmpresa, TipoPeriodo, Anio, Perido);

                            if (ListDatEmisor[0].iCgTipoEmpleadoId != 156)
                            {

                                TSalarioB = new Paragraph("Salario Base:", TTexNegCuerpo);
                                TSalarioB.IndentationLeft = 350;

                                if (ListDatEmisor == null)
                                {
                                    Palabra = "error contcte a sistemas";
                                };
                                if (ListDatEmisor != null)
                                {

                                    decimal SalDi = Convert.ToDecimal(ListDatEmisor[0].dSalarioMensual);
                                    SalDi = Math.Round(SalDi, 2);
                                    Palabra = string.Format("{0:N2}", SalDi);
                                    Palabra = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Palabra);

                                }

                                SalarioB = new Paragraph(-1, Palabra, TexNegCuerpo);
                                SalarioB.IndentationLeft = 435;
                                TSalarioInt = new Paragraph("Salario Integrado:", TTexNegCuerpo);
                                TSalarioInt.IndentationLeft = 350;

                                decimal Salint = Convert.ToDecimal(ListDatEmisor[0].dSalarioInt);
                                Salint = Math.Round(Salint, 2);
                                Palabra = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Salint);

                                SalarioInt = new Paragraph(-1, Palabra, TexNegCuerpo);
                                SalarioInt.IndentationLeft = 435;

                                var culture = System.Globalization.CultureInfo.CreateSpecificCulture("es-MX");
                                var styles = System.Globalization.DateTimeStyles.None;
                                DateTime dt1 = DateTime.Now;
                                DateTime dt2 = dt1;
                                DateTime dt3 = dt1;



                                bool fechaValida = DateTime.TryParse(LFechaPerido[0].sFechaInicio, culture, styles, out dt1);
                                fechaValida = DateTime.TryParse(LFechaPerido[0].sFechaFinal, culture, styles, out dt2);
                                fechaValida = DateTime.TryParse(LFechaPerido[0].sFechaPago, culture, styles, out dt3);

                                string sFechaInicialPago = String.Format("{0:yyyy-MM-dd}", dt1);
                                string sFechaFinalPago = String.Format("{0:yyyy-MM-dd}", dt2);
                                string sFechaPago = String.Format("{0:yyyy-MM-dd}", dt3);
                                string anoarchivo = String.Format("{0:yyyy}", dt2);
                                string sFechaInicioRelLaboral = String.Format("{0:yyyy-MM-dd}", dt3);

                                DateTime f1 = DateTime.Parse(sFechaInicioRelLaboral);
                                DateTime f2 = DateTime.Parse(sFechaFinalPago);
                                TimeSpan diferencia = f2.Subtract(f1);
                                sAntiguedad = "P" + ((int)(diferencia.Days / 7)).ToString() + "W";

                                TAntiguedad = new Paragraph("Antigüedad:", TTexNegCuerpo);
                                TAntiguedad.IndentationLeft = 350;
                                Palabra = sAntiguedad;
                                Antiguedad = new Paragraph(-1, Palabra + "(Semanas)", TexNegCuerpo);
                                Antiguedad.IndentationLeft = 435;


                                TJornada = new Paragraph("Jornada:", TTexNegCuerpo);
                                TJornada.IndentationLeft = 350;
                                Palabra = "06";
                                Jornada = new Paragraph(-1, Palabra, TexNegCuerpo);
                                Jornada.IndentationLeft = 435;

                                TRiesgopu = new Paragraph("Riesgo Puesto:", TTexNegCuerpo);
                                TRiesgopu.IndentationLeft = 350;
                                Palabra = ListDatEmisor[0].sRiesgoTrabajo.ToString();
                                Riesgopu = new Paragraph(-1, Palabra, TexNegCuerpo);
                                Riesgopu.IndentationLeft = 435;
                                TFechaInLab = new Paragraph("Fecha de Inicio Laboral :", TTexNegCuerpo);
                                TFechaInLab.IndentationLeft = 350;
                                Palabra = ListDatEmisor[0].sFechaIngreso;
                                FechaInLab = new Paragraph(-1, Palabra, TexNegCuerpo);
                                FechaInLab.IndentationLeft = 435;


                            }


                            //    /////////////// tipo de pago 

                            Paragraph table6 = new Paragraph();
                            table6.IndentationLeft = 50;
                            PdfPTable table7 = new PdfPTable(1);
                            table7.HorizontalAlignment = 0;
                            table7.PaddingTop = 10;
                            table7.TotalWidth = 500;
                            table7.LockedWidth = true;

                            PdfPCell Cell3 = new PdfPCell();
                            Cell3.BackgroundColor = BaseColor.BLACK;
                            Cell3.AddElement(new Chunk("INFORMACION DE PAGO", TexHatable));
                            table7.AddCell(Cell3);
                            table6.Add(table7);



                            Paragraph TFecPago = new Paragraph("Fecha de Pago:", TTexNegCuerpo);
                            TFecPago.IndentationLeft = 50;
                            Palabra = LFechaPerido[0].sFechaPago;
                            Paragraph FecPago = new Paragraph(-1, Palabra, TexNegCuerpo);
                            FecPago.IndentationLeft = 110;


                            Paragraph TClaveb = new Paragraph("Clave:", TTexNegCuerpo);
                            TClaveb.IndentationLeft = 200;

                            string sBanco = ListDatEmisor[0].sDescripcion;
                            string sbanco;
                            string sCuentaBancaria = ListDatEmisor[0].sCtaCheques;

                            Palabra = ListDatEmisor[0].sCtaCheques;
                            Paragraph Claveb = new Paragraph(-1, Palabra, TexNegCuerpo);
                            Claveb.IndentationLeft = 240;
                            Paragraph TBanco = new Paragraph("Banco:", TTexNegCuerpo);
                            TBanco.IndentationLeft = 50;
                            sbanco = sBanco;
                            if (Palabra != "")
                            {
                                if (Palabra.Length >= 7 && Palabra.Length < 18)
                                {
                                    sbanco = sBanco;
                                }
                                else
                                {
                                    sbanco = Palabra.Substring(1, 3);
                                }

                            }
                            if (Palabra == "")
                            {
                                Palabra = "NA";
                                sbanco = "NA";
                            }


                            List<EmisorReceptorBean> ListEmisor = new List<EmisorReceptorBean>();
                            //ListEmpleadosDao Dao = new ListEmpleadosDao();
                            ListEmisor = Dao2.sp_EmisorReceptor_Retrieve_EmisorReceptor(idEmpresa, ListDatEmisor[0].iIdEmpleado);
                            if (ListEmisor != null)
                            {

                                sbanco = sbanco + " " + ListEmisor[0].sDescripcion;
                            }

                            Paragraph Banco = new Paragraph(-1, sbanco, TexNegCuerpo);
                            Banco.IndentationLeft = 110;



                            Paragraph TPeriodo = new Paragraph(-10, "Periocidad:", TTexNegCuerpo);
                            TPeriodo.IndentationLeft = 200;

                            Palabra = Convert.ToString(TipoPeriodo);

                            if (Palabra == "0")
                            {
                                Palabra = "02 Semanal";
                            }
                            if (Palabra == "1")
                            {
                                Palabra = "10 Decenal";
                            }
                            if (Palabra == "3")
                            {
                                Palabra = "04 Quincenal";
                            }
                            if (Palabra == "2")
                            {
                                Palabra = "03 Catorcenal";
                            }
                            if (Palabra == "4")
                            {
                                Palabra = "05 Mensual";
                            }
                            if (Palabra == "5")
                            {
                                Palabra = "06 Bimestral";
                            }

                            Paragraph Periodo = new Paragraph(-1, Palabra, TexNegCuerpo);
                            Periodo.IndentationLeft = 240;


                            Paragraph TPeridoFec = new Paragraph("Perido:", TTexNegCuerpo);
                            TPeridoFec.IndentationLeft = 200;
                            Palabra = LFechaPerido[0].iPeriodo + "( " + LFechaPerido[0].sFechaInicio + "-" + LFechaPerido[0].sFechaFinal + ")";
                            Paragraph PeridoFec = new Paragraph(-1, Palabra, TexNegCuerpo);
                            PeridoFec.IndentationLeft = 240;

                            Paragraph TMoneda = new Paragraph("Moneda:", TTexNegCuerpo);
                            TMoneda.IndentationLeft = 200;
                            Palabra = "MXP";
                            Paragraph Moneda = new Paragraph(-1, Palabra, TexNegCuerpo);
                            Moneda.IndentationLeft = 240;



                            Paragraph TLugarExp = new Paragraph(-30, "Lugar de Expedicion:", TTexNegCuerpo);
                            TLugarExp.IndentationLeft = 380;
                            Palabra = "04600";
                            Paragraph LugarExp = new Paragraph(-1, "Cp: " + Palabra, TexNegCuerpo);
                            LugarExp.IndentationLeft = 455;


                            Paragraph TTipopago = new Paragraph("Tipo de pago:", TTexNegCuerpo);
                            TTipopago.IndentationLeft = 380;
                            Palabra = Convert.ToString(ListDatEmisor[0].iCgTipoPago);
                            Paragraph Tipopago = new Paragraph(-1, Palabra, TexNegCuerpo);
                            Tipopago.IndentationLeft = 455;



                            Paragraph TDiasPag = new Paragraph("Dias pagados:", TTexNegCuerpo);
                            TDiasPag.IndentationLeft = 50;

                            // dias Efectivos 
                            List<ReciboNominaBean> LisTDiasEmple = new List<ReciboNominaBean>();
                            LisTDiasEmple = null;
                            LisTRecibo = Dao.sp_TpCalculoEmpleado_Retrieve_TpCalculoEmpleado(idEmpresa, ListDatEmisor[0].iIdEmpleado, Perido, TipoPeriodo, Anio, tipoRecibo);
                            LisTDiasEmple = Dao.sp_DiaTrabjEmple_Retrieve_TPlantillaCalculosLn(idEmpresa, ListDatEmisor[0].iIdEmpleado, Perido, TipoPeriodo, Anio);
                            decimal iTdias = LFechaPerido[0].iDiasEfectivos;
                            int TDias = 0;
                            string Dias = LisTRecibo[0].sNombre_Renglon;
                            string sDiasEfectivos = Convert.ToString(iTdias);
                            string sDiasIncapa = "";
                            if (LisTDiasEmple != null)
                            {
                                for (int c = 0; c < LisTDiasEmple.Count; c++)
                                {
                                    if (LisTDiasEmple[c].iIdRenglon == 31)
                                    {
                                        string dias = Convert.ToString(LisTDiasEmple[c].iDiasTrab);
                                        string[] Dias2 = dias.Split('.');
                                        sDiasEfectivos = Dias2[0];

                                    }
                                    if (LisTDiasEmple[c].iIdRenglon == 34)
                                    {
                                        string dias = Convert.ToString(LisTDiasEmple[c].iDiasTrab);
                                        string[] Dias2 = dias.Split('.');
                                        sDiasIncapa = Dias2[0];

                                    }
                                }

                            }
                            if (LisTDiasEmple == null)
                            {
                                sDiasEfectivos = Convert.ToString(LFechaPerido[0].iDiasEfectivos);
                            }


                            Palabra = sDiasEfectivos;
                            Paragraph DiasPag = new Paragraph(-1, Palabra, TexNegCuerpo);
                            DiasPag.IndentationLeft = 110;


                            Paragraph table8 = new Paragraph();
                            table8.IndentationLeft = 50;

                            PdfPTable table9 = new PdfPTable(1);
                            table9.HorizontalAlignment = 0;
                            table9.PaddingTop = 10;
                            table9.TotalWidth = 350;
                            table9.LockedWidth = true;

                            PdfPCell Cell4 = new PdfPCell();
                            Cell4.BackgroundColor = BaseColor.BLACK;
                            Cell4.AddElement(new Chunk("LEYENDA", TexHatable));
                            table9.AddCell(Cell4);
                            table8.Add(table9);

                            Paragraph Espacio5 = new Paragraph(-16, " ");
                            Paragraph table10 = new Paragraph();
                            table10.IndentationLeft = 350;
                            PdfPTable table11 = new PdfPTable(1);
                            table11.HorizontalAlignment = 0;
                            table11.PaddingTop = 10;
                            table11.TotalWidth = 150;
                            table11.LockedWidth = true;

                            PdfPCell Cell5 = new PdfPCell();
                            Cell5.BackgroundColor = BaseColor.BLACK;
                            Cell5.AddElement(new Chunk("PERCEPCIONES", TexHatable));
                            table11.AddCell(Cell5);
                            table10.Add(table11);

                            Paragraph Espacio6 = new Paragraph(-16, " ");
                            Paragraph table12 = new Paragraph();
                            table12.IndentationLeft = 450;
                            PdfPTable table13 = new PdfPTable(1);
                            table13.HorizontalAlignment = 0;
                            table13.PaddingTop = 10;
                            table13.TotalWidth = 100;
                            table13.LockedWidth = true;

                            PdfPCell Cell6 = new PdfPCell();
                            Cell6.BackgroundColor = BaseColor.BLACK;
                            Cell6.AddElement(new Chunk("DEDUCIONES", TexHatable));
                            table13.AddCell(Cell6);
                            table12.Add(table13);

                            /// imprime en documento

                            documento.Add(Espacio);
                            documento.Add(table1);
                            documento.Add(TNoEmpleado);
                            documento.Add(NoEmpleado);
                            documento.Add(TNommbre);
                            documento.Add(Nommbre);
                            documento.Add(TCurp);
                            documento.Add(Curp);
                            documento.Add(TrfcEmp);
                            documento.Add(rfcEmp);

                            if (ListDatEmisor[0].iCgTipoEmpleadoId != 156)
                            {
                                if (ListDatEmisor[0].GrupoEmpresas != 1)
                                {
                                    documento.Add(TNSS);
                                    documento.Add(NSS);
                                    documento.Add(TRegimen);
                                    documento.Add(Regimen);

                                }

                            }
                            if (ListDatEmisor[0].iCgTipoEmpleadoId == 156)
                            {
                                documento.Add(Espacio9);
                                documento.Add(Espacio10);
                            }
                            if (ListDatEmisor[0].iCgTipoEmpleadoId != 156)
                            {
                                documento.Add(Espacio2);
                            }
                            if (ListDatEmisor[0].GrupoEmpresas == 1)
                            {
                                documento.Add(espacio4);

                            }
                            documento.Add(table3);

                            //informe
                            documento.Add(TPuesto);
                            documento.Add(Puesto);
                            if (ListEmisor[0].GrupoEmpresas == 1)
                            {
                                documento.Add(TDepart);
                                documento.Add(Depart);
                            }


                            if (ListDatEmisor[0].GrupoEmpresas != 1)
                            {
                                documento.Add(TContrato);
                                documento.Add(Contrato);
                            }

                            if (ListDatEmisor[0].iCgTipoEmpleadoId != 156)
                            {
                                documento.Add(TSalarioB);
                                documento.Add(SalarioB);
                                documento.Add(TSalarioInt);
                                documento.Add(SalarioInt);

                                if (ListDatEmisor[0].GrupoEmpresas != 1)
                                {

                                    documento.Add(TAntiguedad);
                                    documento.Add(Antiguedad);
                                    documento.Add(TJornada);
                                    documento.Add(Jornada);
                                    documento.Add(TRiesgopu);
                                    documento.Add(Riesgopu);
                                }


                                documento.Add(TFechaInLab);
                                documento.Add(FechaInLab);

                            }

                            if (ListDatEmisor[0].iCgTipoEmpleadoId != 156)
                            {
                                documento.Add(Espacio3);
                            }
                            if (ListDatEmisor[0].iCgTipoEmpleadoId == 156)
                            {
                                documento.Add(Espacio11);
                            }



                            documento.Add(table6);
                            documento.Add(TFecPago);
                            documento.Add(FecPago);
                            if (ListDatEmisor[0].GrupoEmpresas == 1)
                            {
                                documento.Add(Espacio12);
                                documento.Add(TPeridoFec);
                                documento.Add(PeridoFec);

                            }


                            if (ListDatEmisor[0].GrupoEmpresas != 1)
                            {
                                documento.Add(TBanco);
                                documento.Add(Banco);
                                documento.Add(TPeriodo);
                                documento.Add(Periodo);
                                documento.Add(TPeridoFec);
                                documento.Add(PeridoFec);
                                documento.Add(TClaveb);
                                documento.Add(Claveb);
                                documento.Add(TMoneda);
                                documento.Add(Moneda);
                                documento.Add(TLugarExp);
                                documento.Add(LugarExp);
                                documento.Add(TTipopago);
                                documento.Add(Tipopago);

                            }
                            documento.Add(TDiasPag);
                            documento.Add(DiasPag);

                            //if (ListEmisor[0].GrupoEmpresas != 1)
                            //{
                            //    documento.Add(Espacio12);
                            //}
                            //documento.Add(Espacio5);

                            if (ListDatEmisor[0].GrupoEmpresas == 1) { documento.Add(espacio4); }
                            if (ListDatEmisor[0].GrupoEmpresas != 1) { documento.Add(espacio4); }


                            documento.Add(table8);
                            documento.Add(Espacio5);
                            documento.Add(table10);
                            documento.Add(Espacio5);
                            documento.Add(table12);



                            //int a = 0;
                            string Palabra2;
                            decimal valor;
                            decimal per = 0;
                            decimal ded = 0;
                            decimal total;

                            if (LisTRecibo.Count > 0 || LisTRecibo != null)
                            {
                                for (int x = 0; x < LisTRecibo.Count; x++)
                                {
                                    if (LisTRecibo[x].sValor == "Percepciones")
                                    {
                                        //string lengRenglon = "";
                                        string ImporGra = string.Format("{0:N2}", LisTRecibo[x].dSaldo);
                                        ImporGra = ImporGra.Replace(",", "");
                                        string IdRenglon = Convert.ToString(LisTRecibo[x].iIdRenglon);
                                        string concepto = LisTRecibo[x].sNombre_Renglon;
                                        if (IdRenglon == "1")
                                        {
                                            concepto = "Sueldo {" + sDiasEfectivos + " Dias}";
                                            //lengRenglon = "001";
                                        }
                                        //lengRenglon = "010";
                                        int idReglontama = IdRenglon.Length;
                                        //if (idReglontama == 1) { IdRenglon = "00" + IdRenglon; };
                                        //if (idReglontama == 2) { IdRenglon = "0" + IdRenglon; };


                                        Palabra = concepto;
                                        Palabra2 = ImporGra;
                                        valor = decimal.Parse(Palabra2);
                                        per = per + valor;
                                        Paragraph TLeyenda = new Paragraph(Palabra, TexNegCuerpo);
                                        TLeyenda.IndentationLeft = 75;

                                        valor = Math.Round(valor, 2);
                                        Palabra2 = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", valor);
                                        Paragraph TPercep = new Paragraph(-1, Palabra2, TexNegCuerpo);
                                        TPercep.IndentationLeft = 375;
                                        documento.Add(TLeyenda);
                                        documento.Add(TPercep);

                                    }
                                    if (LisTRecibo[x].sValor == "Deducciones")
                                    {
                                        if (LisTRecibo[x].iIdRenglon != 1007)
                                        {

                                            string lengRenglon = "";
                                            string ImporGra = string.Format("{0:N2}", LisTRecibo[x].dSaldo);
                                            ImporGra = ImporGra.Replace(",", "");
                                            string IdRenglon = Convert.ToString(LisTRecibo[x].iIdRenglon);
                                            string concepto = LisTRecibo[x].sNombre_Renglon;

                                            Palabra = concepto;
                                            Palabra2 = ImporGra;
                                            valor = decimal.Parse(Palabra2);
                                            ded = ded + valor;
                                            Paragraph TLeyenda = new Paragraph(Palabra, TexNegCuerpo);
                                            TLeyenda.IndentationLeft = 75;

                                            valor = valor;
                                            Palabra2 = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", valor);
                                            Paragraph TDedu = new Paragraph(-1, Palabra2, TexNegCuerpo);
                                            TDedu.IndentationLeft = 475;

                                            documento.Add(TLeyenda);
                                            documento.Add(TDedu);

                                        }

                                        if (LisTRecibo[x].iIdRenglon == 1007 && iRecibo == 3)
                                        {
                                            row1007Ac = 1;
                                            row1007 = x;


                                        }




                                    }
                                }

                            }

                            Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 0.2f)));
                            p.IndentationLeft = 50;
                            p.IndentationRight = 50;
                            documento.Add(p);
                            Paragraph Ttotal = new Paragraph("Total:", TTexNegCuerpo);
                            Ttotal.IndentationLeft = 350;
                            string perp = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", per);


                            Paragraph Tper = new Paragraph(-1, perp, TTexNegCuerpo);
                            Tper.IndentationLeft = 375;


                            Paragraph Tper2 = new Paragraph(-1, perp, TTexNegCuerpo);
                            Tper2.IndentationLeft = 475;

                            string deduc = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", ded);

                            Paragraph Tdedu = new Paragraph(-1, deduc, TTexNegCuerpo);
                            Tdedu.IndentationLeft = 475;

                            Paragraph Tdedu2 = new Paragraph(-1, deduc, TTexNegCuerpo);
                            Tdedu2.IndentationLeft = 475;

                            total = per - ded;
                            string Rtotal = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", total);


                            Paragraph Ttotal2 = new Paragraph(-1, Rtotal, TTexNegCuerpo);
                            Ttotal2.IndentationLeft = 475;
                            decimal ISR = 0;
                            if (row1007Ac == 1)
                            {
                                ISR = LisTRecibo[row1007].dSaldo;

                            }

                            string RISR = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", ISR);

                            Paragraph TISR = new Paragraph(-1, RISR, TTexNegCuerpo);
                            TISR.IndentationLeft = 475;


                            total = total - ISR;
                            string NetoApagar = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", total);

                            Paragraph TNetoA = new Paragraph(-1, NetoApagar, TTexNegCuerpo);
                            TNetoA.IndentationLeft = 475;


                            Paragraph Espacio7 = new Paragraph(10, " ");
                            Paragraph TSbtotal = new Paragraph("Subtotal:", TTexNegCuerpo);
                            TSbtotal.IndentationLeft = 430;
                            Paragraph TDes = new Paragraph("Descuento:", TTexNegCuerpo);
                            TDes.IndentationLeft = 430;

                            Paragraph TTotal2 = new Paragraph("Total:", TTexNegCuerpo);
                            TTotal2.IndentationLeft = 430;


                            Paragraph TTISR = new Paragraph("ISR:", TTexNegCuerpo);
                            TTISR.IndentationLeft = 430;
                            Paragraph Espacio8 = new Paragraph(5, " ");

                            Paragraph TNETO = new Paragraph("Neto a pagar:", TTexNegCuerpo);
                            TNETO.IndentationLeft = 430;



                            documento.Add(Ttotal);
                            documento.Add(Tper);
                            documento.Add(Tdedu);
                            documento.Add(Espacio7);
                            documento.Add(TSbtotal);
                            documento.Add(Tper2);
                            documento.Add(TDes);
                            documento.Add(Tdedu2);
                            if (iRecibo != 3 && (idEmpresa == 36 || idEmpresa == 37 || idEmpresa == 38 || idEmpresa == 39 || idEmpresa == 40 || idEmpresa == 41 || idEmpresa == 42 || idEmpresa == 44 || idEmpresa == 45 || idEmpresa == 46 || idEmpresa == 47 || idEmpresa == 48))
                            {
                                documento.Add(TTotal2);
                                documento.Add(Ttotal2);
                            }
                            if (iRecibo == 3 && (idEmpresa == 36 || idEmpresa == 37 || idEmpresa == 38 || idEmpresa == 39 || idEmpresa == 40 || idEmpresa == 41 || idEmpresa == 42 || idEmpresa == 44 || idEmpresa == 45 || idEmpresa == 46 || idEmpresa == 47 || idEmpresa == 48))
                            {
                                documento.Add(TTISR);
                                documento.Add(TISR);
                                documento.Add(TNETO);
                                documento.Add(TNetoA);

                            }



                            if (iRecibo != 3 && idEmpresa != 36 && idEmpresa != 37 && idEmpresa != 38 && idEmpresa != 39 && idEmpresa != 40 && idEmpresa != 41 && idEmpresa != 42 && idEmpresa != 44 && idEmpresa != 45 && idEmpresa != 46 && idEmpresa != 47 && idEmpresa != 48)
                            {
                                documento.Add(TTotal2);
                                documento.Add(Ttotal2);
                            }



                            documento.Add(Espacio8);

                            if (iRecibo == 2)
                            {



                                Paragraph table14 = new Paragraph();
                                table14.IndentationLeft = 50;

                                PdfPTable table15 = new PdfPTable(1);
                                table15.HorizontalAlignment = 0;
                                table15.PaddingTop = 10;
                                table15.TotalWidth = 250;
                                table15.LockedWidth = true;



                                PdfPCell Cell7 = new PdfPCell();
                                Cell7.BackgroundColor = BaseColor.BLACK;
                                Cell7.AddElement(new Chunk("SELLO DIGITAL EMISOR", TexHatable));
                                table15.AddCell(Cell7);


                                Palabra = LiTsat[0].sSelloSat; //// sello
                                string selloemi = Palabra;
                                selloemisor = Palabra;
                                Paragraph SelloEmi = new Paragraph(Palabra, TexNegCuerpo);

                                Paragraph TSello = new Paragraph();
                                table15.AddCell(SelloEmi);
                                table14.Add(table15);

                                Paragraph table16 = new Paragraph();
                                table16.IndentationLeft = 50;

                                PdfPTable table17 = new PdfPTable(1);
                                table17.HorizontalAlignment = 0;
                                table17.PaddingTop = 10;
                                table17.TotalWidth = 250;
                                table17.LockedWidth = true;

                                PdfPCell Cell8 = new PdfPCell();
                                Cell8.BackgroundColor = BaseColor.BLACK;
                                Cell8.AddElement(new Chunk("SELLO DIGITAL DEL SAT", TexHatable));
                                table17.AddCell(Cell8);


                                Palabra = LiTsat[0].sSelloCFD;
                                SelloCF = Palabra;
                                Palabra = LiTsat[0].sSelloSat;

                                string SelloSat2 = Palabra;
                                Paragraph SelloSAT = new Paragraph(Palabra, TexNegCuerpo);
                                table17.AddCell(SelloSAT);
                                table16.Add(table17);

                                //--
                                Paragraph table18 = new Paragraph();
                                table18.IndentationLeft = 50;

                                PdfPTable table19 = new PdfPTable(1);
                                table19.HorizontalAlignment = 0;
                                table19.PaddingTop = 10;
                                table19.TotalWidth = 250;
                                table19.LockedWidth = true;

                                PdfPCell Cell9 = new PdfPCell();
                                Cell9.BackgroundColor = BaseColor.BLACK;
                                Cell9.AddElement(new Chunk("Cadena Original del complemento de certificación digital del SAT", TexHatable));
                                table19.AddCell(Cell9);


                                RfcProv = LiTsat[0].Rfcprov;
                                Nomcer = LiTsat[0].sNoCertificado;
                                fechatem = LiTsat[0].Fechatimbrado;
                                string CadeSat2 = "||" + LiTsat[0].sUUID + "|" + LiTsat[0].Fechatimbrado + "|";
                                CadeSat = CadeSat2 + LiTsat[0].Rfcprov + "|" + selloemi + "|" + LiTsat[0].sNoCertificado + "||";
                                Paragraph CaSelloSAT = new Paragraph(CadeSat, TexNegCuerpo);
                                table19.AddCell(CaSelloSAT);
                                table18.Add(table19);



                                //    /// imagen Qr
                                //    /// 


                                string QrSat = "https://verificacfdi.facturaelectronica.sat.gob.mx/Defaul.aspx?id=" + LiTsat[0].sUUID + "&re=" + RfcEmi + "&rr=" + RfcRep + "&tt=" + Rtotal + "&fe=" + selloemi.Substring(selloemi.Length - 8, 8);
                                QRCodeEncoder encoder = new QRCodeEncoder();
                                Bitmap img = encoder.Encode(QrSat);
                                System.Drawing.Image QR = (System.Drawing.Image)img;

                                using (MemoryStream ms = new MemoryStream())
                                {
                                    QR.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    byte[] imageBytes = ms.ToArray();

                                    iTextSharp.text.Image imgQr = iTextSharp.text.Image.GetInstance(ms.ToArray());
                                    imgQr.BorderWidth = 0;
                                    imgQr.SetAbsolutePosition(400, 150);
                                    float porcentaje = 0.0f;
                                    porcentaje = 100 / imgQr.Width;
                                    imgQr.ScalePercent(porcentaje * 100);
                                    documento.Add(imgQr);

                                }


                                documento.Add(table14);
                                documento.Add(table16);
                                documento.Add(table18);



                            }

                            documento.Close();

                            row1007Ac = 0;

                        }


                    }


                }


            }
            LisIdcontrol = Dao2.ps_ControlEje_Insert_CControlEjecEmpr(Idusuario, sDEscripcion, inactivo, idEmpresa, anios, Tipodeperido, Perido, iRecibo, NoEjecuciones);
            EmisorReceptorBean ls = new EmisorReceptorBean();
            {
                ls.iNoEjecutados = NoEjecuciones;
                ls.sUrl = PathCarp;
            }
            url.Add(ls);

            return Json(url);
        }


        /// generacion de pdf para impresion

        public JsonResult GenPDFmv(int Anio, int TipoPeriodo, int iIdperiodo, int iIdempresa)
        {

            List<EmisorReceptorBean> LisEmpresa = new List<EmisorReceptorBean>();
            return Json(LisEmpresa);

        }

        [HttpPost]

        /// Lista empleado Finiquito
        public JsonResult ListEmpleadoFin(int iIdEmpresa, int TipoPeriodo, int periodo, int Anio, int IdEmpleado)
        {
            List<EmpleadosEmpresaBean> ListEmple = new List<EmpleadosEmpresaBean>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            ListEmple = Dao.sp_EmpleadosFiniquito_Retrieve_Tfiniquito_hst(iIdEmpresa, periodo, Anio, IdEmpleado, TipoPeriodo);
            return Json(ListEmple);
        }

        // Lsita de Calculo Finiquito 

        [HttpPost]

        public JsonResult ReciboFiniquito(int iIdEmpresa, int iIdEmpleado, int ianio, int iPeriodo, int idTipFiniquito)
        {

            List<ReciboNominaBean> LCRecibo = new List<ReciboNominaBean>();
            List<TablaNominaBean> LsTabla = new List<TablaNominaBean>();
            List<ReciboNominaBean> LsTotal = new List<ReciboNominaBean>();
            FuncionesNomina dao = new FuncionesNomina();
            LCRecibo = dao.sp_TpCalculoFiniEmpleado_Retrieve_TFiniquito(iIdEmpresa, iIdEmpleado, iPeriodo, ianio, idTipFiniquito);

            if (LCRecibo != null)
            {
                if (LCRecibo.Count > 0)
                {
                    for (int i = 0; i < LCRecibo.Count; i++)
                    {
                        if (LCRecibo[i].iIdRenglon != 990 && LCRecibo[i].iIdRenglon != 1990)
                        {
                            TablaNominaBean ls = new TablaNominaBean();
                            {
                                ls.sConcepto = LCRecibo[i].sNombre_Renglon;

                                if (LCRecibo[i].sValor == "Percepciones")
                                {
                                    ls.dPercepciones = LCRecibo[i].dSaldo.ToString("#.##");
                                    ls.dDeducciones = "0";
                                }
                                if (LCRecibo[i].sValor == "Deducciones")
                                {
                                    ls.dPercepciones = "0";
                                    ls.dDeducciones = LCRecibo[i].dSaldo.ToString();
                                }

                            }

                            ls.dExcento = LCRecibo[i].dExcento.ToString("#.##");
                            if (LCRecibo[i].dExcento < 1) { ls.dExcento = "0.00"; };
                            ls.dGravados = LCRecibo[i].dGravado.ToString("#.##");
                            if (LCRecibo[i].dGravado < 1) { ls.dGravados = "0.00"; };

                            ls.dSaldos = "0";
                            ls.dInformativos = "0";
                            LsTabla.Add(ls);

                        }
                        if (LCRecibo[i].iIdRenglon == 990 || LCRecibo[i].iIdRenglon == 1990)
                        {
                            ReciboNominaBean ls2 = new ReciboNominaBean();
                            ls2.iIdEmpleado = LCRecibo[i].iIdEmpleado;
                            ls2.iIdFiniquito = LCRecibo[i].iIdFiniquito;
                            ls2.iIdRenglon = LCRecibo[i].iIdRenglon;
                            ls2.dSaldo = LCRecibo[i].dSaldo;
                            ls2.sNombre_Renglon = LCRecibo[i].sNombre_Renglon;
                            LsTotal.Add(ls2);
                        }
                    }

                }


            }
            var result = new { Result = LsTabla, LsTotal };

            return Json(result);

        }

        /// Lista de tipo de Finiquito por empleado
        public JsonResult ListFiniquito(int iIdEmpresa, int iIdEmpleado, int Anio, int periodo)
        {
            List<TipoFiniquito> ListFini = new List<TipoFiniquito>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            ListFini = Dao.sp_TpFiniquitosEmpleado_Retrieve_TFiniquito(iIdEmpresa, iIdEmpleado, Anio, periodo);
            return Json(ListFini);
        }


        /// Elimina archivo 

        public JsonResult deletArchivo(string path)
        {
            List<TipoFiniquito> archivo = new List<TipoFiniquito>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            string PathArchivo = Server.MapPath(path);
            PathArchivo = PathArchivo.Replace("\\Empleados", "");
            if (System.IO.File.Exists(PathArchivo))
            {
                System.IO.File.Delete(PathArchivo);
                TipoFiniquito ls = new TipoFiniquito();
                ls.sMensaje = "archivoborrado";
                archivo.Add(ls);

            }
            return Json(archivo);
        }

        /// pdf masivos 

        [HttpPost]
        public JsonResult GPDFMasivos(int IdEmpresa, int Periodo, int anios, int Tipodeperido, int iRecibo)
        {

            int Idusuario = Convert.ToInt32(Session["iIdUsuario"]);
            int inactivo = 0;


            List<EmpresasBean> NoEmple = new List<EmpresasBean>();
            List<EmpleadosBean> Empleados = new List<EmpleadosBean>();
            List<EmisorReceptorBean> ListDatEmisor = new List<EmisorReceptorBean>();
            List<ReciboNominaBean> LisTRecibo = new List<ReciboNominaBean>();
            List<ReciboNominaBean> LisTDiasEmple = new List<ReciboNominaBean>();

            List<EmisorReceptorBean> url = new List<EmisorReceptorBean>();
            List<CInicioFechasPeriodoBean> LFechaPerido = new List<CInicioFechasPeriodoBean>();
            List<SelloSatBean> LiTsat = new List<SelloSatBean>();

            FuncionesNomina Dao = new FuncionesNomina();
            ListEmpleadosDao Dao2 = new ListEmpleadosDao();
            //  Dao2.ps_ControlEje_Insert_CControlEjecEmpr(Idusuario,sDEscripcion,inactivo);
            LFechaPerido = Dao2.sp_DatosPerido_Retrieve_DatosPerido(Periodo);

            string CadeSat, UUID, RfcEmi, RfcRep, SelloCF, RfcProv, Nomcer, fechatem, selloemisor;
            int NumEmpleado, Anio = anios, Tipoperido = Tipodeperido, Version = 12, Folio = 0;
            Folio = Anio * 100000 + Tipoperido * 10000 + Periodo * 10;
            var fileName = "";
            string Empre;
            string sDiasEfectivos;
            string PathPDF = Server.MapPath("Archivos\\");
            string PathZip = Server.MapPath("Archivos\\");
            PathPDF = PathPDF.Replace("\\Empleados", "");
            PathZip = PathZip.Replace("\\Empleados", "");
            PathPDF = PathPDF.Replace("\\Empleados", "");
            string Nombrearc = "RecibosNom";
            int idEmpresa = 0, rows = 0, sinSello = 0;
            Nombrearc = Nombrearc + "_E" + IdEmpresa + "_P" + LFechaPerido[0].iPeriodo + ".pdf";
            rows = 1;
            int idempleado = 0;
            string urlpdf = Nombrearc;

            //nombre y ubicacion del PDF

            Nombrearc = PathPDF + Nombrearc;

            if (System.IO.File.Exists(Nombrearc))
            {
                System.IO.File.Delete(Nombrearc);
                //EmisorReceptorBean ls = new EmisorReceptorBean();
                //ls.sMensaje = "success";
                //ls.sUrl = Nombrearc;
                //ListDatEmisor.Add(ls);

            }

            // crecion del archivo PDF
            FileStream Fs = new FileStream(Nombrearc, FileMode.Create);
            Document documento = new Document(iTextSharp.text.PageSize.LETTER, 2, 2, 5, 2);
            PdfWriter pw = PdfWriter.GetInstance(documento, Fs);
            documento.Open();

            for (int i = 0; i < rows; i++)
            {
                idEmpresa = IdEmpresa;
                NoEmple = Dao.sp_NoEmpleadosEmpresa_Retrieve_TempleadoNomina(idEmpresa, 0);
                Empleados = Dao.sp_EmpleadosEmpresa_Retrieve_TempleadoNomina(idEmpresa, 1);




                int Repetido = 0;
                for (int a = 0; a < NoEmple[0].iNoEmpleados;)
                {
                    Repetido = Repetido + 1;

                    int valido = 0;
                    idempleado = Empleados[a].iIdEmpleado;
                    ListDatEmisor = Dao2.sp_EmisorReceptor_Retrieve_EmisorReceptor(idEmpresa, Empleados[a].iIdEmpleado);
                    LisTRecibo = Dao.sp_TpCalculoEmpleado_Retrieve_TpCalculoEmpleado(idEmpresa, Empleados[a].iIdEmpleado, LFechaPerido[0].iPeriodo, Tipoperido, Anio, 0);
                    LiTsat = Dao2.sp_DatosSat_Retrieve_TSellosSat(idEmpresa, anios, Tipodeperido, LFechaPerido[0].iPeriodo, Empleados[a].iIdEmpleado);


                    if ((LisTRecibo != null && iRecibo == 1) || (LisTRecibo != null && LiTsat[0].sUUID != "" && iRecibo == 2))
                    {


                        string sAntiguedad = "";
                        List<XMLBean> LisCer = new List<XMLBean>();



                        // con QR
                        LiTsat = Dao2.sp_DatosSat_Retrieve_TSellosSat(idEmpresa, anios, Tipodeperido, LFechaPerido[0].iPeriodo, Empleados[a].iIdEmpleado);

                        if (ListDatEmisor != null && LiTsat[0].sUUID != "")
                        {
                            if (iRecibo == 1 && ListDatEmisor[0].sRFC.Length > 3)
                            {
                                valido = 1;
                            };
                            if (iRecibo == 2 && LiTsat != null && ListDatEmisor[0].sRFC.Length > 3)
                            {
                                if (LiTsat[0].sUUID.Length > 3)
                                {
                                    valido = 1;
                                };

                            };
                        };
                        if (valido == 1)
                        {
                            ListDatEmisor[0].sUrl = PathPDF;
                            LisCer = Dao2.sp_FileCer_Retrieve_CCertificados(ListDatEmisor[0].sRFC);
                            string pathCert = Server.MapPath("Archivos\\certificados\\");
                            pathCert = pathCert.Replace("\\Empleados", "");
                            string s_certificadoKey = pathCert + LisCer[0].sfilekey;
                            string s_certificadoCer = pathCert + LisCer[0].sfilecer;
                            string s_transitorio = LisCer[0].stransitorio;
                            if (System.IO.File.Exists(s_certificadoKey))
                            {

                                System.Security.Cryptography.X509Certificates.X509Certificate CerSAT;
                                CerSAT = System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromCertFile(s_certificadoCer);
                                byte[] bcert = CerSAT.GetSerialNumber();
                                string CerNo = LibreriasFacturas.StrReverse((string)Encoding.UTF8.GetString(bcert));
                                byte[] CERT_SIS = CerSAT.GetRawCertData();


                                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.EMBEDDED);
                                BaseFont arial = BaseFont.CreateFont(@"c:\windows\fonts\arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                                iTextSharp.text.Font TTexNeg = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.BOLD);
                                iTextSharp.text.Font TexNom = new iTextSharp.text.Font(bf, 7, iTextSharp.text.Font.NORMAL);
                                iTextSharp.text.Font TexNomSig = new iTextSharp.text.Font(arial, 7, iTextSharp.text.Font.NORMAL);


                                iTextSharp.text.Font Texchica = new iTextSharp.text.Font(bf, 4, iTextSharp.text.Font.NORMAL);
                                iTextSharp.text.Font TexNeg = new iTextSharp.text.Font(bf, 7, iTextSharp.text.Font.BOLD);
                                iTextSharp.text.Font TTexNegCuerpo = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.BOLD);
                                iTextSharp.text.Font TexNegCuerpo = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL);



                                //////Cabecera  



                                string Palabra = ListDatEmisor[0].sNombreEmpresa;
                                Empre = Palabra;
                                Paragraph Empresa = new Paragraph(10, Palabra, TTexNeg);
                                Empresa.IndentationLeft = 40;


                                Paragraph Trfc = new Paragraph("R.F.C.:", TexNeg);
                                Trfc.IndentationLeft = 40;
                                Palabra = ListDatEmisor[0].sRFC;
                                RfcEmi = Palabra;
                                Paragraph Rfc = new Paragraph(-1, Palabra, TexNom);
                                Rfc.IndentationLeft = 64;

                                Palabra = ListDatEmisor[0].sAfiliacionIMSS;
                                Paragraph Rfcpatron = new Paragraph(-1, Palabra, TexNom);
                                Rfcpatron.IndentationLeft = 86;
                                Paragraph TrfcPatron = new Paragraph("Reg.Pat:", TexNeg);
                                TrfcPatron.IndentationLeft = 40;


                                Paragraph TRegPat = new Paragraph(" ", TexNeg);
                                TRegPat.IndentationLeft = 40;
                                Paragraph RegPat = new Paragraph(-1, " ", TexNom);
                                RegPat.IndentationLeft = 68;

                                Paragraph espacioSalDi = new Paragraph(20, " ", TexNom);

                                Paragraph espacioRe2 = new Paragraph(20, " ", TexNom);

                                Paragraph espacioRe = new Paragraph(18, " ", TexNom);
                                /// Emprime Cabecera
                                if (Repetido == 2)
                                {

                                    documento.Add(espacioRe2);
                                };
                                documento.Add(espacioRe);
                                documento.Add(Empresa);
                                documento.Add(Trfc);
                                documento.Add(Rfc);
                                documento.Add(TrfcPatron);
                                documento.Add(Rfcpatron);
                                documento.Add(TRegPat);
                                documento.Add(RegPat);


                                /// cabesera direccion

                                Paragraph TDireccion = new Paragraph(-25, "Direccion:", TexNeg);
                                TDireccion.IndentationLeft = 350;
                                Palabra = ListDatEmisor[0].sCalle;
                                Paragraph Direccion = new Paragraph(-1, Palabra, TexNom);
                                Direccion.IndentationLeft = 382;


                                Paragraph TCol = new Paragraph("Col.", TexNeg);
                                TCol.IndentationLeft = 350;
                                Palabra = ListDatEmisor[0].sColonia;
                                Paragraph Col = new Paragraph(-1, Palabra, TexNom);
                                Col.IndentationLeft = 370;

                                Paragraph TCp = new Paragraph("CP:", TexNeg);
                                TCp.IndentationLeft = 350;
                                Palabra = Convert.ToString(ListDatEmisor[0].iCP);
                                if (Palabra.Length < 5)
                                {
                                    Palabra = "0" + Palabra;
                                }
                                Paragraph cp = new Paragraph(-1, Palabra, TexNom);
                                cp.IndentationLeft = 370;

                                Palabra = " ";
                                Paragraph espacio = new Paragraph(4, Palabra, TexNom);


                                Paragraph TEmpleado = new Paragraph("Datos del Empleado", TexNeg);
                                TEmpleado.IndentationLeft = 40;

                                Paragraph TNoNomina = new Paragraph("No Empleado:", TexNeg);
                                TNoNomina.IndentationLeft = 40;
                                Palabra = ListDatEmisor[0].sNombreComp;
                                Paragraph NoNomina = new Paragraph(-1, Palabra, TexNomSig);
                                NoNomina.IndentationLeft = 88;



                                Paragraph TRFCEmpleado = new Paragraph("RFC:   ", TexNeg);
                                TRFCEmpleado.IndentationLeft = 40;

                                Palabra = Convert.ToString(ListDatEmisor[0].sRFCEmpleado);
                                Paragraph RFCempleado = new Paragraph(-1, Palabra, TexNom);
                                RFCempleado.IndentationLeft = 60;


                                Paragraph TISSM = new Paragraph("AFIL. IMSS: ", TexNeg);
                                TISSM.IndentationLeft = 40;

                                Palabra = Convert.ToString(ListDatEmisor[0].sRegistroImss);
                                Paragraph NoISSM = new Paragraph(-1, Palabra, TexNom);
                                NoISSM.IndentationLeft = 80;


                                Paragraph TDepto = new Paragraph("DEPTO: ", TexNeg);
                                TDepto.IndentationLeft = 40;


                                Palabra = Convert.ToString(ListDatEmisor[0].sDescripcionDepartamento);
                                Paragraph Depto = new Paragraph(-1, Palabra, TexNom);
                                Depto.IndentationLeft = 75;

                                Paragraph TDiast = new Paragraph("Dias Trab.", TexNeg);
                                TDiast.IndentationLeft = 40;


                                // dias Efectivos 
                                decimal iTdias = LFechaPerido[0].iDiasEfectivos;


                                string Dias = "0";
                                string DiaIncap = "0";
                                if (LisTDiasEmple != null)
                                {
                                    for (int c = 0; c < LisTDiasEmple.Count; c++)
                                    {
                                        if (LisTDiasEmple[c].iIdRenglon == 31)
                                        {
                                            string dias = Convert.ToString(LisTDiasEmple[c].iDiasTrab);
                                            string[] Dias2 = dias.Split('.');
                                            Dias = Dias2[0];

                                        }

                                        if (LisTDiasEmple[c].iIdRenglon == 34)
                                        {
                                            string dias = Convert.ToString(LisTDiasEmple[c].iDiasTrab);
                                            string[] Dias2 = dias.Split('.');
                                            DiaIncap = Dias2[0];

                                        }
                                    }




                                }
                                if (LisTDiasEmple == null)
                                {
                                    Dias = Convert.ToString(LFechaPerido[0].iDiasEfectivos);
                                }

                                Convert.ToString(LisTRecibo[0].iDiasTrab);
                                sDiasEfectivos = Convert.ToString(iTdias);
                                string Dtota = Convert.ToString(LFechaPerido[0].iDiasEfectivos);



                                Palabra = Convert.ToString(Dtota);
                                Paragraph DiasT = new Paragraph(-1, Palabra, TexNom);
                                DiasT.IndentationLeft = 75;

                                Paragraph TTipoPe = new Paragraph(-45, "Periodo de pago: ", TexNeg);
                                TTipoPe.IndentationLeft = 350;

                                if (Tipodeperido == 0)
                                {
                                    Palabra = "Semanal";
                                }
                                if (Tipodeperido == 1)
                                {
                                    Palabra = "Decenal";
                                }
                                if (Tipodeperido == 2)
                                {
                                    Palabra = "Catorcenal";
                                }
                                if (Tipodeperido == 3)
                                {
                                    Palabra = "Quincenal";
                                }
                                if (Tipodeperido == 4)
                                {
                                    Palabra = "Mensual";
                                }
                                if (Tipodeperido == 5)
                                {
                                    Palabra = "Bimestral";
                                }


                                Paragraph TipoPe = new Paragraph(-1, Palabra, TexNom);
                                TipoPe.IndentationLeft = 405;

                                Paragraph TPeriodo = new Paragraph("Periodo de pago: ", TexNeg);
                                TPeriodo.IndentationLeft = 350;

                                Palabra = Convert.ToString(LFechaPerido[0].iPeriodo + " " + LFechaPerido[0].sFechaInicio + " AL " + LFechaPerido[0].sFechaFinal);
                                Paragraph Periodos = new Paragraph(-1, Palabra, TexNom);
                                Periodos.IndentationLeft = 405;

                                Paragraph Tpuesto = new Paragraph("Puesto: ", TexNeg);
                                Tpuesto.IndentationLeft = 350;


                                Palabra = Convert.ToString(ListDatEmisor[0].sNombrePuesto);
                                Paragraph puesto = new Paragraph(-1, Palabra, TexNom);
                                puesto.IndentationLeft = 380;



                                Paragraph TSalariod = new Paragraph("Sala. Diario:   ", TexNeg);
                                TSalariod.IndentationLeft = 350;

                                decimal SD = Convert.ToDecimal(ListDatEmisor[0].dSalarioMensual);
                                decimal SD2 = 0;
                                SD2 = SD / 30;

                                Palabra = string.Format("{0:N2}", SD2);
                                Paragraph Salariod = new Paragraph(-1, Palabra, TexNom);
                                Salariod.IndentationLeft = 395;


                                Paragraph TSalariodInt = new Paragraph("Sala. Dirario Int: ", TexNeg);
                                TSalariodInt.IndentationLeft = 350;

                                decimal Sdi = Convert.ToDecimal(ListDatEmisor[0].SDINT);
                                string dosDecimal = Sdi.ToString("0.##");
                                Sdi = Convert.ToDecimal(dosDecimal);
                                Palabra = string.Format("{0:N2}", Sdi);

                                Paragraph Salariodint = new Paragraph(-1, Palabra, TexNom);
                                Salariodint.IndentationLeft = 405;

                                Paragraph TCentroCost = new Paragraph("CEN.DE COSTOS:", TexNeg);
                                TCentroCost.IndentationLeft = 350;

                                Palabra = Convert.ToString(ListDatEmisor[0].sCentroCosto);
                                Paragraph CentroCost = new Paragraph(-1, Palabra, TexNom);
                                CentroCost.IndentationLeft = 410;

                                documento.Add(TDireccion);
                                documento.Add(Direccion);
                                documento.Add(TCol);
                                documento.Add(Col);
                                documento.Add(TCp);
                                documento.Add(cp);
                                documento.Add(espacio);
                                documento.Add(espacio);
                                documento.Add(TEmpleado);
                                documento.Add(espacio);
                                documento.Add(TNoNomina);
                                documento.Add(NoNomina);
                                documento.Add(TRFCEmpleado);
                                documento.Add(RFCempleado);
                                documento.Add(TISSM);
                                documento.Add(NoISSM);
                                documento.Add(TDepto);
                                documento.Add(Depto);
                                documento.Add(TDiast);
                                documento.Add(DiasT);
                                documento.Add(TTipoPe);
                                documento.Add(TipoPe);
                                documento.Add(TPeriodo);
                                documento.Add(Periodos);
                                documento.Add(Tpuesto);
                                documento.Add(puesto);

                                if (IdEmpresa != 2083 && idEmpresa != 2084)
                                {

                                    documento.Add(TSalariod);
                                    documento.Add(Salariod);
                                    documento.Add(TSalariodInt);
                                    documento.Add(Salariodint);

                                }

                                if (idEmpresa == 2083 || IdEmpresa == 2084) { documento.Add(espacioSalDi); }


                                documento.Add(TCentroCost);
                                documento.Add(CentroCost);




                                Paragraph Espacio2 = new Paragraph(-1, " ");
                                Paragraph table1 = new Paragraph();
                                table1.IndentationLeft = 100;
                                PdfPTable table2 = new PdfPTable(2);
                                table2.HorizontalAlignment = 0;
                                table2.PaddingTop = 10;
                                table2.TotalWidth = 500;
                                table2.LockedWidth = true;

                                Paragraph table3 = new Paragraph();
                                table3.IndentationLeft = 50;


                                PdfPTable table4 = new PdfPTable(2);
                                table4.HorizontalAlignment = 0;
                                table4.PaddingTop = 10;
                                table4.TotalWidth = 250;
                                table4.LockedWidth = true;

                                Paragraph table5 = new Paragraph();
                                table5.IndentationLeft = 310;

                                PdfPTable table6 = new PdfPTable(2);
                                table6.HorizontalAlignment = 0;
                                table6.PaddingTop = 10;
                                table6.TotalWidth = 250;
                                table6.LockedWidth = true;




                                PdfPCell Cell5 = new PdfPCell();
                                Cell5.BackgroundColor = BaseColor.WHITE;
                                Cell5.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                Cell5.AddElement(new Chunk("PERCEPCIONES", TexNeg));


                                PdfPCell Cell6 = new PdfPCell();
                                Cell6.BackgroundColor = BaseColor.WHITE;
                                Cell6.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                Cell6.AddElement(new Chunk("DEDUCCIONES", TexNeg));
                                table2.AddCell(Cell5);
                                table2.AddCell(Cell6);
                                table1.Add(table2);

                                PdfPCell Cell7 = new PdfPCell();
                                Cell7.BackgroundColor = BaseColor.WHITE;
                                Cell7.MinimumHeight = 120f;
                                //Cell7.HasMinimumHeight = 350f;
                                Cell7.Border = iTextSharp.text.Rectangle.NO_BORDER;

                                PdfPCell Cell8 = new PdfPCell();
                                Cell8.BackgroundColor = BaseColor.WHITE;
                                Cell8.MinimumHeight = 120f;
                                Cell8.Border = iTextSharp.text.Rectangle.NO_BORDER;

                                PdfPCell Cell9 = new PdfPCell();
                                Cell9.BackgroundColor = BaseColor.WHITE;
                                Cell9.MinimumHeight = 120f;
                                Cell9.Border = iTextSharp.text.Rectangle.NO_BORDER;

                                PdfPCell Cell10 = new PdfPCell();
                                Cell10.MinimumHeight = 120f;
                                Cell10.BackgroundColor = BaseColor.WHITE;
                                Cell10.Border = iTextSharp.text.Rectangle.NO_BORDER;


                                documento.Add(espacio);
                                documento.Add(table1);

                                int b = 0;
                                string Palabra2;
                                decimal valor;
                                decimal per = 0;
                                decimal ded = 0;
                                decimal total;

                                if (LisTRecibo.Count > 0)
                                {
                                    for (int x = 0; x < LisTRecibo.Count; x++)
                                    {
                                        if (LisTRecibo[x].sValor == "Percepciones")
                                        {
                                            string lengRenglon = "";
                                            string ImporGra = string.Format("{0:N2}", LisTRecibo[x].dSaldo);
                                            string imporSald = ImporGra;
                                            ImporGra = ImporGra.Replace(",", "");
                                            string IdRenglon = Convert.ToString(LisTRecibo[x].iIdRenglon);
                                            string concepto = LisTRecibo[x].sRengNom;
                                            if (IdRenglon == "1")
                                            {
                                                concepto = LisTRecibo[x].sNombre_Renglon + " " + Dias + " Dias}";
                                                lengRenglon = "001";
                                                if (ListDatEmisor[0].iPagopor == 364)
                                                {

                                                    concepto = LisTRecibo[x].sNombre_Renglon + " " + Dias + " Dias}";
                                                    if (IdEmpresa == 2075 || IdEmpresa == 2076 || IdEmpresa == 2077 || IdEmpresa == 2078)
                                                    {
                                                        concepto = LisTRecibo[x].sNombre_Renglon;
                                                    }


                                                }




                                            }
                                            lengRenglon = "010";
                                            int idReglontama = IdRenglon.Length;
                                            if (idReglontama == 1) { IdRenglon = "00" + IdRenglon; };
                                            if (idReglontama == 2) { IdRenglon = "0" + IdRenglon; };


                                            Palabra = concepto;
                                            Palabra2 = ImporGra;
                                            valor = decimal.Parse(Palabra2);
                                            per = per + valor;
                                            Paragraph TLeyenda = new Paragraph(Palabra, TexNegCuerpo);
                                            TLeyenda.IndentationLeft = 75;
                                            Paragraph TPercep = new Paragraph(-1, Palabra2, TexNegCuerpo);
                                            TPercep.IndentationLeft = 180;

                                            string Perp = concepto + "    " + ImporGra;

                                            Cell7.AddElement(new Chunk(Palabra, TexNeg));
                                            Cell8.AddElement(new Chunk(imporSald, TexNeg));


                                        }
                                        if (LisTRecibo[x].sValor == "Deducciones")
                                        {

                                            string lengRenglon = "";
                                            string ImporGra = string.Format("{0:N2}", LisTRecibo[x].dSaldo);
                                            string IporSal = ImporGra;
                                            ImporGra = ImporGra.Replace(",", "");
                                            string IdRenglon = Convert.ToString(LisTRecibo[x].iIdRenglon);
                                            string concepto = LisTRecibo[x].sRengNom;

                                            Palabra = concepto;
                                            Palabra2 = ImporGra;
                                            valor = decimal.Parse(Palabra2);
                                            ded = ded + valor;
                                            Paragraph TLeyenda = new Paragraph(Palabra, TexNegCuerpo);
                                            TLeyenda.IndentationLeft = 300;
                                            Paragraph TDedu = new Paragraph(-1, string.Format("{0:N2}", LisTRecibo[x].dSaldo), TexNegCuerpo);
                                            TDedu.IndentationLeft = 450;
                                            Cell9.AddElement(new Chunk(Palabra, TexNeg));
                                            Cell10.AddElement(new Chunk(IporSal, TexNeg));

                                        }
                                    }

                                }


                                table4.AddCell(Cell7);
                                table4.AddCell(Cell8);
                                table3.Add(table4);

                                table6.AddCell(Cell9);
                                table6.AddCell(Cell10);
                                table5.Add(table6);

                                Palabra = " ";
                                Paragraph espacio3 = new Paragraph(-130, Palabra, TexNom);
                                documento.Add(espacio);
                                documento.Add(espacio);
                                documento.Add(table3);
                                documento.Add(espacio3);
                                documento.Add(table5);
                                documento.Add(espacio);
                                documento.Add(espacio);
                                Paragraph TTOtalPer = new Paragraph(" Total Percepciones: ", TexNeg);
                                TTOtalPer.IndentationLeft = 100;

                                Palabra = string.Format("{0:N2}", per);
                                Paragraph Totaper = new Paragraph(-1, Palabra, TexNom);
                                Totaper.IndentationLeft = 170;

                                Paragraph TTotaldeduc = new Paragraph(-1, " Total deduccion: ", TexNeg);
                                TTotaldeduc.IndentationLeft = 350;

                                Palabra = string.Format("{0:N2}", ded);
                                Paragraph Totadeduc = new Paragraph(-1, Palabra, TexNom);
                                Totadeduc.IndentationLeft = 420;


                                Paragraph TTipopago = new Paragraph("99:Otros   PAGO EN UNA SOLA EXHIBICIÓN ", TexNeg);
                                TTipopago.IndentationLeft = 40;




                                string cantidad = string.Format("{0:N2}", per - ded);

                                cantidad = NumeroALetras(cantidad);

                                Paragraph TTipogoEmpra = new Paragraph("RECIBI " + Empre + ", LA CANTIDAD DE: " + cantidad + " M/N", TexNeg);
                                TTipogoEmpra.IndentationLeft = 40;

                                Palabra = " " /*Convert.ToString()*/;
                                Paragraph CantidaLetr = new Paragraph(-1, Palabra, TexNom);
                                CantidaLetr.IndentationLeft = 220;

                                Paragraph TTotoal = new Paragraph(-8, "Total a Pagar: ", TexNeg);
                                TTotoal.IndentationLeft = 350;
                                Paragraph TTotoal2 = new Paragraph(-1, string.Format("{0:N2}", per - ded), TexNom);
                                TTotoal2.IndentationLeft = 400;

                                documento.Add(TTOtalPer);
                                documento.Add(Totaper);
                                documento.Add(TTotaldeduc);
                                documento.Add(Totadeduc);
                                documento.Add(espacio);
                                documento.Add(TTipopago);
                                documento.Add(TTipogoEmpra);
                                documento.Add(CantidaLetr);
                                documento.Add(TTotoal);
                                documento.Add(TTotoal2);

                                PdfPTable tableQR = new PdfPTable(1);
                                tableQR.HorizontalAlignment = 0;
                                tableQR.PaddingTop = 10;
                                tableQR.TotalWidth = 50;
                                tableQR.LockedWidth = true;
                                Paragraph tablqr = new Paragraph();
                                tablqr.IndentationLeft = 40;

                                PdfPTable tableSell = new PdfPTable(1);
                                tableSell.HorizontalAlignment = 0;
                                tableSell.PaddingTop = 10;
                                tableSell.TotalWidth = 250;
                                tableSell.LockedWidth = true;
                                Paragraph tablSello = new Paragraph();
                                tablSello.IndentationLeft = 100;



                                PdfPCell Cellqr = new PdfPCell();
                                Cellqr.MinimumHeight = 25f;
                                Cellqr.FixedHeight = PageSize.LETTER.Height / 15;
                                Cellqr.BackgroundColor = BaseColor.WHITE;
                                Cellqr.Border = iTextSharp.text.Rectangle.NO_BORDER;

                                PdfPCell CellSello = new PdfPCell();
                                CellSello.MinimumHeight = 25f;
                                CellSello.BackgroundColor = BaseColor.WHITE;
                                CellSello.Border = iTextSharp.text.Rectangle.NO_BORDER;


                                LiTsat = Dao2.sp_DatosSat_Retrieve_TSellosSat(idEmpresa, anios, Tipodeperido, LFechaPerido[0].iPeriodo, Empleados[a].iIdEmpleado);
                                if (LiTsat != null)
                                {

                                    Palabra = Convert.ToString(LiTsat[0].sUUID);
                                    Paragraph TFolio = new Paragraph("Folio Fiscal: " + Palabra, TexNeg);
                                    TFolio.IndentationLeft = 40;
                                    documento.Add(espacio);
                                    documento.Add(espacio);
                                    documento.Add(TFolio);


                                    Palabra = Convert.ToString(LiTsat[0].sSelloCFD);
                                    CellSello.AddElement(new Chunk(Palabra, Texchica));


                                    if (LiTsat[0].sUUID != "")
                                    {
                                        documento.Add(espacio);
                                        documento.Add(espacio);
                                        documento.Add(espacio);

                                        string selloemi = LiTsat[0].sSelloSat;
                                        string QrSat = "https://verificacfdi.facturaelectronica.sat.gob.mx/Defaul.aspx?id=" + LiTsat[0].sUUID + "&re=" + ListDatEmisor[0].sRFC + "&rr=" + ListDatEmisor[0].sRFCEmpleado + "&tt=" + (per - ded) + "&fe=" + selloemi.Substring(selloemi.Length - 8, 8);
                                        QRCodeEncoder encoder = new QRCodeEncoder();
                                        Bitmap img = encoder.Encode(QrSat);
                                        System.Drawing.Image QR = (System.Drawing.Image)img;
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            QR.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                            byte[] imageBytes = ms.ToArray();

                                            iTextSharp.text.Image imgQr = iTextSharp.text.Image.GetInstance(ms.ToArray());
                                            imgQr.BorderWidth = 0;
                                            //imgQr.SetAbsolutePosition(450, 40);
                                            imgQr.IndentationLeft = 40;
                                            float porcentaje = 0.0f;
                                            porcentaje = 10 / imgQr.Width;
                                            imgQr.ScalePercent(porcentaje * 50);

                                            Cellqr.Image = iTextSharp.text.Image.GetInstance(imgQr);  //.AddElement(new Chunk(imgQr, Texchica));
                                                                                                      //documento.Add(imgQr);

                                        }


                                    };
                                    if (LiTsat[0].sUUID == "" || LiTsat[0].sUUID == " " || LiTsat[0].sUUID == null)
                                    {
                                        documento.Add(espacio);
                                        documento.Add(espacio);
                                        documento.Add(espacio);
                                        documento.Add(espacio);
                                        documento.Add(espacio);
                                        documento.Add(espacio);
                                        documento.Add(espacio);
                                        documento.Add(espacio);
                                        documento.Add(espacio);


                                    };


                                }
                                Paragraph espacioSin = new Paragraph(32, " ", TexNom);
                                if (LiTsat == null)
                                {
                                    sinSello = 1;
                                    documento.Add(espacioSin);


                                }

                                Paragraph espaciotablaSe = new Paragraph(-55, " ", TexNeg);
                                Paragraph TFirmaEmple = new Paragraph(-25, "Firma Empleado", TexNeg);
                                TFirmaEmple.IndentationLeft = 400;
                                Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0f, 70f, BaseColor.BLACK, Element.ALIGN_LEFT, 0.2f)));
                                p.IndentationLeft = 390;
                                p.IndentationRight = 100;

                                Paragraph espaciofin2 = new Paragraph(-25, " ", TexNom);
                                if (LiTsat[0].sUUID == "" || LiTsat[0].sUUID == " ")
                                {
                                    documento.Add(espaciofin2);

                                }


                                documento.Add(p);
                                tableQR.AddCell(Cellqr);
                                tablqr.Add(tableQR);
                                tableSell.AddCell(CellSello);
                                tablSello.Add(tableSell);
                                documento.Add(tablqr);
                                documento.Add(espaciotablaSe);
                                documento.Add(tablSello);
                                documento.Add(espacio);
                                documento.Add(espacio);

                                if (sinSello == 1)
                                {
                                    documento.Add(espacioSin);
                                }


                                Paragraph espacioSinR2 = new Paragraph(10, " ", TexNom);
                                if (sinSello == 1 && Repetido == 2)
                                {
                                    documento.Add(espacioSinR2);
                                }

                                documento.Add(TFirmaEmple);
                                documento.Add(espacio);
                                documento.Add(espacio);
                                documento.Add(espacio);
                                documento.Add(espacio);
                                documento.Add(espacio);
                                documento.Add(espacio);
                                documento.Add(espacio);

                                Paragraph espaciofin = new Paragraph(10, " ", TexNom);

                                if (Repetido == 2)
                                {
                                    if (sinSello != 1)
                                    {
                                        documento.Add(espaciofin);
                                    }

                                }
                            };

                        }
                    }


                    if (Repetido == 2) { a = a + 1; Repetido = 0; }


                }

            }

            documento.Close();

            EmisorReceptorBean ls = new EmisorReceptorBean();
            {
                ls.sUrl = urlpdf;

            }
            return Json(ListDatEmisor);
        }

        //// Cantidad en letra 
        public string NumeroALetras(string num)
        {
            string res, dec = "";
            Int64 entero;
            int decimales;
            double nro;

            try
            {
                nro = Convert.ToDouble(num);
            }
            catch
            {
                return "";
            }

            entero = Convert.ToInt64(Math.Truncate(nro));
            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));

            if (decimales > 0)
            {
                dec = " CON " + decimales.ToString() + "/100";
            }

            res = NumeroALetras(Convert.ToDouble(entero)) + dec;
            return res;
        }

        // cambia la cantidad en letra 
        private static string NumeroALetras(double value)
        {
            string Num2Text = "";
            value = Math.Truncate(value);

            if (value == 0) Num2Text = "CERO";
            else if (value == 1) Num2Text = "UNO";
            else if (value == 2) Num2Text = "DOS";
            else if (value == 3) Num2Text = "TRES";
            else if (value == 4) Num2Text = "CUATRO";
            else if (value == 5) Num2Text = "CINCO";
            else if (value == 6) Num2Text = "SEIS";
            else if (value == 7) Num2Text = "SIETE";
            else if (value == 8) Num2Text = "OCHO";
            else if (value == 9) Num2Text = "NUEVE";
            else if (value == 10) Num2Text = "DIEZ";
            else if (value == 11) Num2Text = "ONCE";
            else if (value == 12) Num2Text = "DOCE";
            else if (value == 13) Num2Text = "TRECE";
            else if (value == 14) Num2Text = "CATORCE";
            else if (value == 15) Num2Text = "QUINCE";
            else if (value < 20) Num2Text = "DIECI" + NumeroALetras(value - 10);
            else if (value == 20) Num2Text = "VEINTE";
            else if (value < 30) Num2Text = "VEINTI" + NumeroALetras(value - 20);
            else if (value == 30) Num2Text = "TREINTA";
            else if (value == 40) Num2Text = "CUARENTA";
            else if (value == 50) Num2Text = "CINCUENTA";
            else if (value == 60) Num2Text = "SESENTA";
            else if (value == 70) Num2Text = "SETENTA";
            else if (value == 80) Num2Text = "OCHENTA";
            else if (value == 90) Num2Text = "NOVENTA";

            else if (value < 100) Num2Text = NumeroALetras(Math.Truncate(value / 10) * 10) + " Y " + NumeroALetras(value % 10);
            else if (value == 100) Num2Text = "CIEN";
            else if (value < 200) Num2Text = "CIENTO " + NumeroALetras(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = NumeroALetras(Math.Truncate(value / 100)) + "CIENTOS";

            else if (value == 500) Num2Text = "QUINIENTOS";
            else if (value == 700) Num2Text = "SETECIENTOS";
            else if (value == 900) Num2Text = "NOVECIENTOS";
            else if (value < 1000) Num2Text = NumeroALetras(Math.Truncate(value / 100) * 100) + " " + NumeroALetras(value % 100);
            else if (value == 1000) Num2Text = "MIL";
            else if (value < 2000) Num2Text = "MIL " + NumeroALetras(value % 1000);
            else if (value < 1000000)
            {
                Num2Text = NumeroALetras(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0) Num2Text = Num2Text + " " + NumeroALetras(value % 1000);
            }

            else if (value == 1000000) Num2Text = "UN MILLON";
            else if (value < 2000000) Num2Text = "UN MILLON " + NumeroALetras(value % 1000000);
            else if (value < 1000000000000)
            {
                Num2Text = NumeroALetras(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000) * 1000000);
            }
            else if (value == 1000000000000) Num2Text = "UN BILLON";
            else if (value < 2000000000000) Num2Text = "UN BILLON " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            else
            {
                Num2Text = NumeroALetras(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }

            return Num2Text;
        }

        // Carga la ultima ejecuion de la tabla Control de ejecucion 
        [HttpPost]
        public JsonResult TheLastEjecution()
        {
            string IdEmpresas = "";
            List<SelloSatBean> LSelloSat = new List<SelloSatBean>();
            List<ControlEjecucionBean> LisIdcontrol = new List<ControlEjecucionBean>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            LisIdcontrol = Dao.sp_UltimaEje_Retrieve_CControlejecEmpr();
            IdEmpresas = Convert.ToString(LisIdcontrol[0].iIdempresa);
            if (LisIdcontrol.Count > 1)
            {
                IdEmpresas = "";
                for (int i = 0; i < LisIdcontrol.Count; i++)
                {

                    IdEmpresas = IdEmpresas + Convert.ToString(LisIdcontrol[i].iIdempresa) + ",";

                };

                int startIndex = 0;
                int length = IdEmpresas.Length - 1;
                IdEmpresas = IdEmpresas.Substring(startIndex, length);
            };

            // LSelloSat = Dao.sp_EjectadosAndSend_Retrieve_TSelloSat(IdEmpresas, LisIdcontrol[0].iAnio, LisIdcontrol[0].iTipoPeriodo, LisIdcontrol[0].iPeriodo, 1, 0, 0, LisIdcontrol[0].iRecibo);
            var TablasDat = new { TablasDat = LisIdcontrol, LSelloSat };

            return Json(TablasDat);
        }
        // envia los las facturas por correo 

        [HttpPost]
        public JsonResult EnvioEmail(int Anio, int TipoPeriodo, int Perido, String sIdEmpresas, int iRecibo)
        {

            string[] valor = sIdEmpresas.Split(' ');
            int idEmpre = int.Parse(valor[0].ToString());
            string IdEmpresas = valor[0].ToString();
            int NoEmailSend = 0;
            int NoEmailNotSend = 0;
            int PdfXX = 0;
            List<SelloSatBean> LSelloSat = new List<SelloSatBean>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            if (valor.Length > 1)
            {
                IdEmpresas = "";
                for (int i = 0; i < valor.Length; i++)
                {

                    IdEmpresas = Convert.ToString(valor[i]) + ",";

                };

                int startIndex = 0;
                int length = IdEmpresas.Length - 1;
                IdEmpresas = IdEmpresas.Substring(startIndex, length);
            };

            idEmpre = Convert.ToInt32(sIdEmpresas);
            LSelloSat = Dao.sp_EjectadosAndSend_Retrieve_TSelloSat(idEmpre, Anio, TipoPeriodo, Perido, 1, 0, 0, iRecibo);
            if (LSelloSat != null)
            {
                if (iRecibo == 1)
                {
                    for (int i = 0; i < LSelloSat.Count; i++)
                    {
                        if (LSelloSat[i].sUurReciboSim != null && LSelloSat[i].sEmailSendSim != "Enviado" && LSelloSat[i].sEmailEmpresa != "" && LSelloSat[i].sEmailPErsona != "" && LSelloSat[i].sEmailPErsona != " ")
                        {

                            string EmailPer = LSelloSat[i].sEmailPErsona;
                            string Mensaje = "<b>Estimado:  </b>" + LSelloSat[i].sNombre + " <br><br> Esperando que tenga un buen día, se le hace llegar a través de este correo de forma anexa su <b>recibo de nómina</b>, correspondiente al Periodo:" + LSelloSat[i].iPeriodo + " del año:" + LSelloSat[i].ianio + ".<br><br> Sin más por el momento, quedamos a sus órdenes para cualquier aclaración.<br><br><b> Atentamente.</b> <br><br> Capital Humano. ";
                            Correo EmailEnvio = new Correo(EmailPer, "Recibo de Nómina", Mensaje, LSelloSat[i].sUurReciboSim, LSelloSat[i].sEmailEmpresa, LSelloSat[i].sPassword);
                            if (EmailEnvio.Estado)
                            {
                                //Correo enviado
                                Dao.sp_CCejecucionAndSen_update_TsellosSat(LSelloSat[i].iIdEmpresa, LSelloSat[i].iIdEmpleado, Anio, TipoPeriodo, Perido, 3, " ");
                                // LSelloSat[0].sMensaje = "success";
                                NoEmailSend = NoEmailSend + 1;
                            }
                            else
                            {
                                NoEmailNotSend = NoEmailNotSend + 1;
                                //Error al enviar correo
                                // LSelloSat[0].sMensaje = "error";
                            }

                        }
                        else
                        {
                            if (LSelloSat[i].sEmailSendSim != "Enviado")
                            {
                                NoEmailNotSend = NoEmailNotSend + 1;
                                //Error al enviar correo
                                // LSelloSat[0].sMensaje = "error";
                            }

                        }
                    };
                }
                if (iRecibo == 2)
                {
                    for (int i = 0; i < LSelloSat.Count; i++)
                    {
                        if (LSelloSat[i].sUrllReciboFis != null && LSelloSat[i].bEmailSent != "Enviado")
                        {
                            // string EmailEmple = LSelloSat[i].sEmailSent;
                            if (LSelloSat[i].sEmailEmpresa != "" && LSelloSat[i].sEmailPErsona != "")
                            {

                                if (System.IO.File.Exists(LSelloSat[i].sUrllReciboFis))
                                {
                                    string EmailPer = LSelloSat[i].sEmailPErsona;
                                    string EmailEmpresa = LSelloSat[i].sEmailEmpresa;
                                    string Mensaje = "<b>Estimado:  </b>" + LSelloSat[i].sNombre + " <br><br> Esperando que tenga un buen día, se le hace llegar a través de este correo de forma anexa su <b>recibo de nómina</b>, correspondiente al Periodo:" + LSelloSat[i].iPeriodo + " del año:" + LSelloSat[i].ianio + ".<br><br> Sin más por el momento, quedamos a sus órdenes para cualquier aclaración.<br><br><b> Atentamente.</b> <br><br> Capital Humano. ";
                                    Correo EmailEnvio = new Correo(EmailPer, "Recibos Nómina", Mensaje, LSelloSat[i].sUrllReciboFis, LSelloSat[i].sEmailEmpresa, LSelloSat[i].sPassword);
                                    if (EmailEnvio.Estado)
                                    {
                                        //Correo enviado
                                        Dao.sp_CCejecucionAndSen_update_TsellosSat(LSelloSat[i].iIdEmpresa, LSelloSat[i].iIdEmpleado, Anio, TipoPeriodo, Perido, 1, " ");
                                        NoEmailSend = NoEmailSend + 1;
                                    }
                                    else
                                    {
                                        NoEmailNotSend = NoEmailNotSend + 1;
                                        //Error al enviar correo

                                    }


                                }

                                else
                                {

                                    PdfXX = PdfXX + 1;
                                }

                            }
                            else
                            {

                                NoEmailNotSend = NoEmailNotSend + 1;
                                //Error al enviar correo
                            }
                        }

                        else
                        {
                            if (LSelloSat[i].bEmailSent != "Enviado")
                            {
                                NoEmailNotSend = NoEmailNotSend + 1;
                                //Error al enviar correo


                            }

                        }

                    };
                };
                LSelloSat = Dao.sp_EjectadosAndSend_Retrieve_TSelloSat(idEmpre, Anio, TipoPeriodo, Perido, 1, 0, 0, iRecibo);

                LSelloSat[0].iNoEnviados = NoEmailSend;
                LSelloSat[0].iNoNoEnviados = NoEmailNotSend;
                LSelloSat[0].iNoPdfError = PdfXX;
            };


            return Json(LSelloSat);
        }


        /// lista periodo por empresa

        [HttpPost]
        public JsonResult ListPeriodoEmpresa(int IdDefinicionHD, int iperiodo, int NomCerr, int Anio)
        {
            List<CInicioFechasPeriodoBean> LPe = new List<CInicioFechasPeriodoBean>();
            FuncionesNomina dao = new FuncionesNomina();
            LPe = dao.sp_PeridosEmpresa_Retrieve_CinicioFechasPeriodo(IdDefinicionHD, iperiodo, NomCerr, Anio);
            return Json(LPe);
        }
        [HttpPost]
        public JsonResult LoadEmpleadosAutorizados()
        {
            pruebaEmpleadosDao Dao = new pruebaEmpleadosDao();
            int Usuario_id = int.Parse(Session["iIdUsuario"].ToString());
            int Empresa_id = int.Parse(Session["IdEmpresa"].ToString());
            List<AutorizaVacaciones> list = Dao.sp_TAutorizaVacaciones_retrieve_Empleados(Empresa_id, Usuario_id);
            return Json(list);
        }
        [HttpPost]
        public JsonResult AddEmpleadosAutorizados(int Empleado_id)
        {
            pruebaEmpleadosDao Dao = new pruebaEmpleadosDao();
            int Empresa_id = int.Parse(Session["IdEmpresa"].ToString());
            List<string> list = Dao.sp_TAutorizaVacaciones_insert_Empleado(Empresa_id, Empleado_id, 1);
            return Json(list);
        }
        [HttpPost]
        public JsonResult EnviarCorreoAutorizadores(int Empleado_id, string Inicio, string Fin, string Dias)
        {
            pruebaEmpleadosDao Dao = new pruebaEmpleadosDao();
            //int Empresa_id = int.Parse(Session["IdEmpresa"].ToString());
            //List<string> empleado = Dao.sp_Templeado_Retrieve_DatosEmpleado(Empresa_id, Empleado_id, 1);
            List<string> empleado = Dao.sp_Templeado_Retrieve_DatosEmpleado(31, 907, 1);
            ReturnBean returnBean = new ReturnBean();
            if (empleado[20] == "" || empleado[20].Length < 12 || empleado[20] == null)
            {

            }
            else
            {

            }

            MandarCorreos sender = new MandarCorreos();
            string mensaje = "" +
                "<h3>Solicitud de Vacaciones: <span style='color: darkred;'> " + Empleado_id + " - " + empleado[1] + " " + empleado[2] + " " + empleado[3] + " </span></h3>" +
                "<hr>" +
                "<br>" +
                "<h4> Se a generado una nueva solicitud de Vacaciones para el empleado<span style='color: darkred;'> " + Empleado_id + " - " + empleado[1] + " " + empleado[2] + " " + empleado[3] + " </span> con las siguientes caracteristicas:  </h5>" +
                "<br>" +
                "<table>" +
                    "<tr>" +
                        "<td style='width: 5%;'> <img src='cid:calendar1'> </td>" +
                        "<td style='width: 25%;'> <h4> Fecha Inicio: </h4> </td>" +
                        "<td style='width: 70%;'> 01/04/2021 </td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td style='width: 5%;'> <img src='cid:calendar2'> </td>" +
                        "<td style='width: 25%;'> <h4> Fecha Fin: </h4> </td>" +
                        "<td style='width: 70%;'> 01/06/2021 </td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td style='width: 5%;'> <img src='cid:hashtag'> </td>" +
                        "<td style='width: 25%;'> <h4> Dias: </h4> </td>" +
                        "<td style='width: 70%;'> 60 </td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td colspan='3'><br><br>Para aceptar o rechazar la solicitud ingrese al sistema<a href='https://emisionrecibo.gruposeri.com:58510/Home/Index'> IPSNet </a> en el apartado de<strong> Kiosko</strong> en el submenu de<strong> Autorización de vacaciones</strong>.</td>" +
                    "</tr>" +
                "</table>" +
                "<br><br><br><p><small> Este es un correo electronico unicamente informativo, no responder notificaciones dentro del mismo.</small></p>";

            //mensaje.Replace("@@Nombre@@", empleado[2] + " " + empleado[3] + " " + empleado[4]);
            //mensaje.Replace("@@Nomina@@", empleado[1] + "");
            ////mensaje.Replace("@@Inicio@@", Inicio);
            //mensaje.Replace("@@Inicio@@", "01/01/2021");
            ////mensaje.Replace("@@Fin@@", Fin);
            //mensaje.Replace("@@Fin@@", "01/06/2021");
            ////mensaje.Replace("@@Dias@@", Dias);
            //mensaje.Replace("@@Dias@@", "60");
            //mensaje.Replace("@@Empresa@@", empleado[0]);

            bool mailOk = false;

            try
            {
                mailOk = sender.CustomMail("oscarm@raciti.com.mx", "Solicitud vacaciones Empleado: " + Empleado_id + " Empresa: " + 31, mensaje);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (mailOk)
            {
                returnBean.iFlag = 0;
                returnBean.sRespuesta = "Correo enviado correctamente";
            }
            else
            {
                returnBean.iFlag = 1;
                returnBean.sRespuesta = "Error al mandar correo a autorizadores";
            }


            return Json(returnBean);
        }

        [HttpPost]
        public JsonResult DataListEmpleadoFi(int iIdEmpresa, int periodo, int Anio)
        {
            List<EmpleadosEmpresaBean> ListEmple = new List<EmpleadosEmpresaBean>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            ListEmple = Dao.sp_EmpledoFi_Retrieve_TFiniquito(iIdEmpresa, periodo, Anio);
            return Json(ListEmple);
        }


        // consulta informacion de la tabla sat sellos;
        [HttpPost]
        public JsonResult TheLastSend(int Anio, int Tipoperiodo, int Perido, int iRecibo, string sIdEmpresas)
        {


            List<SelloSatBean> LSelloSat = new List<SelloSatBean>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();

            string SidEmpre = sIdEmpresas;
            int iIdEmpre = Convert.ToInt16(SidEmpre);



            LSelloSat = Dao.sp_EjectadosAndSend_Retrieve_TSelloSat(iIdEmpre, Anio, Tipoperiodo, Perido, 1, 0, 0, iRecibo);

            return Json(LSelloSat);
        }

        // consulta archivo recibo en la tabla TsellosSat;
        [HttpPost]
        public JsonResult FileRecibos(int Anio, int Tipoperiodo, int Perido, int iRecibo, int IdEmpresas, int EmpleId)
        {
            List<TsellosBean> Archi = new List<TsellosBean>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            Archi = Dao.sp_Recibos_Retrieve_TsellosSat(IdEmpresas, EmpleId, Anio, Tipoperiodo, Perido, iRecibo);
            string path = Server.MapPath("Archivos\\Temporal\\");
            path = path.Replace("\\Empleados", "");
            string Nombarch = "";
            if (iRecibo == 1)
            {
                Nombarch = EmpleId + "Recibosimple" + DateTime.Now.ToString();
                Nombarch = Nombarch.Replace(".", "");
                Nombarch = Nombarch.Replace("/", "");
                Nombarch = Nombarch.Replace(":", "");
                Nombarch = Nombarch.Replace(" ", "");
                Nombarch = Nombarch + ".pdf";
            }
            if (iRecibo == 2)
            {
                Nombarch = EmpleId + "ReciboFiscal" + DateTime.Now.ToString();
                Nombarch = Nombarch.Replace(".", "");
                Nombarch = Nombarch.Replace("/", "");
                Nombarch = Nombarch.Replace(":", "");
                Nombarch = Nombarch.Replace(" ", "");
                Nombarch = Nombarch + ".pdf";
            }

            if (iRecibo == 3)
            {
                Nombarch = EmpleId + "Recibo2" + DateTime.Now.ToString();
                Nombarch = Nombarch.Replace(".", "");
                Nombarch = Nombarch.Replace("/", "");
                Nombarch = Nombarch.Replace(":", "");
                Nombarch = Nombarch.Replace(" ", "");
                Nombarch = Nombarch + ".pdf";
            }


            string pathDire = path + "\\" + Nombarch;
            if (!System.IO.Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (iRecibo == 1)
            {

                string Pathini = Archi[0].sURreciboSimple;
                if (System.IO.File.Exists(Pathini))
                {
                    System.IO.File.Copy(Pathini, pathDire, true);
                    Archi[0].sURTemp = pathDire;
                    Archi[0].sURreciboSimple = Nombarch;
                }


            }

            if (iRecibo == 2)
            {

                string Pathini = Archi[0].sURreciboFiscal;
                if (System.IO.File.Exists(Pathini))
                {
                    System.IO.File.Copy(Pathini, pathDire, true);
                    Archi[0].sURTemp = pathDire;
                    Archi[0].sURreciboFiscal = Nombarch;
                }

            }

            if (iRecibo == 3)
            {
                string Pathini = Archi[0].sURrecibo2;
                if (System.IO.File.Exists(Pathini))
                {
                    System.IO.File.Copy(Pathini, pathDire, true);
                    Archi[0].sURTemp = pathDire;
                    Archi[0].sURrecibo2 = Nombarch;
                }
            }

            return Json(Archi);
        }



        // borrar archivo temporal ;
        [HttpPost]
        public JsonResult PathArcDelete(string Path)
        {

            String pathLog = Server.MapPath("~/Content/");
            String messageError = "none";
            List<TsellosBean> Archi = new List<TsellosBean>();
            ListEmpleadosDao Dao = new ListEmpleadosDao();
            try
            {
                if (System.IO.File.Exists(Path))
                {
                    System.IO.File.Delete(Path);

                }
            }
            catch (Exception exc)
            {
                messageError = exc.Message.ToString();
                if (!System.IO.Directory.Exists(pathLog + "LOGS"))
                {
                    Directory.CreateDirectory(pathLog + "LOGS");
                }
                using (StreamWriter file = new StreamWriter(pathLog + "LOGS" + @"\\" + "LogXMLNOMINA" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt"))
                {
                    file.Write(messageError + "\n");
                    file.Close();
                }
            }
            return Json(Archi);
        }

    }
}