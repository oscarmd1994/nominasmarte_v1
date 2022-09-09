using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;
using Payroll.Models.Beans;
using Payroll.Models.Daos;
using Payroll.Models.Utilerias;

namespace Payroll.Controllers
{
    public class LayoutsController : Controller
    {

        public FileLayout SaveFileLayout(HttpPostedFileBase fileUpload, string typeFile)
        {
            Boolean flag          = false;
            String  messageError  = "none";
            string nameFolderSave = "LayoutsCarga";
            // Ruta Produccion
            //string pathSaveFile = "D:/ArchivosIPSNet/Layouts/Produccion/";
            // Ruta desarrollo
            //string pathSaveFile = "D:/ArchivosIPSNet/Layouts/Desarrollo/";
            // Ruta local
            string pathSaveFile = Server.MapPath("~/Content/");
            string nameFolderType = "";
            string nameFileType   = "";
            string userSession    = Session["sUsuario"].ToString();
            if (typeFile == "posts") {
                nameFolderType = "CambioPuestos";
            } else if (typeFile == "accountBank") {
                nameFolderType = "CambioCuentas";
            } else if (typeFile == "dataPayroll") {
                nameFolderType = "DatosNomina";
            }
            nameFileType = nameFolderType + DateTime.Now.ToString("yyyyMMdd") + "U" + userSession + ".xlsx";
            string pathComplete   = pathSaveFile + nameFolderSave + @"\\" + nameFolderType;
            ValidacionesLayout validaciones = new ValidacionesLayout();
            FileLayout file = new FileLayout();
            try {
                if (!Directory.Exists(pathComplete)) {
                    Directory.CreateDirectory(pathComplete);
                }
                if (System.IO.File.Exists(pathComplete + @"\\" + nameFileType)) {
                    System.IO.File.Delete(pathComplete + @"\\" + nameFileType);
                }
                fileUpload.SaveAs(pathComplete + @"\\" + nameFileType);
                if (System.IO.File.Exists(pathComplete + @"\\" + nameFileType)) {
                    flag = true;
                }
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            file.sRuta    = pathComplete + @"\\" + nameFileType;
            file.sNombre  = fileUpload.FileName;
            file.bBandera = flag;
            file.sMensaje = messageError;
            return file;
        }

        [HttpPost]
        public JsonResult CheckFileLayoutPosts(HttpPostedFileBase fileUpload, string typeFile, int continueLoad) 
        {
            Boolean flag = false;
            Boolean flagValidationData = false;
            String  messageError = "none";
            ValidacionesLayout validations = new ValidacionesLayout();
            FileLayout fileLayout = new FileLayout();
            LayoutsDao layoutsDao = new LayoutsDao();
            FileLayoutValidations layoutValidations = new FileLayoutValidations();
            List<LayoutResult> listLayouts = new List<LayoutResult>();
            Boolean flagErrors = false;
            string userSession = Session["sUsuario"].ToString();
            string pathCompleteLog = "";
            int registersError = 0;
            int registersSuccs = 0;
            int quantityRegisters = 0;
            try {
                if (continueLoad == 0) {
                    fileLayout = SaveFileLayout(fileUpload, typeFile);
                    if (fileLayout.bBandera) {
                        layoutValidations = validations.ValidationsFilePosts(fileLayout);
                        if (layoutValidations.bBanderaHoja) {
                            if (layoutValidations.iErrores == 0) {
                                // TODO: Realiza el proceso de actualización
                                flagValidationData = true;
                                using (var stream = System.IO.File.Open(fileLayout.sRuta, FileMode.Open, FileAccess.Read)) {
                                    using (var reader = ExcelReaderFactory.CreateReader(stream)) {
                                        var result = reader.AsDataSet();
                                        DataTable table   = result.Tables[0];
                                        DataRow dataRow   = table.Rows[0];
                                        quantityRegisters = Convert.ToInt32(dataRow[0]);
                                        int registerActually = 0;
                                        foreach (DataRow data in table.Rows) {
                                            if (data[1].ToString().Trim() != "DATAPOSTS") {
                                                registerActually += 1;
                                                if ((quantityRegisters + 1) == registerActually) {
                                                    break;
                                                }
                                                LayoutResult layout = new LayoutResult();
                                                int business   = Convert.ToInt32(data[2].ToString().Trim());
                                                int payroll    = Convert.ToInt32(data[3].ToString().Trim());
                                                string newPost = validations.ClearStringWordsCharacteres(data[4].ToString().Trim());
                                                string nivelJe = validations.ClearStringWordsCharacteres(data[5].ToString().Trim());
                                                layout         = layoutsDao.sp_Actualiza_Puestos_Empleados(business, payroll, newPost, nivelJe);
                                                if (layout.iBandera == 0) {
                                                    flagErrors = true;
                                                    registersError += 1;
                                                } else {
                                                    registersSuccs += 1;
                                                }
                                                listLayouts.Add(layout);
                                            }
                                        }
                                        if (flagErrors) {
                                            string pathLog = Server.MapPath("~/Content/LayoutsLog/");
                                            if (!Directory.Exists(pathLog)) {
                                                Directory.CreateDirectory(pathLog);
                                            }
                                            using (StreamWriter fileLog = new StreamWriter(pathLog + "LOG_LAYOUT_POSTS.txt", false, Encoding.UTF8)) {
                                                foreach (LayoutResult data in listLayouts) {
                                                    if (data.iBandera == 0) {
                                                        fileLog.WriteLine("[*] Empresa: " + data.iEmpresa.ToString() + ". Nomina: " 
                                                            + data.iNomina.ToString() + ". Mensaje: " + data.sMensaje +
                                                            ". Usuario: " + userSession +". |-> " +
                                                            " Stored Procedure: " + data.sStoredProcedure);
                                                    }
                                                }
                                                fileLog.Close();
                                            }
                                        }
                                        int keyUser = Convert.ToInt32(Session["iIdUsuario"]);
                                        Boolean saveHistory = layoutsDao.sp_Guarda_Historia_Layouts(keyUser, typeFile, fileLayout.sRuta, fileLayout.sNombre);
                                    }
                                }
                            }
                            flag = true;
                        } else {
                            return Json(new {
                                Bandera = false, GuardaArchivo = fileLayout.bBandera,
                                ValidacionHoja = false, ValidacionesHoja = layoutValidations
                            });
                        }
                    } else {
                        return Json(new { 
                            Bandera = false,
                            MensajeError = "Ocurrio un problema al guardar el archivo", GuardaArchivo = fileLayout.bBandera
                        });
                    }
                } else {
                    // TODO: PROCESAR REGISTROS DEL ARCHIVO EXCEL
                }
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, GuardaArchivo = fileLayout.bBandera, Validaciones = layoutValidations, ValidacionHoja = layoutValidations.bBanderaHoja, ValidacionDatos = flagValidationData, BanderaError = flagErrors, Errores = registersError, Correctos = registersSuccs, Cantidad = quantityRegisters, Archivo = "LOG_LAYOUT_POSTS.txt" });
        }

