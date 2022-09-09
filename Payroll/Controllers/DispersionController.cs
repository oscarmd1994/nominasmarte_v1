﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Payroll.Models.Beans;
using Payroll.Models.Daos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web.Mvc;
using static iTextSharp.text.Font;

namespace Payroll.Controllers
{
    public class DispersionController : Controller
    {
        // GET: Dispersion

        [HttpPost]
        public JsonResult UserLogin()
        {
            string messageError = "";
            string userLogin    = "";
            int keyBusiness = 0;
            try {
                userLogin   = Session["sUsuario"].ToString();
                keyBusiness = Convert.ToInt32(Session["IdEmpresa"]);
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            return Json(new { User = userLogin, MensajeError = messageError, Empresa = keyBusiness });
        }

        // Muestra la informacion del periodo actual de la nomina
        [HttpPost]
        public JsonResult LoadInfoPeriodPayroll(string yearAct)
        {
            Boolean bandera     = false;
            String mensajeError = "none";
            LoadTypePeriodPayrollBean periodoBean = new LoadTypePeriodPayrollBean();
            LoadTypePeriodPayrollDaoD periodoDaoD = new LoadTypePeriodPayrollDaoD();
            try
            {
                int IdEmpresa = int.Parse(Session["IdEmpresa"].ToString());
                periodoBean   = periodoDaoD.sp_Load_Info_Periodo_Empr(IdEmpresa, Convert.ToInt32(yearAct.ToString().Trim()));
                bandera       = (periodoBean.sMensaje == "success") ? true : false;
            } catch (Exception exc) {
                mensajeError = exc.Message.ToString();
            }
            return Json(new { Bandera = bandera, InfoPeriodo = periodoBean, MensajeError = mensajeError });
        }

        [HttpPost]
        public JsonResult LoadPeriodsRetainedPayroll(int Anio)
        {
            Boolean flag = false;
            String messageError = "none";
            PayrollRetainedEmployeesDaoD retenidas = new PayrollRetainedEmployeesDaoD();
            LoadTypePeriodPayrollBean periodBean   = new LoadTypePeriodPayrollBean();
            LoadTypePeriodPayrollDaoD periodDaoD   = new LoadTypePeriodPayrollDaoD();
            List<int> periodos = new List<int>();
            try {
                int IdEmpresa = Convert.ToInt32(Session["IdEmpresa"]);
                periodos = retenidas.sp_Periodos_Retenidos_A_Empleados(IdEmpresa, DateTime.Now.Year);
                periodBean = periodDaoD.sp_Load_Info_Periodo_Empr(IdEmpresa, Convert.ToInt32(DateTime.Now.Year));
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            } 
            return Json(new { Bandera = flag, MensajeError = messageError, Periodos = periodos, InfoPeriodo = periodBean });
        }

        // Muestra los datos de los empleados con nomina retenida
        [HttpPost]
        public JsonResult PayrollRetainedEmployees()
        {
            Boolean flag = false;
            String messageError = "none";
            List<PayrollRetainedEmployeesBean> payRetainedBean = new List<PayrollRetainedEmployeesBean>();
            PayrollRetainedEmployeesDaoD payRetainedDaoD = new PayrollRetainedEmployeesDaoD();
            try {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                payRetainedBean = payRetainedDaoD.sp_Retrieve_NominasRetenidas(keyBusiness);
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            var data = new { data = payRetainedBean };
            return Json(data);
        }

        // Muestra los empleados de la empresa a retener nomina
        [HttpPost]
        public JsonResult SearchEmployeesRetainedPayroll(string searchEmployee, string filter)
        {
            Boolean flag = false;
            String messageError = "none";
            Char[] charactersClear = { ' ', '*', '.', '<', '>', '=', '?', '|', '(', ')', '!', '%', '#', '@', '$', '/', '^' };
            string searchClear = searchEmployee.ToString().Trim(charactersClear);
            List<SearchEmployeePayRetainedBean> employeePayRetBean = new List<SearchEmployeePayRetainedBean>();
            SearchEmployeePayRetainedDaoD employeePayRetDaoD = new SearchEmployeePayRetainedDaoD();
            try {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                employeePayRetBean = employeePayRetDaoD.sp_SearchEmploye_Ret_Nomina(keyBusiness, searchClear, filter);
                flag = (employeePayRetBean.Count > 0) ? true : false;
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            return Json(employeePayRetBean);
        }

        // Carga el periodo actual
        [HttpPost]
        public JsonResult LoadTypePeriod(int year, int typePeriod)
        {
            Boolean flag = false;
            String messageError = "none";
            LoadTypePeriodBean loadTypePerBean = new LoadTypePeriodBean();
            LoadTypePeriodDaoD loadTypePerDaoD = new LoadTypePeriodDaoD();
            LoadTypePeriodPayrollBean periodBean = new LoadTypePeriodPayrollBean();
            LoadTypePeriodPayrollDaoD periodDaoD = new LoadTypePeriodPayrollDaoD();
            try {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                periodBean = periodDaoD.sp_Load_Info_Periodo_Empr(keyBusiness, Convert.ToInt32(DateTime.Now.Year));
                loadTypePerBean = loadTypePerDaoD.sp_Load_Type_Period_Empresa(keyBusiness, year, periodBean.iTipoPeriodo);
                flag = (loadTypePerBean.sMensaje == "success") ? true : false;
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = loadTypePerBean });
        }

        // Guarda la retencion de nomina del empleado
        [HttpPost]
        public JsonResult RetainedPayrollEmployee(int keyEmployee, int typePeriod, int periodPayroll, int yearRetained, string descriptionRetained)
        {
            Boolean flag = false;
            String messageError = "none";
            PayrollRetainedEmployeesBean retPayEmployeeBean = new PayrollRetainedEmployeesBean();
            PayrollRetainedEmployeesDaoD retPayEmployeeDaoD = new PayrollRetainedEmployeesDaoD();
            try {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                int keyUser = Convert.ToInt32(Session["iIdUsuario"].ToString());
                retPayEmployeeBean = retPayEmployeeDaoD.sp_Insert_Empleado_Retenida_Nomina(keyBusiness, keyEmployee, typePeriod, periodPayroll, yearRetained, descriptionRetained, keyUser);
                flag = (retPayEmployeeBean.sMensaje == "success") ? true : false;
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        // Remueve la nomina retenida al empleado
        [HttpPost]
        public JsonResult RemovePayrollRetainedEmployee(int keyPayrollRetained)
        {
            Boolean flag = false;
            String messageError = "none";
            PayrollRetainedEmployeesBean retPayEmployeeBean = new PayrollRetainedEmployeesBean();
            PayrollRetainedEmployeesDaoD retPayEmployeeDaoD = new PayrollRetainedEmployeesDaoD();
            try {
                retPayEmployeeBean = retPayEmployeeDaoD.sp_Update_Remove_Nomina_Retenida(keyPayrollRetained);
                flag = (retPayEmployeeBean.sMensaje == "success") ? true : false;
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        // Crea los folders necesarios
        public Boolean CreateFoldersToDeploy()
        {
            Boolean flag = false;
            string directoryTxt   = Server.MapPath("/DispersionTXT").ToString();
            string directoryZip   = Server.MapPath("/DispersionZIP").ToString();
            string nameFolderYear = DateTime.Now.Year.ToString();
            string nameFolderNom  = "NOMINAS";
            string nameFolderInt  = "INTERBANCARIOS";
            try {
                if (!System.IO.Directory.Exists(directoryTxt + @"\\" + nameFolderYear)) {
                    System.IO.Directory.CreateDirectory(directoryTxt + @"\\" + nameFolderYear);
                }
                if (!System.IO.Directory.Exists(directoryZip + @"\\" + nameFolderYear)) {
                    System.IO.Directory.CreateDirectory(directoryZip + @"\\" + nameFolderYear);
                }
                if (!System.IO.Directory.Exists(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderNom)) {
                    System.IO.Directory.CreateDirectory(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderNom);
                }
                if (!System.IO.Directory.Exists(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderInt)) {
                    System.IO.Directory.CreateDirectory(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderInt);
                }
                if (!System.IO.Directory.Exists(directoryZip + @"\\" + nameFolderYear + @"\\" + nameFolderNom)) {
                    System.IO.Directory.CreateDirectory(directoryZip + @"\\" + nameFolderYear + @"\\" + nameFolderNom);
                }
                if (!System.IO.Directory.Exists(directoryZip + @"\\" + nameFolderYear + @"\\" + nameFolderInt)) {
                    System.IO.Directory.CreateDirectory(directoryZip + @"\\" + nameFolderYear + @"\\" + nameFolderInt);
                }
                flag = true;
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            }
            return flag;
        }

        // Muestra informacion al desplegar la dispersion
        [HttpPost]
        public JsonResult ToDeployDispersion(int yearDispersion, int typePeriodDisp, int periodDispersion, string dateDispersion, string type)
        {
            Boolean flag1 = false;
            Boolean flag2 = false;
            String messageError = "none";
            List<DataDepositsBankingBean> dataDepositsRetained = new List<DataDepositsBankingBean>();
            List<DataDepositsBankingBean> daDepBankingBean = new List<DataDepositsBankingBean>();
            List<DataDepositsBankingBean> depositosFinal   = new List<DataDepositsBankingBean>();
            DataDispersionBusiness daDiBusinessDaoD        = new DataDispersionBusiness();
            List<BankDetailsBean> bankDetailsBean          = new List<BankDetailsBean>();
            ReportesDao reportDao = new ReportesDao();
            double resultResta = 0;
            int banderaEmpresa = 0;
            try {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                banderaEmpresa = reportDao.sp_Comprueba_Empresa_Existencia_Grupo(keyBusiness);
                if (banderaEmpresa == 1) {
                    daDepBankingBean = daDiBusinessDaoD.sp_Obtiene_Depositos_Bancarios(keyBusiness, yearDispersion, typePeriodDisp, periodDispersion, type);
                    if (daDepBankingBean.Count > 0) {
                        flag1 = true;
                        bankDetailsBean = daDiBusinessDaoD.sp_Datos_Banco(daDepBankingBean);
                        flag2 = (bankDetailsBean.Count > 0) ? true : false;
                        foreach (DataDepositsBankingBean data in daDepBankingBean) {
                            resultResta = daDiBusinessDaoD.sp_Datos_Totales_Resta_Importe_Bancos_Dispersion(keyBusiness, periodDispersion, typePeriodDisp, data.iIdBanco, yearDispersion);
                            double resultado = data.dImporteSF - resultResta;
                            depositosFinal.Add(new DataDepositsBankingBean { iIdBanco = data.iIdBanco, iDepositos = data.iDepositos, iIdEmpresa = data.iIdEmpresa, iIdRenglon = data.iIdRenglon, sBanco = data.sBanco, sImporte = string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal((resultado)))});
                        }
                    }
                } else {
                    dataDepositsRetained = daDiBusinessDaoD.sp_Depositos_Nominas_Retenidas(keyBusiness, yearDispersion, typePeriodDisp, periodDispersion, type);
                    daDepBankingBean = daDiBusinessDaoD.sp_Obtiene_Depositos_Bancarios(keyBusiness, yearDispersion, typePeriodDisp, periodDispersion, type);
                    if (daDepBankingBean.Count > 0) {
                        flag1 = true;
                        bankDetailsBean = daDiBusinessDaoD.sp_Datos_Banco(daDepBankingBean);
                        flag2 = (bankDetailsBean.Count > 0) ? true : false;
                    }
                }
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            if (banderaEmpresa == 1) {
                return Json(new { BanderaDispersion = flag1, BanderaBancos = flag2, MensajeError = messageError, DatosDepositos = depositosFinal, DatosBancos = bankDetailsBean, Retenidas = dataDepositsRetained });
            } else {
                return Json(new { BanderaDispersion = flag1, BanderaBancos = flag2, MensajeError = messageError, DatosDepositos = daDepBankingBean, DatosBancos = bankDetailsBean, Retenidas = dataDepositsRetained });
            }
        }

        [HttpPost]
        public JsonResult ToDeployDispersionSpecial(int keyGroup, int year, int typePeriod, int period, string date, string type)
        {
            Boolean flag1 = false;
            Boolean flag2 = false;
            String  messageError = "none";
            List<DataDepositsBankingBean> dataDeposits = new List<DataDepositsBankingBean>();
            DataDispersionBusiness dataDispersion      = new DataDispersionBusiness();
            List<BankDetailsBean> bankDetails          = new List<BankDetailsBean>();
            try {
                dataDeposits = dataDispersion.sp_Obtiene_Depositos_Bancarios_Especial(keyGroup, year, typePeriod, period, "");
                if (dataDeposits.Count > 0) {
                    flag1       = true;
                    bankDetails = dataDispersion.sp_Datos_Banco(dataDeposits);
                    flag2       = (bankDetails.Count > 0) ? true : false;
                }
            } catch (Exception exc) {
                flag1        = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { BanderaDispersion = flag1, BanderaBancos = flag2, MensajeError = messageError, DatosDepositos = dataDeposits, DatosBancos = bankDetails });
        }

        // Valida existencia de banco interbancario
        [HttpPost]
        public JsonResult ValidateBankInterbank(int type)
        {
            Boolean flag = false;
            String  messageError = "none";
            LoadDataTableBean loadDataTable    = new LoadDataTableBean();
            LoadDataTableDaoD loadDataTableDao = new LoadDataTableDaoD();
            try {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                loadDataTable = loadDataTableDao.sp_Valida_Existencia_Banco_Interbancario(keyBusiness, type);
                if (loadDataTable.sMensaje == "SUCCESS") {
                    flag = true;
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        // Procesa los depositos de nomina
        [HttpPost]
        public JsonResult ProcessDepositsPayroll(int yearPeriod, int numberPeriod, int typePeriod, string dateDeposits, int mirror, int type, string dateDisC)
        {
            DateTime test;
            if (dateDisC != "") {
                test = DateTime.Parse(dateDisC.ToString());
            }
            Boolean flag            = false;
            Boolean flagMirror      = false;
            Boolean flagProsecutors = false;
            String  messageError = "none";
            DatosEmpresaBeanDispersion datosEmpresaBeanDispersion = new DatosEmpresaBeanDispersion();
            DataDispersionBusiness dataDispersionBusiness         = new DataDispersionBusiness();
            string nameFolder           = "";
            string nameFileError        = "";
            DateTime dateGeneration     = DateTime.Now;
            string dateGenerationFormat = dateGeneration.ToString("MMddyyyy");
            string directoryZip   = Server.MapPath("/DispersionZIP").ToString();
            string directoryTxt   = Server.MapPath("/DispersionTXT").ToString();
            string nameFolderYear = DateTime.Now.Year.ToString();
            string msgEstatus     = ""; 
            string msgEstatusZip  = "";
            try {
                int keyBusiness   = int.Parse(Session["IdEmpresa"].ToString());
                int yearActually  = DateTime.Now.Year;
                int typeReceipt   = (yearPeriod == yearActually) ? 1 : 0;
                int invoiceId     = yearPeriod * 100000 + typePeriod * 10000 + numberPeriod * 10;
                int invoiceIdMirror        = yearPeriod * 100000 + typePeriod * 10000 + numberPeriod * 10 + 8;
                datosEmpresaBeanDispersion = dataDispersionBusiness.sp_Datos_Empresa_Dispersion(keyBusiness, type);
                nameFolder = "DEPOSITOS_" + "E" + keyBusiness.ToString() + "P" + numberPeriod.ToString() + "A" + dateGeneration.ToString("yyyy").Substring(2, 2);
                if (System.IO.File.Exists(directoryZip + @"\\" + nameFolderYear + @"\\" + "NOMINAS" + @"\" + nameFolder  + ".zip")) {
                    System.IO.File.Delete(directoryZip + @"\\" + nameFolderYear + @"\\" + "NOMINAS" + @"\" + nameFolder + ".zip");
                }
                if (Directory.Exists(directoryTxt + @"\\" + nameFolderYear + @"\\" + "NOMINAS" + @"\" + nameFolder)) {
                    Directory.Delete(directoryTxt + @"\\" + nameFolderYear + @"\\" + "NOMINAS" + @"\" + nameFolder, recursive: true);
                }
                flagProsecutors = ProcessDepositsProsecutors(keyBusiness, invoiceId, typeReceipt, dateDeposits, yearPeriod, typePeriod, numberPeriod, datosEmpresaBeanDispersion.sNombreEmpresa, datosEmpresaBeanDispersion.sRfc, dateDisC);
                if (mirror == 1) {
                    flagMirror = ProcessDepositsMirror(keyBusiness, invoiceId, typeReceipt, dateDeposits, yearPeriod, typePeriod, numberPeriod, datosEmpresaBeanDispersion.sNombreEmpresa, datosEmpresaBeanDispersion.sRfc, dateDisC);
                }
                if (flagProsecutors == true || flagMirror == true) {
                    flag = true;
                }
                if (flag) {
                    // CREACCION DEL ZIP CON LOS ARCHIVOS
                    FileStream stream  = new FileStream(directoryZip + @"\\" + nameFolderYear + @"\\" + "NOMINAS" + @"\" + nameFolder + ".zip", FileMode.OpenOrCreate);
                    ZipArchive fileZip = new ZipArchive(stream, ZipArchiveMode.Create);
                    System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(directoryTxt + @"\\" + nameFolderYear + @"\\" + "NOMINAS" + @"\\" + nameFolder);
                    FileInfo[] sourceFiles = directoryInfo.GetFiles();
                    foreach (FileInfo file in sourceFiles) {
                        Stream sourceStream   = file.OpenRead();
                        ZipArchiveEntry entry = fileZip.CreateEntry(file.Name);
                        Stream zipStream      = entry.Open();
                        sourceStream.CopyTo(zipStream);
                        zipStream.Close();
                        sourceStream.Close();
                    }
                    ZipArchiveEntry zEntrys;
                    fileZip.Dispose();
                    stream.Close();
                    try {
                        using (ZipArchive zipArchive = ZipFile.OpenRead(directoryZip + @"\\" + nameFolderYear + @"\\" + "NOMINAS" + @"\\" + nameFolder + ".zip")) {
                            foreach (ZipArchiveEntry archiveEntry in zipArchive.Entries) {
                                using (ZipArchive zipArchives = ZipFile.Open(directoryZip + @"\\" + nameFolderYear + @"\\" + "NOMINAS" + @"\\" + nameFolder + ".zip", ZipArchiveMode.Read)) {
                                    zEntrys       = zipArchives.GetEntry(archiveEntry.ToString());
                                    nameFileError = zEntrys.Name;
                                    using (StreamReader read = new StreamReader(zEntrys.Open())) {
                                        if (read.ReadLine().Length > 0) {
                                            msgEstatusZip = "filesuccess";
                                        } else {
                                            msgEstatusZip = "fileerror";
                                        }
                                    }
                                }
                            }
                        }
                    } catch (InvalidDataException ide) {
                        Console.WriteLine(ide.Message.ToString() + " En el archivo : " + nameFileError);
                        msgEstatus = "fileError";
                    } catch (Exception exc) {
                        msgEstatus = exc.Message.ToString();
                    }
                    if (System.IO.File.Exists(directoryZip + @"\\" + nameFolderYear + @"\\" + "NOMINAS" + @"\\" + nameFolder + ".zip")) {
                        msgEstatus = "success";
                    } else {
                        msgEstatus = "failed";
                    }
                }
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Zip = nameFolder, EstadoZip = msgEstatusZip, Estado = msgEstatus, Anio = nameFolderYear });
        }

        public double Truncate(double value, int decimales)
        {
            double aux_value = Math.Pow(10, decimales);
            return (Math.Truncate(value * aux_value) / aux_value);
        }

        public Boolean ProcessDepositsProsecutors(int keyBusiness, int invoiceId, int typeReceipt, string dateDeposits, int yearPeriod, int typePeriod, int numberPeriod, string nameBusiness, string rfcBusiness, string dateDisC)
        {
            Boolean flag    = false; 
            Boolean notData = true;
            int k; 
            int bankResult   = 0;  
            string nameBankResult = "";
            string fileNamePDF    = "";
            string vFileName      = ""; 
            List<DatosDepositosBancariosBean>   listDatosDepositosBancariosBeans  = new List<DatosDepositosBancariosBean>();
            DataDispersionBusiness              dataDispersionBusiness            = new DataDispersionBusiness();
            List<DatosProcesaChequesNominaBean> listDatosProcesaChequesNominaBean = new List<DatosProcesaChequesNominaBean>();
            DatosCuentaClienteBancoEmpresaBean  datoCuentaClienteBancoEmpresaBean = new DatosCuentaClienteBancoEmpresaBean();
            DatosDispersionArchivosBanamex datosDispersionArchivosBanamex = new DatosDispersionArchivosBanamex();
            List<DataErrorAccountBank> dataErrors = new List<DataErrorAccountBank>();
            double renglon1481 = 0; 
            try {
                listDatosDepositosBancariosBeans = dataDispersionBusiness.sp_Procesa_Cheques_Total_Nomina(keyBusiness, typePeriod, numberPeriod, yearPeriod);
                if (listDatosDepositosBancariosBeans.Count > 0) {
                    notData = false;
                }
                if (notData) {
                    return flag;
                }
                listDatosProcesaChequesNominaBean = dataDispersionBusiness.sp_Procesa_Cheques_Nomina(keyBusiness, typePeriod, numberPeriod, yearPeriod);
                if (listDatosProcesaChequesNominaBean.Count == 0) {
                    return flag;
                }
                Boolean createFolders = CreateFoldersToDeploy();
                foreach (DatosProcesaChequesNominaBean data in listDatosProcesaChequesNominaBean) {
                    if (data.dImporte != 0) {
                        if (data.iIdBanco != bankResult) {
                            if (bankResult != 0) {
                                // Por ejecutar
                            }
                            bankResult     = data.iIdBanco;
                            nameBankResult = data.sBanco;
                            // Ejecutar un sp
                            datoCuentaClienteBancoEmpresaBean = dataDispersionBusiness.sp_Cuenta_Cliente_Banco_Empresa(keyBusiness, bankResult);
                            if (datoCuentaClienteBancoEmpresaBean.sMensaje == "SUCCESS") {
                                DateTime dateGeneration     = DateTime.Now;
                                string dateGenerationFormat = dateGeneration.ToString("MMddyyyy");
                                if (dateDisC != "") {
                                    dateGenerationFormat = DateTime.Parse(dateDisC.ToString()).ToString("MMddyyyy");
                                }
                                Random random = new Random();
                                //-----------
                                string nameFolder = "DEPOSITOS_" + "E" + keyBusiness.ToString() + "P" + numberPeriod.ToString() + "A" + dateGeneration.ToString("yyyy").Substring(2, 2);
                                    //+ "CD" + random.Next(1, 100000).ToString() + "U" + Session["iIdUsuario"].ToString();
                                //-----------
                                fileNamePDF  = "CHQ_NOMINAS_E" + keyBusiness.ToString() + "A" + string.Format("{0:00}", (yearPeriod % 100)) + "P" + string.Format("{0:00}", numberPeriod) + "_B" + bankResult.ToString() + ".PDF";
                                string directoryTxt = Server.MapPath("/DispersionTXT/" + DateTime.Now.Year.ToString()).ToString() + "/NOMINAS/";
                                if (!System.IO.Directory.Exists(directoryTxt + @"\\" + nameFolder)) {
                                    System.IO.Directory.CreateDirectory(directoryTxt + @"\\" + nameFolder);
                                }
                                // -------------------------
                                if (bankResult == 72) {
                                    vFileName = "NOMINAS_NI" + string.Format("{0:00000}", Convert.ToInt32(datoCuentaClienteBancoEmpresaBean.sNumeroCliente)) + "01";
                                } else {
                                    vFileName = "E" + string.Format("{0:000}", keyBusiness.ToString()) + "A" + yearPeriod + yearPeriod.ToString() + "P" + string.Format("{0:000}", numberPeriod.ToString()) + "_B" + bankResult.ToString();
                                }

                                // ARCHIVO TXT DISPERSION BANAMEX -> NOMINA -> OK -> LISTO
                                if (bankResult == 2) {
                                    DateTime dateC = dateGeneration;
                                    if (dateDisC != "") {
                                        dateC = DateTime.Parse(dateDisC.ToString());
                                    } 
                                    // ENCABEZADO  --> ARCHIVO OK
                                    string tipoRegistroBanamexE  = "1";
                                    string numeroClienteBanamexE = "000" + datoCuentaClienteBancoEmpresaBean.sNumeroCliente;
                                    string fechaBanamexE         = dateC.ToString("ddMM") + dateC.ToString("yyyy").Substring(2, 2);
                                    string valorFijoBanamex0     = "0001";
                                    string nombreEmpresaBanamex  = "";
                                    if (nameBusiness.Length > 35) {
                                        nombreEmpresaBanamex = nameBusiness.Substring(0, 35);
                                    } else {
                                        nombreEmpresaBanamex = nameBusiness;
                                    }
                                    int recorrido = 36 - nombreEmpresaBanamex.Length;
                                    for (var c = 0; c < recorrido; c++) {
                                        nombreEmpresaBanamex += " ";
                                    }
                                    string valorFijoBanamex1 = "NOMINA";
                                    string fillerBanamexE1   = " ";
                                    string fechaBanamexE1    = dateC.ToString("ddMMyyyy") + "     ";
                                    string valorFijoBanamex2 = "05";
                                    string fillerBanamexE2   = "                                        "; 
                                    string valorFijoBanamex3 = "C00";
                                    //HEADER
                                    string headerLayoutBanamex = tipoRegistroBanamexE + numeroClienteBanamexE + fechaBanamexE 
                                        + valorFijoBanamex0 + nombreEmpresaBanamex + valorFijoBanamex1 + fillerBanamexE1 
                                        + fechaBanamexE1 + valorFijoBanamex2 + fillerBanamexE2 + valorFijoBanamex3;
                                    // FOREACH DATOS TOTALES
                                    string importeTotalBanamexG = "";
                                    // INICIO CODIGO NUEVO RESTA RENGLON 1481 
                                    decimal sumaImporte = 0;
                                    ReportesDao reportDao = new ReportesDao();
                                    ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                                    foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                       if (bankResult == payroll.iIdBanco) {
                                            renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness, 
                                                Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                            if (renglon1481 > 0) {
                                                importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness, 
                                                    Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                sumaImporte += importe.decimalTotalDispersion;
                                            } else {
                                                sumaImporte += payroll.dImporte;
                                            }
                                        }
                                    }
                                    importeTotalBanamexG = sumaImporte.ToString().Replace(".", "");
                                    // - GLOBAL - \\
                                    string tipoRegistroBanamexG = "2";
                                    string cargoBanamexG        = "1";
                                    string monedaBanamexG       = "001"; 
                                    string tipoCuentaBanamexG   = "01";
                                    // PENDIENTE SUCURSAL
                                    string sucursalBanamexG     = datoCuentaClienteBancoEmpresaBean.iPlaza.ToString();
                                    string valorFijoBanamexG1   = "0000000000000"; 
                                    string numeroCuentaBanamex  = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                    string generaCImporteTBG    = "";
                                    int longImporteTotalBG      = 18;
                                    int longITBG                = longImporteTotalBG - importeTotalBanamexG.Length;
                                    for (var u = 0; u < longITBG; u++) { 
                                        generaCImporteTBG += "0"; 
                                    }
                                    string globalLayoutBanamex = tipoRegistroBanamexG + cargoBanamexG + monedaBanamexG 
                                        + generaCImporteTBG + importeTotalBanamexG + tipoCuentaBanamexG + sucursalBanamexG 
                                        + valorFijoBanamexG1 + numeroCuentaBanamex;
                                    // - DETALLE - \\
                                    string tipoRegistroBanamexD = "3";
                                    string abonoBanamexD        = "0";
                                    string metodoPagoBanamexD   = "001";
                                    string tipoCuentaBanamexD   = "01";
                                    string fillerBanamexD1      = "                              ";
                                    string valorFijoBanamexD1   = "NOMINA";
                                    string fillerBanamexD2      = "                                                          ";
                                    string valorFijoBanamexD2   = "0000";
                                    string fillerBanamexD3      = "       ";
                                    string valorFijoBanamexD3   = "00";
                                    using (StreamWriter fileBanamex = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + vFileName + ".txt", false, Encoding.UTF8)) {
                                        fileBanamex.Write(headerLayoutBanamex + "\n");
                                        fileBanamex.Write(globalLayoutBanamex + "\n");
                                        string cerosImpTotBnxD    = "";
                                        string cerosNumCueBnxD    = "";
                                        string cerosNumNomBnxD    = "";
                                        string espaciosNomEmpBnxD = "";
                                        int longImpTotBnxD        = 18;
                                        int longNumCueBnxD        = 20;
                                        int longNumNomBnxD        = 10;
                                        int cantidadMovBanamexT   = 0;
                                        int sumaImpTotBanamexT    = 0; 
                                        int longNomEmpBnxD        = 55;
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                            if (payroll.iIdBanco == bankResult) {
                                                int longAcortAccount = payroll.sCuenta.Length;
                                                string accountUser   = payroll.sCuenta;
                                                string formatAccount = "";
                                                if (longAcortAccount == 18) {
                                                    string formatAccountSubstring = accountUser.Substring(0, longAcortAccount - 1);
                                                    formatAccount = formatAccountSubstring.Substring(6, 11);
                                                } else {
                                                    formatAccount = payroll.sCuenta;
                                                }
                                                string nameEmployee = payroll.sNombre + " " + payroll.sPaterno + " " + payroll.sMaterno;
                                                cantidadMovBanamexT += 1; 
                                                string importeFinal = "";
                                                renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                        Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                if (renglon1481 > 0) {
                                                    importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness, 
                                                        Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                    importeFinal = importe.decimalTotalDispersion.ToString();
                                                } else {
                                                    importeFinal = payroll.dImporte.ToString();
                                                }
                                                sumaImpTotBanamexT += Convert.ToInt32(importeFinal);
                                                string nombreCEmp  = "";
                                                if (nameEmployee.Length > 57 ) {
                                                    nombreCEmp = nameEmployee.Substring(0, 54);
                                                } else {
                                                    nombreCEmp = nameEmployee;
                                                } 
                                                int longImpTotBnxDResult = longImpTotBnxD - importeFinal.ToString().Length;
                                                int longNumCueBnxDResult = longNumCueBnxD - payroll.sCuenta.Length;
                                                int longNumNomBnxDResult = longNumNomBnxD - payroll.sNomina.Length;
                                                int longNomEmpBnxDResult = longNomEmpBnxD - nombreCEmp.Length;
                                                for (var f = 0; f < longImpTotBnxDResult; f++) { 
                                                    cerosImpTotBnxD += "0"; 
                                                }
                                                for (var r = 0; r < 9; r++) { 
                                                    cerosNumCueBnxD += "0"; 
                                                }
                                                for (var c = 0; c < longNumNomBnxDResult; c++) { 
                                                    cerosNumNomBnxD += "0"; 
                                                }
                                                for (var s = 0; s < longNomEmpBnxDResult; s++) { 
                                                    espaciosNomEmpBnxD += " "; 
                                                } 
                                                fileBanamex.Write(tipoRegistroBanamexD + abonoBanamexD + metodoPagoBanamexD 
                                                    + cerosImpTotBnxD + importeFinal + tipoCuentaBanamexD + cerosNumCueBnxD 
                                                    + formatAccount + fillerBanamexD1 + cerosNumNomBnxD + payroll.sNomina 
                                                    + nombreCEmp + espaciosNomEmpBnxD + valorFijoBanamexD1 + fillerBanamexD2 
                                                    + valorFijoBanamexD2 + fillerBanamexD3 + valorFijoBanamexD3 + "\n");
                                                cerosImpTotBnxD    = "";
                                                cerosNumCueBnxD    = "";
                                                cerosNumNomBnxD    = "";
                                                espaciosNomEmpBnxD = "";
                                            }
                                        }
                                        // - TOTALES - \\
                                        string tipoRegistroBanamexT = "4";
                                        string claveMonedaBanamexT  = "001";
                                        string valorFijoBanamexT1   = "000001";
                                        string cerosCanMovBnxT      = "";
                                        string cerosSumImpTotBnxT   = "";
                                        int longSumMovBnxT          = 6;
                                        int longSumImpTotBnxT       = 18;
                                        int longSumMovBnxtResult    = longSumMovBnxT - cantidadMovBanamexT.ToString().Length;
                                        int longSumImpTotBnxTResult = longSumImpTotBnxT - sumaImpTotBanamexT.ToString().Length;
                                        for (var s = 0; s < longSumMovBnxtResult; s++) { 
                                            cerosCanMovBnxT += "0"; 
                                        }
                                        for (var w = 0; w < longSumImpTotBnxTResult; w++) { 
                                            cerosSumImpTotBnxT += "0"; 
                                        }
                                        string totalesLayoutBanamex = tipoRegistroBanamexT + claveMonedaBanamexT + cerosCanMovBnxT 
                                            + cantidadMovBanamexT.ToString() + cerosSumImpTotBnxT + sumaImpTotBanamexT.ToString() 
                                            + valorFijoBanamexT1 + cerosSumImpTotBnxT + sumaImpTotBanamexT.ToString();
                                        fileBanamex.Write(totalesLayoutBanamex + "\n");
                                        cerosCanMovBnxT    = "";
                                        cerosSumImpTotBnxT = "";
                                        fileBanamex.Close();
                                    }
                                }

                                // ARCHIVO CSV PARA HSBC -> NOMINA
                                if (bankResult == 21) {
                                    // FOREACH DATOS TOTALES
                                    double totalAmountHSBC = 0;
                                    int hQuantityDeposits = 0;
                                    foreach (DatosProcesaChequesNominaBean deposits in listDatosProcesaChequesNominaBean) {
                                        if (deposits.iIdBanco == bankResult) {
                                            totalAmountHSBC   += deposits.doImporte;
                                            hQuantityDeposits += 1;
                                        }
                                    }

                                    string nameBank   = "HSBC";
                                    //string outCsvFile = string.Format(directoryTxt + @"\\" + nameFolder + @"\\" + vFileName + ".csv", nameBank + DateTime.Now.ToString("_yyyyMMdd HHmms"));
                                    String header = "";
                                    //var stream        = System.IO.File.CreateText(outCsvFile);
                                    //// HEADER
                                    string hValuePermanent1    = "MXPRLF";
                                    string hNivelAuthorization = "F";
                                    string hReferenceNumber    = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                    string hTotalAmount        = Truncate(totalAmountHSBC, 2).ToString();
                                    string hDateActually       = DateTime.Now.ToString("ddMMyyyy");
                                    if (dateDisC != "") {
                                        hDateActually = DateTime.Parse(dateDisC.ToString()).ToString("ddMMyyyy");
                                    }
                                    string hSpaceWhite1        = " ";
                                    string hReferenceAlpa      = "PAGONOM" + numberPeriod + "QFEB";
                                    int ii = 0;
                                    // INICIO CODIGO NUEVO
                                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                    using (ExcelPackage excel = new ExcelPackage()) {
                                        string fileName = vFileName + "nuevo" + ".xlsx";
                                        excel.Workbook.Worksheets.Add(Path.GetFileNameWithoutExtension(fileName));
                                        var worksheet = excel.Workbook.Worksheets[Path.GetFileNameWithoutExtension(fileName)];
                                        header = hValuePermanent1 + "," + hNivelAuthorization + "," + hReferenceNumber + "," + hTotalAmount + "," + hQuantityDeposits.ToString() + "," + hDateActually + "," + hSpaceWhite1 + "," + hReferenceAlpa;
                                        worksheet.Cells[1, 1].Value = hValuePermanent1;
                                        worksheet.Cells[1, 2].Value = hNivelAuthorization;
                                        worksheet.Cells[1, 3].Value = hReferenceNumber;
                                        worksheet.Cells[1, 4].Value = hTotalAmount;
                                        worksheet.Cells[1, 5].Value = hQuantityDeposits;
                                        worksheet.Cells[1, 6].Value = hDateActually;
                                        worksheet.Cells[1, 7].Value = hSpaceWhite1;
                                        worksheet.Cells[1, 8].Value = hReferenceAlpa;
                                        ii = 8;
                                        int p = 1;
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                            if (payroll.iIdBanco == bankResult) {
                                                int longAcortAccount = payroll.sCuenta.Length;
                                                string finallyAccount = payroll.sCuenta;
                                                if (longAcortAccount == 18) {
                                                    string accountUser = payroll.sCuenta;
                                                    string formatAccountSubstring = accountUser.Substring(0, longAcortAccount - 1);
                                                    string formatAccount = "";
                                                    if (longAcortAccount == 18) {
                                                        formatAccount = formatAccountSubstring.Substring(0, 7);
                                                    }
                                                    string cerosAccount = "";
                                                    for (var t = 0; t < formatAccount.Length + 1; t++) {
                                                        cerosAccount += "0";
                                                    }
                                                    finallyAccount = formatAccountSubstring.Substring(7, 10);
                                                } else if (longAcortAccount == 9) {
                                                    finallyAccount = "0" + payroll.sCuenta;
                                                } else {
                                                    dataErrors.Add(new DataErrorAccountBank { sBanco = "HSBC", sCuenta = payroll.sCuenta, sNomina = payroll.sNomina });
                                                }
                                                var test = payroll.dImporte.ToString().Insert(payroll.dImporte.ToString().Length - 2, ".");
                                                string amountt = Truncate(Convert.ToDouble(payroll.sImporte), 2).ToString();
                                                string amount = payroll.doImporte.ToString();
                                                string nameEmployee = payroll.sNombre.TrimEnd() + " " + payroll.sPaterno.TrimEnd() + " " + payroll.sMaterno.TrimEnd();
                                                if (nameEmployee.Length > 35) {
                                                    nameEmployee = nameEmployee.Substring(0, 35);
                                                }
                                                if (finallyAccount == "6373294506") {
                                                    string testimport = payroll.doImporte.ToString();
                                                    string dd = "";
                                                    double amountD = payroll.doImporte;
                                                }
                                                worksheet.Cells[p + 1, 1].Value = finallyAccount;
                                                worksheet.Cells[p + 1, 2].Value = test;
                                                worksheet.Cells[p + 1, 3].Value = hReferenceAlpa;
                                                worksheet.Cells[p + 1, 4].Value = nameEmployee;
                                                p += 1;
                                            }
                                        }
                                        FileInfo excelFile = new FileInfo(directoryTxt + @"\\" + nameFolder + "/" + fileName);
                                        excel.SaveAs(excelFile);
                                    }
                                    // FIN CODIGO NUEVO
                                }

                                // ARCHIVO TXT DISPERSION SANTANDER -> NOMINA -> OK -> LISTO

                                if (bankResult == 14) {
                                    decimal resultadoSuma        = 0; 
                                    int initConsecutiveNbOneN    = 1;
                                    string  typeRegisterN        = "1";
                                    string consecutiveNumberOneN = "0000";
                                    string senseA                = "E";
                                    string numCtaBusiness        = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                    string fillerLayout          = "     ";
                                    string typeRegisterD         = "2";
                                    string headerLayout          = typeRegisterN + consecutiveNumberOneN + initConsecutiveNbOneN.ToString() 
                                        + senseA + dateGenerationFormat + numCtaBusiness + fillerLayout + dateGenerationFormat;
                                    using (StreamWriter fileTxt = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + vFileName + ".txt", false, new ASCIIEncoding())) {
                                        fileTxt.Write(headerLayout + "\n");
                                        string spaceGenerate1 = "", 
                                            spaceGenerate2    = "", 
                                            spaceGenerate3    = "", 
                                            numberCeroGene    = "", 
                                            consec1Generat    = "", 
                                            numberNomGener    = "", 
                                            totGenerate       = "";
                                        int longc           = 5, 
                                            long0           = 7, 
                                            long1           = 30, 
                                            long2           = 20, 
                                            long3           = 30, 
                                            long4           = 18, 
                                            consecutiveInit = initConsecutiveNbOneN, 
                                            resultSumTot    = 0, 
                                            longTot         = 18;
                                        int totalRecords    = 0;
                                        double totalAmount  = 0;
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                            if (payroll.iIdBanco == bankResult) {
                                                long1 = 30;
                                                if (payroll.sNomina.Length > 7) {
                                                    int resta = payroll.sNomina.Length - long0;
                                                    long1 = long1 - resta;
                                                } else {
                                                    long1 = 30;
                                                }
                                                if (payroll.sNomina == "800562563")
                                                {
                                                    int test = 0;
                                                }
                                                totalRecords          += 1;
                                                int longAcortAccount  = payroll.sCuenta.Length;
                                                string finallyAccount = payroll.sCuenta;
                                                if (longAcortAccount == 18) {
                                                    string cerosAccount           = "";
                                                    string accountUser            = payroll.sCuenta;
                                                    string formatAccountSubstring = accountUser.Substring(0, longAcortAccount - 1);
                                                    string formatAccount          = "";
                                                    if (longAcortAccount == 18) {
                                                        formatAccount = formatAccountSubstring.Substring(0, 6);
                                                    }
                                                    for (var t = 0; t < formatAccount.Length + 1; t++) {
                                                        cerosAccount += "0";
                                                    }
                                                    finallyAccount = formatAccountSubstring.Substring(6, 11);
                                                } else if (longAcortAccount == 9) {
                                                    finallyAccount = "0" + payroll.sCuenta;
                                                } else if (longAcortAccount == 16) {
                                                    finallyAccount = "00" + payroll.sCuenta;
                                                } else {
                                                    dataErrors.Add( new DataErrorAccountBank{ sBanco = "Santander", sCuenta = payroll.sCuenta,
                                                        sNomina = payroll.sNomina });
                                                }
                                                consecutiveInit     += 1;  
                                                decimal importeFinal = 0;
                                                string importeG      = "";
                                                renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                        Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                                                ReportesDao reportDao             = new ReportesDao();
                                                if (renglon1481 > 0) {
                                                    importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness, 
                                                        Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                    resultadoSuma += importe.decimalTotalDispersion;
                                                    importeFinal   = importe.decimalTotalDispersion;
                                                    importeG       = importeFinal.ToString();
                                                } else {
                                                    resultadoSuma += payroll.dImporte;
                                                    importeFinal   = payroll.dImporte;
                                                    importeG       = importeFinal.ToString();
                                                }
                                                totalAmount   += payroll.doImporte;
                                                int longConsec = longc - consecutiveInit.ToString().Length;
                                                int longNumNom = long0 - payroll.sNomina.Length;
                                                int longApepat = long1 - payroll.sPaterno.Length;
                                                int longApemat = long2 - payroll.sMaterno.Length;
                                                int longNomEmp = long3 - payroll.sNombre.Length;
                                                int longImport = long4 - importeFinal.ToString().Length;
                                                for (var y = 0; y < longConsec; y++) { 
                                                    consec1Generat += "0"; 
                                                }
                                                for (var g = 0; g < longNumNom; g++) { 
                                                    numberNomGener += "0"; 
                                                }
                                                for (var i = 0; i < longApepat; i++) { 
                                                    spaceGenerate1 += " "; 
                                                }
                                                for (var t = 0; t < longApemat; t++) { 
                                                    spaceGenerate2 += " "; 
                                                }
                                                for (var z = 0; z < longNomEmp; z++) { 
                                                    spaceGenerate3 += " "; 
                                                }
                                                for (var x = 0; x < longImport; x++) { 
                                                    numberCeroGene += "0"; 
                                                }
                                                string materno = payroll.sMaterno;
                                                if (payroll.sMaterno.Length >= 20)
                                                {
                                                    materno = payroll.sMaterno.Substring(0, 19) + " ";
                                                }
                                                fileTxt.WriteLine(typeRegisterD + consec1Generat + consecutiveInit.ToString() 
                                                    + numberNomGener + payroll.sNomina + payroll.sPaterno.Replace("Ñ", "N") 
                                                    + spaceGenerate1 + materno.Replace("Ñ", "N") 
                                                    + spaceGenerate2 + payroll.sNombre.Replace("Ñ", "N") 
                                                    + spaceGenerate3 + finallyAccount + "     " 
                                                    + numberCeroGene + importeG.ToString());
                                                consec1Generat = ""; 
                                                numberNomGener = "";
                                                spaceGenerate1 = ""; 
                                                spaceGenerate2 = "";
                                                spaceGenerate3 = ""; 
                                                numberCeroGene = "";
                                            }
                                        }
                                        if (bankResult == 14) {
                                            string importetotal = resultadoSuma.ToString().Replace(",","").Replace(".","");
                                            int longTotGenerate = longTot - importetotal.ToString().Length;
                                            for (var j = 0; j < longTotGenerate; j++) { 
                                                totGenerate += "0"; 
                                            }
                                            int long1TotGenert = longc - (consecutiveInit + 1).ToString().Length;
                                            for (var h = 0; h < long1TotGenert; h++) { 
                                                consec1Generat += "0"; 
                                            }
                                            int longTotalR           = 5;
                                            int resultLTR            = longTotalR - totalRecords.ToString().Length;
                                            string cerosTotalRecords = "";
                                            for (var x = 0; x < resultLTR; x++) {
                                                cerosTotalRecords += "0";
                                            }
                                            string totLayout = "3" + consec1Generat + (consecutiveInit + 1).ToString() 
                                                + cerosTotalRecords + totalRecords.ToString() + totGenerate + importetotal.ToString();
                                            fileTxt.WriteLine(totLayout);
                                        }
                                        fileTxt.Close();
                                    }
                                    // DOMC820415HMCMXR00
                                    // LAYOUT ANEXO A SANTANDER PARA LAS EMPRESAS 36, 37, 38, 39, 40, 41, 46, 47, 48 \\
                                    if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48) {
                                        int longCuentaCargo = 11;
                                        string espaciosBlanco1 = "     ";
                                        int longCuentaAbono = 11;
                                        string espaciosBlanco2 = "     ";
                                        int longImporte  = 13;
                                        int longConcepto = 30;
                                        int longEspaciosBlanco3 = 10;
                                        string fechaAplicacion = DateTime.Now.ToString("ddMMyyyy");
                                        string nombreArchivo = "LOE_Internas_" + keyBusiness.ToString();
                                        string cuentaCargo = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                        int contador = 0;
                                        int vueltas = 1;
                                        List<DatosProcesaChequesNominaBean> santanderNew = new List<DatosProcesaChequesNominaBean>();
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                            if (payroll.iIdBanco == bankResult) {
                                                if (contador > 499) {
                                                    contador = 0;
                                                    vueltas += 1;
                                                }
                                                contador += 1;
                                                santanderNew.Add(new DatosProcesaChequesNominaBean { iIdBanco = payroll.iIdBanco, dImporte = payroll.dImporte, doImporte = payroll.doImporte, iIdEmpresa = payroll.iIdEmpresa, iTipoPago = payroll.iTipoPago, sBanco = payroll.sBanco, sCodigo = payroll.sCodigo, sCuenta = payroll.sCuenta, sImporte = payroll.sImporte, sMaterno = payroll.sMaterno, sNombre = payroll.sNombre, sNomina = payroll.sNomina, sPaterno = payroll.sPaterno, sRfc = payroll.sRfc, iCodigoTXT = vueltas });
                                            }
                                        }
                                        int z = 1;
                                        while (z <= vueltas) {
                                            using (StreamWriter fileTxt = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + nombreArchivo +"_P" + z.ToString() + ".txt", false, new ASCIIEncoding())) {
                                                double suma = 0;
                                                foreach (DatosProcesaChequesNominaBean payroll in santanderNew) {
                                                    if (payroll.iIdBanco == bankResult && payroll.iCodigoTXT == z) {
                                                        // Cuenta Abono
                                                        string cuentaAbono = "";
                                                        if (payroll.sCuenta.Length == 18) {
                                                            cuentaAbono = payroll.sCuenta.Substring(6, 11);
                                                        } else if (payroll.sCuenta.Length == 16) {
                                                            cuentaAbono = payroll.sCuenta.Substring(5, 16);
                                                        } else if (payroll.sCuenta.Length == 11) {
                                                            cuentaAbono = payroll.sCuenta;
                                                        } else if (payroll.sCuenta.Length == 10) {
                                                            cuentaAbono = "0" + payroll.sCuenta;
                                                        }
                                                        // Importe
                                                        string formatoImporte = payroll.doImporte.ToString("0.00", CultureInfo.InvariantCulture);
                                                        string importe = "";
                                                        int resultadoLongImporte = longImporte - formatoImporte.Length;
                                                        for (var i = 0; i < resultadoLongImporte; i++) {
                                                            importe += "0";
                                                        }
                                                        importe = importe + formatoImporte;
                                                        // Concepto
                                                        int longConceptoYEspacio3 = 40;
                                                        string concepto = keyBusiness.ToString() + " PAGO POR CUENTA Y ORDEN SUELDO";
                                                        int resultConceptoYEspacio3 = longConceptoYEspacio3 - concepto.Length;
                                                        string espaciosBlanco3 = "";
                                                        for (var i = 0; i < resultConceptoYEspacio3; i++) {
                                                            espaciosBlanco3 += " ";
                                                        }
                                                        concepto = concepto + espaciosBlanco3;
                                                        // Graba archivo
                                                        string detalle = cuentaCargo + espaciosBlanco1 + cuentaAbono + espaciosBlanco2 + importe + concepto + fechaAplicacion;
                                                        fileTxt.WriteLine(detalle);
                                                        suma += payroll.doImporte;
                                                    }
                                                }
                                                fileTxt.Close();
                                            }
                                            z += 1;
                                        }
                                    }
                                }

                                // ARCHIVO TXT DISPERSION BANORTE -> NOMINA -> OK -> LISTO

                                if (bankResult == 72) {
                                    long[] TotIAbonos = new long[201];
                                    for (k = 0; k <= 200; k++) {
                                        TotIAbonos[k] = 0;
                                    }
                                    long TotalNumAbonos = 0;
                                    foreach (DatosProcesaChequesNominaBean bank in listDatosProcesaChequesNominaBean) {
                                        if (bankResult == bank.iIdBanco) {
                                            TotalNumAbonos += 1;
                                        }
                                    }
                                    StringBuilder sb1;
                                    sb1 = new StringBuilder("");
                                    sb1.Append("H");
                                    sb1.Append("NE");
                                    sb1.Append(string.Format("{0:00000}", Convert.ToInt32(datoCuentaClienteBancoEmpresaBean.sNumeroCliente)));
                                    sb1.Append(dateGeneration.ToString("yyyyMMdd"));
                                    sb1.Append("01");
                                    sb1.Append(string.Format("{0:000000}", TotalNumAbonos));
                                    sb1.Append(string.Format("{0:000000000000000}", TotIAbonos[72]));
                                    sb1.Append("0".PadRight(49, '0'));
                                    sb1.Append(" ".PadLeft(77, ' '));
                                    string ts = sb1.ToString();
                                    string importeTotalBanorte = ""; 
                                    decimal sumaImporte        = 0;
                                    ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                                    ReportesDao reportDao             = new ReportesDao();
                                    foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                        if (bankResult == payroll.iIdBanco) {
                                            renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                    Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                            if (renglon1481 > 0) {
                                                importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness, 
                                                    Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                sumaImporte += importe.decimalTotalDispersion;
                                            } else { 
                                                sumaImporte += payroll.dImporte;
                                            }
                                        }
                                    }
                                    importeTotalBanorte           = sumaImporte.ToString().Replace(",", "").Replace(".", ""); 
                                    string cerosImporteTotal      = "";
                                    string tipoRegistroBanorteE   = "H";
                                    string claveServicioBanorte   = "NE";
                                    string promotorBanorte        = datoCuentaClienteBancoEmpresaBean.sNumeroCliente;
                                    string consecutivoBanorte     = "01";
                                    string importeTotalAYBBanorte = "0000000000000000000000000000000000000000000000000";
                                    string fillerBanorte          = "                                                                             ";
                                    string generaCNumEmpresa      = "";
                                    string generaCNumRegistros    = "";
                                    int longNumEmpresa            = 5; 
                                    int longNumRegistros          = 6;
                                    int longAmoutTotal            = 15;
                                    int resultLongAmount          = longAmoutTotal - importeTotalBanorte.Length;
                                    string cerosImp2              = "";
                                    string quantityRegistersLong  = "";
                                    int quantityRegisters         = 0;
                                    foreach (DatosProcesaChequesNominaBean bank in listDatosProcesaChequesNominaBean) {
                                        if (bankResult == bank.iIdBanco) {
                                            quantityRegisters += 1;
                                        }
                                    }
                                    int longQuantityRegisters       = 6;
                                    int resultLongQuantityRegisters = longQuantityRegisters - quantityRegisters.ToString().Length;
                                    for (var f = 0; f < resultLongQuantityRegisters; f++) {
                                        quantityRegistersLong += "0";
                                    }
                                    quantityRegistersLong     += quantityRegisters.ToString();
                                    string dateGenerationDisp = dateGeneration.ToString("yyyyMMdd");
                                    if (dateDisC != "") {
                                        dateGenerationDisp = DateTime.Parse(dateDisC.ToString()).ToString("yyyyMMdd");
                                    }
                                    for (var j = 0; j < resultLongAmount; j++) { 
                                        cerosImporteTotal += "0"; 
                                    }
                                    string headerLayoutBanorte    = tipoRegistroBanorteE + claveServicioBanorte + promotorBanorte 
                                        + dateGenerationDisp +  consecutivoBanorte + quantityRegistersLong + cerosImp2 
                                        + cerosImporteTotal + importeTotalBanorte + importeTotalAYBBanorte + fillerBanorte;
                                    string tipoRegistroBanorteD     = "D";
                                    string fechaAplicacionBanorte   = dateGenerationDisp;
                                    string numBancoReceptorBanorteD = "072";
                                    string tipoCuentaBanorteD       = "01";
                                    string tipoMovimientoBanorteD   = "0"; 
                                    string fillerBanorteD0          = " ";
                                    string importeIvaBanorteD       = "00000000";
                                    string fillerBanorteD           = "                                                                                ";
                                    string fillerBanorteD1          = "                  ";
                                    double sumTest                  = 0;
                                    decimal sumtests                = 0;
                                    using (StreamWriter fileBanorte = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + vFileName + ".txt", false, Encoding.UTF8)) {
                                        fileBanorte.Write(headerLayoutBanorte + "\n");
                                        string generaCNumEmpleadoB = "", 
                                            generaCNumImporteB     = "", 
                                            generaCNumCuentaB      = "";
                                        int longNumEmpleado        = 10, 
                                            longNumImporte         = 15, 
                                            longNumCuenta          = 18;
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                            if (payroll.iIdBanco == bankResult) {
                                                int longAcortAccount  = payroll.sCuenta.Length;
                                                string finallyAccount = payroll.sCuenta;
                                                if (longAcortAccount == 18) {
                                                    string cerosAccount           = "";
                                                    string accountUser            = payroll.sCuenta;
                                                    string formatAccountSubstring = accountUser.Substring(0, longAcortAccount - 1);
                                                    string formatAccount          = "";
                                                    if (longAcortAccount == 18) {
                                                        formatAccount = formatAccountSubstring.Substring(0, 7);
                                                    }
                                                    for (var t = 0; t < formatAccount.Length + 1; t++) {
                                                        cerosAccount += "0";
                                                    }
                                                    finallyAccount = formatAccountSubstring.Substring(7, 10); 
                                                } else if (longAcortAccount == 9) {
                                                    finallyAccount = "0" + payroll.sCuenta;
                                                } else if (longAcortAccount == 10) {
                                                    finallyAccount = payroll.sCuenta;
                                                } else if (longAcortAccount == 11) {
                                                    finallyAccount = payroll.sCuenta.Remove(0, 1);
                                                } else {
                                                    dataErrors.Add(
                                                            new DataErrorAccountBank { sBanco = "BANORTE", sCuenta = payroll.sCuenta,
                                                                sNomina = payroll.sNomina });
                                                }
                                                sumTest  += payroll.doImporte;
                                                sumtests += payroll.dImporte;  
                                                string importeFinal = "";
                                                renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                        Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                if (renglon1481 > 0) {
                                                    importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness, 
                                                        Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                    importeFinal = importe.decimalTotalDispersion.ToString();
                                                } else {
                                                    importeFinal = payroll.dImporte.ToString();
                                                } 
                                                int longNumEmp = longNumEmpleado - payroll.sNomina.Length; 
                                                int longNumImp = longNumImporte - importeFinal.ToString().Length;
                                                int longNumCta = 10 - finallyAccount.Length;
                                                string cerosAccountDefault = "00000000";
                                                for (var b = 0; b < longNumEmp; b++) { 
                                                    generaCNumEmpleadoB += "0"; 
                                                }
                                                for (var v = 0; v < longNumImp; v++) { 
                                                    generaCNumImporteB += "0"; 
                                                }
                                                for (var p = 0; p < longNumCta; p++) { 
                                                    generaCNumCuentaB += "0"; 
                                                }
                                                // payroll.dImporte.ToString()
                                                fileBanorte.Write(tipoRegistroBanorteD + fechaAplicacionBanorte + generaCNumEmpleadoB 
                                                    + payroll.sNomina + fillerBanorteD + generaCNumImporteB + importeFinal 
                                                    + numBancoReceptorBanorteD + tipoCuentaBanorteD + generaCNumCuentaB 
                                                    + cerosAccountDefault + finallyAccount + tipoMovimientoBanorteD 
                                                    + fillerBanorteD0 + importeIvaBanorteD + fillerBanorteD1 + "\n");
                                                generaCNumEmpleadoB = "";
                                                generaCNumImporteB  = "";
                                                generaCNumCuentaB   = "";
                                            }
                                        }
                                        fileBanorte.Close();
                                    }
                                    // CODIGO NUEVO, LAYOUT DISPERSION NOMINA BANORTE \\
                                    
                                    if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48)
                                    {
                                        List<DatosProcesaChequesNominaBean> banorte = new List<DatosProcesaChequesNominaBean>();
                                        int contador = 0;
                                        int vueltas = 1;
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean)
                                        {
                                            if (payroll.iIdBanco == bankResult)
                                            {
                                                if (contador > 499)
                                                {
                                                    contador = 0;
                                                    vueltas += 1;
                                                }
                                                contador += 1;
                                                banorte.Add(new DatosProcesaChequesNominaBean { iIdBanco = payroll.iIdBanco, dImporte = payroll.dImporte, doImporte = payroll.doImporte, iIdEmpresa = payroll.iIdEmpresa, iTipoPago = payroll.iTipoPago, sBanco = payroll.sBanco, sCodigo = payroll.sCodigo, sCuenta = payroll.sCuenta, sImporte = payroll.sImporte, sMaterno = payroll.sMaterno, sNombre = payroll.sNombre, sNomina = payroll.sNomina, sPaterno = payroll.sPaterno, sRfc = payroll.sRfc, iCodigoTXT = vueltas });
                                            }
                                        }
                                        int i = 1;
                                        while ( i <= vueltas)
                                        {
                                            string tipoOperacion = "02";
                                            string claveId = "";
                                            string cuentaOrigen = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                            string cuentaDestino = "";
                                            string cerosFiller1 = "0000000000";
                                            //
                                            string pathSaveFile = Server.MapPath("~/Content/");
                                            string pathCompleteTXT = directoryTxt + @"\\" + nameFolder + @"\\" + "LOE_Internas_" + bankResult.ToString() + "_P" + i.ToString() + ".txt";
                                            using (StreamWriter fileBanorte = new StreamWriter(pathCompleteTXT, false, new ASCIIEncoding()))
                                            {
                                                foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean)
                                                {
                                                    if (payroll.iIdBanco == bankResult)
                                                    {
                                                        // Clave Id
                                                        int longClaveId = 13;
                                                        string spacesClaveId = "";
                                                        int resultSpacesClaveId = longClaveId - payroll.sNomina.Length;
                                                        for (var v = 0; v < resultSpacesClaveId; v++)
                                                        {
                                                            spacesClaveId += " ";
                                                        }
                                                        // Cuenta destino
                                                        cuentaDestino = payroll.sCuenta;
                                                        if (payroll.sCuenta.Length == 18)
                                                        {
                                                            cuentaDestino = cuentaDestino.Substring(7, 10);
                                                        }
                                                        int longCuentaDestino = 20;
                                                        string cerosCuentaDestino = "";
                                                        int resultCerosCuentaDestino = longCuentaDestino - cuentaDestino.Length;
                                                        for (var t = 0; t < resultCerosCuentaDestino; t++)
                                                        {
                                                            cerosCuentaDestino += "0";
                                                        }
                                                        // Importe
                                                        int longImporte = 14;
                                                        string cerosImporte = "";
                                                        int resultCerosImporte = longImporte - payroll.dImporte.ToString().Length;
                                                        for (var z = 0; z < resultCerosImporte; z++)
                                                        {
                                                            cerosImporte += "0";
                                                        }
                                                        // Referencia
                                                        string fecha = DateTime.Now.ToString("ddMMyyyy");
                                                        fecha = "00" + fecha;
                                                        //Descripcion
                                                        string descripcion = keyBusiness.ToString() + " PAGO POR CUENTA Y ORDEN SUE";
                                                        // Moneda origen
                                                        string monedaOrigen = "1";
                                                        // Moneda destino
                                                        string monedaDestino = "1";
                                                        // RFC ordenante
                                                        string rfc = rfcBusiness;
                                                        int longRfc = 13;
                                                        string spacesRfc = "";
                                                        int resultSpacesRFC = longRfc - rfc.Length;
                                                        for (var f = 0; f < resultSpacesRFC; f++)
                                                        {
                                                            spacesRfc += " ";
                                                        }
                                                        // IVA
                                                        string iva = "00000000000000";
                                                        // EMAIL BENEFICIARIO
                                                        string email = "grupo@dallheim.com.mx                  ";
                                                        // Fecha aplicacion
                                                        string fechaAplicacion = DateTime.Now.ToString("ddMMyyyy");
                                                        //Nombre beneficiario INSTRUCCION DE PAGO
                                                        string nombre = payroll.sPaterno + " " + payroll.sMaterno + " " + payroll.sNombre;
                                                        int longNombre = 70;
                                                        string spacesNombre = "";
                                                        int resultSpacesNombre = longNombre - nombre.Length;
                                                        for (var s = 0; s < resultSpacesNombre; s++)
                                                        {
                                                            spacesNombre += " ";
                                                        }
                                                        string lineTxt = tipoOperacion + payroll.sNomina + spacesClaveId + cerosFiller1 + cuentaOrigen + cerosCuentaDestino + cuentaDestino + cerosImporte + payroll.dImporte.ToString() + fecha + descripcion + monedaOrigen + monedaDestino + rfc + spacesRfc + iva + email + fechaAplicacion + nombre + spacesNombre;
                                                        fileBanorte.WriteLine(lineTxt);
                                                    }
                                                }
                                                fileBanorte.Close();
                                            }
                                            i += 1;
                                        }
                                    }
                                }

                                // ARCHIVO TXT DISPERSION BANCOMER -> NOMINA OK -> LISTO

                                if (bankResult == 12) {
                                    List<DatosProcesaChequesNominaBean> bancomer = new List<DatosProcesaChequesNominaBean>();
                                    int contador = 0;
                                    int vueltas = 1;
                                    foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                        if (payroll.iIdBanco == bankResult) {
                                            if (contador > 499) {
                                                contador = 0;
                                                vueltas += 1;
                                            }
                                            contador += 1;
                                            bancomer.Add(new DatosProcesaChequesNominaBean { iIdBanco = payroll.iIdBanco, dImporte = payroll.dImporte, doImporte = payroll.doImporte, iIdEmpresa = payroll.iIdEmpresa, iTipoPago = payroll.iTipoPago, sBanco = payroll.sBanco, sCodigo = payroll.sCodigo, sCuenta = payroll.sCuenta, sImporte = payroll.sImporte, sMaterno = payroll.sMaterno, sNombre = payroll.sNombre, sNomina = payroll.sNomina, sPaterno = payroll.sPaterno, sRfc = payroll.sRfc, iCodigoTXT = vueltas });
                                        }
                                    }
                                    int i = 1;
                                    while (i <= vueltas) {
                                        // Inicio Codigo Nuevo
                                        string pathSaveFile = Server.MapPath("~/Content/");
                                        string routeTXTBancomer = pathSaveFile + "DISPERSION" + @"\\" + "BANCOMER" + @"\\" + "BANCOMER.txt";
                                        string pathCompleteTXT = directoryTxt + @"\\" + nameFolder + @"\\" + vFileName + "P" + i.ToString() + ".txt";
                                        StringBuilder sb1;
                                        //System.IO.File.Copy(routeTXTBancomer, pathCompleteTXT, true);
                                        int totalRegistros = 0;
                                        string V = "";
                                        ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                                        ReportesDao reportDao = new ReportesDao();
                                        using (StreamWriter fileBancomer = new StreamWriter(pathCompleteTXT, false, new ASCIIEncoding())) {
                                            foreach (DatosProcesaChequesNominaBean payroll in bancomer) {
                                                if (payroll.iIdBanco == bankResult && payroll.iCodigoTXT == i) {
                                                    decimal importeFinal = 0;
                                                    string importeG = "";
                                                    renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                            Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                    if (renglon1481 > 0) {
                                                        importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                            Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                        importeFinal = importe.decimalTotalDispersion;
                                                    } else {
                                                        importeFinal = payroll.dImporte;
                                                        importeG = importeFinal.ToString();
                                                    }
                                                    totalRegistros += 1;
                                                    sb1 = new StringBuilder("");
                                                    sb1.Append(string.Format("{0:000000000}", totalRegistros));
                                                    sb1.Append(" ".PadLeft(16, ' '));
                                                    sb1.Append("99");
                                                    V = payroll.sCuenta.Trim();
                                                    if (V.Length <= 10)
                                                        V = V.PadLeft(10, '0');
                                                    else                            // FCH
                                                        V = V.Substring(7, 10);         // FCH
                                                    sb1.Append(V);
                                                    sb1.Append(" ".PadLeft(10, ' '));
                                                    sb1.Append(string.Format("{0:000000000000000}", importeFinal));
                                                    V = payroll.sNombre + " " + payroll.sPaterno + " " + payroll.sMaterno.Trim();
                                                    if (V.Length < 38)
                                                        V = V.PadRight(38, ' ');
                                                    else
                                                        V = V.Substring(0, 38);
                                                    sb1.Append(V);
                                                    sb1.Append("  001001");
                                                    fileBancomer.WriteLine(sb1.ToString());
                                                }
                                            }
                                            fileBancomer.Close();
                                        }
                                        i += 1;
                                    }
                                    if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48) {
                                        // CÓDIGO NUEVO \\
                                        // Version bancomer mismo banco
                                        List<DatosProcesaChequesNominaBean> bancomerNew = new List<DatosProcesaChequesNominaBean>();
                                        int count = 0;
                                        int road = 1;
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean)
                                        {
                                            if (payroll.iIdBanco == bankResult)
                                            {
                                                if (count > 199)
                                                {
                                                    count = 0;
                                                    road += 1;
                                                }
                                                count += 1;
                                                bancomerNew.Add(new DatosProcesaChequesNominaBean { iIdBanco = payroll.iIdBanco, dImporte = payroll.dImporte, doImporte = payroll.doImporte, iIdEmpresa = payroll.iIdEmpresa, iTipoPago = payroll.iTipoPago, sBanco = payroll.sBanco, sCodigo = payroll.sCodigo, sCuenta = payroll.sCuenta, sImporte = payroll.sImporte, sMaterno = payroll.sMaterno, sNombre = payroll.sNombre, sNomina = payroll.sNomina, sPaterno = payroll.sPaterno, sRfc = payroll.sRfc, iCodigoTXT = road });
                                            }
                                        }
                                        int z = 1;
                                        while (z <= road)
                                        {
                                            string routeFile = directoryTxt + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_BBVA" + keyBusiness.ToString() + "P" + z.ToString() + ".txt";
                                            using (StreamWriter fileBancomer = new StreamWriter(routeFile, false, new ASCIIEncoding()))
                                            {
                                                foreach (DatosProcesaChequesNominaBean payroll in bancomerNew)
                                                {
                                                    if (payroll.iIdBanco == bankResult && payroll.iCodigoTXT == z)
                                                    {
                                                        int longAccountDestinty = 18;
                                                        string cerosAccountDestiny = "";
                                                        int resultCerosAccountDestiny = 0;
                                                        string accountDestiny = payroll.sCuenta;
                                                        if (payroll.sCuenta.Length == 18)
                                                        {
                                                            accountDestiny = accountDestiny.Substring(7, 10);
                                                        }
                                                        resultCerosAccountDestiny = longAccountDestinty - accountDestiny.Length;
                                                        for (var u = 0; u < resultCerosAccountDestiny; u++)
                                                        {
                                                            cerosAccountDestiny += "0";
                                                        }
                                                        accountDestiny = cerosAccountDestiny + accountDestiny;
                                                        int longAccountOrigin = 18;
                                                        string cerosAccountOrigin = "";
                                                        int resultCerosAccountOrigin = 0;
                                                        string accountOrigin = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                                        resultCerosAccountOrigin = longAccountOrigin - accountOrigin.Length;
                                                        for (var h = 0; h < resultCerosAccountOrigin; h++)
                                                        {
                                                            cerosAccountOrigin += "0";
                                                        }
                                                        accountOrigin = cerosAccountOrigin + accountOrigin;
                                                        string divisaOperation = "MXP";
                                                        int longAmountOperation = 16;
                                                        string cerosAmountOperation = "";
                                                        string amountOperation = payroll.doImporte.ToString("0.00");
                                                        int resultCerosAmountOperation = longAmountOperation - amountOperation.Length;
                                                        for (var v = 0; v < resultCerosAmountOperation; v++)
                                                        {
                                                            cerosAmountOperation += "0";
                                                        }
                                                        amountOperation = cerosAmountOperation + amountOperation;
                                                        string reasonForPayment = keyBusiness.ToString() + " PAGO POR CUENTAY ORDEN SUEL";
                                                        fileBancomer.WriteLine(accountDestiny + accountOrigin + divisaOperation + amountOperation + reasonForPayment);
                                                    }
                                                }
                                                fileBancomer.Close();
                                            }
                                            z += 1;
                                        }
                                    }
                                }

                                // Dispersion BANCOPPEL OK -> LISTO PARA PRUEBAS
                                if (bankResult == 137) {
                                    // Longitudes nombre archivo
                                    int longNumberBusiness = 3;
                                    // Nombre del archivo
                                    int numberBusiness = longNumberBusiness - keyBusiness.ToString().Length;
                                    string business = "";
                                    for (var i = 0; i < numberBusiness; i++) {
                                        business += "0";
                                    }
                                    business = business + keyBusiness.ToString();
                                    string dateGenFile = DateTime.Now.ToString("yyyyMMdd");
                                    string sequence = "01";
                                    string nameFileTxt = business + dateGenFile + sequence + ".txt";
                                    //Longitudes encabezado
                                    int longNumberAccount = 16;
                                    //Encabezado
                                    string typeOfRegister = "1";
                                    string sequenceHeader = "00001";
                                    string senseHeader    = "E";
                                    string dateGenHeader  = DateTime.Now.ToString("MMddyyyy");
                                    int accountSpaces     = longNumberAccount - datoCuentaClienteBancoEmpresaBean.sNumeroCuenta.Length;
                                    string accountHeader  = "";
                                    for (var i = 0; i < accountSpaces; i++) {
                                        accountHeader += " ";
                                    }
                                    accountHeader = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta + accountHeader;
                                    string dateOfApplication = (dateDisC != "") ? DateTime.Parse(dateDisC).ToString("MMddyyyy") : DateTime.Now.ToString("MMddyyyy");
                                    string headerFileTxt = typeOfRegister + sequenceHeader + senseHeader + dateGenHeader + accountHeader + dateOfApplication;
                                    using (StreamWriter fileBancoppel = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + nameFileTxt, false, Encoding.UTF8)) {
                                        fileBancoppel.Write(headerFileTxt + "\n");
                                        int sequenceNumber = 2;
                                        int quantityRegisters = 0;
                                        decimal sumaImporte = 0;
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                            if (payroll.iIdBanco == bankResult) {
                                                // RegistroEmpleados \\
                                                // Obtener importe final
                                                ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                                                ReportesDao reportDao = new ReportesDao();
                                                string importeFinal = "";
                                                renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                        Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                if (renglon1481 > 0) {
                                                    importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                        Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                    importeFinal = importe.decimalTotalDispersion.ToString();
                                                    sumaImporte += importe.decimalTotalDispersion;
                                                } else {
                                                    importeFinal = payroll.dImporte.ToString();
                                                    sumaImporte += payroll.dImporte;
                                                }
                                                // Longitudes
                                                int longSequenceDetail = 5;
                                                int longNumberEmployee = 10;
                                                int longLastNamePaterno = 30;
                                                int longLastNameMaterno = 20;
                                                int longNameEmployee = 30;
                                                int longAccountEmployee = 18;
                                                int longAmountEmployee = 18;
                                                // Detalle
                                                string typeOfRegisterDetail = "2";
                                                // Sequencia
                                                string sequenceDetail = "";
                                                int resultSequence = longSequenceDetail - sequenceNumber.ToString().Length;
                                                for (var i = 0; i < resultSequence; i++) {
                                                    sequenceDetail += "0";
                                                }
                                                sequenceDetail = sequenceDetail + sequenceNumber;
                                                // Numero de empleado
                                                string numberEmployee = "";
                                                int resultNumberEmployee = longNumberEmployee - payroll.sNomina.Length;
                                                for (var i = 0; i < resultNumberEmployee; i++) {
                                                    numberEmployee += "0";
                                                }
                                                numberEmployee = numberEmployee + payroll.sNomina;
                                                // Apellido paterno
                                                string lastNamePaterno = "";
                                                int resultLastNamePaterno = longLastNamePaterno - payroll.sPaterno.Length;
                                                for (var i = 0; i < resultLastNamePaterno; i++) {
                                                    lastNamePaterno += " ";
                                                }
                                                lastNamePaterno = payroll.sPaterno + lastNamePaterno;
                                                // Apellido materno
                                                string lastNameMaterno = "";
                                                int resultLastNameMaterno = longLastNameMaterno - payroll.sMaterno.Length;
                                                for (var i = 0; i < resultLastNameMaterno; i++) {
                                                    lastNameMaterno += " ";
                                                }
                                                lastNameMaterno = payroll.sMaterno + lastNameMaterno;
                                                //Nombre del empleado
                                                string nameEmployee = "";
                                                int resultNameEmployee = longNameEmployee - payroll.sNombre.Length;
                                                for (var i = 0; i < resultNameEmployee; i++) {
                                                    nameEmployee += " ";
                                                }
                                                nameEmployee = payroll.sNombre + nameEmployee;
                                                // Cuenta del empleado ( // TODO: Verificar si se acortara la cuenta a 11 digitos )
                                                string accountEmployee = "";
                                                int resultAccountEmployee = longAccountEmployee - payroll.sCuenta.Length;
                                                for (var i = 0; i < resultAccountEmployee; i++) {
                                                    accountEmployee += " ";
                                                }
                                                accountEmployee = payroll.sCuenta + accountEmployee;
                                                // Concepto
                                                string conceptDetail = "01"; // Concepto de nómina
                                                // Monto a pagar
                                                string amountEmployee = "";
                                                int resultAmountEmployee = longAmountEmployee - importeFinal.Length;
                                                for (var i = 0; i < resultAmountEmployee; i++) {
                                                    amountEmployee += "0";
                                                }
                                                amountEmployee = amountEmployee + payroll.dImporte.ToString();
                                                // Estructura del detalle 
                                                string detail = typeOfRegisterDetail + sequenceDetail + numberEmployee + lastNamePaterno + lastNameMaterno + nameEmployee + accountEmployee + conceptDetail + amountEmployee;
                                                sequenceNumber += 1;
                                                quantityRegisters += 1;
                                                fileBancoppel.Write(detail + "\n");
                                            }
                                        }
                                        // Sumario del archivo de dispersion
                                        // Longitudes
                                        int longSequenceDetailS = 5;
                                        int longQuantityRegisters = 5;
                                        int longAmountTotal = 18;
                                        // Tipo de registro
                                        string typeOfRegisterSummary = "3";
                                        // Secuencia
                                        string sequenceDetailS = "";
                                        int resultSequenceS = longSequenceDetailS - sequenceNumber.ToString().Length;
                                        for (var i = 0; i < resultSequenceS; i++) {
                                            sequenceDetailS += "0";
                                        }
                                        sequenceDetailS = sequenceDetailS + sequenceNumber;
                                        // Total de registros
                                        string totalRegisters = "";
                                        int resultTotalRegisters = longQuantityRegisters - quantityRegisters.ToString().Length;
                                        for (var i = 0; i < resultTotalRegisters; i++) {
                                            totalRegisters += "0";
                                        }
                                        totalRegisters = totalRegisters + quantityRegisters.ToString();
                                        // Importe total ( // TODO: Se realiza la suma dentro del recorrido para escribir el detalle en el archivo)
                                        string amountTotal      = sumaImporte.ToString().Replace(",", "").Replace(".", "");
                                        int resultAmountTotal   = longAmountTotal - amountTotal.Length;
                                        string amountTotalWrite = "";
                                        for ( var i = 0; i < resultAmountTotal; i++ ) {
                                            amountTotalWrite += "0";
                                        }
                                        amountTotalWrite = amountTotalWrite + amountTotal;
                                        // Estructura
                                        string summary = typeOfRegisterSummary + sequenceDetailS + totalRegisters + amountTotalWrite;
                                        fileBancoppel.Write(summary + "\n");
                                        fileBancoppel.Close();
                                    }
                                }