        [HttpPost]
        public JsonResult CheckFileLayoutAccountBank(HttpPostedFileBase fileUpload, string typeFile, int continueLoad)
        {
            Boolean flag = false;
            Boolean flagValidationData = false;
            String messageError = "none";
            ValidacionesLayout validations = new ValidacionesLayout();
            FileLayout fileLayout = new FileLayout();
            LayoutsDao layoutsDao = new LayoutsDao();
            FileLayoutValidations layoutValidations = new FileLayoutValidations();
            List<LayoutResult> listLayouts = new List<LayoutResult>();
            Boolean flagErrors = false;
            string userSession = Session["sUsuario"].ToString();
            int registersError = 0;
            int registersSuccs = 0;
            int quantityRegisters = 0;
            try {
                int keyUser = Convert.ToInt32(Session["iIdUsuario"]);
                if (continueLoad == 0) {
                    fileLayout = SaveFileLayout(fileUpload, typeFile);
                    if (fileLayout.bBandera) {
                        layoutValidations = validations.ValidationsFileAccountBank(fileLayout);
                        if (layoutValidations.bBanderaHoja) {
                            if (layoutValidations.iErrores == 0) {
                                // TODO: Realiza el proceso de actualización
                                flagValidationData = true;
                                using (var stream = System.IO.File.Open(fileLayout.sRuta, FileMode.Open, FileAccess.Read)) {
                                    using (var reader = ExcelReaderFactory.CreateReader(stream)) {
                                        var result      = reader.AsDataSet();
                                        DataTable table = result.Tables[0];
                                        DataRow dataRow = table.Rows[0];
                                        quantityRegisters    = Convert.ToInt32(dataRow[0]);
                                        int registerActually = 0;
                                        foreach (DataRow data in table.Rows) {
                                            if (data[1].ToString().Trim() != "DATAACCOUNT") {
                                                registerActually += 1;
                                                if ((quantityRegisters + 1) == registerActually) {
                                                    break;
                                                }
                                                LayoutResult layout = new LayoutResult();
                                                int business = Convert.ToInt32(data[2].ToString().Trim());
                                                int payroll  = Convert.ToInt32(data[3].ToString().Trim());
                                                int bank     = Convert.ToInt32(data[4].ToString().Trim());
                                                string account = data[5].ToString().Trim().Replace("Cta_", "").Replace("cta_", "");
                                                layout = layoutsDao.sp_Actualiza_Datos_Bancarios(business, payroll, bank, account, keyUser);
                                                if (layout.iBandera == 0) {
                                                    flagErrors = true;
                                                    registersError += 1;
                                                } else {
                                                    registersSuccs += 1;
                                                }
                                                listLayouts.Add(layout);
                                            }
                                        }
                                        if (flagErrors) {
                                            string pathLog = Server.MapPath("~/Content/LayoutsLog/");
                                            if (!Directory.Exists(pathLog)) {
                                                Directory.CreateDirectory(pathLog);
                                            }
                                            using (StreamWriter fileLog = new StreamWriter(pathLog + "LOG_LAYOUT_ACCOUNT.txt", false, Encoding.UTF8)) {
                                                foreach (LayoutResult data in listLayouts) {
                                                    if (data.iBandera == 0) {
                                                        fileLog.WriteLine("[*] Empresa: " + data.iEmpresa.ToString() + ". Nomina: "
                                                            + data.iNomina.ToString() + ". Mensaje: " + data.sMensaje +
                                                            ". Usuario: " + userSession + ". |-> " +
                                                            " Stored Procedure: " + data.sStoredProcedure);
                                                    }
                                                }
                                                fileLog.Close();
                                            }
                                        }
                                        Boolean saveHistory = layoutsDao.sp_Guarda_Historia_Layouts(keyUser, typeFile, fileLayout.sRuta, fileLayout.sNombre);
                                    }
                                }
                            }
                            flag = true;
                        } else {
                            return Json(new {
                                Bandera = false,
                                GuardaArchivo = fileLayout.bBandera,
                                ValidacionHoja = false,
                                ValidacionesHoja = layoutValidations
                            });
                        }
                    } else {
                        return Json(new {
                            Bandera = false,
                            MensajeError = "Ocurrio un problema al guardar el archivo",
                            GuardaArchivo = fileLayout.bBandera
                        });
                    }
                } else {

                }
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, GuardaArchivo = fileLayout.bBandera, Validaciones = layoutValidations, ValidacionHoja = layoutValidations.bBanderaHoja, ValidacionDatos = flagValidationData, BanderaError = flagErrors, Errores = registersError, Correctos = registersSuccs, Cantidad = quantityRegisters, Archivo = "LOG_LAYOUT_ACCOUNT.txt" });
        }