                                FileStream fs = new FileStream(directoryTxt + @"\\" + nameFolder + @"\\" + fileNamePDF, FileMode.Create);
                                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 20, 40, 20, 40);
                                PdfWriter pw = PdfWriter.GetInstance(doc, fs);
                                doc.AddTitle("Reporte de Dispersion");
                                doc.AddAuthor("");
                                doc.Open();

                                // Creamos el tipo de Font que vamos utilizar
                                iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                                // Escribimos el encabezamiento en el documento
                                Font fontDefault = new Font(FontFamily.HELVETICA, 10);
                                Paragraph pr = new Paragraph();
                                DateTime datePdf = DateTime.Now;
                                pr.Font = fontDefault;
                                pr.Add("Fecha: " + datePdf.ToString("yyyy-MM-dd") + " Periodo: " + numberPeriod.ToString());
                                pr.Alignment = Element.ALIGN_LEFT;
                                doc.Add(pr);
                                pr.Clear();
                                pr.Add("IPSNet \n Dépositos " + nameBankResult);
                                pr.Alignment = Element.ALIGN_CENTER;
                                doc.Add(pr);
                                doc.Add(Chunk.NEWLINE);
                                pr.Clear();
                                pr.Add(nameBusiness + "\n" + rfcBusiness);
                                pr.Alignment = Element.ALIGN_CENTER;
                                doc.Add(pr);
                                doc.Add(Chunk.NEWLINE);
                                ListRenglonesGruposRestas importePDF = new ListRenglonesGruposRestas();
                                ReportesDao reportDaoPDF = new ReportesDao();
                                PdfPTable tblPrueba = new PdfPTable(4);
                                tblPrueba.WidthPercentage = 100;
                                PdfPCell clCtaCheques = new PdfPCell(new Phrase("Cta. Cheques", _standardFont));
                                clCtaCheques.BorderWidth = 0;
                                clCtaCheques.BorderWidthBottom = 0.75f;
                                clCtaCheques.Bottom = 80;
                                PdfPCell clBeneficiario = new PdfPCell(new Phrase("Beneficiario", _standardFont));
                                clBeneficiario.BorderWidth = 0;
                                clBeneficiario.BorderWidthBottom = 0.75f;
                                clBeneficiario.Bottom = 60;
                                PdfPCell clImporte = new PdfPCell(new Phrase("Importe", _standardFont));
                                clImporte.BorderWidth = 0;
                                clImporte.BorderWidthBottom = 0.75f;
                                clImporte.Bottom = 40;
                                PdfPCell clNomina = new PdfPCell(new Phrase("Nomina", _standardFont));
                                clNomina.BorderWidth = 0;
                                clNomina.BorderWidthBottom = 0.75f;
                                clNomina.Bottom = 20; 
                                tblPrueba.AddCell(clCtaCheques);
                                tblPrueba.AddCell(clBeneficiario);
                                tblPrueba.AddCell(clImporte);
                                tblPrueba.AddCell(clNomina);
                                int d = 0;
                                Decimal sumTotal = 0;
                                int cantidadEmpleados = 0;
                                foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                    if (payroll.iIdBanco == bankResult) {
                                        // INICIO CODIGO NUEVO (RESTA RENGLON 1481)
                                        double restaImporte = 0;
                                        double importeFinal = 0;
                                        renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                        if (renglon1481 > 0) {
                                            importePDF = reportDaoPDF.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness, Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                                            importeFinal = importePDF.dTotal;
                                        } else {
                                            importeFinal = payroll.doImporte;
                                        }
                                        // FIN CODIGO NUEVO
                                        cantidadEmpleados += 1;
                                        //sumTotal += Convert.ToDecimal(payroll.doImporte);
                                        sumTotal += Convert.ToDecimal(importeFinal);
                                        d += 1; 
                                        clCtaCheques = new PdfPCell(new Phrase(payroll.sCuenta, _standardFont));
                                        clCtaCheques.BorderWidth = 0;
                                        clCtaCheques.Bottom = 80;
                                        clBeneficiario = new PdfPCell(new Phrase(payroll.sNombre + " " + payroll.sPaterno + " " + payroll.sMaterno, _standardFont));
                                        clBeneficiario.BorderWidth = 0;
                                        clBeneficiario.Bottom = 80;
                                        //clImporte = new PdfPCell(new Phrase("$ " + Convert.ToDecimal(payroll.doImporte).ToString("#,##0.00"), _standardFont));
                                        clImporte = new PdfPCell(new Phrase("$ " + Convert.ToDecimal(importeFinal).ToString("#,##0.00"), _standardFont));
                                        clImporte.BorderWidth = 0;
                                        clImporte.Bottom = 80;
                                        clNomina = new PdfPCell(new Phrase(payroll.sNomina, _standardFont));
                                        clNomina.BorderWidth = 0;
                                        clNomina.Bottom = 80; 
                                        tblPrueba.AddCell(clCtaCheques);
                                        tblPrueba.AddCell(clBeneficiario);
                                        tblPrueba.AddCell(clImporte);
                                        tblPrueba.AddCell(clNomina);
                                    }
                                }
                                doc.Add(tblPrueba);
                                doc.Add(Chunk.NEWLINE);
                                iTextSharp.text.Font _standardFont2 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                                // Creamos una tabla que contendrá los datos
                                PdfPTable tblTotal = new PdfPTable(1);
                                tblTotal.WidthPercentage = 100;
                                PdfPCell clTotal = new PdfPCell(new Phrase("Total: " + "$ " + sumTotal.ToString("#,##0.00"), _standardFont2));
                                clTotal.BorderWidth = 0; 
                                clTotal.Bottom = 80;
                                tblTotal.AddCell(clTotal);
                                doc.Add(tblTotal);
                                // Creamos la tabla del total de empleados
                                PdfPTable tblTotalEmpleados = new PdfPTable(1);
                                tblTotalEmpleados.WidthPercentage = 100;
                                PdfPCell clTotalEmpleados = new PdfPCell(new Phrase("Empleados: " + cantidadEmpleados.ToString() + " registros.", _standardFont2));
                                clTotalEmpleados.BorderWidth = 0;
                                clTotalEmpleados.Bottom = 80;
                                tblTotalEmpleados.AddCell(clTotalEmpleados);
                                doc.Add(tblTotalEmpleados);
                                doc.Close();
                                pw.Close();
                                flag = true;
                                if (dataErrors.Count > 0) {
                                    using (StreamWriter fileLog = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + "LOG_ACCOUNTS" + ".txt", false, Encoding.UTF8))
                                    {
                                        fileLog.Write("Fecha: " + DateTime.Now.ToString() + "\n");
                                        fileLog.Write("\n");
                                        fileLog.Write("CUENTAS CON LONGITUD MENOR A 18 CARACTERES");
                                        fileLog.Write("\n");
                                        foreach (DataErrorAccountBank err in dataErrors) {
                                            fileLog.Write("Nomina: " + err.sNomina + ". Banco: " + err.sBanco + ". Cuenta: " + err.sCuenta);
                                            fileLog.Write("\n");
                                        }
                                        fileLog.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
                flag = false;
            }
            return flag;
        }

        public Boolean ProcessDepositsMirror(int keyBusiness, int invoiceId, int typeReceipt, string dateDeposits, int yearPeriod, int typePeriod, int numberPeriod, string nameBusiness, string rfcBusiness, string dateDisC)
        {
            Boolean flag    = false;
            Boolean notData = true;
            int k;
            int bankResult   = 0;
            string nameBankResult = "";
            string fileNamePDF    = "";
            string vFileName      = "";
            List<DataErrorAccountBank> dataErrors = new List<DataErrorAccountBank>();
            List<DatosDepositosBancariosBean> listDatosDepositosBancariosBeans = new List<DatosDepositosBancariosBean>();
            DataDispersionBusiness dataDispersionBusiness = new DataDispersionBusiness();
            List<DatosProcesaChequesNominaBean> listDatosProcesaChequesNominaBean = new List<DatosProcesaChequesNominaBean>();
            DatosCuentaClienteBancoEmpresaBean datoCuentaClienteBancoEmpresaBean = new DatosCuentaClienteBancoEmpresaBean();
            DatosDispersionArchivosBanamex datosDispersionArchivosBanamex = new DatosDispersionArchivosBanamex();
            try {
                listDatosDepositosBancariosBeans = dataDispersionBusiness.sp_Procesa_Cheques_Total_Nomina_Espejo(keyBusiness, typePeriod, numberPeriod, yearPeriod);
                if (listDatosDepositosBancariosBeans.Count > 0) {
                    notData = false;
                }
                if (notData) {
                    return flag;
                }
                listDatosProcesaChequesNominaBean = dataDispersionBusiness.sp_Procesa_Cheques_Nomina_Espejo(keyBusiness, typePeriod, numberPeriod, yearPeriod);
                if (listDatosProcesaChequesNominaBean.Count == 0) {
                    return flag;
                }
                Boolean createFolders = CreateFoldersToDeploy();
                foreach (DatosProcesaChequesNominaBean data in listDatosProcesaChequesNominaBean) {
                    if (data.dImporte != 0) {
                        if (data.iIdBanco != bankResult) {
                            if (bankResult != 0)  {
                                // Por ejecutar
                            }
                            bankResult = data.iIdBanco;
                            nameBankResult = data.sBanco;
                            // Ejecutar un sp
                            datoCuentaClienteBancoEmpresaBean = dataDispersionBusiness.sp_Cuenta_Cliente_Banco_Empresa(keyBusiness, bankResult);
                            if (datoCuentaClienteBancoEmpresaBean.sMensaje == "SUCCESS") {
                                DateTime dateGeneration = DateTime.Now;
                                string dateGenerationFormat = dateGeneration.ToString("MMddyyyy");
                                //-----------
                                string nameFolder = "DEPOSITOS_" + "E" + keyBusiness.ToString() + "P" + numberPeriod.ToString() + "A" + dateGeneration.ToString("yyyy").Substring(2, 2);
                                //-----------
                                fileNamePDF = "CHQ_NOMINAS_E" + keyBusiness.ToString() + "A" + string.Format("{0:00}", (yearPeriod % 100)) + "P" + string.Format("{0:00}", numberPeriod) + "_BE_" + bankResult.ToString() + ".PDF";
                                // -------------------------
                                string directoryTxt = Server.MapPath("/DispersionTXT/" + DateTime.Now.Year.ToString()).ToString() + "/NOMINAS/";
                                // -------------------------
                                if (!System.IO.Directory.Exists(directoryTxt + @"\\" + nameFolder)) {
                                    System.IO.Directory.CreateDirectory(directoryTxt + @"\\" + nameFolder);
                                }
                                // -------------------------
                                if (bankResult == 72) {
                                    vFileName = "NOMINAS_NI" + string.Format("{0:00000}", Convert.ToInt32(datoCuentaClienteBancoEmpresaBean.sNumeroCliente)) + "01_ESP";
                                } else {
                                    vFileName = "E" + string.Format("{0:000}", keyBusiness.ToString()) + "A" + yearPeriod + yearPeriod.ToString() + "P" + string.Format("{0:000}", numberPeriod.ToString()) + "_BE_" + bankResult.ToString();
                                }

                                // BANAMEX -> NOMINA
                                if (bankResult == 2) {
                                    DateTime dateC = dateGeneration;
                                    if (dateDisC != "") {
                                        dateC = DateTime.Parse(dateDisC.ToString());
                                    }
                                    // ENCABEZADO  --> ARCHIVO OK
                                    string tipoRegistroBanamexE = "1";
                                    string numeroClienteBanamexE = "000" + datoCuentaClienteBancoEmpresaBean.sNumeroCliente;
                                    string fechaBanamexE = dateC.ToString("ddMM") + dateC.ToString("yyyy").Substring(2, 2);
                                    string valorFijoBanamex0 = "0001";
                                    string nombreEmpresaBanamex = "";
                                    if (nameBusiness.Length > 35) {
                                        nombreEmpresaBanamex = nameBusiness.Substring(0, 35);
                                    } else {
                                        nombreEmpresaBanamex = nameBusiness;
                                    }
                                    string valorFijoBanamex1 = "CNOMINA";
                                    string fillerBanamexE1 = " ";
                                    string fechaBanamexE1 = dateC.ToString("ddMMyyyy") + "     ";
                                    string valorFijoBanamex2 = "05";
                                    string fillerBanamexE2 = "                                        ";
                                    string valorFijoBanamex3 = "C00";
                                    //HEADER
                                    string headerLayoutBanamex = tipoRegistroBanamexE + numeroClienteBanamexE + fechaBanamexE + valorFijoBanamex0 + nombreEmpresaBanamex + valorFijoBanamex1 + fillerBanamexE1 + fechaBanamexE1 + valorFijoBanamex2 + fillerBanamexE2 + valorFijoBanamex3;
                                    // FOREACH DATOS TOTALES
                                    string importeTotalBanamexG = "";
                                    foreach (DatosDepositosBancariosBean deposits in listDatosDepositosBancariosBeans) {
                                        if (deposits.iIdBanco == bankResult) {
                                            importeTotalBanamexG = deposits.sImporte;
                                            break;
                                        }
                                    }
                                    // - GLOBAL - \\
                                    string tipoRegistroBanamexG = "2";
                                    string cargoBanamexG = "1";
                                    string monedaBanamexG = "001";
                                    string tipoCuentaBanamexG = "01";
                                    // PENDIENTE SUCURSAL
                                    string sucursalBanamexG = "7009";
                                    string valorFijoBanamexG1 = "0000000000000";
                                    string numeroCuentaBanamex = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                    string generaCImporteTBG = "";
                                    int longImporteTotalBG = 18;
                                    int longITBG = longImporteTotalBG - importeTotalBanamexG.Length;
                                    for (var u = 0; u < longITBG; u++) { generaCImporteTBG += "0"; }
                                    string globalLayoutBanamex = tipoRegistroBanamexG + cargoBanamexG + monedaBanamexG + generaCImporteTBG + importeTotalBanamexG + tipoCuentaBanamexG + sucursalBanamexG + valorFijoBanamexG1 + numeroCuentaBanamex;
                                    // - DETALLE - \\
                                    string tipoRegistroBanamexD = "3";
                                    string abonoBanamexD = "0";
                                    string metodoPagoBanamexD = "001";
                                    string tipoCuentaBanamexD = "01";
                                    string fillerBanamexD1 = "                              ";
                                    string valorFijoBanamexD1 = "NOMINA";
                                    string fillerBanamexD2 = "                                                          ";
                                    string valorFijoBanamexD2 = "0000";
                                    string fillerBanamexD3 = "       ";
                                    string valorFijoBanamexD3 = "00";
                                    using (StreamWriter fileBanamex = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + vFileName + ".txt", false, Encoding.UTF8)) {
                                        fileBanamex.Write(headerLayoutBanamex + "\n");
                                        fileBanamex.Write(globalLayoutBanamex + "\n");
                                        string cerosImpTotBnxD = "";
                                        string cerosNumCueBnxD = "";
                                        string cerosNumNomBnxD = "";
                                        string espaciosNomEmpBnxD = "";
                                        int longImpTotBnxD = 18;
                                        int longNumCueBnxD = 20;
                                        int longNumNomBnxD = 10;
                                        int cantidadMovBanamexT = 0;
                                        int sumaImpTotBanamexT = 0;
                                        int longNomEmpBnxD = 55;
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                            if (payroll.iIdBanco == bankResult) {
                                                int longAcortAccount = payroll.sCuenta.Length;
                                                string accountUser = payroll.sCuenta;
                                                string formatAccount = "";
                                                if (longAcortAccount == 18) {
                                                    string formatAccountSubstring = accountUser.Substring(0, longAcortAccount - 1);
                                                    formatAccount = formatAccountSubstring.Substring(6, 11);
                                                } else {
                                                    formatAccount = payroll.sCuenta;
                                                }
                                                string nameEmployee = payroll.sNombre + " " + payroll.sPaterno + " " + payroll.sMaterno;
                                                cantidadMovBanamexT += 1;
                                                sumaImpTotBanamexT += Convert.ToInt32(payroll.dImporte);
                                                string nombreCEmp = "";
                                                if (nameEmployee.Length > 57) {
                                                    nombreCEmp = nameEmployee.Substring(0, 54);
                                                } else {
                                                    nombreCEmp = nameEmployee;
                                                }
                                                int longImpTotBnxDResult = longImpTotBnxD - payroll.dImporte.ToString().Length;
                                                int longNumCueBnxDResult = longNumCueBnxD - payroll.sCuenta.Length;
                                                int longNumNomBnxDResult = longNumNomBnxD - payroll.sNomina.Length;
                                                int longNomEmpBnxDResult = longNomEmpBnxD - nombreCEmp.Length;
                                                for (var f = 0; f < longImpTotBnxDResult; f++) { cerosImpTotBnxD += "0"; }
                                                for (var r = 0; r < 9; r++) { cerosNumCueBnxD += "0"; }
                                                for (var c = 0; c < longNumNomBnxDResult; c++) { cerosNumNomBnxD += "0"; }
                                                for (var s = 0; s < longNomEmpBnxDResult; s++) { espaciosNomEmpBnxD += " "; }
                                                fileBanamex.Write(tipoRegistroBanamexD + abonoBanamexD + metodoPagoBanamexD + cerosImpTotBnxD + payroll.dImporte.ToString() + tipoCuentaBanamexD + cerosNumCueBnxD + formatAccount + fillerBanamexD1 + cerosNumNomBnxD + payroll.sNomina + nombreCEmp + espaciosNomEmpBnxD + valorFijoBanamexD1 + fillerBanamexD2 + valorFijoBanamexD2 + fillerBanamexD3 + valorFijoBanamexD3 + "\n");
                                                cerosImpTotBnxD = "";
                                                cerosNumCueBnxD = "";
                                                cerosNumNomBnxD = "";
                                                espaciosNomEmpBnxD = "";
                                            }
                                        }
                                        // - TOTALES - \\
                                        string tipoRegistroBanamexT = "4";
                                        string claveMonedaBanamexT = "001";
                                        string valorFijoBanamexT1 = "000001";
                                        string cerosCanMovBnxT = "";
                                        string cerosSumImpTotBnxT = "";
                                        int longSumMovBnxT = 6;
                                        int longSumImpTotBnxT = 18;
                                        int longSumMovBnxtResult = longSumMovBnxT - cantidadMovBanamexT.ToString().Length;
                                        int longSumImpTotBnxTResult = longSumImpTotBnxT - sumaImpTotBanamexT.ToString().Length;
                                        for (var s = 0; s < longSumMovBnxtResult; s++) { cerosCanMovBnxT += "0"; }
                                        for (var w = 0; w < longSumImpTotBnxTResult; w++) { cerosSumImpTotBnxT += "0"; }
                                        string totalesLayoutBanamex = tipoRegistroBanamexT + claveMonedaBanamexT + cerosCanMovBnxT + cantidadMovBanamexT.ToString() + cerosSumImpTotBnxT + sumaImpTotBanamexT.ToString() + valorFijoBanamexT1 + cerosSumImpTotBnxT + sumaImpTotBanamexT.ToString();
                                        fileBanamex.Write(totalesLayoutBanamex + "\n");
                                        cerosCanMovBnxT = "";
                                        cerosSumImpTotBnxT = "";
                                        fileBanamex.Close();
                                    }
                                }

                                // ARCHIVO CSV PARA HSBC -> NOMINA
                                if (bankResult == 21) {
                                    // FOREACH DATOS TOTALES
                                    double totalAmountHSBC = 0;
                                    int hQuantityDeposits = 0;
                                    foreach (DatosProcesaChequesNominaBean deposits in listDatosProcesaChequesNominaBean) {
                                        if (deposits.iIdBanco == bankResult) {
                                            totalAmountHSBC += deposits.doImporte;
                                            hQuantityDeposits += 1;
                                        }
                                    }
                                    string nameBank = "HSBC";
                                    string outCsvFile = string.Format(directoryTxt + @"\\" + nameFolder + @"\\" + vFileName + ".csv", nameBank + DateTime.Now.ToString("_yyyyMMdd HHmms"));
                                    String header = "";
                                    var stream = System.IO.File.CreateText(outCsvFile);
                                    // HEADER
                                    string hValuePermanent1 = "MXPRLF";
                                    string hNivelAuthorization = "F";
                                    string hReferenceNumber = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                    string hTotalAmount = Truncate(totalAmountHSBC, 2).ToString();
                                    string hDateActually = DateTime.Now.ToString("ddMMyyyy");
                                    if (dateDisC != "") {
                                        hDateActually = DateTime.Parse(dateDisC.ToString()).ToString("ddMMyyyy");
                                    }
                                    string hSpaceWhite1 = "";
                                    string hReferenceAlpa = "PAGONOM" + numberPeriod + "QFEB";
                                    header = hValuePermanent1 + "," + hNivelAuthorization + "," + hReferenceNumber + "," + hTotalAmount + "," + hQuantityDeposits.ToString() + "," + hDateActually + "," + hSpaceWhite1 + "," + hReferenceAlpa;
                                    stream.WriteLine(header);
                                    foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                        if (payroll.iIdBanco == bankResult) {
                                            int longAcortAccount = payroll.sCuenta.Length;
                                            string finallyAccount = payroll.sCuenta;
                                            if (longAcortAccount == 18) {
                                                string accountUser = payroll.sCuenta;
                                                string formatAccountSubstring = accountUser.Substring(0, longAcortAccount - 1);
                                                string formatAccount = "";
                                                if (longAcortAccount == 18) {
                                                    formatAccount = formatAccountSubstring.Substring(0, 7);
                                                }
                                                string cerosAccount = "";
                                                for (var t = 0; t < formatAccount.Length + 1; t++) {
                                                    cerosAccount += "0";
                                                }
                                                finallyAccount = formatAccountSubstring.Substring(7, 10);
                                            } else if (longAcortAccount == 9) {
                                                finallyAccount = "0" + payroll.sCuenta;
                                            } else {
                                                dataErrors.Add( new DataErrorAccountBank { sBanco = "HSBC", sCuenta = payroll.sCuenta, sNomina = payroll.sNomina });
                                            }
                                            string amount = payroll.doImporte.ToString();
                                            string nameBen = payroll.sNombre.TrimEnd() + " " + payroll.sPaterno.TrimEnd() + " " + payroll.sMaterno.TrimEnd();
                                            header = finallyAccount + "," + amount + "," + hReferenceAlpa + "," + nameBen;
                                            stream.WriteLine(header);
                                        }
                                    }
                                    stream.Close();
                                }

                                // ARCHIVO TXT PARA SANTANDER -> NOMINA

                                if (bankResult == 14) {
                                    // - ENCABEZADO - \\ ARCHIVO OK
                                    int initConsecutiveNbOneN = 1;
                                    string typeRegisterN = "1";
                                    string consecutiveNumberOneN = "0000";
                                    string senseA = "E";
                                    string numCtaBusiness = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                    string fillerLayout = "     ";
                                    string headerLayout = typeRegisterN + consecutiveNumberOneN + initConsecutiveNbOneN.ToString() + senseA + dateGenerationFormat + numCtaBusiness + fillerLayout + dateGenerationFormat;
                                    // - DETALLE - \\                                                                          
                                    string typeRegisterD = "2";
                                    using (StreamWriter fileTxt = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + vFileName + ".txt", false, Encoding.UTF8)) {
                                        fileTxt.Write(headerLayout + "\n");
                                        string spaceGenerate1 = "", spaceGenerate2 = "", spaceGenerate3 = "", numberCeroGene = "", consec1Generat = "", numberNomGener = "", totGenerate = "";
                                        int longc = 5, long0 = 7, long1 = 30, long2 = 20, long3 = 30, long4 = 18, consecutiveInit = initConsecutiveNbOneN, resultSumTot = 0, longTot = 18;
                                        int totalRecords = 0;
                                        double totalAmount = 0;
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                            if (payroll.iIdBanco == bankResult) {
                                                totalRecords += 1;
                                                totalAmount += payroll.doImporte;
                                                int longAcortAccount = payroll.sCuenta.Length;
                                                string finallyAccount = payroll.sCuenta;
                                                if (longAcortAccount == 18) {
                                                    string accountUser = payroll.sCuenta;
                                                    string formatAccountSubstring = accountUser.Substring(0, longAcortAccount - 1);
                                                    string formatAccount = "";
                                                    if (longAcortAccount == 18) {
                                                        formatAccount = formatAccountSubstring.Substring(0, 6);
                                                    }
                                                    string cerosAccount = "";
                                                    for (var t = 0; t < formatAccount.Length + 1; t++) {
                                                        cerosAccount += "0";
                                                    }
                                                    finallyAccount = formatAccountSubstring.Substring(6, 11);
                                                } else if (longAcortAccount == 9) {
                                                    finallyAccount = "0" + payroll.sCuenta;
                                                } else {
                                                    dataErrors.Add( new DataErrorAccountBank { sBanco = "Santander", sCuenta = payroll.sCuenta, sNomina = payroll.sNomina });
                                                }
                                                consecutiveInit += 1;
                                                int longConsec = longc - consecutiveInit.ToString().Length;
                                                int longNumNom = long0 - payroll.sNomina.Length;
                                                int longApepat = long1 - payroll.sPaterno.Length;
                                                int longApemat = long2 - payroll.sMaterno.Length;
                                                int longNomEmp = long3 - payroll.sNombre.Length;
                                                int longImport = long4 - payroll.dImporte.ToString().Length;
                                                for (var y = 0; y < longConsec; y++) { consec1Generat += "0"; }
                                                for (var g = 0; g < longNumNom; g++) { numberNomGener += "0"; }
                                                for (var i = 0; i < longApepat; i++) { spaceGenerate1 += " "; }
                                                for (var t = 0; t < longApemat; t++) { spaceGenerate2 += " "; }
                                                for (var z = 0; z < longNomEmp; z++) { spaceGenerate3 += " "; }
                                                for (var x = 0; x < longImport; x++) { numberCeroGene += "0"; }
                                                string materno = "";
                                                if (payroll.sMaterno.Length >= 20) {
                                                    materno = payroll.sMaterno.Substring(0, 19) + " ";
                                                }
                                                resultSumTot += Convert.ToInt32(payroll.dImporte);
                                                fileTxt.Write(typeRegisterD + consec1Generat + consecutiveInit.ToString() + numberNomGener + payroll.sNomina + payroll.sPaterno.Replace("Ñ", "N") + spaceGenerate1 + materno.Replace("Ñ", "N") + spaceGenerate2 + payroll.sNombre.Replace("Ñ", "N") + spaceGenerate3 + finallyAccount + "     " + numberCeroGene + payroll.dImporte.ToString() + "\n");
                                                consec1Generat = ""; numberNomGener = "";
                                                spaceGenerate1 = ""; spaceGenerate2 = "";
                                                spaceGenerate3 = ""; numberCeroGene = "";
                                            }
                                        }
                                        if (bankResult == 14) {
                                            int longTotGenerate = longTot - resultSumTot.ToString().Length;
                                            for (var j = 0; j < longTotGenerate; j++) { totGenerate += "0"; }
                                            int long1TotGenert = longc - (consecutiveInit + 1).ToString().Length;
                                            for (var h = 0; h < long1TotGenert; h++) { consec1Generat += "0"; }
                                            int longTotalR = 5;
                                            int resultLTR = longTotalR - totalRecords.ToString().Length;
                                            string cerosTotalRecords = "";
                                            for (var x = 0; x < resultLTR; x++) {
                                                cerosTotalRecords += "0";
                                            }
                                            string totLayout = "3" + consec1Generat + (consecutiveInit + 1).ToString() + cerosTotalRecords + totalRecords.ToString() + totGenerate + resultSumTot.ToString();
                                            fileTxt.Write(totLayout + "\n");
                                        }
                                        fileTxt.Close();
                                    }
                                }

                                // ARCHIVO DISPERSION BANORTE -> NOMINA -> OK

                                if (bankResult == 72) {

                                    // INICIO CODIGO NUEVO

                                    long[] TotIAbonos = new long[201];
                                    for (k = 0; k <= 200; k++) {
                                        TotIAbonos[k] = 0;
                                    }
                                    long TotalNumAbonos = 0;
                                    foreach (DatosProcesaChequesNominaBean bank in listDatosProcesaChequesNominaBean) {
                                        if (bankResult == bank.iIdBanco) {
                                            TotalNumAbonos += 1;
                                        }
                                    }
                                    StringBuilder sb1;
                                    sb1 = new StringBuilder("");
                                    sb1.Append("H");
                                    sb1.Append("NE");
                                    sb1.Append(string.Format("{0:00000}", Convert.ToInt32(datoCuentaClienteBancoEmpresaBean.sNumeroCliente)));
                                    sb1.Append(dateGeneration.ToString("yyyyMMdd"));
                                    sb1.Append("01");
                                    sb1.Append(string.Format("{0:000000}", TotalNumAbonos));
                                    sb1.Append(string.Format("{0:000000000000000}", TotIAbonos[72]));
                                    sb1.Append("0".PadRight(49, '0'));
                                    sb1.Append(" ".PadLeft(77, ' '));
                                    string ts = sb1.ToString();

                                    // FIN CODIGO NUEVO

                                    string importeTotalBanorte = "";
                                    foreach (DatosDepositosBancariosBean deposits in listDatosDepositosBancariosBeans) {
                                        if (deposits.iIdBanco == bankResult) { importeTotalBanorte = deposits.sImporte; break; }
                                    }
                                    // - ENCABEZADO - \\ 
                                    string cerosImporteTotal = "";
                                    string tipoRegistroBanorteE = "H";
                                    string claveServicioBanorte = "NE";
                                    string promotorBanorte = datoCuentaClienteBancoEmpresaBean.sNumeroCliente;
                                    string consecutivoBanorte = "01";
                                    string importeTotalAYBBanorte = "0000000000000000000000000000000000000000000000000";
                                    string fillerBanorte = "                                                                             ";
                                    string generaCNumEmpresa = "";
                                    string generaCNumRegistros = "";
                                    int longNumEmpresa = 5;
                                    int longNumRegistros = 6;
                                    int longAmoutTotal = 15;
                                    int resultLongAmount = longAmoutTotal - importeTotalBanorte.Length;
                                    string cerosImp2 = "";
                                    string quantityRegistersLong = "";
                                    int quantityRegisters = 0;
                                    foreach (DatosProcesaChequesNominaBean bank in listDatosProcesaChequesNominaBean) {
                                        if (bankResult == bank.iIdBanco) {
                                            quantityRegisters += 1;
                                        }
                                    }
                                    int longQuantityRegisters = 6;
                                    int resultLongQuantityRegisters = longQuantityRegisters - quantityRegisters.ToString().Length;
                                    for (var f = 0; f < resultLongQuantityRegisters; f++) {
                                        quantityRegistersLong += "0";
                                    }
                                    quantityRegistersLong += quantityRegisters.ToString();
                                    string dateGenerationDisp = dateGeneration.ToString("yyyyMMdd");
                                    if (dateDisC != "")  {
                                        dateGenerationDisp = DateTime.Parse(dateDisC.ToString()).ToString("yyyyMMdd");
                                    }
                                    for (var j = 0; j < resultLongAmount; j++) { cerosImporteTotal += "0"; }
                                    string headerLayoutBanorte = tipoRegistroBanorteE + claveServicioBanorte + promotorBanorte + dateGenerationDisp + consecutivoBanorte + quantityRegistersLong + cerosImp2 + cerosImporteTotal + importeTotalBanorte + importeTotalAYBBanorte + fillerBanorte;
                                    // - DETALLE - \\
                                    string tipoRegistroBanorteD = "D";
                                    string fechaAplicacionBanorte = dateGenerationDisp;
                                    string numBancoReceptorBanorteD = "072";
                                    string tipoCuentaBanorteD = "01";
                                    string tipoMovimientoBanorteD = "0";
                                    string fillerBanorteD0 = " ";
                                    string importeIvaBanorteD = "00000000";
                                    string fillerBanorteD = "                                                                                ";
                                    string fillerBanorteD1 = "                  ";
                                    double sumTest = 0;
                                    decimal sumtests = 0;
                                    using (StreamWriter fileBanorte = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + vFileName + ".txt", false, Encoding.UTF8)) {
                                        fileBanorte.Write(headerLayoutBanorte + "\n");
                                        string generaCNumEmpleadoB = "", generaCNumImporteB = "", generaCNumCuentaB = "";
                                        int longNumEmpleado = 10, longNumImporte = 15, longNumCuenta = 18;
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                            if (payroll.iIdBanco == bankResult) {
                                                int longAcortAccount = payroll.sCuenta.Length;
                                                string finallyAccount = payroll.sCuenta;
                                                if (longAcortAccount == 18) {
                                                    string accountUser = payroll.sCuenta;
                                                    string formatAccountSubstring = accountUser.Substring(0, longAcortAccount - 1);
                                                    string formatAccount = "";
                                                    if (longAcortAccount == 18) {
                                                        formatAccount = formatAccountSubstring.Substring(0, 7);
                                                    }
                                                    string cerosAccount = "";
                                                    for (var t = 0; t < formatAccount.Length + 1; t++) {
                                                        cerosAccount += "0";
                                                    }
                                                    finallyAccount = formatAccountSubstring.Substring(7, 10);
                                                    // finallyAccount = cerosAccount + formatAccountSubstring.Substring(7, 10);
                                                } else if (longAcortAccount == 9) {
                                                    finallyAccount = "0" + payroll.sCuenta;
                                                } else if (longAcortAccount == 10) {
                                                    finallyAccount = payroll.sCuenta;
                                                }
                                                else if (longAcortAccount == 11)
                                                {
                                                    finallyAccount = payroll.sCuenta.Remove(0, 1);
                                                }
                                                else
                                                {
                                                    dataErrors.Add(
                                                            new DataErrorAccountBank
                                                            {
                                                                sBanco = "BANORTE",
                                                                sCuenta = payroll.sCuenta,
                                                                sNomina = payroll.sNomina
                                                            });
                                                }
                                                sumTest += payroll.doImporte;
                                                sumtests += payroll.dImporte;
                                                int longNumEmp = longNumEmpleado - payroll.sNomina.Length;
                                                int longNumImp = longNumImporte - payroll.dImporte.ToString().Length;
                                                int longNumCta = 10 - finallyAccount.Length;
                                                string cerosAccountDefault = "00000000";
                                                for (var b = 0; b < longNumEmp; b++) { generaCNumEmpleadoB += "0"; }
                                                for (var v = 0; v < longNumImp; v++) { generaCNumImporteB += "0"; }
                                                for (var p = 0; p < longNumCta; p++) { generaCNumCuentaB += "0"; }
                                                fileBanorte.Write(tipoRegistroBanorteD + fechaAplicacionBanorte + generaCNumEmpleadoB + payroll.sNomina + fillerBanorteD + generaCNumImporteB + payroll.dImporte.ToString() + numBancoReceptorBanorteD + tipoCuentaBanorteD + generaCNumCuentaB + cerosAccountDefault + finallyAccount + tipoMovimientoBanorteD + fillerBanorteD0 + importeIvaBanorteD + fillerBanorteD1 + "\n");
                                                generaCNumEmpleadoB = "";
                                                generaCNumImporteB = "";
                                                generaCNumCuentaB = "";
                                            }
                                        }
                                        string test = sumTest.ToString();
                                        string test2 = sumtests.ToString();
                                        //fileBanorte.WriteLine(sumtests.ToString() + " test  " + test2);
                                        fileBanorte.Close();
                                    }
                                }

                                // ARCHIVO DISPERSION BANCOMER -> NOMINA

                                if (bankResult == 12) {
                                    // Inicio Codigo Nuevo
                                    string pathSaveFile = Server.MapPath("~/Content/");
                                    string routeTXTBancomer = pathSaveFile + "DISPERSION" + @"\\" + "BANCOMER" + @"\\" + "BANCOMER.txt";
                                    string pathCompleteTXT = directoryTxt + @"\\" + nameFolder + @"\\" + vFileName + ".txt";
                                    StringBuilder sb1;
                                    //System.IO.File.Copy(routeTXTBancomer, pathCompleteTXT, true);
                                    int totalRegistros = 0;
                                    string V = "";
                                    using (StreamWriter fileBancomer = new StreamWriter(pathCompleteTXT, false, new ASCIIEncoding())) {
                                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                            if (payroll.iIdBanco == bankResult) {
                                                totalRegistros += 1;
                                                sb1 = new StringBuilder("");
                                                sb1.Append(string.Format("{0:000000000}", totalRegistros));
                                                sb1.Append(" ".PadLeft(16, ' '));
                                                sb1.Append("99");
                                                V = payroll.sCuenta.Trim();
                                                if (V.Length <= 10)
                                                    V = V.PadLeft(10, '0');
                                                else                            // FCH
                                                    V = V.Substring(7, 10);         // FCH
                                                sb1.Append(V);
                                                sb1.Append(" ".PadLeft(10, ' '));
                                                sb1.Append(string.Format("{0:000000000000000}", payroll.dImporte));
                                                V = payroll.sNombre + " " + payroll.sPaterno + " " + payroll.sMaterno.Trim();
                                                if (V.Length < 38)
                                                    V = V.PadRight(38, ' ');
                                                else
                                                    V = V.Substring(0, 38);
                                                sb1.Append(V);
                                                sb1.Append("  001001");
                                                fileBancomer.WriteLine(sb1.ToString());
                                            }
                                        }
                                        fileBancomer.Close();
                                    }
                                }


                                FileStream fs = new FileStream(directoryTxt + @"\\" + nameFolder + @"\\" + fileNamePDF, FileMode.Create);
                                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 20, 40, 20, 40);
                                PdfWriter pw = PdfWriter.GetInstance(doc, fs);
                                doc.AddTitle("Reporte de Dispersion");
                                doc.AddAuthor("");
                                doc.Open();

                                // Creamos el tipo de Font que vamos utilizar
                                iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                                // Escribimos el encabezamiento en el documento
                                Font fontDefault = new Font(FontFamily.HELVETICA, 10);
                                Paragraph pr = new Paragraph();
                                DateTime datePdf = DateTime.Now;
                                pr.Font = fontDefault;
                                pr.Add("Fecha: " + datePdf.ToString("yyyy-MM-dd") + " Periodo: " + numberPeriod.ToString());
                                pr.Alignment = Element.ALIGN_LEFT;
                                doc.Add(pr);
                                pr.Clear();
                                pr.Add("IPSNet \n Dépositos " + nameBankResult);
                                pr.Alignment = Element.ALIGN_CENTER;
                                doc.Add(pr);
                                doc.Add(Chunk.NEWLINE);
                                pr.Clear();
                                pr.Add(nameBusiness + "\n" + rfcBusiness);
                                pr.Alignment = Element.ALIGN_CENTER;
                                doc.Add(pr);
                                doc.Add(Chunk.NEWLINE);
                                // Creamos una tabla que contendrá los datos
                                PdfPTable tblPrueba       = new PdfPTable(4);
                                tblPrueba.WidthPercentage = 100;
                                // Configuramos el título de las columnas de la tabla
                                PdfPCell clCtaCheques          = new PdfPCell(new Phrase("Cta. Cheques", _standardFont));
                                clCtaCheques.BorderWidth       = 0;
                                clCtaCheques.BorderWidthBottom = 0.75f;
                                clCtaCheques.Bottom            = 80;
                                PdfPCell clBeneficiario          = new PdfPCell(new Phrase("Beneficiario", _standardFont));
                                clBeneficiario.BorderWidth       = 0;
                                clBeneficiario.BorderWidthBottom = 0.75f;
                                clBeneficiario.Bottom            = 60;
                                PdfPCell clImporte          = new PdfPCell(new Phrase("Importe", _standardFont));
                                clImporte.BorderWidth       = 0;
                                clImporte.BorderWidthBottom = 0.75f;
                                clImporte.Bottom            = 40;
                                PdfPCell clNomina           = new PdfPCell(new Phrase("Nomina", _standardFont));
                                clNomina.BorderWidth        = 0;
                                clNomina.BorderWidthBottom  = 0.75f;
                                clNomina.Bottom             = 20;
                                // Añadimos las celdas a la tabla
                                tblPrueba.AddCell(clCtaCheques);
                                tblPrueba.AddCell(clBeneficiario);
                                tblPrueba.AddCell(clImporte);
                                tblPrueba.AddCell(clNomina);
                                double sumTotal = 0;
                                int cantidadEmpleados = 0;
                                foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                                    if (payroll.iIdBanco == bankResult) {
                                        cantidadEmpleados += 1;
                                        sumTotal += payroll.doImporte;
                                        // Llenamos la tabla con información
                                        clCtaCheques = new PdfPCell(new Phrase(payroll.sCuenta, _standardFont));
                                        clCtaCheques.BorderWidth = 0;
                                        clCtaCheques.Bottom = 80;
                                        clBeneficiario = new PdfPCell(new Phrase(payroll.sNombre + " " + payroll.sPaterno + " " + payroll.sMaterno, _standardFont));
                                        clBeneficiario.BorderWidth = 0;
                                        clBeneficiario.Bottom = 80;
                                        clImporte = new PdfPCell(new Phrase("$ " + Convert.ToDecimal(payroll.doImporte).ToString("#,##0.00"), _standardFont));
                                        clImporte.BorderWidth = 0;
                                        clImporte.Bottom = 80;
                                        clNomina = new PdfPCell(new Phrase(payroll.sNomina, _standardFont));
                                        clNomina.BorderWidth = 0;
                                        clNomina.Bottom = 80;
                                        // Añadimos las celdas a la tabla
                                        tblPrueba.AddCell(clCtaCheques);
                                        tblPrueba.AddCell(clBeneficiario);
                                        tblPrueba.AddCell(clImporte);
                                        tblPrueba.AddCell(clNomina);
                                    }
                                }
                                doc.Add(tblPrueba);
                                doc.Add(Chunk.NEWLINE);
                                iTextSharp.text.Font _standardFont2 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                                // Creamos una tabla que contendrá los datos
                                PdfPTable tblTotal = new PdfPTable(1);
                                tblTotal.WidthPercentage = 100;
                                PdfPCell clTotal = new PdfPCell(new Phrase("Total: " + "$ " + sumTotal.ToString("#,##0.00"), _standardFont2));
                                clTotal.BorderWidth = 0;
                                clTotal.Bottom = 80;
                                tblTotal.AddCell(clTotal);
                                doc.Add(tblTotal);
                                // Creamos la tabla del total de empleados
                                PdfPTable tblTotalEmpleados = new PdfPTable(1);
                                tblTotalEmpleados.WidthPercentage = 100;
                                PdfPCell clTotalEmpleados = new PdfPCell(new Phrase("Empleados: " + cantidadEmpleados.ToString() + " registros.", _standardFont2));
                                clTotalEmpleados.BorderWidth = 0;
                                clTotalEmpleados.Bottom = 80;
                                tblTotalEmpleados.AddCell(clTotalEmpleados);
                                doc.Add(tblTotalEmpleados);
                                doc.Close();
                                pw.Close();
                                flag = true;
                            }
                        }
                    }
                }
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
                flag = false;
            }
            return flag;
        }

        [HttpPost]
        public JsonResult RestartToDeploy(string paramNameFile, int paramYear, string paramCode)
        {
            Boolean flag         = false;
            Boolean flagZIP      = false;
            Boolean flagTXT      = false;
            String  messageError = "none";
            string  nameFileFZ   = (paramCode == "NOM") ? "NOMINAS" : "INTERBANCARIOS";
            string  directoryZip = Server.MapPath("/DispersionZIP/" + paramYear.ToString() + "/" + nameFileFZ + "/" + paramNameFile + ".zip");
            string  directoryTxt = Server.MapPath("/DispersionTXT/" + paramYear.ToString() + "/" + nameFileFZ + "/" + paramNameFile);
            try {
                if (System.IO.File.Exists(directoryZip)) {
                    System.IO.File.Delete(directoryZip);
                    flagZIP = true;
                }
                if (System.IO.Directory.Exists(directoryTxt)) {
                    System.IO.Directory.Delete(directoryTxt, recursive: true);
                    flagTXT = true;
                }
                flag = true;
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, TXT = flagTXT, ZIP = flagZIP });
        }

        [HttpPost]
        public JsonResult ProcessDepositsInterbank(int yearPeriod, int numberPeriod, int typePeriod, string dateDeposits, int mirror, int type, string dateDisC, int tipPago)
        {
            Boolean flag            = false;
            Boolean flagMirror      = false;
            Boolean flagProsecutors = false;
            Boolean first           = false;
            String messageError     = "none";
            DatosEmpresaBeanDispersion datosEmpresaBeanDispersion = new DatosEmpresaBeanDispersion();
            DataDispersionBusiness     dataDispersionBusiness     = new DataDispersionBusiness();
            List<DatosDepositosBancariosBean> listDatosDepositosBancariosBeans = new List<DatosDepositosBancariosBean>();
            DatosCuentaClienteBancoEmpresaBean datoCuentaClienteBancoEmpresaBean = new DatosCuentaClienteBancoEmpresaBean();
            List<DatosProcesaChequesNominaBean> listDatosProcesaChequesNominaBean = new List<DatosProcesaChequesNominaBean>();
            string nameFolder    = "";
            string nameFileError = "";
            DateTime dateGeneration     = DateTime.Now;
            string dateGenerationFormat = dateGeneration.ToString("MMddyyyy");
            string directoryZip   = Server.MapPath("/DispersionZIP").ToString();
            string directoryTxt   = Server.MapPath("/DispersionTXT").ToString() + "/" + DateTime.Now.Year.ToString() + "/INTERBANCARIOS/";
            string nameFolderYear = DateTime.Now.Year.ToString();
            string msgEstatus     = "";
            string msgEstatusZip  = "";
            int bankInterbank     = 2;
            int turns             = 0;
            int totalRecords      = 0;
            string fileNamePdfPM  = "";
            string fileNameTxtPM  = "";
            Boolean createFolders = CreateFoldersToDeploy();
            try {
                int keyBusiness  = int.Parse(Session["IdEmpresa"].ToString());
                int yearActually = DateTime.Now.Year;
                int typeReceipt  = (yearPeriod == yearActually) ? 1 : 0;
                int invoiceId       = yearPeriod * 100000 + typePeriod * 10000 + numberPeriod * 10;
                int invoiceIdMirror = yearPeriod * 100000 + typePeriod * 10000 + numberPeriod * 10 + 8;
                int invoiceSendSP;
                datosEmpresaBeanDispersion = dataDispersionBusiness.sp_Datos_Empresa_Dispersion(keyBusiness, type);
                if (datosEmpresaBeanDispersion.iBanco_id.GetType().Name == "DBNull") {
                    // Retornar error
                }
                nameFolder = "DEPOSITOS_" + "E" + keyBusiness.ToString() + "P" + numberPeriod.ToString() + "A" + dateGeneration.ToString("yyyy").Substring(2, 2);
                // -------------------------
                if (System.IO.File.Exists(directoryZip + @"\\" + nameFolderYear + @"\\" + "INTERBANCARIOS" + @"\" + nameFolder + ".zip")) {
                    System.IO.File.Delete(directoryZip + @"\\" + nameFolderYear + @"\\" + "INTERBANCARIOS" + @"\" + nameFolder + ".zip");
                }
                if (Directory.Exists(directoryTxt + @"\\" + nameFolder)) {
                    Directory.Delete(directoryTxt + @"\\" + nameFolder, recursive: true);
                }
                if (!System.IO.Directory.Exists(directoryTxt + @"\\" + nameFolder)) {
                    System.IO.Directory.CreateDirectory(directoryTxt + @"\\" + nameFolder);
                }
                while (turns < 2) {
                    first = true;
                    turns += 1;
                    totalRecords = 0;
                    invoiceId = (turns == 1) ? invoiceId : invoiceIdMirror ;
                    if (turns == 2 && mirror == 0) {
                        break;
                    }
                    if (turns == 1) {
                        listDatosDepositosBancariosBeans = dataDispersionBusiness.sp_Procesa_Cheques_Total_Interbancarios(keyBusiness, typePeriod, numberPeriod, yearPeriod);
                    } else {
                        listDatosDepositosBancariosBeans = dataDispersionBusiness.sp_Procesa_Cheques_Total_Interbancarios_Espejo(keyBusiness, typePeriod, numberPeriod, yearPeriod);
                    }
                    if (listDatosDepositosBancariosBeans.Count == 0) {
                        flagProsecutors = false;
                        string msjInterbanks = "SIN DEPOSITOS";
                    }
                    bankInterbank = datosEmpresaBeanDispersion.iBanco_id;
                    datoCuentaClienteBancoEmpresaBean = dataDispersionBusiness.sp_Cuenta_Cliente_Banco_Empresa(keyBusiness, bankInterbank);
                    // Genera nombre del pdf
                    if (turns == 1) {
                        fileNamePdfPM = "CHQ_NOMINAS_E" + keyBusiness.ToString() + "A" + string.Format("{0:00}", (yearPeriod % 100)) + "P" + string.Format("{0:000}", Convert.ToInt16(numberPeriod)) + "B" + string.Format("{0:000}", bankInterbank) + "_INTERBANCOS.PDF";
                    } else {
                        fileNamePdfPM = "CHQ_NOMINAS_E" + keyBusiness.ToString() + "A" + string.Format("{0:00}", (yearPeriod % 100)) + "P" + string.Format("{0:000}", Convert.ToInt16(numberPeriod)) + "B" + string.Format("{0:000}", bankInterbank) + "_INTERBANCOSESP.PDF";
                    }
                    // Genera el nombre de los archivos txt
                    if (turns == 1) {
                        if (bankInterbank == 72) {
                            fileNameTxtPM = "NOMINAS_" + "PAG" + string.Format("{0:000000}", Convert.ToInt32(datoCuentaClienteBancoEmpresaBean.iPlaza)) + "01.txt";
                        } else {
                            fileNameTxtPM = "NOMINAS_" + "E" + string.Format("{0:00}", keyBusiness.ToString()) + "A" + yearPeriod.ToString() + "P" + string.Format("{0:00}", Convert.ToInt16(numberPeriod)) + "B" + string.Format("{0:000}", bankInterbank) + "_INTERBANCOS.txt";
                        }
                    } else {
                        if (bankInterbank == 72) {
                            fileNameTxtPM = "NOMINAS_" + "PAG" + string.Format("{0:000000}", Convert.ToInt32(datoCuentaClienteBancoEmpresaBean.iPlaza)) + "01_ESP.txt";
                        } else {
                            fileNameTxtPM = "NOMINAS_" + "E" + string.Format("{0:00}", keyBusiness.ToString()) + "A" + yearPeriod.ToString() + "P" + string.Format("{0:00}", Convert.ToInt16(numberPeriod)) + "B" + string.Format("{0:000}", bankInterbank) + "_INTERBANCOSESP.txt";
                        }
                    }
                    // Ejecuta el sp que obtiene los datos de los depositos
                    if (turns == 1) {
                        listDatosProcesaChequesNominaBean = dataDispersionBusiness.sp_Procesa_Cheques_Interbancarios(keyBusiness, typePeriod, numberPeriod, yearPeriod);
                    } else {
                        listDatosProcesaChequesNominaBean = dataDispersionBusiness.sp_Procesa_Cheques_Interbancarios_Espejo(keyBusiness, typePeriod, numberPeriod, yearPeriod);
                    }
                    if (listDatosProcesaChequesNominaBean.Count > 0) {
                        FileStream fs = new FileStream(directoryTxt + @"\\" + nameFolder + @"\\" + fileNamePdfPM, FileMode.Create);
                        Document doc = new Document(iTextSharp.text.PageSize.LETTER, 20, 40, 20, 40);
                        PdfWriter pw = PdfWriter.GetInstance(doc, fs);
                        doc.AddTitle("Reporte de Dispersion");
                        doc.AddAuthor("");
                        doc.Open();

                        // Creamos el tipo de Font que vamos utilizar
                        iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                        // Escribimos el encabezamiento en el documento
                        Font fontDefault = new Font(FontFamily.HELVETICA, 10);
                        Paragraph pr = new Paragraph();
                        DateTime datePdf = DateTime.Now;
                        pr.Font = fontDefault;
                        pr.Add("Fecha: " + datePdf.ToString("yyyy-MM-dd") + " Periodo: " + numberPeriod.ToString());
                        pr.Alignment = Element.ALIGN_LEFT;
                        doc.Add(pr);
                        pr.Clear();
                        pr.Add("IPSNet \n Dépositos " + datosEmpresaBeanDispersion.sDescripcion);
                        pr.Alignment = Element.ALIGN_CENTER;
                        doc.Add(pr);
                        doc.Add(Chunk.NEWLINE);
                        pr.Clear();
                        pr.Add(datosEmpresaBeanDispersion.sNombreEmpresa + "\n" + datosEmpresaBeanDispersion.sRfc);
                        pr.Alignment = Element.ALIGN_CENTER;
                        doc.Add(pr);
                        doc.Add(Chunk.NEWLINE);
                        // Creamos una tabla que contendrá los datos
                        PdfPTable tblPrueba = new PdfPTable(4);
                        tblPrueba.WidthPercentage = 100;
                        PdfPCell clCtaCheques = new PdfPCell(new Phrase("Cta. Cheques", _standardFont));
                        clCtaCheques.BorderWidth = 0;
                        clCtaCheques.BorderWidthBottom = 0.75f;
                        clCtaCheques.Bottom = 80;
                        PdfPCell clBeneficiario = new PdfPCell(new Phrase("Beneficiario", _standardFont));
                        clBeneficiario.BorderWidth = 0;
                        clBeneficiario.BorderWidthBottom = 0.75f;
                        clBeneficiario.Bottom = 60;
                        PdfPCell clImporte = new PdfPCell(new Phrase("Importe", _standardFont));
                        clImporte.BorderWidth = 0;
                        clImporte.BorderWidthBottom = 0.75f;
                        clImporte.Bottom = 40;
                        PdfPCell clNomina = new PdfPCell(new Phrase("Nomina", _standardFont));
                        clNomina.BorderWidth = 0;
                        clNomina.BorderWidthBottom = 0.75f;
                        clNomina.Bottom = 20;
                        // Añadimos las celdas a la tabla
                        //tblPrueba.AddCell(consecutive);
                        tblPrueba.AddCell(clCtaCheques);
                        tblPrueba.AddCell(clBeneficiario);
                        tblPrueba.AddCell(clImporte);
                        tblPrueba.AddCell(clNomina);
                        int dc = 0;
                        double sumTotal = 0;
                        int cantidadEmpleados = 0;
                        double renglon1481 = 0;
                        foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean) {
                            dc += 1;
                            // INICIO CODIGO NUEVO (RESTA RENGLON 1481)
                            double restaImporte = 0;
                            double importeFinal = 0;
                            renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                    Convert.ToInt32(payroll.sNomina), numberPeriod, typePeriod, yearPeriod);
                            if (renglon1481 > 0) {
                                restaImporte = payroll.doImporte - renglon1481;
                                importeFinal = restaImporte;
                            } else {
                                importeFinal = payroll.doImporte;
                            }
                            // FIN CODIGO NUEVO
                            //sumTotal += payroll.doImporte;
                            sumTotal += importeFinal;
                            cantidadEmpleados += 1;
                            clCtaCheques = new PdfPCell(new Phrase(payroll.sCuenta, _standardFont));
                            clCtaCheques.BorderWidth = 0;
                            clCtaCheques.Bottom = 80;
                            clBeneficiario = new PdfPCell(new Phrase(payroll.sNombre + " " + payroll.sPaterno + " " + payroll.sMaterno, _standardFont));
                            clBeneficiario.BorderWidth = 0;
                            clBeneficiario.Bottom = 80;
                            //clImporte = new PdfPCell(new Phrase("$ " + Convert.ToDecimal(payroll.doImporte).ToString("#,##0.00"), _standardFont));
                            clImporte = new PdfPCell(new Phrase("$ " + Convert.ToDecimal(importeFinal).ToString("#,##0.00"), _standardFont));
                            clImporte.BorderWidth = 0;
                            clImporte.Bottom = 80;
                            clNomina = new PdfPCell(new Phrase(payroll.sNomina, _standardFont));
                            clNomina.BorderWidth = 0;
                            clNomina.Bottom = 80;
                            tblPrueba.AddCell(clCtaCheques);
                            tblPrueba.AddCell(clBeneficiario);
                            tblPrueba.AddCell(clImporte);
                            tblPrueba.AddCell(clNomina);
                        }
                        doc.Add(tblPrueba);
                        doc.Add(Chunk.NEWLINE);
                        iTextSharp.text.Font _standardFont2 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                        // Creamos una tabla que contendrá los datos
                        PdfPTable tblTotal = new PdfPTable(1);
                        tblTotal.WidthPercentage = 100;
                        PdfPCell clTotal = new PdfPCell(new Phrase("Total: " + "$ " + sumTotal.ToString("#,##0.00"), _standardFont2));
                        clTotal.BorderWidth = 0;
                        clTotal.Bottom = 80;
                        tblTotal.AddCell(clTotal);
                        doc.Add(tblTotal);
                        // Creamos la tabla del total de empleados
                        PdfPTable tblTotalEmpleados = new PdfPTable(1);
                        tblTotalEmpleados.WidthPercentage = 100;
                        PdfPCell clTotalEmpleados = new PdfPCell(new Phrase("Empleados: " + cantidadEmpleados.ToString() + " registros.", _standardFont2));
                        clTotalEmpleados.BorderWidth = 0;
                        clTotalEmpleados.Bottom = 80;
                        tblTotalEmpleados.AddCell(clTotalEmpleados);
                        doc.Add(tblTotalEmpleados);
                        doc.Close();
                        pw.Close();

                        // BANCOMER -> INTERBANCARIO -> OK -> LISTO
                        if (bankInterbank == 12) {
                            // Longitudes Header
                            int longNumberRegistersOK = 7;
                            int longAmountTotalOK = 15;
                            int longNumberRegistersNOK = 7;
                            int longAmountTotalNOK = 15;
                            int longContractPagel = 12;
                            int longNumberBusiness = 10;
                            int longCodeLeyend = 3;
                            int longTypeService = 3;
                            int longFillerHeader = 142;


                            // Header
                            string identifierHeader = "1";
                            int numberRegisters     = 0;
                            // Importe total y cantidad de registros
                            ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                            ReportesDao reportDao = new ReportesDao();
                            decimal resultadoSuma = 0;
                            foreach (DatosProcesaChequesNominaBean bank in listDatosProcesaChequesNominaBean) {
                                renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness, Convert.ToInt32(bank.sNomina), numberPeriod, typePeriod, yearPeriod);
                                if (renglon1481 > 0) {
                                    resultadoSuma += importe.decimalTotalDispersion;
                                } else {
                                    resultadoSuma += bank.dImporte;
                                }
                                numberRegisters += 1;
                            }
                            int resultNumberRegisters = longNumberRegistersOK - numberRegisters.ToString().Length;
                            string numberRegistersOkWrite = "";
                            for (var i = 0; i < resultNumberRegisters; i++) {
                                numberRegistersOkWrite += "0";
                            }
                            numberRegistersOkWrite = numberRegistersOkWrite + numberRegisters.ToString();
                            string amountTotalOk = resultadoSuma.ToString().Replace(",", "").Replace(".", "");
                            int resultAmountTotalOk = longAmountTotalOK - amountTotalOk.Length;
                            string amountTotalOkWrite = "";
                            for (var i = 0; i < resultAmountTotalOk; i++) {
                                amountTotalOkWrite += "0";
                            }
                            amountTotalOkWrite = amountTotalOkWrite + amountTotalOk;
                            string numberRegistersNOkWrite = "0000000";
                            string amountTotalNOkWrite     = "000000000000000";
                            string contractPagel           = "000000000000";
                            string numberBusinessContract  = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta; // TODO: Verificar si es el numero de cliente o numero de cuenta
                            int resultNumberBusinessContract = longNumberBusiness - numberBusinessContract.Length;
                            string numberBusinessContractWrite = "";
                            for (var i = 0; i < resultNumberBusinessContract; i++) {
                                numberBusinessContractWrite += 0;
                            }
                            numberBusinessContractWrite = numberBusinessContractWrite + numberBusinessContract;
                            string codeLeyend  = "R05";
                            string typeService = "101"; // 101 -> Nomina tradicional, 110 -> Tarjeta de pagos
                            string keyEntry    = "1"; // FIJO
                            string creationDateFileTxt = DateTime.Now.ToString("yyyyMMdd");
                            string dateOfOperation = (dateDisC != "") ? DateTime.Parse("dateDisC").ToString("yyyyMMdd") : DateTime.Now.ToString("yyyyMMdd");
                            string fillerHeader = "                                                                                                                                              ";
                            // Estructura Header
                            string header = identifierHeader + numberRegistersOkWrite + amountTotalOkWrite + numberRegistersNOkWrite + amountTotalNOkWrite + contractPagel + numberBusinessContractWrite + codeLeyend + typeService + keyEntry + creationDateFileTxt + dateOfOperation + fillerHeader;
                            using (StreamWriter fileBancomer = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + fileNameTxtPM, false, new ASCIIEncoding())) {
                                fileBancomer.Write(header + "\n");
                                int registerNumber = 0;
                                Random random = new Random();
                                List<int> listReferences = new List<int>();
                                foreach (DatosProcesaChequesNominaBean bank in listDatosProcesaChequesNominaBean) {
                                    // Longitudes
                                    int longRfcOCurp      = 18;
                                    int longNumberAccount = 16;
                                    int longAmountPaid    = 15;
                                    int longNameEmployee  = 40;
                                    // Detalle
                                    string identifierDetail = "3"; // Valor fijo
                                    // Referencia númerica
                                    string referenceNumber = "";
                                    if (bank.iIdBanco == 12) {
                                        referenceNumber = "0000000";
                                    } else {
                                        registerNumber += 1;
                                        int randomReference = random.Next(1000000, 7000000);// TODO: Confirmar si la referencia numerica es aleatoria en cada registro o unica
                                        if (listReferences.Count > 0) {
                                            int i = 0;
                                            while (i == 0) {
                                                if (listReferences.Contains(randomReference)) {
                                                    randomReference = random.Next(1000000, 7000000);
                                                } else {
                                                    i = 1;
                                                }
                                            }
                                        }
                                        referenceNumber = randomReference.ToString();
                                        listReferences.Add(randomReference);

                                    }
                                    // Rfc O Curp
                                    int resultRfcOCurp = longRfcOCurp - bank.sRfc.Length;
                                    string rfcOCurpWrite = "";
                                    for (var i = 0; i < resultRfcOCurp; i++) {
                                        rfcOCurpWrite += " ";
                                    }
                                    rfcOCurpWrite = bank.sRfc + rfcOCurpWrite;
                                    // Tipo de cuenta
                                    string typeAccount = "";
                                    if (bank.sCuenta.Length == 16) { // Tarjeta de debito (TODO : VERIFICAR)
                                        typeAccount = "03";
                                    } else if (bank.sCuenta.Length == 18) { // Cuenta CLABE (TODO : VERIFICAR)
                                        typeAccount = "40";
                                    } else if (bank.sCuenta.Length == 11 || bank.sCuenta.Length == 10) { // Cuenta de cheques (TODO : VERIFICAR)
                                        typeAccount = "01";
                                    } else {
                                        typeAccount = "98";
                                    }
                                    // Banco destino
                                    string destinationBank = "";
                                    if (bank.iIdBanco.ToString().Length == 1) {
                                        destinationBank = "00" + bank.iIdBanco.ToString();
                                    } else if (bank.iIdBanco.ToString().Length == 2) {
                                        destinationBank = "0" + bank.iIdBanco.ToString();
                                    } else if (bank.iIdBanco.ToString().Length == 3) {
                                        destinationBank = bank.iIdBanco.ToString();
                                    }
                                    // Plaza destino
                                    string destinationSquare = "";
                                    if (bank.sCuenta.Length == 18) {
                                        destinationSquare = bank.sCuenta.Substring(3, 3); // Si es cuenta clabe obtener el numero de la plaza de la cuenta posiciones 4, 5 y 6
                                    } else {
                                        destinationSquare = "000";
                                    }
                                    // Numero de cuenta
                                    string destinationAccount = "";
                                    if (bank.iIdBanco == 12) {
                                        if (bank.sCuenta.Length == 18) { // Cuenta nomina recortar a 10 caracteres ( TODO: Verificar )
                                            destinationAccount = bank.sCuenta.Substring(8, 10);
                                        } else if (bank.sCuenta.Length == 16) { // Tarjeta de debito nomina ( TODO: Verificar )
                                            destinationAccount = bank.sCuenta;
                                        } else if (bank.sCuenta.Length == 11) { // Cuenta nomina a 11 caracteres recortar a 10 ( TODO: Verificar )
                                            destinationAccount = bank.sCuenta.Substring(1, 10);
                                        }
                                    } else {
                                        if (bank.sCuenta.Length == 18) { // Cuenta clabe 12 posiciones interbancaria
                                            destinationAccount = bank.sCuenta.Substring(6, 12);
                                        } else if (bank.sCuenta.Length == 16) { // Tarjeta de debito 16 posiciones interbancaria
                                            destinationAccount = bank.sCuenta;
                                        }
                                    }
                                    int resultDestinationAccount   = longNumberAccount - destinationAccount.Length;
                                    string destinationAccountWrite = "";
                                    for ( var i = 0; i < resultDestinationAccount; i++ ) {
                                        destinationAccountWrite += "0";
                                    }
                                    destinationAccountWrite = destinationAccountWrite + destinationAccount;
                                    // Importe del pago
                                    string amountPaid      = bank.dImporte.ToString().Replace(",", ".").Replace(".", "");
                                    int resultAmountPaid   = longAmountPaid - amountPaid.Length;
                                    string amountPaidWrite = "";
                                    for ( var i = 0; i < resultAmountPaid; i++ ) {
                                        amountPaidWrite += "0";
                                    }
                                    amountPaidWrite = amountPaidWrite + amountPaid;
                                    // Codigo del estado del pago
                                    string codeStatePaid = "0000000";
                                    // Descripcion del estado del pago
                                    string descriptionStatePaid = "                                                                                ";
                                    // Titular de la cuenta
                                    string nameEmployee    = bank.sNombre.Trim() + " " + bank.sPaterno.Trim() + " " + bank.sMaterno.Trim();
                                    string nameEmployeeW   = (nameEmployee.Length > 40) ? nameEmployee.Substring(0, 40) : nameEmployee;
                                    int resultNameEmployee = longNameEmployee - nameEmployeeW.Length;
                                    string nameEmployeeWrite = "";
                                    for ( var i = 0; i < resultNameEmployee; i++ ) {
                                        nameEmployeeWrite += " ";
                                    }
                                    nameEmployeeWrite = nameEmployee + nameEmployeeWrite;
                                    // Motivo de pago
                                    string reasonForPayment = "                                        ";
                                    if (bank.iIdBanco != 12) {
                                        reasonForPayment = "PAGO INTERBANCARIO                      ";
                                    }
                                    // Estructura del detalle
                                    string detail = identifierDetail + referenceNumber + rfcOCurpWrite + typeAccount + destinationBank + destinationSquare + destinationAccountWrite + amountPaidWrite + codeStatePaid + descriptionStatePaid + nameEmployeeWrite + reasonForPayment;
                                    fileBancomer.Write(detail + "\n");
                                }
                                fileBancomer.Close();
                            }
                            if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48) {
                                // CÓDIGO NUEVO INTERBANCARIO BANCOMER \\
                                // Cada archivo debe de separarse mediante 200 registros
                                int contador = 0;
                                int vueltas = 1;
                                List<DatosProcesaChequesNominaBean> bancomerNew = new List<DatosProcesaChequesNominaBean>();
                                foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean)
                                {
                                    if (contador > 199)
                                    {
                                        contador = 0;
                                        vueltas += 1;
                                    }
                                    contador += 1;
                                    bancomerNew.Add(new DatosProcesaChequesNominaBean { iIdBanco = payroll.iIdBanco, dImporte = payroll.dImporte, doImporte = payroll.doImporte, iIdEmpresa = payroll.iIdEmpresa, iTipoPago = payroll.iTipoPago, sBanco = payroll.sBanco, sCodigo = payroll.sCodigo, sCuenta = payroll.sCuenta, sImporte = payroll.sImporte, sMaterno = payroll.sMaterno, sNombre = payroll.sNombre, sNomina = payroll.sNomina, sPaterno = payroll.sPaterno, sRfc = payroll.sRfc, iCodigoTXT = vueltas });
                                }
                                int z = 1;
                                while (z <= vueltas)
                                {
                                    using (StreamWriter fileBancomer = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_BBVA_" + keyBusiness.ToString() + "_P" + z.ToString() + ".txt", false, new ASCIIEncoding()))
                                    {
                                        foreach (DatosProcesaChequesNominaBean payroll in bancomerNew)
                                        {
                                            if (payroll.iCodigoTXT == z)
                                            {
                                                int longAccountDestinty = 18;
                                                string cerosAccountDestiny = "";
                                                int resultCerosAccountDestiny = 0;
                                                string accountDestiny = payroll.sCuenta;
                                                if (payroll.sCuenta.Length == 18)
                                                {
                                                    accountDestiny = accountDestiny.Substring(7, 10);
                                                }
                                                resultCerosAccountDestiny = longAccountDestinty - accountDestiny.Length;
                                                for (var u = 0; u < resultCerosAccountDestiny; u++)
                                                {
                                                    cerosAccountDestiny += "0";
                                                }
                                                accountDestiny = cerosAccountDestiny + accountDestiny;
                                                int longAccountOrigin = 18;
                                                string cerosAccountOrigin = "";
                                                int resultCerosAccountOrigin = 0;
                                                string accountOrigin = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                                resultCerosAccountOrigin = longAccountOrigin - accountOrigin.Length;
                                                for (var h = 0; h < resultCerosAccountOrigin; h++)
                                                {
                                                    cerosAccountOrigin += "0";
                                                }
                                                accountOrigin = cerosAccountOrigin + accountOrigin;
                                                string divisaOperation = "MXP";
                                                int longAmountOperation = 16;
                                                string cerosAmountOperation = "";
                                                string amountOperation = payroll.doImporte.ToString("0.00");
                                                int resultCerosAmountOperation = longAmountOperation - amountOperation.Length;
                                                for (var v = 0; v < resultCerosAmountOperation; v++)
                                                {
                                                    cerosAmountOperation += "0";
                                                }
                                                amountOperation = cerosAmountOperation + amountOperation;
                                                int longNameEmployee = 30;
                                                string spacesNameEmployee = "";
                                                string nameEmployee = payroll.sNombre + " " + payroll.sPaterno + " " + payroll.sMaterno;
                                                if (nameEmployee.Length < 30)
                                                {
                                                    int resultSpacesNameEmployee = longNameEmployee - nameEmployee.Length;
                                                    for (var b = 0; b < resultSpacesNameEmployee; b++)
                                                    {
                                                        spacesNameEmployee += " ";
                                                    }
                                                    nameEmployee = nameEmployee + spacesNameEmployee;
                                                }
                                                else if (nameEmployee.Length > 30)
                                                {
                                                    nameEmployee = nameEmployee.Substring(0, 30);
                                                }
                                                string typeAccount = "40";
                                                string codeBank = "";
                                                if (payroll.iIdBanco.ToString().Length == 1)
                                                {
                                                    codeBank = "00" + payroll.iIdBanco.ToString();
                                                }
                                                else if (payroll.iIdBanco.ToString().Length == 2)
                                                {
                                                    codeBank = "0" + payroll.iIdBanco.ToString();
                                                }
                                                else if (payroll.iIdBanco.ToString().Length == 3)
                                                {
                                                    codeBank = payroll.iIdBanco.ToString();
                                                }
                                                string referenceNumber = (payroll.sNomina.Length > 7) ? payroll.sNomina.Substring(0, 7) : payroll.sNomina;
                                                int resultCerosReferenceNumber = (referenceNumber.Length < 7) ? 7 - referenceNumber.Length : 0;
                                                string cerosReferenceNumber = "";
                                                for (var g = 0; g < resultCerosReferenceNumber; g++)
                                                {
                                                    cerosReferenceNumber += "0";
                                                }
                                                referenceNumber = cerosReferenceNumber + referenceNumber;
                                                string disponibility = "H";
                                                string reasonForPayment = keyBusiness.ToString() + " PAGO POR CUENTAY ORDEN SUEL";
                                                fileBancomer.WriteLine(accountDestiny + accountOrigin + divisaOperation + amountOperation + nameEmployee + typeAccount + codeBank + reasonForPayment + referenceNumber + disponibility);
                                            }
                                        }
                                        fileBancomer.Close();
                                    }
                                    z += 1;
                                }
                            }
                        }

                        // SANTANDER -> INTERBANCARIO -> OK -> LISTO
                        if (bankInterbank == 14) {
                            // SANTANDER - ARCHIVO OK (INTERBANCARIO)
                            // - DETALLE - \\
                            string campoFijoIntSantanderD1 = "NOMINA";
                            string fillerIntSantanderD3 = "                                                                                                                            ";
                            if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48) {
                                campoFijoIntSantanderD1 = keyBusiness.ToString() + " PAGO POR CUENTA Y ORDEN SUELDO";
                                fillerIntSantanderD3 = "                                                                                                 ";
                            }
                            if (tipPago == 2) {
                                campoFijoIntSantanderD1 = "HON";
                                fillerIntSantanderD3 = "                                                                                                                               ";
                            }
                            string numCuentaEmpresaSantanderD = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta, 
                                fillerIntSantanderD1          = "     ", 
                                fillerIntSantanderD2          = "  ", 
                                sucursalIntSantanderD1        = "1001", 
                                plazaIntSantanderD1           = "01001";
                            int consecutivoIntSantanderD1     = 0;
                            if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48) {
                                // CÓDIGO NUEVO
                                // Separa archivos con 500 registros cada uno
                                List<DatosProcesaChequesNominaBean> santanderNew = new List<DatosProcesaChequesNominaBean>();
                                int contador = 0;
                                int vueltas = 1;
                                foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean)
                                {
                                    if (contador > 499)
                                    {
                                        contador = 0;
                                        vueltas += 1;
                                    }
                                    contador += 1;
                                    santanderNew.Add(new DatosProcesaChequesNominaBean { iIdBanco = payroll.iIdBanco, dImporte = payroll.dImporte, doImporte = payroll.doImporte, iIdEmpresa = payroll.iIdEmpresa, iTipoPago = payroll.iTipoPago, sBanco = payroll.sBanco, sCodigo = payroll.sCodigo, sCuenta = payroll.sCuenta, sImporte = payroll.sImporte, sMaterno = payroll.sMaterno, sNombre = payroll.sNombre, sNomina = payroll.sNomina, sPaterno = payroll.sPaterno, sRfc = payroll.sRfc, iCodigoTXT = vueltas });
                                }
                                int z = 1;
                                while (z <= vueltas)
                                {
                                    using (StreamWriter fileIntSantander = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_SANT_" + keyBusiness.ToString() + "_P" + z.ToString() + ".txt"))
                                    {
                                        string espaciosNomEmpIntSantander = "",
                                            nombreEmpIntSantander = "",
                                            cerosImpIntSantander = "",
                                            cerosConIntSantander = "";
                                        int longNomEmpIntSan = 40,
                                            longImpIntSan = 15,
                                            longConIntSan = 7;
                                        foreach (DatosProcesaChequesNominaBean bank in santanderNew)
                                        {
                                            if (bank.iCodigoTXT == z)
                                            {
                                                consecutivoIntSantanderD1 += 1;
                                                string nameEmployee = bank.sNombre.Replace("Ñ", "N") + " " + bank.sPaterno.Replace("Ñ", "N")
                                                    + " " + bank.sMaterno.Replace("Ñ", "N");
                                                if (nameEmployee.Length > 40)
                                                {
                                                    nombreEmpIntSantander = nameEmployee.Substring(0, 39);
                                                }
                                                else
                                                {
                                                    nombreEmpIntSantander = nameEmployee;
                                                }
                                                string cuentaFinal = bank.sCuenta;
                                                if (bank.sCuenta.Length == 16)
                                                {
                                                    cuentaFinal = "00" + bank.sCuenta;
                                                }
                                                ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                                                ReportesDao reportDao = new ReportesDao();
                                                string importeFinal = "";
                                                renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                        Convert.ToInt32(bank.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                if (renglon1481 > 0)
                                                {
                                                    importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                        Convert.ToInt32(bank.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                    importeFinal = importe.decimalTotalDispersion.ToString();
                                                }
                                                else
                                                {
                                                    importeFinal = bank.dImporte.ToString();
                                                }
                                                string clave = "       ";
                                                if (bank.sCodigo != "") {
                                                    clave = "  " + bank.sCodigo;
                                                }
                                                int longNomEmpIntSantanderResult = longNomEmpIntSan - nombreEmpIntSantander.Length;
                                                int longImpIntSantanderResult = longImpIntSan - importeFinal.ToString().Length;
                                                int longConIntSantanderResult = longConIntSan - consecutivoIntSantanderD1.ToString().Length;
                                                for (var b = 0; b < longNomEmpIntSantanderResult; b++) { espaciosNomEmpIntSantander += " "; }
                                                for (var t = 0; t < longImpIntSantanderResult; t++) { cerosImpIntSantander += "0"; }
                                                for (var p = 0; p < longConIntSantanderResult; p++) { cerosConIntSantander += "0"; }
                                                fileIntSantander.Write(numCuentaEmpresaSantanderD + fillerIntSantanderD1 + cuentaFinal
                                                    + fillerIntSantanderD2 + clave + nombreEmpIntSantander + espaciosNomEmpIntSantander
                                                    + sucursalIntSantanderD1 + cerosImpIntSantander + importeFinal + plazaIntSantanderD1
                                                    + campoFijoIntSantanderD1 + fillerIntSantanderD3 + cerosConIntSantander
                                                    + consecutivoIntSantanderD1.ToString() + "\n");
                                                espaciosNomEmpIntSantander = "";
                                                cerosImpIntSantander = "";
                                                cerosConIntSantander = "";
                                            }
                                        }
                                        fileIntSantander.Close();
                                    }
                                    z += 1;
                                }
                            } else {
                                using (StreamWriter fileIntSantander = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_SANT_" + keyBusiness.ToString() +".txt"))
                                {
                                    string espaciosNomEmpIntSantander = "",
                                        nombreEmpIntSantander = "",
                                        cerosImpIntSantander = "",
                                        cerosConIntSantander = "";
                                    int longNomEmpIntSan = 40,
                                        longImpIntSan = 15,
                                        longConIntSan = 7;
                                    foreach (DatosProcesaChequesNominaBean bank in listDatosProcesaChequesNominaBean)
                                    {
                                        if (1 == 1)
                                        {
                                            consecutivoIntSantanderD1 += 1;
                                            string nameEmployee = bank.sNombre.Replace("Ñ", "N") + " " + bank.sPaterno.Replace("Ñ", "N")
                                                + " " + bank.sMaterno.Replace("Ñ", "N");
                                            if (nameEmployee.Length > 40)
                                            {
                                                nombreEmpIntSantander = nameEmployee.Substring(0, 39);
                                            }
                                            else
                                            {
                                                nombreEmpIntSantander = nameEmployee;
                                            }
                                            string cuentaFinal = bank.sCuenta;
                                            if (bank.sCuenta.Length == 16)
                                            {
                                                cuentaFinal = "00" + bank.sCuenta;
                                            }
                                            ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                                            ReportesDao reportDao = new ReportesDao();
                                            string importeFinal = "";
                                            renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                    Convert.ToInt32(bank.sNomina), numberPeriod, typePeriod, yearPeriod);
                                            if (renglon1481 > 0)
                                            {
                                                importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                    Convert.ToInt32(bank.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                importeFinal = importe.decimalTotalDispersion.ToString();
                                            }
                                            else
                                            {
                                                importeFinal = bank.dImporte.ToString();
                                            }
                                            string clave = bank.sCodigo;
                                            int longNomEmpIntSantanderResult = longNomEmpIntSan - nombreEmpIntSantander.Length;
                                            int longImpIntSantanderResult = longImpIntSan - importeFinal.ToString().Length;
                                            int longConIntSantanderResult = longConIntSan - consecutivoIntSantanderD1.ToString().Length;
                                            for (var b = 0; b < longNomEmpIntSantanderResult; b++) { espaciosNomEmpIntSantander += " "; }
                                            for (var t = 0; t < longImpIntSantanderResult; t++) { cerosImpIntSantander += "0"; }
                                            for (var p = 0; p < longConIntSantanderResult; p++) { cerosConIntSantander += "0"; }
                                            fileIntSantander.Write(numCuentaEmpresaSantanderD + fillerIntSantanderD1 + cuentaFinal
                                                + fillerIntSantanderD2 + clave + nombreEmpIntSantander + espaciosNomEmpIntSantander
                                                + sucursalIntSantanderD1 + cerosImpIntSantander + importeFinal + plazaIntSantanderD1
                                                + campoFijoIntSantanderD1 + fillerIntSantanderD3 + cerosConIntSantander
                                                + consecutivoIntSantanderD1.ToString() + "\n");
                                            espaciosNomEmpIntSantander = "";
                                            cerosImpIntSantander = "";
                                            cerosConIntSantander = "";
                                        }
                                    }
                                    fileIntSantander.Close();
                                }
                            }
                            
                            // # [ FIN -> CREACION DE DISPERSION DE SANTANDER (INTERBANCARIO) ] * \\
                        }

                        // SCOTIABANK -> INTERBANCARIO -> OK -> LISTO
                        if (bankInterbank == 44) {
                            decimal resultadoSuma = 0;
                            // SCOTIABANK -- ARCHIVO OK (INTERBANCARIO)
                            string tipoArchivoIntScotiabank = "EE", 
                                tipoRegistroIntScotiabank   = "HA", 
                                numeroContratoIntScotiabank = "", 
                                secuenciaIntScotiabank      = "01",
                                fillerIntScotiabankHA1      = "                                                                                                                                                                                                                                                                                                                                                                       ";

                            if (keyBusiness == 8) {
                                numeroContratoIntScotiabank = "88178";
                            } else if (keyBusiness == 10) {
                                numeroContratoIntScotiabank = "47848";
                            } else if (keyBusiness == 7) {
                                numeroContratoIntScotiabank = "48426";
                            } else if (keyBusiness == 15) {
                                numeroContratoIntScotiabank = "85301";
                            } else {
                                numeroContratoIntScotiabank = "00000";
                            }

                            numeroContratoIntScotiabank = datoCuentaClienteBancoEmpresaBean.sNumeroCliente;

                            string headerLayoutAIntScotiabank = tipoArchivoIntScotiabank + tipoRegistroIntScotiabank 
                                + numeroContratoIntScotiabank + secuenciaIntScotiabank + fillerIntScotiabankHA1;
                            // - ENCABEZADO BLOQUE - \\
                            string tipoRegistroBIntScotiabank = "HB", 
                                monedaCuentaBIntScotiabank = "00", 
                                usoFuturoIntScotiabank = "0000", 
                                cuentaCargoIntScotiabank = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta, 
                                referenciaEmpresaIntScotiabank = "0000000001", 
                                codigoStatusIntScotiabank = "000", 
                                fillerIntScotiabankHB1 = "                                                                                                                                                                                                                                                                                                                                                ";
                            string headerLayoutBIntScotiabank = tipoArchivoIntScotiabank + tipoRegistroBIntScotiabank 
                                + monedaCuentaBIntScotiabank + usoFuturoIntScotiabank + cuentaCargoIntScotiabank 
                                + referenciaEmpresaIntScotiabank + codigoStatusIntScotiabank + fillerIntScotiabankHB1;
                            // - DETALLE - \\
                            string fechaIntScotiabankD = dateGeneration.ToString("yyyyMMdd");
                            if (dateDisC != "") {
                                fechaIntScotiabankD = DateTime.Parse(dateDisC.ToString()).ToString("yyyyMMdd");
                            }
                            string conceptoPagoIntScotiabankD = "PAGO NOMINA";
                            if (tipPago == 2) {
                                conceptoPagoIntScotiabankD = "HONORARIOS ";
                            }
                            string tipoRegistroCIntScotiabankD = "DA", 
                                tipoPagoIntScotiabankD         = "04", 
                                claveMonedaIntScotiabank       = "00",  
                                servicioIntScotiabankD         = "01", 
                                fillerIntScotiabankD1          = "                            ", 
                                plazaIntScotiabankD            = "00000", 
                                sucursalIntScotiabankD         = "00000", 
                                paisIntScotiabankD             = "00000", 
                                fillerIntScotiabankD2          = "                                        ", 
                                tipoCuentaIntScotiabankD1      = "9", 
                                digitoIntScotiabankD1          = " ", 
                                bancoEmisorIntScotiabankD1     = "044", 
                                diasVigenciaIntScotiabankD     = "001",  
                                fillerIntScotiabankD3          = "                                       ", 
                                fillerIntScotiabankD4          = "                                                            ", 
                                fillerIntScotiabankD5          = "                      ";
                            int consecutivoIntScotiabankD1     = 0;
                            if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48) {
                                conceptoPagoIntScotiabankD = keyBusiness.ToString() + " PAGO POR CUENTA Y ORDEN SUELDO";
                                fillerIntScotiabankD3 = "                 ";
                            }
                            // - CREACION DE LISTA PARA LLENAR EL DETALLE - \\
                            if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48) {
                                int cont = 0;
                                int vueltas = 1;
                                List<DatosProcesaChequesNominaBean> scotiabankNew = new List<DatosProcesaChequesNominaBean>();
                                foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean)
                                {
                                    if (cont > 499)
                                    {
                                        cont = 0;
                                        vueltas += 1;
                                    }
                                    cont += 1;
                                    scotiabankNew.Add(new DatosProcesaChequesNominaBean { iIdBanco = payroll.iIdBanco, dImporte = payroll.dImporte, doImporte = payroll.doImporte, iIdEmpresa = payroll.iIdEmpresa, iTipoPago = payroll.iTipoPago, sBanco = payroll.sBanco, sCodigo = payroll.sCodigo, sCuenta = payroll.sCuenta, sImporte = payroll.sImporte, sMaterno = payroll.sMaterno, sNombre = payroll.sNombre, sNomina = payroll.sNomina, sPaterno = payroll.sPaterno, sRfc = payroll.sRfc, iCodigoTXT = vueltas });
                                }
                                int z = 1;
                                while (z <= vueltas)
                                {
                                    using (StreamWriter fileIntScotiabank = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_SCOT_" + keyBusiness.ToString() + "_P" + z.ToString() + ".txt"))
                                    {
                                        string sequentia = z.ToString();
                                        if (z.ToString().Length == 1)
                                        {
                                            sequentia = "0" + z.ToString();
                                        }
                                        headerLayoutAIntScotiabank = tipoArchivoIntScotiabank + tipoRegistroIntScotiabank + numeroContratoIntScotiabank + sequentia + fillerIntScotiabankHA1;
                                        fileIntScotiabank.Write(headerLayoutAIntScotiabank + "\n");
                                        fileIntScotiabank.Write(headerLayoutBIntScotiabank + "\n");
                                        string cerosImpIntScotiabankD = "",
                                            cerosNumNomIntScotiabankD = "",
                                            espaciosNomEmpIntScotiabankD = "",
                                            nombreEmpIntScotiabankD = "",
                                            cerosConsecIntScotiabankD1 = "",
                                            cerosCtaCheIntScotiabankD1 = "",
                                            cerosCodStaIntScotiabankD1 = "",
                                            cerosTotMovIntScotiabank = "",
                                            cerosImpTotIntScotiabank = "";
                                        int longImpIntScotiabankD = 15,
                                            longNumNomIntScotiabankD = 5,
                                            longNomEmpIntScotiabankD = 40,
                                            longConIntScotiabankD1 = 16,
                                            longCtaCheIntScotiabankD = 20,
                                            longCodStaIntScotiabankD = 25,
                                            totalMoviIntScotiabank = 0,
                                            longTotMovIntScotiabank = 7,
                                            importeTotalIntScotiabank = 0,
                                            longImpTotIntScotiabank = 17;
                                        resultadoSuma = 0;
                                        foreach (DatosProcesaChequesNominaBean bank in scotiabankNew)
                                        {
                                            if (bank.iCodigoTXT == z)
                                            {
                                                int contador = 1;
                                                int clvBank = bank.iIdBanco;
                                                string sufBank = "";
                                                if (clvBank.ToString().Length == 1)
                                                {
                                                    sufBank = "00" + clvBank.ToString();
                                                }
                                                else if (clvBank.ToString().Length == 2)
                                                {
                                                    sufBank = "0" + clvBank.ToString();
                                                }
                                                else
                                                {
                                                    sufBank = clvBank.ToString();
                                                }
                                                string nameEmployee = bank.sPaterno + " " + bank.sMaterno + " " + bank.sNombre;
                                                if (nameEmployee.Length > 40)
                                                {
                                                    nombreEmpIntScotiabankD = nameEmployee.Substring(0, 39);
                                                }
                                                else
                                                {
                                                    nombreEmpIntScotiabankD = nameEmployee;
                                                }
                                                int filler28 = 28;
                                                string filler28F = "";
                                                int payrollEmp = bank.sNomina.ToString().Length;
                                                int longPayroll = 5;
                                                int accortPayroll = payrollEmp - 5;
                                                if (accortPayroll != 0)
                                                {
                                                    int length28 = filler28;
                                                    if (accortPayroll > 0) {
                                                        length28 = filler28 - accortPayroll;
                                                    }
                                                    for (var v = 0; v < length28; v++)
                                                    {
                                                        filler28F += " ";
                                                    }
                                                }
                                                else
                                                {
                                                    filler28F = fillerIntScotiabankD1;
                                                }
                                                consecutivoIntScotiabankD1 += 1;
                                                totalMoviIntScotiabank += 1;
                                                ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                                                ReportesDao reportDao = new ReportesDao();
                                                string importeFinal = "";
                                                renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                        Convert.ToInt32(bank.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                if (renglon1481 > 0)
                                                {
                                                    importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                         Convert.ToInt32(bank.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                    importeFinal = importe.decimalTotalDispersion.ToString();
                                                    resultadoSuma += importe.decimalTotalDispersion;
                                                }
                                                else
                                                {
                                                    importeFinal = bank.dImporte.ToString();
                                                    resultadoSuma += bank.dImporte;
                                                }
                                                importeTotalIntScotiabank += Convert.ToInt32(importeFinal);
                                                int longImpIntScotiabankDResult = longImpIntScotiabankD - importeFinal.ToString().Length;
                                                int longNumNomIntScotiabankDResult = longNumNomIntScotiabankD - bank.sNomina.Length;
                                                int longNomEmpIntScotiabankDResult = longNomEmpIntScotiabankD - nombreEmpIntScotiabankD.Length;
                                                int longConsecIntScotiabankDResult = longConIntScotiabankD1 - consecutivoIntScotiabankD1.ToString().Length;
                                                int longCtaCheIntScotiabankDResult = longCtaCheIntScotiabankD - bank.sCuenta.Length;
                                                int longCodStaIntScotiabankDResult = longCodStaIntScotiabankD - bank.sCuenta.Length;
                                                for (var q = 0; q < longImpIntScotiabankDResult; q++)
                                                {
                                                    cerosImpIntScotiabankD += "0";
                                                }
                                                for (var a = 0; a < longNumNomIntScotiabankDResult; a++)
                                                {
                                                    cerosNumNomIntScotiabankD += "0";
                                                }
                                                for (var u = 0; u < longNomEmpIntScotiabankDResult; u++)
                                                {
                                                    espaciosNomEmpIntScotiabankD += " ";
                                                }
                                                for (var v = 0; v < longConsecIntScotiabankDResult; v++)
                                                {
                                                    cerosConsecIntScotiabankD1 += "0";
                                                }
                                                for (var r = 0; r < longCtaCheIntScotiabankDResult; r++)
                                                {
                                                    cerosCtaCheIntScotiabankD1 += "0";
                                                }
                                                for (var e = 0; e < longCodStaIntScotiabankDResult; e++)
                                                {
                                                    cerosCodStaIntScotiabankD1 += "0";
                                                }
                                                fileIntScotiabank.Write(tipoArchivoIntScotiabank + tipoRegistroCIntScotiabankD
                                                    + tipoPagoIntScotiabankD + claveMonedaIntScotiabank + cerosImpIntScotiabankD
                                                    + importeFinal + fechaIntScotiabankD + servicioIntScotiabankD + cerosNumNomIntScotiabankD
                                                    + bank.sNomina + filler28F + nombreEmpIntScotiabankD + espaciosNomEmpIntScotiabankD
                                                    + cerosConsecIntScotiabankD1 + consecutivoIntScotiabankD1.ToString()
                                                    + plazaIntScotiabankD + sucursalIntScotiabankD + cerosCtaCheIntScotiabankD1
                                                    + bank.sCuenta + paisIntScotiabankD + fillerIntScotiabankD2
                                                    + tipoCuentaIntScotiabankD1 + digitoIntScotiabankD1 + plazaIntScotiabankD
                                                    + bancoEmisorIntScotiabankD1 + sufBank + diasVigenciaIntScotiabankD
                                                    + conceptoPagoIntScotiabankD + fillerIntScotiabankD3 + fillerIntScotiabankD4
                                                    + cerosCodStaIntScotiabankD1 + bank.sCuenta + fillerIntScotiabankD5 + "\n");
                                                cerosImpIntScotiabankD = "";
                                                cerosNumNomIntScotiabankD = "";
                                                espaciosNomEmpIntScotiabankD = "";
                                                cerosConsecIntScotiabankD1 = "";
                                                cerosCtaCheIntScotiabankD1 = "";
                                                cerosCodStaIntScotiabankD1 = "";
                                            }
                                        }
                                        // - TRAILER BLOQUE - \\
                                        int longTotMovIntScotiabankResult = longTotMovIntScotiabank - totalMoviIntScotiabank.ToString().Length;
                                        int longImpTotIntScotiabankResult = longImpTotIntScotiabank - resultadoSuma.ToString().Length;
                                        string tipoRegistroDIntScotiabank = "TB", tipoRegistroEIntScotiabank = "TA", cantidadMovIntScotiabank = "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", fillerIntScotiabankTB = "                                                                                                                           ";
                                        for (var d = 0; d < longTotMovIntScotiabankResult; d++)
                                        {
                                            cerosTotMovIntScotiabank += "0";
                                        }
                                        for (var w = 0; w < longImpTotIntScotiabankResult; w++)
                                        {
                                            cerosImpTotIntScotiabank += "0";
                                        }
                                        string trailerBloqueIntScotiabank = tipoArchivoIntScotiabank + tipoRegistroDIntScotiabank
                                            + cerosTotMovIntScotiabank + totalMoviIntScotiabank.ToString() + cerosImpTotIntScotiabank
                                            + resultadoSuma.ToString() + cantidadMovIntScotiabank + fillerIntScotiabankTB;
                                        fileIntScotiabank.Write(trailerBloqueIntScotiabank + "\n");
                                        string trailerArchivoIntScotiabank = tipoArchivoIntScotiabank + tipoRegistroEIntScotiabank
                                            + cerosTotMovIntScotiabank + totalMoviIntScotiabank.ToString() + cerosImpTotIntScotiabank
                                            + resultadoSuma.ToString() + cantidadMovIntScotiabank + fillerIntScotiabankTB;
                                        fileIntScotiabank.Write(trailerArchivoIntScotiabank + "\n");
                                        // - TRAILER ARCHIVO - \\
                                        fileIntScotiabank.Close();
                                    }
                                    z += 1;
                                }
                            } else {
                                using (StreamWriter fileIntScotiabank = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_SCOT_" + keyBusiness.ToString() + ".txt"))
                                {
                                    headerLayoutAIntScotiabank = tipoArchivoIntScotiabank + tipoRegistroIntScotiabank + numeroContratoIntScotiabank + secuenciaIntScotiabank + fillerIntScotiabankHA1;
                                    fileIntScotiabank.Write(headerLayoutAIntScotiabank + "\n");
                                    fileIntScotiabank.Write(headerLayoutBIntScotiabank + "\n");
                                    string cerosImpIntScotiabankD = "",
                                        cerosNumNomIntScotiabankD = "",
                                        espaciosNomEmpIntScotiabankD = "",
                                        nombreEmpIntScotiabankD = "",
                                        cerosConsecIntScotiabankD1 = "",
                                        cerosCtaCheIntScotiabankD1 = "",
                                        cerosCodStaIntScotiabankD1 = "",
                                        cerosTotMovIntScotiabank = "",
                                        cerosImpTotIntScotiabank = "";
                                    int longImpIntScotiabankD = 15,
                                        longNumNomIntScotiabankD = 5,
                                        longNomEmpIntScotiabankD = 40,
                                        longConIntScotiabankD1 = 16,
                                        longCtaCheIntScotiabankD = 20,
                                        longCodStaIntScotiabankD = 25,
                                        totalMoviIntScotiabank = 0,
                                        longTotMovIntScotiabank = 7,
                                        importeTotalIntScotiabank = 0,
                                        longImpTotIntScotiabank = 17;
                                    resultadoSuma = 0;
                                    foreach (DatosProcesaChequesNominaBean bank in listDatosProcesaChequesNominaBean)
                                    {
                                        if (1 == 1)
                                        {
                                            int contador = 1;
                                            int clvBank = bank.iIdBanco;
                                            string sufBank = "";
                                            if (clvBank.ToString().Length == 1)
                                            {
                                                sufBank = "00" + clvBank.ToString();
                                            }
                                            else if (clvBank.ToString().Length == 2)
                                            {
                                                sufBank = "0" + clvBank.ToString();
                                            }
                                            else
                                            {
                                                sufBank = clvBank.ToString();
                                            }
                                            string nameEmployee = bank.sPaterno + " " + bank.sMaterno + " " + bank.sNombre;
                                            if (nameEmployee.Length > 40)
                                            {
                                                nombreEmpIntScotiabankD = nameEmployee.Substring(0, 39);
                                            }
                                            else
                                            {
                                                nombreEmpIntScotiabankD = nameEmployee;
                                            }
                                            int filler28 = 28;
                                            string filler28F = "";
                                            int payrollEmp = bank.sNomina.ToString().Length;
                                            int longPayroll = 5;
                                            int accortPayroll = payrollEmp - 5;
                                            if (accortPayroll != 0)
                                            {
                                                int length28 = filler28 - accortPayroll;
                                                for (var v = 0; v < length28; v++)
                                                {
                                                    filler28F += " ";
                                                }
                                            }
                                            else
                                            {
                                                filler28F = fillerIntScotiabankD1;
                                            }
                                            consecutivoIntScotiabankD1 += 1;
                                            totalMoviIntScotiabank += 1;
                                            ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                                            ReportesDao reportDao = new ReportesDao();
                                            string importeFinal = "";
                                            renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                    Convert.ToInt32(bank.sNomina), numberPeriod, typePeriod, yearPeriod);
                                            if (renglon1481 > 0)
                                            {
                                                importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                     Convert.ToInt32(bank.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                importeFinal = importe.decimalTotalDispersion.ToString();
                                                resultadoSuma += importe.decimalTotalDispersion;
                                            }
                                            else
                                            {
                                                importeFinal = bank.dImporte.ToString();
                                                resultadoSuma += bank.dImporte;
                                            }
                                            importeTotalIntScotiabank += Convert.ToInt32(importeFinal);
                                            int longImpIntScotiabankDResult = longImpIntScotiabankD - importeFinal.ToString().Length;
                                            int longNumNomIntScotiabankDResult = longNumNomIntScotiabankD - bank.sNomina.Length;
                                            int longNomEmpIntScotiabankDResult = longNomEmpIntScotiabankD - nombreEmpIntScotiabankD.Length;
                                            int longConsecIntScotiabankDResult = longConIntScotiabankD1 - consecutivoIntScotiabankD1.ToString().Length;
                                            int longCtaCheIntScotiabankDResult = longCtaCheIntScotiabankD - bank.sCuenta.Length;
                                            int longCodStaIntScotiabankDResult = longCodStaIntScotiabankD - bank.sCuenta.Length;
                                            for (var q = 0; q < longImpIntScotiabankDResult; q++)
                                            {
                                                cerosImpIntScotiabankD += "0";
                                            }
                                            for (var a = 0; a < longNumNomIntScotiabankDResult; a++)
                                            {
                                                cerosNumNomIntScotiabankD += "0";
                                            }
                                            for (var u = 0; u < longNomEmpIntScotiabankDResult; u++)
                                            {
                                                espaciosNomEmpIntScotiabankD += " ";
                                            }
                                            for (var v = 0; v < longConsecIntScotiabankDResult; v++)
                                            {
                                                cerosConsecIntScotiabankD1 += "0";
                                            }
                                            for (var r = 0; r < longCtaCheIntScotiabankDResult; r++)
                                            {
                                                cerosCtaCheIntScotiabankD1 += "0";
                                            }
                                            for (var e = 0; e < longCodStaIntScotiabankDResult; e++)
                                            {
                                                cerosCodStaIntScotiabankD1 += "0";
                                            }
                                            fileIntScotiabank.Write(tipoArchivoIntScotiabank + tipoRegistroCIntScotiabankD
                                                + tipoPagoIntScotiabankD + claveMonedaIntScotiabank + cerosImpIntScotiabankD
                                                + importeFinal + fechaIntScotiabankD + servicioIntScotiabankD + cerosNumNomIntScotiabankD
                                                + bank.sNomina + filler28F + nameEmployee + espaciosNomEmpIntScotiabankD
                                                + cerosConsecIntScotiabankD1 + consecutivoIntScotiabankD1.ToString()
                                                + plazaIntScotiabankD + sucursalIntScotiabankD + cerosCtaCheIntScotiabankD1
                                                + bank.sCuenta + paisIntScotiabankD + fillerIntScotiabankD2
                                                + tipoCuentaIntScotiabankD1 + digitoIntScotiabankD1 + plazaIntScotiabankD
                                                + bancoEmisorIntScotiabankD1 + sufBank + diasVigenciaIntScotiabankD
                                                + conceptoPagoIntScotiabankD + fillerIntScotiabankD3 + fillerIntScotiabankD4
                                                + cerosCodStaIntScotiabankD1 + bank.sCuenta + fillerIntScotiabankD5 + "\n");
                                            cerosImpIntScotiabankD = "";
                                            cerosNumNomIntScotiabankD = "";
                                            espaciosNomEmpIntScotiabankD = "";
                                            cerosConsecIntScotiabankD1 = "";
                                            cerosCtaCheIntScotiabankD1 = "";
                                            cerosCodStaIntScotiabankD1 = "";
                                        }
                                    }
                                    // - TRAILER BLOQUE - \\
                                    int longTotMovIntScotiabankResult = longTotMovIntScotiabank - totalMoviIntScotiabank.ToString().Length;
                                    int longImpTotIntScotiabankResult = longImpTotIntScotiabank - resultadoSuma.ToString().Length;
                                    string tipoRegistroDIntScotiabank = "TB", tipoRegistroEIntScotiabank = "TA", cantidadMovIntScotiabank = "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", fillerIntScotiabankTB = "                                                                                                                           ";
                                    for (var d = 0; d < longTotMovIntScotiabankResult; d++)
                                    {
                                        cerosTotMovIntScotiabank += "0";
                                    }
                                    for (var w = 0; w < longImpTotIntScotiabankResult; w++)
                                    {
                                        cerosImpTotIntScotiabank += "0";
                                    }
                                    string trailerBloqueIntScotiabank = tipoArchivoIntScotiabank + tipoRegistroDIntScotiabank
                                        + cerosTotMovIntScotiabank + totalMoviIntScotiabank.ToString() + cerosImpTotIntScotiabank
                                        + resultadoSuma.ToString() + cantidadMovIntScotiabank + fillerIntScotiabankTB;
                                    fileIntScotiabank.Write(trailerBloqueIntScotiabank + "\n");
                                    string trailerArchivoIntScotiabank = tipoArchivoIntScotiabank + tipoRegistroEIntScotiabank
                                        + cerosTotMovIntScotiabank + totalMoviIntScotiabank.ToString() + cerosImpTotIntScotiabank
                                        + resultadoSuma.ToString() + cantidadMovIntScotiabank + fillerIntScotiabankTB;
                                    fileIntScotiabank.Write(trailerArchivoIntScotiabank + "\n");
                                    // - TRAILER ARCHIVO - \\
                                    fileIntScotiabank.Close();
                                }
                            }
                            
                        }

                        // BANORTE -> INTERBANCARIO -> OK -> LISTO
                        if (bankInterbank == 72) {
                            // LONGITUDES \\
                            int longitudOperacion     = 2;
                            int longitudClaveId       = 12;
                            int longitudCuentaOrigen  = 20;
                            int longitudCuentaDestino = 20;

                            string tipoOperacion       = "04";
                            string cuentaOrigen        = "";
                            string cuentaDestino       = "";
                            string apartCeros1         = "00000000000";
                            string fillerCuentaOrigen  = "";
                            string fillerCuentaDestino = "";
                            string apartCeros2         = "0";
                            string apartCeros3         = "00";
                            string referenceDate       = DateTime.Now.ToString("ddMMyyyy");
                            if (dateDisC != "") {
                                referenceDate = DateTime.Parse(dateDisC.ToString()).ToString("ddMMyyyy");
                            }
                            string descriptionPd = "PAGO NOMINA                   ";
                            if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48) {
                                descriptionPd = keyBusiness.ToString() + " PAGO POR CUENTA Y ORDEN SUE";
                            }
                            if (tipPago == 2) {
                                descriptionPd = "HONORARIOS                    ";
                            }
                            string coinOrigin    = "1";
                            string coingDestiny  = "1";
                            string ivaBanorte    = "00000000000000";
                            string emailBusiness = "";
                            if (keyBusiness == 2074) {
                                emailBusiness = "asesoresimpasrh@gmail.com              ";
                            } else {
                                emailBusiness = "dgarcia@gruposeri.com                  ";
                            }
                            // Longitudes campos
                            int longNumberPayroll  = 13;
                            int longNumberADestiny = 10;
                            int longNumberImport   = 14;
                            int longNumberBRfc     = 13;
                            int longNumberEmail    = 70;
                            int longDetailsTotal   = 255;
                            if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48) {
                                int contador = 0;
                                int vueltas = 1;
                                List<DatosProcesaChequesNominaBean> banorteNew = new List<DatosProcesaChequesNominaBean>();
                                foreach (DatosProcesaChequesNominaBean payroll in listDatosProcesaChequesNominaBean)
                                {
                                    if (contador > 499)
                                    {
                                        contador = 0;
                                        vueltas += 1;
                                    }
                                    contador += 1;
                                    banorteNew.Add(new DatosProcesaChequesNominaBean { iIdBanco = payroll.iIdBanco, dImporte = payroll.dImporte, doImporte = payroll.doImporte, iIdEmpresa = payroll.iIdEmpresa, iTipoPago = payroll.iTipoPago, sBanco = payroll.sBanco, sCodigo = payroll.sCodigo, sCuenta = payroll.sCuenta, sImporte = payroll.sImporte, sMaterno = payroll.sMaterno, sNombre = payroll.sNombre, sNomina = payroll.sNomina, sPaterno = payroll.sPaterno, sRfc = payroll.sRfc, iCodigoTXT = vueltas });
                                }
                                int z = 1;
                                while (z <= vueltas)
                                {
                                    using (StreamWriter fileIntBanorte = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_BANO_" + keyBusiness.ToString() + "_P" + z.ToString() + ".txt", false, new ASCIIEncoding()))
                                    {
                                        foreach (DatosProcesaChequesNominaBean data in banorteNew)
                                        {
                                            if (data.iCodigoTXT == z)
                                            {
                                                fillerCuentaOrigen = "";
                                                string nameEmployee = data.sNombre.TrimEnd() + " " + data.sPaterno.TrimEnd() + " " + data.sMaterno.TrimEnd();
                                                if (nameEmployee.Length > 70)
                                                {
                                                    nameEmployee.Substring(0, 69);
                                                }
                                                ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                                                ReportesDao reportDao = new ReportesDao();
                                                string importeFinal = "";
                                                renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                        Convert.ToInt32(data.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                if (renglon1481 > 0)
                                                {
                                                    importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                        Convert.ToInt32(data.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                    importeFinal = importe.decimalTotalDispersion.ToString();
                                                }
                                                else
                                                {
                                                    importeFinal = data.dImporte.ToString();
                                                }
                                                string payroll = data.sNomina;
                                                int longPayroll = longNumberPayroll - payroll.Length;
                                                string accountOrigin = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                                int longAcountOrigin = longNumberADestiny - accountOrigin.Length;
                                                string accountDestiny = data.sCuenta;
                                                int resultadoFillerCuentaOrigen = longitudCuentaOrigen - accountOrigin.Length;
                                                for (var y = 0; y < resultadoFillerCuentaOrigen; y++)
                                                {
                                                    fillerCuentaOrigen += "0";
                                                }
                                                if (accountDestiny.Length == 16)
                                                {
                                                    accountDestiny = "00" + accountDestiny;
                                                }
                                                string importPaid = "";
                                                int longImportPaid = longNumberImport - importeFinal.ToString().Length;
                                                string rfcBusiness = datosEmpresaBeanDispersion.sRfc;
                                                int longRfcBusiness = longNumberBRfc - rfcBusiness.Length;
                                                int longEmailBusiness = longNumberEmail - emailBusiness.Length;
                                                for (var i = 0; i < longPayroll; i++)
                                                {
                                                    payroll += " ";
                                                }
                                                for (var k = 0; k < longImportPaid; k++)
                                                {
                                                    importPaid += "0";
                                                }
                                                for (var p = 0; p < longRfcBusiness; p++)
                                                {
                                                    rfcBusiness += " ";
                                                }
                                                importPaid += importeFinal.ToString();
                                                string fillerFinal = "";
                                                string cadenaFinal = "";
                                                // QUINCENALES CRISTINA 158 , 159
                                                if (keyBusiness == 2074 || keyBusiness == 2073 || keyBusiness == 2067 || keyBusiness == 158 || keyBusiness == 159)
                                                {
                                                    cadenaFinal = tipoOperacion + payroll + "0000000000" + accountOrigin + apartCeros3 + accountDestiny + importPaid + apartCeros3 + referenceDate + descriptionPd + coinOrigin + coingDestiny + rfcBusiness + ivaBanorte + emailBusiness + referenceDate;
                                                }
                                                else
                                                {
                                                    cadenaFinal = tipoOperacion + payroll + fillerCuentaOrigen + accountOrigin + apartCeros3 + accountDestiny + importPaid + apartCeros3 + referenceDate + descriptionPd + coinOrigin + coingDestiny + rfcBusiness + ivaBanorte + emailBusiness + referenceDate + nameEmployee;
                                                }
                                                int longFinalFiller = longDetailsTotal - cadenaFinal.Length;
                                                for (var x = 0; x < longFinalFiller; x++)
                                                {
                                                    fillerFinal += " ";
                                                }
                                                if (keyBusiness == 2074 || keyBusiness == 2073 || keyBusiness == 2067 || keyBusiness == 158 || keyBusiness == 159)
                                                {
                                                    fileIntBanorte.Write(tipoOperacion + payroll + "0000000000" + accountOrigin + apartCeros3 + accountDestiny + importPaid + apartCeros3 + referenceDate + descriptionPd + coinOrigin + coingDestiny + rfcBusiness + ivaBanorte + emailBusiness + referenceDate + fillerFinal + "\n");
                                                }
                                                else
                                                {
                                                    fileIntBanorte.Write(tipoOperacion + payroll + fillerCuentaOrigen + accountOrigin + apartCeros3 + accountDestiny + importPaid + apartCeros3 + referenceDate + descriptionPd + coinOrigin + coingDestiny + rfcBusiness + ivaBanorte + emailBusiness + referenceDate + nameEmployee + fillerFinal + "\n");
                                                }
                                            }
                                        }
                                        fileIntBanorte.Close();
                                    }
                                    z += 1;
                                }
                            } else {
                                using (StreamWriter fileIntBanorte = new StreamWriter(directoryTxt + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_BANO_" + keyBusiness.ToString() + ".txt", false, new ASCIIEncoding()))
                                {
                                    foreach (DatosProcesaChequesNominaBean data in listDatosProcesaChequesNominaBean)
                                    {
                                        if (1 == 1)
                                        {
                                            fillerCuentaOrigen = "";
                                            string nameEmployee = data.sNombre.TrimEnd() + " " + data.sPaterno.TrimEnd() + " " + data.sMaterno.TrimEnd();
                                            if (nameEmployee.Length > 70)
                                            {
                                                nameEmployee.Substring(0, 69);
                                            }
                                            ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
                                            ReportesDao reportDao = new ReportesDao();
                                            string importeFinal = "";
                                            renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                                    Convert.ToInt32(data.sNomina), numberPeriod, typePeriod, yearPeriod);
                                            if (renglon1481 > 0)
                                            {
                                                importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                    Convert.ToInt32(data.sNomina), numberPeriod, typePeriod, yearPeriod);
                                                importeFinal = importe.decimalTotalDispersion.ToString();
                                            }
                                            else
                                            {
                                                importeFinal = data.dImporte.ToString();
                                            }
                                            string payroll = data.sNomina;
                                            int longPayroll = longNumberPayroll - payroll.Length;
                                            string accountOrigin = datoCuentaClienteBancoEmpresaBean.sNumeroCuenta;
                                            int longAcountOrigin = longNumberADestiny - accountOrigin.Length;
                                            string accountDestiny = data.sCuenta;
                                            int resultadoFillerCuentaOrigen = longitudCuentaOrigen - accountOrigin.Length;
                                            for (var y = 0; y < resultadoFillerCuentaOrigen; y++)
                                            {
                                                fillerCuentaOrigen += "0";
                                            }
                                            if (accountDestiny.Length == 16)
                                            {
                                                accountDestiny = "00" + accountDestiny;
                                            }
                                            string importPaid = "";
                                            int longImportPaid = longNumberImport - importeFinal.ToString().Length;
                                            string rfcBusiness = datosEmpresaBeanDispersion.sRfc;
                                            int longRfcBusiness = longNumberBRfc - rfcBusiness.Length;
                                            int longEmailBusiness = longNumberEmail - emailBusiness.Length;
                                            for (var i = 0; i < longPayroll; i++)
                                            {
                                                payroll += " ";
                                            }
                                            for (var k = 0; k < longImportPaid; k++)
                                            {
                                                importPaid += "0";
                                            }
                                            for (var p = 0; p < longRfcBusiness; p++)
                                            {
                                                rfcBusiness += " ";
                                            }
                                            importPaid += importeFinal.ToString();
                                            string fillerFinal = "";
                                            string cadenaFinal = "";
                                            // QUINCENALES CRISTINA 158 , 159
                                            if (keyBusiness == 2074 || keyBusiness == 2073 || keyBusiness == 2067 || keyBusiness == 158 || keyBusiness == 159)
                                            {
                                                cadenaFinal = tipoOperacion + payroll + "0000000000" + accountOrigin + apartCeros3 + accountDestiny + importPaid + apartCeros3 + referenceDate + descriptionPd + coinOrigin + coingDestiny + rfcBusiness + ivaBanorte + emailBusiness + referenceDate;
                                            }
                                            else
                                            {
                                                cadenaFinal = tipoOperacion + payroll + fillerCuentaOrigen + accountOrigin + apartCeros3 + accountDestiny + importPaid + apartCeros3 + referenceDate + descriptionPd + coinOrigin + coingDestiny + rfcBusiness + ivaBanorte + emailBusiness + referenceDate + nameEmployee;
                                            }
                                            int longFinalFiller = longDetailsTotal - cadenaFinal.Length;
                                            for (var x = 0; x < longFinalFiller; x++)
                                            {
                                                fillerFinal += " ";
                                            }
                                            if (keyBusiness == 2074 || keyBusiness == 2073 || keyBusiness == 2067 || keyBusiness == 158 || keyBusiness == 159)
                                            {
                                                fileIntBanorte.Write(tipoOperacion + payroll + "0000000000" + accountOrigin + apartCeros3 + accountDestiny + importPaid + apartCeros3 + referenceDate + descriptionPd + coinOrigin + coingDestiny + rfcBusiness + ivaBanorte + emailBusiness + referenceDate + fillerFinal + "\n");
                                            }
                                            else
                                            {
                                                fileIntBanorte.Write(tipoOperacion + payroll + fillerCuentaOrigen + accountOrigin + apartCeros3 + accountDestiny + importPaid + apartCeros3 + referenceDate + descriptionPd + coinOrigin + coingDestiny + rfcBusiness + ivaBanorte + emailBusiness + referenceDate + nameEmployee + fillerFinal + "\n");
                                            }
                                        }
                                    }
                                    fileIntBanorte.Close();
                                }
                            }
                            
                        }
                        flag = true;
                    } 
                }
                if (flag) {
                    // CREACCION DEL ZIP CON LOS ARCHIVOS
                    FileStream stream = new FileStream(directoryZip + @"\\" + nameFolderYear + @"\\" + "INTERBANCARIOS" + @"\" + nameFolder + ".zip", FileMode.OpenOrCreate);
                    ZipArchive fileZip = new ZipArchive(stream, ZipArchiveMode.Create);
                    System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(directoryTxt + @"\\" + nameFolder);
                    FileInfo[] sourceFiles = directoryInfo.GetFiles();
                    foreach (FileInfo file in sourceFiles)
                    {
                        Stream sourceStream = file.OpenRead();
                        ZipArchiveEntry entry = fileZip.CreateEntry(file.Name);
                        Stream zipStream = entry.Open();
                        sourceStream.CopyTo(zipStream);
                        zipStream.Close();
                        sourceStream.Close();
                    }
                    ZipArchiveEntry zEntrys;
                    fileZip.Dispose();
                    stream.Close();
                    try
                    {
                        using (ZipArchive zipArchive = ZipFile.OpenRead(directoryZip + @"\\" + nameFolderYear + @"\\" + "INTERBANCARIOS" + @"\\" + nameFolder + ".zip"))
                        {
                            foreach (ZipArchiveEntry archiveEntry in zipArchive.Entries)
                            {
                                using (ZipArchive zipArchives = ZipFile.Open(directoryZip + @"\\" + nameFolderYear + @"\\" + "INTERBANCARIOS" + @"\\" + nameFolder + ".zip", ZipArchiveMode.Read))
                                {
                                    zEntrys = zipArchives.GetEntry(archiveEntry.ToString());
                                    nameFileError = zEntrys.Name;
                                    using (StreamReader read = new StreamReader(zEntrys.Open()))
                                    {
                                        if (read.ReadLine().Length > 0)
                                        {
                                            msgEstatusZip = "filesuccess";
                                        }
                                        else
                                        {
                                            msgEstatusZip = "fileerror";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (InvalidDataException ide)
                    {
                        Console.WriteLine(ide.Message.ToString() + " En el archivo : " + nameFileError);
                        msgEstatus = "fileError";
                    }
                    catch (Exception exc)
                    {
                        msgEstatus = exc.Message.ToString();
                    }
                    if (System.IO.File.Exists(directoryZip + @"\\" + nameFolderYear + @"\\" + "NOMINAS" + @"\\" + nameFolder + ".zip"))
                    {
                        msgEstatus = "success";
                    }
                    else
                    {
                        msgEstatus = "failed";
                    }
                }
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Zip = nameFolder, EstadoZip = msgEstatusZip, Estado = msgEstatus, Anio = nameFolderYear });
        }

        [HttpPost]
        public JsonResult LoadGroupBusiness()
        {
            Boolean flag = false;
            String  messageError = "none";
            StringBuilder htmlTableBody = new StringBuilder();
            List<GroupBusinessDispersionBean> groupBusinesses = new List<GroupBusinessDispersionBean>();
            DataDispersionBusiness dataDispersion = new DataDispersionBusiness();
            try {
                groupBusinesses = dataDispersion.sp_Load_Group_Business_Dispersion();
                if (groupBusinesses.Count > 0) {
                    flag = true;
                    foreach (GroupBusinessDispersionBean data in groupBusinesses) {
                        htmlTableBody.Append(
                            "<tr><td> " + data.sNombreGrupo + " </td>" +
                            "<td> <button onclick='fViewBusinessGroup("+ data.iIdGrupoEmpresa +", \"" + data.sNombreGrupo + "\")' type='button' class='btn btn-success btn-sm btn-icon-split shadow'> <span class='icon text-white-50'><i class='fas fa-eye'></i></span> <span class='text'>Empresas</span> </button> " +
                            "<button onclick='fViewBanks(" + data.iIdGrupoEmpresa + ", \"" + data.sNombreGrupo + "\")' type='button' class='btn btn-success btn-sm btn-icon-split shadow'> <span class='icon text-white-50'><i class='fas fa-piggy-bank'></i></span> <span class='text'>Bancos</span> </button> " +
                            "</td></tr>");
                    }
                }
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Html = htmlTableBody.ToString(), Datos = groupBusinesses });
        }

        /*
         *  hola este es un comentario bro 
         */

        [HttpPost]
        public JsonResult SaveNewGroupBusiness(string name)
        {
            Boolean flag = false;
            String  messageError = "none";
            GroupBusinessDispersionBean groupBusiness = new GroupBusinessDispersionBean();
            DataDispersionBusiness dataDispersion     = new DataDispersionBusiness();
            try {
                int keyUser = Convert.ToInt32(Session["iIdUsuario"].ToString());
                groupBusiness = dataDispersion.sp_Save_New_Group_Business_Dispersion(name.Trim(), keyUser);
                if (groupBusiness.sMensaje == "SUCCESS") {
                    flag = true;
                } else if (groupBusiness.sMensaje == "EXISTS") {
                    return Json(new { Bandera = false, Mensaje = "EXISTS" });
                } else {
                    return Json(new { Bandera = false, Mensaje = "ERROR" });
                }
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
                flag = false;
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Mensaje = "SUCCESS" });
        } 

        [HttpPost]
        public JsonResult LoadBusinessNotGroup()
        {
            Boolean flag = false;
            String  messageError = "none";
            List<EmpresasBean> empresasBean = new List<EmpresasBean>();
            DataDispersionBusiness dataDispersion = new DataDispersionBusiness();
            try {
                empresasBean = dataDispersion.sp_Load_Business_Not_In_Groups_Dispersion();
                if (empresasBean.Count > 0) {
                    flag = true;
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = empresasBean });
        }

        [HttpPost]
        public JsonResult SaveAsignGroupBusiness(int group, int business)
        {
            Boolean flag = false;
            String  messageError = "none";
            GroupBusinessDispersionBean groupBusiness = new GroupBusinessDispersionBean();
            DataDispersionBusiness dataDispersion = new DataDispersionBusiness();
            try {
                groupBusiness = dataDispersion.sp_Save_Asign_Group_Business(group, business);
                if (groupBusiness.sMensaje == "INSERT") {
                    flag = true;
                } else if (groupBusiness.sMensaje == "NOTINSERT") {
                    return Json(new { Bandera = false, Mensaje = "NOTINSERT" });
                } else {
                    return Json(new { Bandera = false, Mensaje = "ERROR" });
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        [HttpPost]
        public JsonResult ViewBusinessGroup(int keyGroup)
        {
            Boolean flag = false;
            String messageError = "none";
            List<EmpresasBean> empresas = new List<EmpresasBean>();
            DataDispersionBusiness dataDispersion = new DataDispersionBusiness();
            try {
                empresas = dataDispersion.sp_View_Business_Group_Dispersion(keyGroup);
                if (empresas.Count > 0) {
                    flag = true;
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = empresas });
        }

        [HttpPost]
        public JsonResult RemoveBusinessGroup(int keyBusinessGroup)
        {
            Boolean flag          = false;
            String  messageError  = "none";
            EmpresasBean empresas = new EmpresasBean();
            DataDispersionBusiness dataDispersion = new DataDispersionBusiness();
            try {
                empresas = dataDispersion.sp_Remove_Business_Group(keyBusinessGroup);
                if (empresas.sMensaje == "SUCCESS") {
                    flag = true;
                } else if (empresas.sMensaje == "NOTDELETE") {
                    return Json(new { Bandera = false, MensajeError = "NOTDELETE" });
                } else {
                    return Json(new { Bandera = false, MensajeError = "ERROR" });
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        /*
         * Bancos dispersion especial
         */

        [HttpPost]
        public JsonResult ViewBanks(string key, int keyGroup)
        {
            Boolean flag         = false;
            String  messageError = "none";
            DataDispersionBusiness dataDispersion = new DataDispersionBusiness();
            List<BancosBean> bancosBean           = new List<BancosBean>();
            try {
                bancosBean = dataDispersion.sp_View_Banks_Group_Business_Dispersion(keyGroup);
                if (bancosBean.Count > 0) {
                    flag = true;
                }
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = bancosBean });
        }

        [HttpPost]
        public JsonResult ShowConfigBanks(int keyGroup, string type, string option)
        {
            Boolean flag = false;
            String  messageError = "none";
            List<BankInt> bankInts = new List<BankInt>();
            List<BancosBean> bancosBean = new List<BancosBean>();
            DataDispersionBusiness dataDispersion = new DataDispersionBusiness();
            try {
                bankInts = dataDispersion.sp_View_Config_Banks(keyGroup, type, option);
                bancosBean = dataDispersion.sp_View_Banks_Available_Dispersion(keyGroup, type, option, 0);
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = bankInts, Bancos = bancosBean });
        }

        [HttpPost]
        public JsonResult SaveBanksDSBank (int keyGroup, string type, int banorte, int santander, int scotiabank, int banamex, int bancomer)
        {
            Boolean flag         = false;
            String  messageError = "none";
            BancosBean bancosBean = new BancosBean();
            List<BankInt> bankInt = new List<BankInt>();
            DataDispersionBusiness dataDispersion = new DataDispersionBusiness();
            string typeSend = (type == "INT") ? "INTERBANCARIO" : "NOMINA";
            try { 
                bankInt.Add(new BankInt { sNombre = "BANORTE",    iBanco = 72, iActivo = banorte    });
                bankInt.Add(new BankInt { sNombre = "SANTANDER",  iBanco = 14, iActivo = santander  });
                bankInt.Add(new BankInt { sNombre = "SCOTIABANK", iBanco = 44, iActivo = scotiabank });
                if (typeSend == "NOMINA") {
                    bankInt.Add(new BankInt { sNombre = "BANAMEX",  iBanco = 2,  iActivo = banamex  });
                    bankInt.Add(new BankInt { sNombre = "BANCOMER", iBanco = 12, iActivo = bancomer });
                } 
                int keyUser = Convert.ToInt32(Session["iIdUsuario"].ToString());
                bancosBean = dataDispersion.sp_Save_Banks_Group_Interbank(keyGroup, keyUser, bankInt, typeSend);
                if (bancosBean.sMensaje == "SUCCESS") {
                    flag = true;
                }
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        [HttpPost]
        public JsonResult ShowBanksConfigDetails(int keyGroup, string type, string option, int configuration)
        {
            Boolean flag = false;
            String  messageError = "none";
            List<BancosBean> bancosBean = new List<BancosBean>();
            DataDispersionBusiness dataDispersion = new DataDispersionBusiness();
            LoadTypePeriodBean loadTypePerBean = new LoadTypePeriodBean();
            LoadTypePeriodDaoD loadTypePerDaoD = new LoadTypePeriodDaoD();
            Decimal total = 0;
            string resultadoTotal = "";
            try {  
                    int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                    loadTypePerBean = loadTypePerDaoD.sp_Load_Type_Period_Empresa(keyBusiness, DateTime.Now.Year, 3);
                    bancosBean = dataDispersion.sp_View_Banks_Available_Dispersion(keyGroup, type, option, configuration);
                if (bancosBean.Count > 0) {
                    total = dataDispersion.sp_Totales_Dispersion_Especial(DateTime.Now.Year, loadTypePerBean.iPeriodo, 3, keyGroup, configuration);
                    resultadoTotal = "$ " + Convert.ToDecimal(total).ToString("#,##0.00");
                    flag = true;
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = bancosBean, Total = resultadoTotal });
        }

        [HttpPost]
        public JsonResult SaveConfigDetails(int keyGroup, string type, int configurationId, int bank)
        {
            Boolean flag          = false;
            String  messageError  = "none";
            BancosBean bancosBean = new BancosBean();
            DataDispersionBusiness dataDispersion = new DataDispersionBusiness();
            try {
                int keyUser = Convert.ToInt32(Session["iIdUsuario"].ToString());
                bancosBean  = dataDispersion.sp_Save_Config_Details_Banks(keyGroup, type, configurationId, bank, keyUser);
                if (bancosBean.sMensaje == "success") {
                    flag = true;
                }
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        [HttpPost]
        public JsonResult RemoveBankDetail (int keyConfigBank, int keyDetailConfig)
        {
            Boolean flag = false;
            String  messageError  = "none";
            BancosBean bancosBean = new BancosBean();
            DataDispersionBusiness dataDispersion = new DataDispersionBusiness();
            try {
                bancosBean = dataDispersion.sp_Remove_Bank_Details(keyDetailConfig, keyConfigBank);
                if (bancosBean.sMensaje == "success") {
                    flag = true;
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        [HttpPost]
        public JsonResult ConfigDataBank(int keyConfig)
        {
            Boolean flag = false;
            String  messageError = "none";
            DataDispersionBusiness dataDispersion          = new DataDispersionBusiness();
            DatosCuentaClienteBancoEmpresaBean datosCuenta = new DatosCuentaClienteBancoEmpresaBean();
            try {
                datosCuenta = dataDispersion.sp_View_Config_Data_Account_Bank(keyConfig);
                if (datosCuenta.sMensaje == "SUCCESS") {
                    flag = true;
                } else if (datosCuenta.sMensaje == "NOTFOUND") {
                    return Json(new { Bandera = flag, MensajeError = datosCuenta.sMensaje });
                } else {
                    return Json(new { Bandera = flag, MensajeError = datosCuenta.sMensaje });
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = datosCuenta });
        }

        [HttpPost]
        public JsonResult SaveConfigDataBank(string nClient, string nAccount, string nClabe, string nSquare, int keyConfig, string rfc)
        {
            Boolean flag         = false;
            String  messageError = "none";
            DataDispersionBusiness dataDispersion          = new DataDispersionBusiness();
            DatosCuentaClienteBancoEmpresaBean datosCuenta = new DatosCuentaClienteBancoEmpresaBean();
            try {
                datosCuenta = dataDispersion.sp_Save_Config_Data_Account_Bank(nClient, nAccount, nClabe, nSquare, keyConfig, rfc);
                if (datosCuenta.sMensaje == "SUCCESS") {
                    flag = true;
                } else {
                    return Json(new { Bandera = flag, MensajeError = datosCuenta.sMensaje });
                }
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        public Boolean GenerateFoldersReports(string folderFather, string folderChild, string nameFile)
        {
            Boolean flag = false;
            string pathSaveFile = Server.MapPath("~/Content/");
            try
            {
                if (!Directory.Exists(pathSaveFile + folderFather))
                {
                    Directory.CreateDirectory(pathSaveFile + folderFather);
                }
                if (!Directory.Exists(pathSaveFile + folderFather + @"\\" + folderChild))
                {
                    Directory.CreateDirectory(pathSaveFile + folderFather + @"\\" + folderChild);
                }
                string pathComplete = pathSaveFile + folderFather + @"\\" + folderChild + @"\\" + nameFile;
                if (System.IO.File.Exists(pathComplete))
                {
                    System.IO.File.Delete(pathComplete);
                }
                flag = true;
            }
            catch (Exception exc)
            {
                flag = false;
            }
            return flag;
        }

        [HttpPost]
        public JsonResult GenerateReportDispersion(int yearPeriod, int numberPeriod, int typePeriod)
        {
            Boolean flag = false;
            String messageError = "none";
            DispersionSpecialDao dispersion = new DispersionSpecialDao();
            DispersionBean dispersionBean = new DispersionBean();
            string pathSaveFile = Server.MapPath("~/Content/");
            string nameFolder = "DISPERSION";
            string nameFolderRe = "NORMAL";
            string nameFileRepr = "DISPERSIONNM" + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".xlsx";
            ReportesDao reportDao = new ReportesDao();
            string pathComplete = pathSaveFile + nameFolder + @"\\" + nameFolderRe + @"\\";
            int rowsDataTable = 1, columnsDataTable = 0;
            int banderaEmpresa = 0;
            List<ListRenglonesGruposRestas> datos = new List<ListRenglonesGruposRestas>();
            ListRenglonesGruposRestas importe = new ListRenglonesGruposRestas();
            try {
                int keyBusiness = Convert.ToInt32(Session["IdEmpresa"]);
                // Comprobamos el grupo de la empresa en sesion
                banderaEmpresa = reportDao.sp_Comprueba_Empresa_Existencia_Grupo(keyBusiness);
                Boolean createFolders = GenerateFoldersReports(nameFolder, nameFolderRe, nameFileRepr);
                if (banderaEmpresa == 1) {
                    if (createFolders) {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (ExcelPackage excel = new ExcelPackage()) {
                            excel.Workbook.Worksheets.Add(Path.GetFileNameWithoutExtension(nameFileRepr));
                            var worksheet = excel.Workbook.Worksheets[Path.GetFileNameWithoutExtension(nameFileRepr)];
                            int ii = 0;
                            int i = 0;
                            worksheet.Cells[1, 1].Value  = "Anio";
                            worksheet.Cells[1, 2].Value  = "Periodo";
                            worksheet.Cells[1, 3].Value  = "Espejo";
                            worksheet.Cells[1, 4].Value  = "Empresa";
                            worksheet.Cells[1, 5].Value  = "Nombre Empresa";
                            worksheet.Cells[1, 6].Value  = "Nomina";
                            worksheet.Cells[1, 7].Value  = "Apellido Paterno";
                            worksheet.Cells[1, 8].Value  = "Apellido Materno";
                            worksheet.Cells[1, 9].Value  = "Nombre Empleado";
                            worksheet.Cells[1, 10].Value = "Tipo de Pago";
                            worksheet.Cells[1, 11].Value = "IdBanco";
                            worksheet.Cells[1, 12].Value = "Nombre Banco";
                            worksheet.Cells[1, 13].Value = "Cuenta";
                            worksheet.Cells[1, 14].Value = "Importe";
                            worksheet.Cells[1, 15].Value = "Valor";
                            for (i = 1; i < 16; i++) { 
                                worksheet.Cells[1, i].Style.Fill.SetBackground(System.Drawing.Color.LightSkyBlue);
                            }
                            ii = 15; 
                            datos = reportDao.sp_Reporte_Dispersion_Empresas_Resta(keyBusiness, yearPeriod, typePeriod, numberPeriod);
                            int p = 1;
                            foreach (ListRenglonesGruposRestas dato in datos) { 
                                worksheet.Cells[p + 1, 1].Value = dato.iAnio;
                                worksheet.Cells[p + 1, 2].Value = dato.iPeriodo;
                                worksheet.Cells[p + 1, 3].Value = dato.sEspejo;
                                worksheet.Cells[p + 1, 4].Value = dato.iEmpresa;
                                worksheet.Cells[p + 1, 5].Value = dato.sNombreEmpresa;
                                worksheet.Cells[p + 1, 6].Value = dato.iNomina;
                                worksheet.Cells[p + 1, 7].Value = dato.sPaterno;
                                worksheet.Cells[p + 1, 8].Value = dato.sMaterno;
                                worksheet.Cells[p + 1, 9].Value = dato.sNombre;
                                worksheet.Cells[p + 1, 10].Value = dato.sTipoPago;
                                worksheet.Cells[p + 1, 11].Value = dato.iBanco;
                                worksheet.Cells[p + 1, 12].Value = dato.sNombreBanco;
                                worksheet.Cells[p + 1, 13].Value = dato.sCuenta; 
                                importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness, dato.iNomina, numberPeriod, typePeriod, yearPeriod); 
                                if (importe.dTotal != 0) {
                                    worksheet.Cells[p + 1, 14].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[p + 1, 14].Value = importe.dTotal;
                                } else {
                                    worksheet.Cells[p + 1, 14].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[p + 1, 14].Value = 0.00;
                                }
                                worksheet.Cells[p + 1, 15].Value = dato.sValor;
                                p += 1;
                            }
                            FileInfo excelFile = new FileInfo(pathComplete + nameFileRepr);
                            excel.SaveAs(excelFile); 
                            string final = DateTime.Now.ToString("hh:mm:ss");
                            DateTime termino = DateTime.Parse(final); 
                            flag = true;
                        }
                    }
                } else {
                    if (createFolders) {
                        DataTable dataTable = new DataTable();
                        dataTable.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;
                        dataTable = dispersion.sp_Reporte_Dispersion(keyBusiness, yearPeriod, typePeriod, numberPeriod);
                        columnsDataTable = dataTable.Columns.Count + 1;
                        rowsDataTable = dataTable.Rows.Count;
                        if (rowsDataTable > 0) {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage excel = new ExcelPackage()) {
                                excel.Workbook.Worksheets.Add(Path.GetFileNameWithoutExtension(nameFileRepr));
                                var worksheet = excel.Workbook.Worksheets[Path.GetFileNameWithoutExtension(nameFileRepr)];
                                for (var i = 1; i < columnsDataTable; i++) {
                                    worksheet.Cells[1, i].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                                    worksheet.Cells[1, i].Style.Font.Bold = true;
                                    worksheet.Cells[1, i].Style.WrapText = true;
                                    worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                }
                                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                                FileInfo excelFile = new FileInfo(pathComplete + nameFileRepr);
                                excel.SaveAs(excelFile);
                                flag = true;
                                excel.Dispose();
                            }
                        }
                    }
                }
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Archivo = nameFileRepr, Folder = nameFolderRe, Rows = rowsDataTable, Columns = columnsDataTable });
        }

    }
}