        [HttpPost]
        public JsonResult CheckFileLayoutDataPayroll(HttpPostedFileBase fileUpload, string typeFile, int continueLoad)
        {
            Boolean flag = false;
            Boolean flagValidationData = false;
            String messageError = "none";
            ValidacionesLayout validations = new ValidacionesLayout();
            FileLayout fileLayout = new FileLayout();
            LayoutsDao layoutsDao = new LayoutsDao();
            FileLayoutValidations layoutValidations = new FileLayoutValidations();
            List<LayoutResult> listLayouts = new List<LayoutResult>();
            Boolean flagErrors = false;
            string userSession = Session["sUsuario"].ToString();
            int registersError = 0;
            int registersSuccs = 0;
            int quantityRegisters = 0;
            try {
                int keyUser = Convert.ToInt32(Session["iIdUsuario"]);
                if (continueLoad == 0) {
                    fileLayout = SaveFileLayout(fileUpload, typeFile);
                    if (fileLayout.bBandera) {
                        layoutValidations = validations.ValidationsFileDataPayroll(fileLayout);
                        if (layoutValidations.bBanderaHoja) {
                            if (layoutValidations.iErrores == 0) {
                                flagValidationData = true;
                                using (var stream = System.IO.File.Open(fileLayout.sRuta, FileMode.Open, FileAccess.Read)) {
                                    using (var reader = ExcelReaderFactory.CreateReader(stream)) {
                                        var result      = reader.AsDataSet();
                                        DataTable table = result.Tables[0];
                                        DataRow dataRow = table.Rows[0];
                                        quantityRegisters    = Convert.ToInt32(dataRow[0]);
                                        string code = dataRow[2].ToString().Trim();
                                        int registerActually = 0;
                                        foreach (DataRow data in table.Rows) {
                                            if (data[1].ToString().Trim() != "DATAPAYROLL") {
                                                registerActually += 1;
                                                if ((quantityRegisters + 1) == registerActually) {
                                                    break;
                                                }
                                                LayoutResult layout = new LayoutResult();
                                                int business = Convert.ToInt32(data[3].ToString().Trim());
                                                int payroll  = Convert.ToInt32(data[4].ToString().Trim());
                                                string value = "";
                                                if (code == "PREMIOS") {
                                                    value = data[5].ToString();
                                                } else if (code == "RETROACTIVO") {
                                                    value = data[6].ToString();
                                                } else if (code == "SDI") {
                                                    value = data[7].ToString();
                                                } else if (code == "TRANSPORTE") {
                                                    value = data[8].ToString();
                                                } else if (code == "DIFERENCIA") {
                                                    value = data[9].ToString();
                                                } else if (code == "COMPLEMENTO") {
                                                    value = data[10].ToString();
                                                } else if (code == "EMPRESAORIGEN") {
                                                    value = data[11].ToString();
                                                } else if (code == "SALARIO") {
                                                    value = data[12].ToString();
                                                }
                                                layout = layoutsDao.sp_Actualiza_Datos_Diversos_Nominas(business, payroll, value, code, keyUser);
                                                if (layout.iBandera == 0) {
                                                    flagErrors = true;
                                                    registersError += 1;
                                                } else {
                                                    registersSuccs += 1;
                                                }
                                                listLayouts.Add(layout);
                                            }
                                        }
                                        if (flagErrors) {
                                            string pathLog = Server.MapPath("~/Content/LayoutsLog/");
                                            if (!Directory.Exists(pathLog)) {
                                                Directory.CreateDirectory(pathLog);
                                            }
                                            using (StreamWriter fileLog = new StreamWriter(pathLog + "LOG_LAYOUT_DATAPAYROLL.txt", false, Encoding.UTF8)) {
                                                foreach (LayoutResult data in listLayouts) {
                                                    if (data.iBandera == 0) {
                                                        fileLog.WriteLine("[*] Empresa: " + data.iEmpresa.ToString() + ". Nomina: "
                                                            + data.iNomina.ToString() + ". Mensaje: " + data.sMensaje +
                                                            ". Usuario: " + userSession + ". |-> " +
                                                            " Stored Procedure: " + data.sStoredProcedure);
                                                    }
                                                }
                                                fileLog.Close();
                                            }
                                        }
                                        Boolean saveHistory = layoutsDao.sp_Guarda_Historia_Layouts(keyUser, typeFile, fileLayout.sRuta, fileLayout.sNombre);
                                    }
                                }
                            }
                            flag = true;
                        } else {
                            return Json(new {
                                Bandera = false,
                                GuardaArchivo = fileLayout.bBandera,
                                ValidacionHoja = false,
                                ValidacionesHoja = layoutValidations
                            });
                        }
                    } else {
                        return Json(new {
                            Bandera = false,
                            MensajeError = "Ocurrio un problema al guardar el archivo",
                            GuardaArchivo = fileLayout.bBandera
                        });
                    }
                }
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, GuardaArchivo = fileLayout.bBandera, Validaciones = layoutValidations, ValidacionHoja = layoutValidations.bBanderaHoja, ValidacionDatos = flagValidationData, BanderaError = flagErrors, Errores = registersError, Correctos = registersSuccs, Cantidad = quantityRegisters, Archivo = "LOG_LAYOUT_DATAPAYROLL.txt" });
        }

    }

}