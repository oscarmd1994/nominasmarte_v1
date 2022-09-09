﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Payroll.Models.Beans;
using Payroll.Models.Daos;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;
using static iTextSharp.text.Font;
using Payroll.Models.Utilerias;
using System.Globalization;

namespace Payroll.Controllers
{
    public class BajasEmpleadosController : Controller
    {
        // GET: BajasEmpleados
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult ReactiveEmploye(int keyEmployee)
        {
            Boolean flag         = false;
            String  messageError = "none";
            BajasEmpleadosBean bajasEmpleados = new BajasEmpleadosBean();
            BajasEmpleadosDaoD bajasEmpleadosDao = new BajasEmpleadosDaoD();
            try {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                bajasEmpleados = bajasEmpleadosDao.sp_Cancel_Settlement_Employee_Reactive(keyEmployee, keyBusiness);
                if (bajasEmpleados.sMensaje == "SUCCESS") {
                    flag = true;
                } else {
                    messageError = bajasEmpleados.sMensaje;
                }
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        [HttpPost]
        public JsonResult ApplyDown(int keySettlement, int keyEmployee)
        {
            Boolean flag         = false;
            String  messageError = "none";
            BajasEmpleadosBean bajasEmpleados = new BajasEmpleadosBean();
            BajasEmpleadosDaoD empleadosDaoD  = new BajasEmpleadosDaoD();
            try {
                int keyBusiness = Convert.ToInt32(Session["IdEmpresa"].ToString());
                bajasEmpleados  = empleadosDaoD.sp_Apply_Down_Employee(keySettlement, keyEmployee, keyBusiness);
                if (bajasEmpleados.sMensaje == "SUCCESS") {
                    flag = true;
                } else {
                    messageError = bajasEmpleados.sMensaje;
                }
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        [HttpPost]
        public JsonResult SendDataDownSettlement(int keyEmployee, string dateAntiquityEmp, int idTypeDown, int idReasonsDown, string dateDownEmp, int daysPending, int typeDate, int typeCompensation, Boolean flagTypeSettlement, int typeOper, int propSet, int daysYearsAftr)
        {
            Boolean flag        = false;
            Boolean validation  = true;
            String messageError = "none";
            DateTime dateAct = DateTime.Now;
            int yearAct = Convert.ToInt32(dateAct.Year);
            string dateDownFormat = DateTime.Parse(dateDownEmp.ToString(), CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd");
            string dateReceiptFormat = DateTime.Parse(dateAct.ToString(), CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd");
            string dateStartPayment = "";
            string dateEndPayment = "";
            BajasEmpleadosBean downEmployeeBean = new BajasEmpleadosBean();
            BajasEmpleadosBean downVerifyBean   = new BajasEmpleadosBean();
            BajasEmpleadosDaoD downEmployeeDaoD = new BajasEmpleadosDaoD();
            PeriodoActualBean periodActBean = new PeriodoActualBean();
            int propSetSend = (propSet == 1) ? 4 : 0;
            try {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                int keyUser     = int.Parse(Session["iIdUsuario"].ToString());
                int keyPeriodAct = 0;
                periodActBean = downEmployeeDaoD.sp_Load_Info_Periodo_Empr(keyBusiness, yearAct);
                if (periodActBean.sMensaje == "success") {
                    keyPeriodAct = periodActBean.iPeriodo;
                    yearAct = periodActBean.iAnio;
                    dateStartPayment = periodActBean.sFecha_Inicio;
                    dateEndPayment = periodActBean.sFecha_Final;
                }
                downVerifyBean = downEmployeeDaoD.sp_Valida_Existencia_Finiquito(keyEmployee, keyBusiness, yearAct, keyPeriodAct);
                if (downVerifyBean.sMensaje == "EXISTS" && downVerifyBean.sFecha_baja == dateDownEmp) {
                    validation = false;
                }
                if (downVerifyBean.sMensaje == "NOTEXISTS") {
                    validation = false;
                }
                if (!validation) {
                    if (flagTypeSettlement) {
                        downEmployeeBean = downEmployeeDaoD.sp_CNomina_Finiquito(keyBusiness, keyEmployee, dateAntiquityEmp, idTypeDown, idReasonsDown, dateDownFormat, dateReceiptFormat, typeDate, typeCompensation, daysPending, yearAct, keyPeriodAct, dateStartPayment, dateEndPayment, typeOper, propSetSend, daysYearsAftr, keyUser);
                    } else {
                        downEmployeeBean = downEmployeeDaoD.sp_Crea_Baja_Sin_Baja_Calculos(keyBusiness, keyEmployee, dateDownFormat, idTypeDown, idReasonsDown, yearAct, keyPeriodAct, keyUser);
                    }
                    if (downEmployeeBean.sMensaje == "SUCCESS") {
                        if (propSet == 0) {
                            downEmployeeBean = downEmployeeDaoD.sp_BajaEmpleado_Update_EmpleadoNomina(keyEmployee, keyBusiness, idTypeDown, dateDownFormat, keyUser);
                            if (downEmployeeBean.sMensaje == "SUCCESSUPD") {
                                flag = true;
                            } else {
                                messageError = "ERRUPDTE";
                            }
                        }
                        flag = true;
                    } else {
                        messageError = "ERRINSFINIQ";
                    }
                } else {
                    validation = true;
                }
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Existencia = validation });
        }

        [HttpPost]
        public JsonResult InfoDaysYearsBefore(int business, int employee)
        {
            Boolean flag = false;
            String messageError = "none";
            DescEmpleadoVacacionesBean desc = new DescEmpleadoVacacionesBean();
            BajasEmpleadosDaoD daoD = new BajasEmpleadosDaoD();
            try {
                desc = daoD.sp_Select_Dias_A_Anteriores(business, employee);
                if (desc.DiasAAnteriores != 0) {
                    flag = true;
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, days = desc.DiasAAnteriores });
        }

        [HttpPost]
        public JsonResult ShowDataDown(int keyEmployee)
        {
            Boolean flag = false;
            String messageError = "none";
            List<BajasEmpleadosBean> listDataDownEmp = new List<BajasEmpleadosBean>();
            BajasEmpleadosDaoD downEmployeeDao = new BajasEmpleadosDaoD();
            try
            {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                int keySettlement = 0;
                listDataDownEmp = downEmployeeDao.sp_Finiquitos_Empleado(keyEmployee, keyBusiness, keySettlement);
                if (listDataDownEmp.Count > 0)
                {
                    flag = true;
                }
                else
                {
                    messageError = "NOTLOADINFO";
                }
            }
            catch (Exception exc)
            {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, DatosFiniquito = listDataDownEmp });
        }

        // Genera pdf de finiquito cancelado
        //[HttpPost]
        public DatosPDFCancelado GenerateReceiptDownCancel(List<BajasEmpleadosBean> dataDownEmployee, List<DatosFiniquito> listDataDownBean, int keyEmployee, int keyBusiness)
        {
            string pathSaveDocs = Server.MapPath("~/Content/");
            string nameFolder   = "DOCSFINIQUITOSCANCELADOS";
            string nameFileTest = "test.txt";
            string nameEmployee = "";
            string nameFilePdf  = "";
            DatosPDFCancelado dataR = new DatosPDFCancelado();
            Utilerias utilerias     = new Utilerias();
            string totalPercepcion = "0";
            string totalDeduccion  = "0";
            string totalBalance = "";
            try
            {
                // Comprobamos que exista la carpeta, si no la creamos
                if (!Directory.Exists(pathSaveDocs + nameFolder))
                {
                    Directory.CreateDirectory(pathSaveDocs + nameFolder);
                }
                // Comprobamos que exista un archivo de prueba, si no lo creamos
                if (!System.IO.File.Exists(pathSaveDocs + nameFolder + @"\\" + nameFileTest))
                {
                    using (StreamWriter file = new StreamWriter(pathSaveDocs + nameFolder + @"\\" + nameFileTest, false, Encoding.UTF8))
                    {
                        file.Write("NO REMOVER, DEJAME AQUI");
                        file.Close();
                    }
                }
                // ** Definimos las variables a utilizar para mostrar la informacion del finiquito seleccionado en el pdf ** \\
                string dateReceipt = "";
                string dateDown = "";
                string dateEntry = "";
                string dateAntiquity = "";
                string rfcEmployee = "";
                string sAniosAntiquity = "";
                string sDaysAntiquity = "";
                string typeSettlement = "";
                string nameBusiness = "";
                string centrCost = "";
                string salaryMonth = "";
                string salaryDay = "";
                string jobEmployee = "";
                string jobCodeEmployee = "";
                string centroCostName = "";
                string departamentName = "";
                string departamentCode = "";
                string registerImss = "";
                string countBank = "";
                string dateStartPay = "";
                string dateEndPay = "";
                string periodPay = "";
                // Foreach para obtener datos de la empresa
                foreach (BajasEmpleadosBean data in dataDownEmployee)
                {
                    dateReceipt = data.sFecha_recibo;
                    typeSettlement = data.sFiniquito_valor;
                    nameBusiness = data.sEmpresa;
                    dateDown = data.sFecha_baja;
                    dateEntry = data.sFecha_ingreso;
                    dateAntiquity = data.sFecha_antiguedad;
                    rfcEmployee = data.sRFC;
                    sAniosAntiquity = data.iAnios.ToString();
                    sDaysAntiquity = data.sDias;
                    centrCost = data.iCentro_costo_id.ToString();
                    salaryDay = data.sSalario_diario;
                    salaryMonth = data.sSalario_mensual;
                    jobEmployee = data.sPuesto;
                    jobCodeEmployee = data.sPuesto_codigo;
                    centroCostName = data.sCentro_costo;
                    departamentName = data.sDepartamento;
                    departamentCode = data.sDepto_codigo;
                    registerImss = data.sRegistroImss;
                    countBank = data.sCta_Cheques;
                    dateStartPay = data.sFecha_Pago_Inicio;
                    dateEndPay = data.sFecha_Pago_Fin;
                    periodPay = data.iPeriodo.ToString();
                }
                //Foreach para sacar el saldo total
                foreach (DatosFiniquito data in listDataDownBean)
                {
                    nameEmployee = data.sNombre;
                    if (data.iIdValor == 5 && data.iRenglon_id == 9999)
                    {
                        totalBalance = data.sSaldo;
                        break;
                    }
                }
                ConversorMoneda convertMon = new ConversorMoneda();
                nameFilePdf = "CANCELACION_" + nameEmployee.Replace(" ", "").ToUpper() + "_" + keyEmployee.ToString() + "_E" + keyBusiness + ".pdf";
                if (System.IO.File.Exists(pathSaveDocs + nameFolder + @"\\" + nameFilePdf))
                {
                    System.IO.File.Delete(pathSaveDocs + nameFolder + @"\\" + nameFilePdf);
                }
                FileStream fs = new FileStream(pathSaveDocs + nameFolder + @"\\" + nameFilePdf, FileMode.Create);
                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 20, 40, 20, 40);
                PdfWriter pw = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font fontBold       = new Font(FontFamily.HELVETICA, 10, Font.BOLD);
                Font fontDefault    = new Font(FontFamily.HELVETICA, 10);
                Font fontTitle      = new Font(FontFamily.HELVETICA, 14);
                Font fontParagraph  = new Font(FontFamily.HELVETICA, 8, Font.ITALIC);
                Font fontCells      = new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                Paragraph pr        = new Paragraph();
                DateTime datePdf    = DateTime.Now;
                string convertDateDown      = utilerias.ConvertDateFormat_year_month_day(dateDown);
                string convertDateAnti      = utilerias.ConvertDateFormat_year_month_day(dateAntiquity);
                string datePrintReceipt     = utilerias.ConvertDateText(convertDateDown);
                string dateAntiquityConvert = utilerias.ConvertDateText(convertDateAnti);
                pr.Font = fontTitle;
                pr.Add("COMPROBANTE DE PERCEPCIONES Y DEDUCCIONES");
                pr.Alignment = Element.ALIGN_CENTER;
                doc.Add(pr);
                pr.Clear();
                // Creamos la tabla con los datos del usuario
                doc.Add(new Chunk("\n"));
                PdfPTable tableDataEm = new PdfPTable(2);
                tableDataEm.WidthPercentage = 100;
                // Creamos la celda 1 
                pr.Font = fontCells;
                pr.Alignment = Element.ALIGN_LEFT;
                pr.Add("NOMBRE: " + keyEmployee.ToString() + " - " + nameEmployee.ToUpper());
                PdfPCell cell1 = new PdfPCell();
                cell1.BorderWidth = 0.75f;
                cell1.PaddingTop = -3;
                cell1.AddElement(pr);
                tableDataEm.AddCell(cell1);
                pr.Clear();
                // Creamos la celda 2
                pr.Font = fontCells;
                pr.Add("ANTIGUEDAD: " + dateAntiquityConvert);
                PdfPCell cell2 = new PdfPCell();
                cell2.BorderWidth = 0.75f;
                cell2.PaddingTop = -3;
                cell2.AddElement(pr);
                tableDataEm.AddCell(cell2);
                pr.Clear();
                // Agregamos el puesto a la celda 1
                pr.Font = fontCells;
                pr.Add("PUESTO: " + jobCodeEmployee + " - " + jobEmployee);
                cell1 = new PdfPCell();
                cell1.BorderWidth = 0.75f;
                cell1.PaddingTop = -3;
                cell1.AddElement(pr);
                tableDataEm.AddCell(cell1);
                pr.Clear();
                // Agregamos el rfc a la celda 2
                pr.Font = fontCells;
                pr.Add("RFC: " + rfcEmployee);
                cell2 = new PdfPCell();
                cell2.BorderWidth = 0.75f;
                cell2.PaddingTop = -3;
                cell2.AddElement(pr);
                tableDataEm.AddCell(cell2);
                pr.Clear();
                // Agregamos el departamento a la celda 1
                pr.Font = fontCells;
                pr.Add("DEPARTAMENTO: " + departamentCode + " - " + departamentName);
                cell1 = new PdfPCell();
                cell1.BorderWidth = 0.75f;
                cell1.PaddingTop = -3;
                cell1.AddElement(pr);
                tableDataEm.AddCell(cell1);
                pr.Clear();
                // Agregamos el Imss a la celda 2
                pr.Font = fontCells;
                pr.Add("IMSS: " + registerImss);
                cell2 = new PdfPCell();
                cell2.BorderWidth = 0.75f;
                cell2.PaddingTop = -3;
                cell2.AddElement(pr);
                tableDataEm.AddCell(cell2);
                pr.Clear();
                doc.Add(tableDataEm);
                doc.Add(new Chunk("\n"));
                pr.Font = fontBold;
                string convertDateStartPay = utilerias.ConvertDateFormat_year_month_day(dateStartPay);
                string convertDateEndPay   = utilerias.ConvertDateFormat_year_month_day(dateEndPay);
                string startPay   = utilerias.ConvertDateText(convertDateStartPay);
                string   endPay   = utilerias.ConvertDateText(convertDateEndPay);
                pr.Add("Fecha de pago del " + startPay + " al " + endPay + " Periodo " + periodPay);
                pr.Alignment = Element.ALIGN_RIGHT;
                doc.Add(pr);
                pr.Clear();
                doc.Add(new Chunk("\n"));
                // Creamos la tabla que contendra la informacion del finiquito
                PdfPTable tableInfo = new PdfPTable(6);
                tableInfo.WidthPercentage = 100;
                // Creamos la celda de percepciones
                pr.Font = new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                pr.Alignment = Element.ALIGN_CENTER;
                pr.Add("PERCEPCIONES");
                PdfPCell clPercepciones = new PdfPCell();
                clPercepciones.BorderWidth = 0.75f;
                clPercepciones.PaddingTop = -7;
                clPercepciones.Colspan = 3;
                clPercepciones.AddElement(pr);
                pr.Clear();
                // Creamos la celda de deducciones
                pr.Font = new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                pr.Alignment = Element.ALIGN_CENTER;
                pr.Add("DEDUCCIONES");
                PdfPCell clDeducciones = new PdfPCell();
                clDeducciones.BorderWidth = 0.75f;
                clDeducciones.PaddingTop = -7;
                clDeducciones.Colspan = 3;
                clDeducciones.AddElement(pr);
                pr.Clear();
                tableInfo.AddCell(clPercepciones);
                tableInfo.AddCell(clDeducciones);
                // Foreach para llenar los datos de las celdas
                foreach (DatosFiniquito data in listDataDownBean)
                {
                    int lengthPer = 0;
                    int lengthDed = 0;
                    if (data.iIdValor == 1 && data.iRenglon_id != 990)
                    {
                        lengthPer += 1;
                    }
                    if (data.iIdValor == 2 && data.iRenglon_id != 1990)
                    {
                        lengthDed += 1;
                    }
                    string descPerc = "";
                    string totaPerc = "";
                    string descDedu = "";
                    string totaDedu = "";
                    if (lengthPer > 0)
                    {
                        if (data.iIdValor == 1 && data.iRenglon_id != 990)
                        {
                            descPerc = data.sNombre_Renglon + " " + data.sLeyenda;
                            totaPerc = "-$" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo));
                        }
                    }
                    // Descripcion de la percepcion
                    clPercepciones = new PdfPCell(new Phrase("     " + descPerc, _standardFont));
                    clPercepciones.Colspan = 2;
                    tableInfo.AddCell(clPercepciones);
                    // Total de la percepcion
                    clPercepciones = new PdfPCell(new Phrase("     " + totaPerc, _standardFont));
                    clPercepciones.Colspan = 1;
                    tableInfo.AddCell(clPercepciones);
                    if (lengthDed > 0)
                    {
                        if (data.iIdValor == 2 && data.iRenglon_id != 1990)
                        {
                            descDedu = data.sNombre_Renglon + " " + data.sLeyenda;
                            totaDedu = "-$" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo));
                        }
                    }
                    // Descripcion de la deduccion
                    clDeducciones = new PdfPCell(new Phrase("     " + descDedu, _standardFont));
                    clDeducciones.Colspan = 2;
                    tableInfo.AddCell(clDeducciones);
                    // Total de la deduccion
                    clDeducciones = new PdfPCell(new Phrase("     " + totaDedu, _standardFont));
                    clDeducciones.Colspan = 1;
                    tableInfo.AddCell(clDeducciones);

                }
                // Foreach para agregar los totales de deduccion y percepcion
                foreach (DatosFiniquito data in listDataDownBean)
                {
                    if (data.iIdValor == 1 && data.iRenglon_id == 990)
                    {
                        // Descripcion de la percepcion
                        clPercepciones = new PdfPCell(new Phrase("     " + data.sNombre_Renglon, _standardFont));
                        clPercepciones.Colspan = 2;
                        tableInfo.AddCell(clPercepciones);
                        // Total de la percepcion
                        clPercepciones = new PdfPCell(new Phrase("    - $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo)), _standardFont));
                        clPercepciones.Colspan = 1;
                        tableInfo.AddCell(clPercepciones);
                        totalPercepcion = data.sSaldo;
                    }
                    if (data.iIdValor == 2 && data.iRenglon_id == 1990)
                    {
                        // Descripcion de la deduccion
                        clDeducciones = new PdfPCell(new Phrase("     " + data.sNombre_Renglon, _standardFont));
                        clDeducciones.Colspan = 2;
                        tableInfo.AddCell(clDeducciones);
                        // Total de la deduccion
                        clDeducciones = new PdfPCell(new Phrase("    - $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo)), _standardFont));
                        clDeducciones.Colspan = 1;
                        tableInfo.AddCell(clDeducciones);
                        totalDeduccion = data.sSaldo;
                    }
                }
                pr.Clear();
                pr.Font = _standardFont;
                pr.Add("     Liquidacion a favor: " + countBank);
                pr.Alignment = Element.ALIGN_CENTER;
                clPercepciones = new PdfPCell();
                clPercepciones.Colspan = 3;
                clPercepciones.PaddingTop = -3;
                clPercepciones.AddElement(pr);
                tableInfo.AddCell(clPercepciones);
                pr.Clear();
                pr.Font = _standardFont;
                pr.Add("     -$" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(totalBalance)));
                pr.Alignment = Element.ALIGN_CENTER;
                clDeducciones = new PdfPCell();
                clDeducciones.Colspan = 3;
                clDeducciones.PaddingTop = -3;
                clDeducciones.AddElement(pr);
                tableInfo.AddCell(clDeducciones);
                pr.Clear();
                // Añadimos la tabla al documento
                doc.Add(tableInfo);
                // Añadimos un salto de linea
                doc.Add(new Chunk("\n"));
                // Cambiamos el tipo de fuente
                pr.Font = fontParagraph;
                // Asignamos una variable parrafo y le asignamos su valor
                string paragraphFooter = "Ambas partes manifiestan su conformidad para solicitar aclaraciones al presente recibo o estado de cuenta en un lapso de 15 días naturales. Transcurrido dicho lapso se entiende que esta de acuerdo con el pago y todos los conceptos que se especifican.";
                // Añadimos la variable parrafo
                pr.Add(paragraphFooter);
                // Definimos una alineacion 
                pr.Alignment = Element.ALIGN_JUSTIFIED;
                // Añadimos la lista pr al documento
                doc.Add(pr);
                // Quitamos los elementos de la lista
                pr.Clear();
                doc.Close();
                pw.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            dataR.sNombrePDF    = nameFilePdf;
            dataR.sNombreFolder = nameFolder;
            dataR.sTotalDeduccion  = totalDeduccion;
            dataR.sTotalPercepcion = totalPercepcion;
            dataR.sTotal = totalBalance;
            return dataR;
        }

        [HttpPost]
        public JsonResult GenerateReceiptDown(int keySettlement, int keyEmployee)
        {
            Boolean flag        = false;
            String messageError = "none";
            string nameFilePdf  = "";
            string nameFolder   = "";
            Utilerias utilerias = new Utilerias();
            List<BajasEmpleadosBean> dataDownEmployee = new List<BajasEmpleadosBean>();
            List<DatosFiniquito> listDataDownBean     = new List<DatosFiniquito>();
            BajasEmpleadosDaoD dataDownEmplDaoD       = new BajasEmpleadosDaoD();
            string nameEmployee = "";
            string totalPercepcion = "0";
            string totalDeduccion  = "0";
            string totalBalance = "";
            try
            { 
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                dataDownEmployee = dataDownEmplDaoD.sp_Finiquitos_Empleado(keyEmployee, keyBusiness, keySettlement);
                if (dataDownEmployee.Count > 0)
                {
                    listDataDownBean = dataDownEmplDaoD.sp_Info_Finiquito_Empleado(keySettlement);
                    if (listDataDownBean.Count > 0)
                    {
                        Boolean flagTypePDF = false;
                        foreach (BajasEmpleadosBean data in dataDownEmployee)
                        {
                            if (data.sCancelado == "True")
                            {
                                flagTypePDF = true;
                            }
                        }
                        // Comprobamos que el finiquito este cancelado
                        // Si lo esta genera el pdf dentro del if
                        // Si no lo esta genera el pdf dentro del else
                        if (flagTypePDF == true)
                        {
                            flag = true;
                            DatosPDFCancelado data = new DatosPDFCancelado();
                            data = GenerateReceiptDownCancel(dataDownEmployee, listDataDownBean, keyEmployee, keyBusiness);
                            nameFilePdf = data.sNombrePDF;
                            nameFolder  = data.sNombreFolder;
                            totalDeduccion  = data.sTotalDeduccion;
                            totalPercepcion = data.sTotalPercepcion;
                            totalBalance    = data.sTotal;
                        }
                        else
                        {
                            flag = true;
                            string pathSaveDocs = Server.MapPath("~/Content/");
                            nameFolder = "DOCSFINIQUITOS";
                            string nameFileTest = "test.txt";
                            if (!Directory.Exists(pathSaveDocs + nameFolder))
                            {
                                Directory.CreateDirectory(pathSaveDocs + nameFolder);
                            }
                            if (!System.IO.File.Exists(pathSaveDocs + nameFolder + @"\\" + nameFileTest))
                            {
                                using (StreamWriter fileTest = new StreamWriter(pathSaveDocs + nameFolder + @"\\" + nameFileTest, false, Encoding.UTF8))
                                {
                                    fileTest.Write("NO REMOVER");
                                    fileTest.Close();
                                }
                            }
                            // ** Definimos las variables a utilizar para mostrar la informacion del finiquito seleccionado en el pdf ** \\
                            string dateReceipt = "";
                            string dateDown = "";
                            string dateEntry = "";
                            string dateAntiquity = "";
                            string rfcEmployee = "";
                            string sAniosAntiquity = "";
                            string sDaysAntiquity = "";
                            string typeSettlement = "";
                            string nameBusiness = "";
                            
                            string centrCost = "";
                            string salaryMonth = "";
                            string salaryDay = "";
                            string jobEmployee = "";
                            string centroCostName = "";
                            string departamentName = "";
                            string departamentCode = "";
                            // Foreach para obtener datos de la empresa
                            foreach (BajasEmpleadosBean data in dataDownEmployee)
                            {
                                dateReceipt = data.sFecha_recibo;
                                typeSettlement = data.sFiniquito_valor;
                                nameBusiness = data.sEmpresa;
                                dateDown = data.sFecha_baja;
                                dateEntry = data.sFecha_ingreso;
                                dateAntiquity = data.sFecha_antiguedad;
                                rfcEmployee = data.sRFC;
                                sAniosAntiquity = data.iAnios.ToString();
                                sDaysAntiquity = data.sDias;
                                centrCost = data.iCentro_costo_id.ToString();
                                salaryDay = data.sSalario_diario;
                                salaryMonth = data.sSalario_mensual;
                                jobEmployee = data.sPuesto;
                                centroCostName = data.sCentro_costo;
                                departamentName = data.sDepartamento;
                                departamentCode = data.sDepto_codigo;
                            }
                            //Foreach para sacar el saldo total
                            foreach (DatosFiniquito data in listDataDownBean)
                            {
                                nameEmployee = data.sNombre;
                                if (data.iIdValor == 5 && data.iRenglon_id == 9999)
                                {
                                    totalBalance = data.sSaldo;
                                    break;
                                }
                            }
                            if (totalBalance == "") {
                                totalBalance = "0";
                            }
                            ConversorMoneda convertMon = new ConversorMoneda();
                            // Definimos el nombre del archivo pdf
                            nameFilePdf = nameEmployee.Replace(" ", "").ToUpper() + "_" + keyEmployee.ToString() + "_E" + keyBusiness + ".pdf";
                            //nameFilePdf = "E" + keyBusiness.ToString() + "_" + typeSettlement.Replace(" ", "_").ToUpper() + "_" + keyEmployee.ToString() + ".pdf";
                            if (System.IO.File.Exists(pathSaveDocs + nameFolder + @"\\" + nameFilePdf))
                            {
                                System.IO.File.Delete(pathSaveDocs + nameFolder + @"\\" + nameFilePdf);
                            }
                            FileStream fs = new FileStream(pathSaveDocs + nameFolder + @"\\" + nameFilePdf, FileMode.Create);
                            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 20, 40, 20, 40);
                            PdfWriter pw = PdfWriter.GetInstance(doc, fs);
                            doc.Open();
                            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                            Font fontBold = new Font(FontFamily.HELVETICA, 10, Font.BOLD);
                            Font fontDefault = new Font(FontFamily.HELVETICA, 10);
                            Font fontTitle = new Font(FontFamily.HELVETICA, 14);
                            Font fontParagraph = new Font(FontFamily.HELVETICA, 8, Font.ITALIC);
                            Paragraph pr = new Paragraph();
                            DateTime datePdf = DateTime.Now;
                            string convertDateDown  = utilerias.ConvertDateFormat_year_month_day(dateDown);
                            string datePrintReceipt = utilerias.ConvertDateText(convertDateDown);
                            pr.Font = fontBold;
                            pr.Add(nameBusiness.ToUpper() + ".");
                            doc.Add(pr);
                            pr.Clear();
                            //doc.Add(new Chunk("\n"));
                            pr.Font = fontBold;
                            pr.Add("Fecha: " + datePrintReceipt + ".");
                            pr.Alignment = Element.ALIGN_LEFT;
                            doc.Add(pr);
                            pr.Clear();
                            //doc.Add(new Chunk("\n"));
                            pr.Font = fontTitle;
                            pr.Add(typeSettlement);
                            pr.Alignment = Element.ALIGN_CENTER;
                            doc.Add(pr);
                            pr.Clear();
                            doc.Add(new Chunk("\n"));
                            pr.Font = fontParagraph;
                            string balanceConvertText = convertMon.Convertir(Convert.ToDouble(totalBalance).ToString(), true, "PESOS");
                            string dateDownConvert = utilerias.ConvertDateText(convertDateDown);
                            string paragraphDescription = "Recibí de " + nameBusiness.ToUpper() + " la cantidad neta de $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(totalBalance)) + " (" + balanceConvertText + "). Por los conceptos detallados en la hoja de desglose de finiquito que me corresponde, con motivo de mi renuncia a esta institución, por lo que dejo de prestar mis servicios de manera voluntaria a partir del día " + dateDownConvert + ".";
                            pr.Add(paragraphDescription);
                            pr.Alignment = Element.ALIGN_JUSTIFIED;
                            doc.Add(pr);
                            pr.Clear();
                            doc.Add(new Chunk("\n"));
                            string paragrahpDescription2 = "En esa virtud manifiesto que estoy conforme con el finiquito mencionado, en cuanto a su importe y conceptos que se especifican, el cual recibo a mi entera satisfación y por lo mismo, les otorgo mi más amplio finiquito, sin reserva de ninguna acción, ni derecho que ejercitar posteriormente en contra de " + nameBusiness + ", " + "ya que durante el tiempo que presté mis servicios, siempre me fueron cubiertas íntegra y puntualmente todas las prestaciones a que tuve derecho, tales como: aguinaldo, vacaciones, prima vacacional, salarios, tiempo extra, descansos obligatorios y en si cualquier otra prestación derivada de mi contrato individual de trabajo, o de la normatividad de la materia, por consiguiente me doy por pagado de todas y cada una de ellas.";
                            pr.Font = fontParagraph;
                            pr.Add(paragrahpDescription2);
                            pr.Alignment = Element.ALIGN_JUSTIFIED;
                            doc.Add(pr);
                            pr.Clear();
                            doc.Add(new Chunk("\n"));
                            // Creamos la tabla que contendra la informacion del finiquito
                            PdfPTable tableInfo = new PdfPTable(6);
                            tableInfo.WidthPercentage = 100;
                            // Creamos la celda de percepciones
                            pr.Font = new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("PERCEPCIONES");
                            PdfPCell clPercepciones = new PdfPCell();
                            clPercepciones.BorderWidth = 0.75f;
                            clPercepciones.PaddingTop = -7;
                            clPercepciones.Colspan = 3;
                            clPercepciones.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de deducciones
                            pr.Font = new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("DEDUCCIONES");
                            PdfPCell clDeducciones = new PdfPCell();
                            clDeducciones.BorderWidth = 0.75f;
                            clDeducciones.PaddingTop = -7;
                            clDeducciones.Colspan = 3;
                            clDeducciones.AddElement(pr);
                            pr.Clear();
                            tableInfo.AddCell(clPercepciones);
                            tableInfo.AddCell(clDeducciones);
                            // Foreach para llenar los datos de las celdas
                            foreach (DatosFiniquito data in listDataDownBean)
                            {
                                int lengthPer = 0;
                                int lengthDed = 0;
                                if (data.iIdValor == 1 && data.iRenglon_id != 990)
                                {
                                    lengthPer += 1;
                                }
                                if (data.iIdValor == 2 && data.iRenglon_id != 1990)
                                {
                                    lengthDed += 1;
                                }
                                string descPerc = "";
                                string totaPerc = "";
                                string descDedu = "";
                                string totaDedu = "";
                                if (lengthPer > 0)
                                {
                                    if (data.iIdValor == 1 && data.iRenglon_id != 990)
                                    {
                                        descPerc = data.sNombre_Renglon + " " + data.sLeyenda;
                                        totaPerc = "$" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo));
                                    }
                                }
                                // Descripcion de la percepcion
                                clPercepciones = new PdfPCell(new Phrase("     " + descPerc, _standardFont));
                                clPercepciones.Colspan = 2;
                                tableInfo.AddCell(clPercepciones);
                                // Total de la percepcion
                                clPercepciones = new PdfPCell(new Phrase("     " + totaPerc, _standardFont));
                                clPercepciones.Colspan = 1;
                                tableInfo.AddCell(clPercepciones);
                                if (lengthDed > 0)
                                {
                                    if (data.iIdValor == 2 && data.iRenglon_id != 1990)
                                    {
                                        descDedu = data.sNombre_Renglon + " " + data.sLeyenda;
                                        totaDedu = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo));
                                    }
                                }
                                // Descripcion de la deduccion
                                clDeducciones = new PdfPCell(new Phrase("     " + descDedu, _standardFont));
                                clDeducciones.Colspan = 2;
                                tableInfo.AddCell(clDeducciones);
                                // Total de la deduccion
                                clDeducciones = new PdfPCell(new Phrase("     " + totaDedu, _standardFont));
                                clDeducciones.Colspan = 1;
                                tableInfo.AddCell(clDeducciones);

                            }
                            // Foreach para agregar los totales de deduccion y percepcion
                            foreach (DatosFiniquito data in listDataDownBean)
                            {
                                if (data.iIdValor == 1 && data.iRenglon_id == 990)
                                {
                                    // Descripcion de la percepcion
                                    clPercepciones = new PdfPCell(new Phrase("     " + data.sNombre_Renglon, _standardFont));
                                    clPercepciones.Colspan = 2;
                                    tableInfo.AddCell(clPercepciones);
                                    // Total de la percepcion
                                    clPercepciones = new PdfPCell(new Phrase("     $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo)), _standardFont));
                                    clPercepciones.Colspan = 1;
                                    tableInfo.AddCell(clPercepciones);
                                    totalPercepcion = data.sSaldo;
                                }
                                if (data.iIdValor == 2 && data.iRenglon_id == 1990)
                                {
                                    // Descripcion de la deduccion
                                    clDeducciones = new PdfPCell(new Phrase("     " + data.sNombre_Renglon, _standardFont));
                                    clDeducciones.Colspan = 2;
                                    tableInfo.AddCell(clDeducciones);
                                    // Total de la deduccion
                                    clDeducciones = new PdfPCell(new Phrase("     $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo)), _standardFont));
                                    clDeducciones.Colspan = 1;
                                    tableInfo.AddCell(clDeducciones);
                                    totalDeduccion = data.sSaldo;
                                }
                            }
                            Font fontCells = new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                            tableInfo.AddCell(clDeducciones);
                            doc.Add(tableInfo);
                            doc.Add(new Chunk("\n"));
                            pr.Clear();
                            pr.Font = new Font(FontFactory.GetFont("ARIAL", 10, Font.BOLD));
                            pr.Add("Finiquito a favor: $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(totalBalance)));
                            pr.Alignment = Element.ALIGN_RIGHT;
                            doc.Add(pr);
                            pr.Clear();
                            doc.Add(new Chunk("\n"));
                            // Creamos una nueva tabla para mostrar los ultimos detalles del empleado
                            PdfPTable detailsJobTable2 = new PdfPTable(6);
                            detailsJobTable2.WidthPercentage = 100;
                            // Creamos la celda de salario mensual
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Salario Mensual");
                            PdfPCell clSalarioMensual = new PdfPCell();
                            clSalarioMensual.BorderWidth = 0.75f;
                            clSalarioMensual.PaddingTop = -7;
                            clSalarioMensual.Colspan = 2;
                            clSalarioMensual.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de salario diario
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Salario Diario");
                            PdfPCell clSalarioDiario = new PdfPCell();
                            clSalarioDiario.BorderWidth = 0.75f;
                            clSalarioDiario.PaddingTop = -7;
                            clSalarioDiario.Colspan = 2;
                            clSalarioDiario.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de nomina
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Nomina");
                            PdfPCell clNomina = new PdfPCell();
                            clNomina.BorderWidth = 0.75f;
                            clNomina.PaddingTop = -7;
                            clNomina.Colspan = 2;
                            clNomina.AddElement(pr);
                            pr.Clear();
                            // Agregamos las celdas a la tabla
                            detailsJobTable2.AddCell(clSalarioMensual);
                            detailsJobTable2.AddCell(clSalarioDiario);
                            detailsJobTable2.AddCell(clNomina);
                            // Dato del salario mensual
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("$" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(salaryMonth)));
                            clSalarioMensual = new PdfPCell();
                            clSalarioMensual.BorderWidth = 0.75f;
                            clSalarioMensual.PaddingTop = -7;
                            clSalarioMensual.Colspan = 2;
                            clSalarioMensual.AddElement(pr);
                            pr.Clear();
                            detailsJobTable2.AddCell(clSalarioMensual);
                            // Dato del salario diario
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("$" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(salaryDay)));
                            clSalarioDiario = new PdfPCell();
                            clSalarioDiario.BorderWidth = 0.75f;
                            clSalarioDiario.PaddingTop = -7;
                            clSalarioDiario.Colspan = 2;
                            clSalarioDiario.AddElement(pr);
                            pr.Clear();
                            detailsJobTable2.AddCell(clSalarioDiario);
                            // Dato de la nomina
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(keyEmployee.ToString());
                            clNomina = new PdfPCell();
                            clNomina.BorderWidth = 0.75f;
                            clNomina.PaddingTop = -7;
                            clNomina.Colspan = 2;
                            clNomina.AddElement(pr);
                            pr.Clear();
                            detailsJobTable2.AddCell(clNomina);
                            // Agregamos la tabla al pdf
                            doc.Add(detailsJobTable2);
                            //doc.Add(new Chunk("\n"));
                            PdfPTable detailsTable = new PdfPTable(10);
                            detailsTable.WidthPercentage = 100;
                            // Creamos la celda de fecha antiguedad
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Fecha de Antiguedad");
                            PdfPCell clFechaAntiguedad = new PdfPCell();
                            clFechaAntiguedad.BorderWidth = 0.75f;
                            clFechaAntiguedad.PaddingTop = -7;
                            clFechaAntiguedad.Colspan = 2;
                            clFechaAntiguedad.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de fecha ingreso
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Fecha de Ingreso");
                            PdfPCell clFechaIngreso = new PdfPCell();
                            clFechaIngreso.BorderWidth = 0.75f;
                            clFechaIngreso.PaddingTop = -7;
                            clFechaIngreso.Colspan = 2;
                            clFechaIngreso.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de fecha de baja
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Fecha de Baja");
                            PdfPCell clFechaBaja = new PdfPCell();
                            clFechaBaja.BorderWidth = 0.75f;
                            clFechaBaja.PaddingTop = -7;
                            clFechaBaja.Colspan = 2;
                            clFechaBaja.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de rfc
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("RFC");
                            PdfPCell clRFC = new PdfPCell();
                            clRFC.BorderWidth = 0.75f;
                            clRFC.PaddingTop = -7;
                            clRFC.Colspan = 2;
                            clRFC.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de antiguedad
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Antiguedad");
                            PdfPCell clAntiguedad = new PdfPCell();
                            clAntiguedad.BorderWidth = 0.75f;
                            clAntiguedad.PaddingTop = -7;
                            clAntiguedad.Colspan = 2;
                            clAntiguedad.AddElement(pr);
                            pr.Clear();
                            // Añadimos las celdas a la tabla
                            detailsTable.AddCell(clFechaAntiguedad);
                            detailsTable.AddCell(clFechaIngreso);
                            detailsTable.AddCell(clFechaBaja);
                            detailsTable.AddCell(clRFC);
                            detailsTable.AddCell(clAntiguedad);
                            // LLenamos con informacion las celda fecha de antiguedad
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(dateAntiquity);
                            clFechaAntiguedad = new PdfPCell();
                            clFechaAntiguedad.BorderWidth = 0.75f;
                            clFechaAntiguedad.PaddingTop = -7;
                            clFechaAntiguedad.Colspan = 2;
                            clFechaAntiguedad.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clFechaAntiguedad);
                            // LLenamos con informacion la celda de fecha de ingreso
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(dateEntry);
                            clFechaIngreso = new PdfPCell();
                            clFechaIngreso.BorderWidth = 0.75f;
                            clFechaIngreso.PaddingTop = -7;
                            clFechaIngreso.Colspan = 2;
                            clFechaIngreso.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clFechaIngreso);
                            // LLenamos con informacion la celda de fecha de baja
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(dateDown);
                            clFechaBaja = new PdfPCell();
                            clFechaBaja.BorderWidth = 0.75f;
                            clFechaBaja.PaddingTop = -7;
                            clFechaBaja.Colspan = 2;
                            clFechaBaja.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clFechaBaja);
                            // LLenamos con informacion la celda de rfc
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(rfcEmployee);
                            clRFC = new PdfPCell();
                            clRFC.BorderWidth = 0.75f;
                            clRFC.PaddingTop = -7;
                            clRFC.Colspan = 2;
                            clRFC.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clRFC);
                            // LLenamos con informaicon la celda de antiguedad
                            // Agregamos los días
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(sDaysAntiquity + " dias");
                            clAntiguedad = new PdfPCell();
                            clAntiguedad.BorderWidth = 0.75f;
                            clAntiguedad.PaddingTop = -7;
                            clAntiguedad.Colspan = 1;
                            clAntiguedad.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clAntiguedad);
                            // Agregamos los años
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(sAniosAntiquity + " años");
                            clAntiguedad = new PdfPCell();
                            clAntiguedad.BorderWidth = 0.75f;
                            clAntiguedad.PaddingTop = -7;
                            clAntiguedad.Colspan = 1;
                            clAntiguedad.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clAntiguedad);
                            doc.Add(detailsTable);
                            // Creamos la tabla de los datos del puesto del empleado
                            PdfPTable detailsJobTable = new PdfPTable(8);
                            detailsJobTable.WidthPercentage = 100;
                            // Creamos la celda del centro de costo
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Centro de costo");
                            PdfPCell clCentroCosto = new PdfPCell();
                            clCentroCosto.BorderWidth = 0.75f;
                            clCentroCosto.PaddingTop = -7;
                            clCentroCosto.Colspan = 4;
                            clCentroCosto.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de puesto
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Puesto");
                            PdfPCell clPuesto = new PdfPCell();
                            clPuesto.BorderWidth = 0.75f;
                            clPuesto.PaddingTop = -7;
                            clPuesto.Colspan = 4;
                            clPuesto.AddElement(pr);
                            pr.Clear();
                            // Añadimos las celdas a la tabla
                            detailsJobTable.AddCell(clCentroCosto);
                            //detailsJobTable.AddCell(clDepartamento);
                            detailsJobTable.AddCell(clPuesto);
                            // Añadimos datos a las celdas de la tabla detailsJobTable
                            // Dato del centro de costo
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(centrCost + " - " + centroCostName);
                            clCentroCosto = new PdfPCell();
                            clCentroCosto.BorderWidth = 0.75f;
                            clCentroCosto.PaddingTop = -7;
                            clCentroCosto.Colspan = 4;
                            clCentroCosto.AddElement(pr);
                            pr.Clear();
                            detailsJobTable.AddCell(clCentroCosto);
                            // Dato del puesto
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(jobEmployee);
                            clPuesto = new PdfPCell();
                            clPuesto.BorderWidth = 0.75f;
                            clPuesto.PaddingTop = -7;
                            clPuesto.Colspan = 4;
                            clPuesto.AddElement(pr);
                            pr.Clear();
                            detailsJobTable.AddCell(clPuesto);
                            // Agregamos la tabla al pdf
                            doc.Add(detailsJobTable);
                            doc.Add(new Chunk("\n"));
                            //doc.Add(new Chunk("\n"));
                            // Creamos la ultima tabla para el nombre y firma
                            PdfPTable tableFirm = new PdfPTable(9);
                            tableFirm.WidthPercentage = 100;
                            // Creamos la celda de departamento
                            Font fontFirm = new Font(FontFactory.GetFont("ARIAL", 10, Font.BOLD));
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Departamento");
                            PdfPCell clDepartamento = new PdfPCell();
                            clDepartamento.Border = 0;
                            clDepartamento.PaddingTop = -7;
                            clDepartamento.Colspan = 3;
                            clDepartamento.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de fecha de baja
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Fecha Baja");
                            PdfPCell clFechaBajaTexto = new PdfPCell();
                            clFechaBajaTexto.Border = 0;
                            clFechaBajaTexto.PaddingTop = -7;
                            clFechaBajaTexto.Colspan = 3;
                            clFechaBajaTexto.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de nombre
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(nameEmployee);
                            PdfPCell clNombreEmpleado = new PdfPCell();
                            clNombreEmpleado.Border = 0;
                            clNombreEmpleado.PaddingTop = -7;
                            clNombreEmpleado.Colspan = 3;
                            clNombreEmpleado.AddElement(pr);
                            pr.Clear();
                            //Agregamos la celda a la tabla
                            tableFirm.AddCell(clDepartamento);
                            tableFirm.AddCell(clFechaBajaTexto);
                            tableFirm.AddCell(clNombreEmpleado);
                            //Agregamos el valor de nombre y firma abajo de la celda de nombre
                            //Dato del departamento
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(departamentName);
                            clDepartamento = new PdfPCell();
                            clDepartamento.BorderWidth = 0;
                            clDepartamento.PaddingTop = 7;
                            clDepartamento.Colspan = 3;
                            clDepartamento.AddElement(pr);
                            pr.Clear();
                            tableFirm.AddCell(clDepartamento);
                            //Dato de la fecha de baja texto
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(dateDownConvert);
                            clDepartamento = new PdfPCell();
                            clDepartamento.BorderWidth = 0;
                            clDepartamento.PaddingTop = 7;
                            clDepartamento.Colspan = 3;
                            clDepartamento.AddElement(pr);
                            pr.Clear();
                            tableFirm.AddCell(clDepartamento);
                            // Dato de la nomina
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Nombre y Firma");
                            clNombreEmpleado = new PdfPCell();
                            clNombreEmpleado.BorderWidth = 0;
                            clNombreEmpleado.PaddingTop = 7;
                            clNombreEmpleado.Colspan = 3;
                            clNombreEmpleado.AddElement(pr);
                            tableFirm.AddCell(clNombreEmpleado);
                            pr.Clear();
                            // Agegamos la tabla al pdf
                            doc.Add(tableFirm);
                            // Agregamos el numero de nomina del empleado
                            doc.Add(new Chunk("\n"));
                            PdfPTable tablePayroll = new PdfPTable(1);
                            tablePayroll.WidthPercentage = 100;
                            // Creamos la celda de Nomina
                            Font fontPayroll = new Font(FontFactory.GetFont("ARIAL", 10, Font.BOLD));
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("NOMINA: " + keyEmployee.ToString());
                            PdfPCell clNominaE = new PdfPCell();
                            clNominaE.Border = 0;
                            clNominaE.PaddingTop = -7;
                            clNominaE.Colspan = 3;
                            clNominaE.AddElement(pr);
                            pr.Clear();
                            tablePayroll.AddCell(clNominaE);
                            doc.Add(tablePayroll);
                            doc.Close();
                            pw.Close();
                        }
                    }
                    else
                    {
                        messageError = "ERRNOTDATA";
                    }
                }
                else
                {
                    messageError = "ERRLOADINFFINIQUITO";
                }
            }
            catch (Exception exc)
            {
                messageError = exc.Message.ToString();
            }
            string nameFileSession = Path.GetFileNameWithoutExtension(nameFilePdf);
            Session[nameFilePdf] = nameFilePdf;
            return Json(new { Bandera = flag, MensajeError = messageError, NombrePDF = nameFilePdf, InfoFiniquito = dataDownEmployee, NombreFolder = nameFolder, Datos = listDataDownBean, Deduccion = string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(totalDeduccion)), Percepcion = string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(totalPercepcion)), Total = string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(totalBalance)) });
        }

        [HttpPost]
        public JsonResult DeletePdfSettlement(string namePdfSettlement, string nameFolderLocation)
        {
            Boolean flagV = false;
            Boolean flagC = false;
            Boolean flagD = false;
            String messageError = "none";
            string pathSaveDocs = Server.MapPath("~/Content/");
            string folderFiles = nameFolderLocation;
            try
            {
                string namePdfClear = namePdfSettlement.Trim();
                if (Session[namePdfSettlement] != null)
                {
                    flagV = true;
                    if (System.IO.File.Exists(pathSaveDocs + folderFiles + @"\\" + Session[namePdfSettlement].ToString()))
                    {
                        flagC = true;
                        System.IO.File.Delete(pathSaveDocs + folderFiles + @"\\" + Session[namePdfSettlement].ToString());
                        if (!System.IO.File.Exists(pathSaveDocs + folderFiles + @"\\" + Session[namePdfSettlement].ToString()))
                        {
                            flagD = true;
                            Session.Remove(namePdfSettlement);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                messageError = exc.Message.ToString();
            }
            return Json(new { BanderaValida = flagV, BanderaComprueba = flagV, BanderaElimina = flagD, MensajeError = messageError });
        }

        [HttpPost]
        public JsonResult SelectSettlementPaid(int keySettlement)
        {
            Boolean flag = false;
            String messageError = "none";
            BajasEmpleadosBean selectSettlementPaidBean = new BajasEmpleadosBean();
            BajasEmpleadosDaoD selectSettlementPaidDaoD = new BajasEmpleadosDaoD();
            try
            {
                selectSettlementPaidBean = selectSettlementPaidDaoD.sp_Selecciona_Finiquito_Pago(keySettlement);
                if (selectSettlementPaidBean.sMensaje == "UPDATE")
                {
                    flag = true;
                }
                else
                {
                    messageError = "ERRUPD";
                }
            }
            catch (Exception exc)
            {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

        [HttpPost]
        public JsonResult CancelSettlement(int keySettlement, int typeCancel, int keyEmployee)
        {
            Boolean flag = false;
            String messageError = "none";
            String typeAction = (typeCancel == 1) ? "Cancelado" : "Reactivado";
            BajasEmpleadosBean downEmployeeBean = new BajasEmpleadosBean();
            BajasEmpleadosDaoD downEmployeeDaoD = new BajasEmpleadosDaoD();
            try
            {
                int keyBusiness = Convert.ToInt32(Session["IdEmpresa"]);
                downEmployeeBean = downEmployeeDaoD.sp_Cancela_Finiquito(keySettlement, typeCancel, keyEmployee, keyBusiness);
                if (downEmployeeBean.sMensaje == "success")
                {
                    flag = true;
                }
                else
                {
                    messageError = "ERRORCANCEL";
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            return Json(new { Bandera = flag, MensajeError = messageError, TipoAccion = typeAction });
        }

        [HttpPost]
        public JsonResult ConfirmPaidSuccess(int keySettlement)
        {
            Boolean flag         = false;
            String  messageError = "none";
            BajasEmpleadosBean downEmployeeBean = new BajasEmpleadosBean();
            BajasEmpleadosDaoD downEmployeeDaoD = new BajasEmpleadosDaoD();
            try {
                downEmployeeBean = downEmployeeDaoD.sp_Finiquito_UpdateEstatus_Pagado(keySettlement);
                if (downEmployeeBean.sMensaje != "SUCCESS") {
                    messageError = downEmployeeBean.sMensaje;
                }
                if (downEmployeeBean.sMensaje == "SUCCESS") {
                    flag = true;
                }
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }


        /// genera pdf fmar&&Ab
        [HttpPost]
        public JsonResult GeneraPdf(int keySettlement, int keyEmployee, int KeyEmpresa,int Option, string url)
        {
            Boolean flag = false;
            String messageError = "none";
            string nameFilePdf = "";
            string nameFolder = "";
            Utilerias utilerias = new Utilerias();
            List<BajasEmpleadosBean> dataDownEmployee = new List<BajasEmpleadosBean>();
            List<DatosFiniquito> listDataDownBean = new List<DatosFiniquito>();
            BajasEmpleadosDaoD dataDownEmplDaoD = new BajasEmpleadosDaoD();
            string nameEmployee = "";
            try
            {
                int keyBusiness = KeyEmpresa;
                dataDownEmployee = dataDownEmplDaoD.sp_Finiquitos_Empleado(keyEmployee, keyBusiness, keySettlement);
                if (dataDownEmployee.Count > 0)
                {
                    listDataDownBean = dataDownEmplDaoD.sp_Info_Finiquito_Empleado(keySettlement);
                    if (listDataDownBean.Count > 0)
                    {
                        Boolean flagTypePDF = false;
                        foreach (BajasEmpleadosBean data in dataDownEmployee)
                        {
                            if (data.sCancelado == "True")
                            {
                                flagTypePDF = true;
                            }
                        }
                        // Comprobamos que el finiquito este cancelado
                        // Si lo esta genera el pdf dentro del if
                        // Si no lo esta genera el pdf dentro del else
                        if (flagTypePDF == true)
                        {
                            flag = true;
                            DatosPDFCancelado data = new DatosPDFCancelado();
                            data = GenerateReceiptDownCancel(dataDownEmployee, listDataDownBean, keyEmployee, keyBusiness);
                            nameFilePdf = data.sNombrePDF;
                            nameFolder = data.sNombreFolder;
                        }
                        else
                        {
                            flag = true;
                            string pathSaveDocs = " ";
                            if (Option == 0) {
                                pathSaveDocs= Server.MapPath("~/Content/");
                            }

                            if (Option == 1) {

                                pathSaveDocs = url;
                            }
                            nameFolder = "DOCSFINIQUITOS";
                            string nameFileTest = "test.txt";
                            if (!Directory.Exists(pathSaveDocs + nameFolder))
                            {
                                Directory.CreateDirectory(pathSaveDocs + nameFolder);
                            }
                            if (!System.IO.File.Exists(pathSaveDocs + nameFolder + @"\\" + nameFileTest))
                            {
                                using (StreamWriter fileTest = new StreamWriter(pathSaveDocs + nameFolder + @"\\" + nameFileTest, false, Encoding.UTF8))
                                {
                                    fileTest.Write("NO REMOVER");
                                    fileTest.Close();
                                }
                            }
                            // ** Definimos las variables a utilizar para mostrar la informacion del finiquito seleccionado en el pdf ** \\
                            string dateReceipt = "";
                            string dateDown = "";
                            string dateEntry = "";
                            string dateAntiquity = "";
                            string rfcEmployee = "";
                            string sAniosAntiquity = "";
                            string sDaysAntiquity = "";
                            string typeSettlement = "";
                            string nameBusiness = "";
                            string totalBalance = "";
                            string centrCost = "";
                            string salaryMonth = "";
                            string salaryDay = "";
                            string jobEmployee = "";
                            string centroCostName = "";
                            string departamentName = "";
                            string departamentCode = "";
                            // Foreach para obtener datos de la empresa
                            foreach (BajasEmpleadosBean data in dataDownEmployee)
                            {
                                dateReceipt = data.sFecha_recibo;
                                typeSettlement = data.sFiniquito_valor;
                                nameBusiness = data.sEmpresa;
                                dateDown = data.sFecha_baja;
                                dateEntry = data.sFecha_ingreso;
                                dateAntiquity = data.sFecha_antiguedad;
                                rfcEmployee = data.sRFC;
                                sAniosAntiquity = data.iAnios.ToString();
                                sDaysAntiquity = data.sDias;
                                centrCost = data.iCentro_costo_id.ToString();
                                salaryDay = data.sSalario_diario;
                                salaryMonth = data.sSalario_mensual;
                                jobEmployee = data.sPuesto;
                                centroCostName = data.sCentro_costo;
                                departamentName = data.sDepartamento;
                                departamentCode = data.sDepto_codigo;
                            }
                            //Foreach para sacar el saldo total
                            foreach (DatosFiniquito data in listDataDownBean)
                            {
                                nameEmployee = data.sNombre;
                                if (data.iIdValor == 5 && data.iRenglon_id == 9999)
                                {
                                    totalBalance = data.sSaldo;
                                    break;
                                }
                            }
                            ConversorMoneda convertMon = new ConversorMoneda();
                            // Definimos el nombre del archivo pdf
                            nameFilePdf = nameEmployee.Replace(" ", "").ToUpper() + "_" + keyEmployee.ToString() + "_E" + keyBusiness + ".pdf";
                            //nameFilePdf = "E" + keyBusiness.ToString() + "_" + typeSettlement.Replace(" ", "_").ToUpper() + "_" + keyEmployee.ToString() + ".pdf";
                            if (System.IO.File.Exists(pathSaveDocs + nameFolder + @"\\" + nameFilePdf))
                            {
                                System.IO.File.Delete(pathSaveDocs + nameFolder + @"\\" + nameFilePdf);
                            }
                            FileStream fs = new FileStream(pathSaveDocs + nameFolder + @"\\" + nameFilePdf, FileMode.Create);
                            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 20, 40, 20, 40);
                            PdfWriter pw = PdfWriter.GetInstance(doc, fs);
                            doc.Open();
                            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                            Font fontBold = new Font(FontFamily.HELVETICA, 10, Font.BOLD);
                            Font fontDefault = new Font(FontFamily.HELVETICA, 10);
                            Font fontTitle = new Font(FontFamily.HELVETICA, 14);
                            Font fontParagraph = new Font(FontFamily.HELVETICA, 8, Font.ITALIC);
                            Paragraph pr = new Paragraph();
                            DateTime datePdf = DateTime.Now;
                            string convertDateDown  = utilerias.ConvertDateFormat_year_month_day(dateDown);
                            string datePrintReceipt = utilerias.ConvertDateText(convertDateDown);
                            pr.Font = fontBold;
                            pr.Add(nameBusiness.ToUpper() + ".");
                            doc.Add(pr);
                            pr.Clear();
                            //doc.Add(new Chunk("\n"));
                            pr.Font = fontBold;
                            pr.Add("Fecha: " + datePrintReceipt + ".");
                            pr.Alignment = Element.ALIGN_LEFT;
                            doc.Add(pr);
                            pr.Clear();
                            //doc.Add(new Chunk("\n"));
                            pr.Font = fontTitle;
                            pr.Add(typeSettlement);
                            pr.Alignment = Element.ALIGN_CENTER;
                            doc.Add(pr);
                            pr.Clear();
                            doc.Add(new Chunk("\n"));
                            pr.Font = fontParagraph;
                            string balanceConvertText = convertMon.Convertir(Convert.ToDouble(totalBalance).ToString(), true, "PESOS");
                            string dateDownConvert = utilerias.ConvertDateText(convertDateDown);
                            string paragraphDescription = "Recibí de " + nameBusiness.ToUpper() + " la cantidad neta de $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(totalBalance)) + " (" + balanceConvertText + "). Por los conceptos detallados en la hoja de desglose de finiquito que me corresponde, con motivo de mi renuncia a esta institución, por lo que dejo de prestar mis servicios de manera voluntaria a partir del día " + dateDownConvert + ".";
                            pr.Add(paragraphDescription);
                            pr.Alignment = Element.ALIGN_JUSTIFIED;
                            doc.Add(pr);
                            pr.Clear();
                            doc.Add(new Chunk("\n"));
                            string paragrahpDescription2 = "En esa virtud manifiesto que estoy conforme con el finiquito mencionado, en cuanto a su importe y conceptos que se especifican, el cual recibo a mi entera satisfación y por lo mismo, les otorgo mi más amplio finiquito, sin reserva de ninguna acción, ni derecho que ejercitar posteriormente en contra de " + nameBusiness + ", " + "ya que durante el tiempo que presté mis servicios, siempre me fueron cubiertas íntegra y puntualmente todas las prestaciones a que tuve derecho, tales como: aguinaldo, vacaciones, prima vacacional, salarios, tiempo extra, descansos obligatorios y en si cualquier otra prestación derivada de mi contrato individual de trabajo, o de la normatividad de la materia, por consiguiente me doy por pagado de todas y cada una de ellas.";
                            pr.Font = fontParagraph;
                            pr.Add(paragrahpDescription2);
                            pr.Alignment = Element.ALIGN_JUSTIFIED;
                            doc.Add(pr);
                            pr.Clear();
                            doc.Add(new Chunk("\n"));
                            // Creamos la tabla que contendra la informacion del finiquito
                            PdfPTable tableInfo = new PdfPTable(6);
                            tableInfo.WidthPercentage = 100;
                            // Creamos la celda de percepciones
                            pr.Font = new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("PERCEPCIONES");
                            PdfPCell clPercepciones = new PdfPCell();
                            clPercepciones.BorderWidth = 0.75f;
                            clPercepciones.PaddingTop = -7;
                            clPercepciones.Colspan = 3;
                            clPercepciones.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de deducciones
                            pr.Font = new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("DEDUCCIONES");
                            PdfPCell clDeducciones = new PdfPCell();
                            clDeducciones.BorderWidth = 0.75f;
                            clDeducciones.PaddingTop = -7;
                            clDeducciones.Colspan = 3;
                            clDeducciones.AddElement(pr);
                            pr.Clear();
                            tableInfo.AddCell(clPercepciones);
                            tableInfo.AddCell(clDeducciones);
                            // Foreach para llenar los datos de las celdas
                            foreach (DatosFiniquito data in listDataDownBean)
                            {
                                int lengthPer = 0;
                                int lengthDed = 0;
                                if (data.iIdValor == 1 && data.iRenglon_id != 990)
                                {
                                    lengthPer += 1;
                                }
                                if (data.iIdValor == 2 && data.iRenglon_id != 1990)
                                {
                                    lengthDed += 1;
                                }
                                string descPerc = "";
                                string totaPerc = "";
                                string descDedu = "";
                                string totaDedu = "";
                                if (lengthPer > 0)
                                {
                                    if (data.iIdValor == 1 && data.iRenglon_id != 990)
                                    {
                                        descPerc = data.sNombre_Renglon + " " + data.sLeyenda;
                                        totaPerc = "$" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo));
                                    }
                                }
                                // Descripcion de la percepcion
                                clPercepciones = new PdfPCell(new Phrase("     " + descPerc, _standardFont));
                                clPercepciones.Colspan = 2;
                                tableInfo.AddCell(clPercepciones);
                                // Total de la percepcion
                                clPercepciones = new PdfPCell(new Phrase("     " + totaPerc, _standardFont));
                                clPercepciones.Colspan = 1;
                                tableInfo.AddCell(clPercepciones);
                                if (lengthDed > 0)
                                {
                                    if (data.iIdValor == 2 && data.iRenglon_id != 1990)
                                    {
                                        descDedu = data.sNombre_Renglon + " " + data.sLeyenda;
                                        totaDedu = "$ " + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo));
                                    }
                                }
                                // Descripcion de la deduccion
                                clDeducciones = new PdfPCell(new Phrase("     " + descDedu, _standardFont));
                                clDeducciones.Colspan = 2;
                                tableInfo.AddCell(clDeducciones);
                                // Total de la deduccion
                                clDeducciones = new PdfPCell(new Phrase("     " + totaDedu, _standardFont));
                                clDeducciones.Colspan = 1;
                                tableInfo.AddCell(clDeducciones);

                            }
                            // Foreach para agregar los totales de deduccion y percepcion
                            foreach (DatosFiniquito data in listDataDownBean)
                            {
                                if (data.iIdValor == 1 && data.iRenglon_id == 990)
                                {
                                    // Descripcion de la percepcion
                                    clPercepciones = new PdfPCell(new Phrase("     " + data.sNombre_Renglon, _standardFont));
                                    clPercepciones.Colspan = 2;
                                    tableInfo.AddCell(clPercepciones);
                                    // Total de la percepcion
                                    clPercepciones = new PdfPCell(new Phrase("     $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo)), _standardFont));
                                    clPercepciones.Colspan = 1;
                                    tableInfo.AddCell(clPercepciones);
                                }
                                if (data.iIdValor == 2 && data.iRenglon_id == 1990)
                                {
                                    // Descripcion de la deduccion
                                    clDeducciones = new PdfPCell(new Phrase("     " + data.sNombre_Renglon, _standardFont));
                                    clDeducciones.Colspan = 2;
                                    tableInfo.AddCell(clDeducciones);
                                    // Total de la deduccion
                                    clDeducciones = new PdfPCell(new Phrase("     $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.sSaldo)), _standardFont));
                                    clDeducciones.Colspan = 1;
                                    tableInfo.AddCell(clDeducciones);
                                }
                            }
                            Font fontCells = new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                            tableInfo.AddCell(clDeducciones);
                            doc.Add(tableInfo);
                            doc.Add(new Chunk("\n"));
                            pr.Clear();
                            pr.Font = new Font(FontFactory.GetFont("ARIAL", 10, Font.BOLD));
                            pr.Add("Finiquito a favor: $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(totalBalance)));
                            pr.Alignment = Element.ALIGN_RIGHT;
                            doc.Add(pr);
                            pr.Clear();
                            doc.Add(new Chunk("\n"));
                            // Creamos una nueva tabla para mostrar los ultimos detalles del empleado
                            PdfPTable detailsJobTable2 = new PdfPTable(6);
                            detailsJobTable2.WidthPercentage = 100;
                            // Creamos la celda de salario mensual
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Salario Mensual");
                            PdfPCell clSalarioMensual = new PdfPCell();
                            clSalarioMensual.BorderWidth = 0.75f;
                            clSalarioMensual.PaddingTop = -7;
                            clSalarioMensual.Colspan = 2;
                            clSalarioMensual.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de salario diario
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Salario Diario");
                            PdfPCell clSalarioDiario = new PdfPCell();
                            clSalarioDiario.BorderWidth = 0.75f;
                            clSalarioDiario.PaddingTop = -7;
                            clSalarioDiario.Colspan = 2;
                            clSalarioDiario.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de nomina
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Nomina");
                            PdfPCell clNomina = new PdfPCell();
                            clNomina.BorderWidth = 0.75f;
                            clNomina.PaddingTop = -7;
                            clNomina.Colspan = 2;
                            clNomina.AddElement(pr);
                            pr.Clear();
                            // Agregamos las celdas a la tabla
                            detailsJobTable2.AddCell(clSalarioMensual);
                            detailsJobTable2.AddCell(clSalarioDiario);
                            detailsJobTable2.AddCell(clNomina);
                            // Dato del salario mensual
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("$" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(salaryMonth)));
                            clSalarioMensual = new PdfPCell();
                            clSalarioMensual.BorderWidth = 0.75f;
                            clSalarioMensual.PaddingTop = -7;
                            clSalarioMensual.Colspan = 2;
                            clSalarioMensual.AddElement(pr);
                            pr.Clear();
                            detailsJobTable2.AddCell(clSalarioMensual);
                            // Dato del salario diario
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("$" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(salaryDay)));
                            clSalarioDiario = new PdfPCell();
                            clSalarioDiario.BorderWidth = 0.75f;
                            clSalarioDiario.PaddingTop = -7;
                            clSalarioDiario.Colspan = 2;
                            clSalarioDiario.AddElement(pr);
                            pr.Clear();
                            detailsJobTable2.AddCell(clSalarioDiario);
                            // Dato de la nomina
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(keyEmployee.ToString());
                            clNomina = new PdfPCell();
                            clNomina.BorderWidth = 0.75f;
                            clNomina.PaddingTop = -7;
                            clNomina.Colspan = 2;
                            clNomina.AddElement(pr);
                            pr.Clear();
                            detailsJobTable2.AddCell(clNomina);
                            // Agregamos la tabla al pdf
                            doc.Add(detailsJobTable2);
                            //doc.Add(new Chunk("\n"));
                            PdfPTable detailsTable = new PdfPTable(10);
                            detailsTable.WidthPercentage = 100;
                            // Creamos la celda de fecha antiguedad
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Fecha de Antiguedad");
                            PdfPCell clFechaAntiguedad = new PdfPCell();
                            clFechaAntiguedad.BorderWidth = 0.75f;
                            clFechaAntiguedad.PaddingTop = -7;
                            clFechaAntiguedad.Colspan = 2;
                            clFechaAntiguedad.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de fecha ingreso
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Fecha de Ingreso");
                            PdfPCell clFechaIngreso = new PdfPCell();
                            clFechaIngreso.BorderWidth = 0.75f;
                            clFechaIngreso.PaddingTop = -7;
                            clFechaIngreso.Colspan = 2;
                            clFechaIngreso.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de fecha de baja
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Fecha de Baja");
                            PdfPCell clFechaBaja = new PdfPCell();
                            clFechaBaja.BorderWidth = 0.75f;
                            clFechaBaja.PaddingTop = -7;
                            clFechaBaja.Colspan = 2;
                            clFechaBaja.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de rfc
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("RFC");
                            PdfPCell clRFC = new PdfPCell();
                            clRFC.BorderWidth = 0.75f;
                            clRFC.PaddingTop = -7;
                            clRFC.Colspan = 2;
                            clRFC.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de antiguedad
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Antiguedad");
                            PdfPCell clAntiguedad = new PdfPCell();
                            clAntiguedad.BorderWidth = 0.75f;
                            clAntiguedad.PaddingTop = -7;
                            clAntiguedad.Colspan = 2;
                            clAntiguedad.AddElement(pr);
                            pr.Clear();
                            // Añadimos las celdas a la tabla
                            detailsTable.AddCell(clFechaAntiguedad);
                            detailsTable.AddCell(clFechaIngreso);
                            detailsTable.AddCell(clFechaBaja);
                            detailsTable.AddCell(clRFC);
                            detailsTable.AddCell(clAntiguedad);
                            // LLenamos con informacion las celda fecha de antiguedad
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(dateAntiquity);
                            clFechaAntiguedad = new PdfPCell();
                            clFechaAntiguedad.BorderWidth = 0.75f;
                            clFechaAntiguedad.PaddingTop = -7;
                            clFechaAntiguedad.Colspan = 2;
                            clFechaAntiguedad.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clFechaAntiguedad);
                            // LLenamos con informacion la celda de fecha de ingreso
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(dateEntry);
                            clFechaIngreso = new PdfPCell();
                            clFechaIngreso.BorderWidth = 0.75f;
                            clFechaIngreso.PaddingTop = -7;
                            clFechaIngreso.Colspan = 2;
                            clFechaIngreso.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clFechaIngreso);
                            // LLenamos con informacion la celda de fecha de baja
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(dateDown);
                            clFechaBaja = new PdfPCell();
                            clFechaBaja.BorderWidth = 0.75f;
                            clFechaBaja.PaddingTop = -7;
                            clFechaBaja.Colspan = 2;
                            clFechaBaja.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clFechaBaja);
                            // LLenamos con informacion la celda de rfc
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(rfcEmployee);
                            clRFC = new PdfPCell();
                            clRFC.BorderWidth = 0.75f;
                            clRFC.PaddingTop = -7;
                            clRFC.Colspan = 2;
                            clRFC.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clRFC);
                            // LLenamos con informaicon la celda de antiguedad
                            // Agregamos los días
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(sDaysAntiquity + " dias");
                            clAntiguedad = new PdfPCell();
                            clAntiguedad.BorderWidth = 0.75f;
                            clAntiguedad.PaddingTop = -7;
                            clAntiguedad.Colspan = 1;
                            clAntiguedad.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clAntiguedad);
                            // Agregamos los años
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(sAniosAntiquity + " años");
                            clAntiguedad = new PdfPCell();
                            clAntiguedad.BorderWidth = 0.75f;
                            clAntiguedad.PaddingTop = -7;
                            clAntiguedad.Colspan = 1;
                            clAntiguedad.AddElement(pr);
                            pr.Clear();
                            detailsTable.AddCell(clAntiguedad);
                            doc.Add(detailsTable);
                            // Creamos la tabla de los datos del puesto del empleado
                            PdfPTable detailsJobTable = new PdfPTable(8);
                            detailsJobTable.WidthPercentage = 100;
                            // Creamos la celda del centro de costo
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Centro de costo");
                            PdfPCell clCentroCosto = new PdfPCell();
                            clCentroCosto.BorderWidth = 0.75f;
                            clCentroCosto.PaddingTop = -7;
                            clCentroCosto.Colspan = 4;
                            clCentroCosto.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de puesto
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Puesto");
                            PdfPCell clPuesto = new PdfPCell();
                            clPuesto.BorderWidth = 0.75f;
                            clPuesto.PaddingTop = -7;
                            clPuesto.Colspan = 4;
                            clPuesto.AddElement(pr);
                            pr.Clear();
                            // Añadimos las celdas a la tabla
                            detailsJobTable.AddCell(clCentroCosto);
                            //detailsJobTable.AddCell(clDepartamento);
                            detailsJobTable.AddCell(clPuesto);
                            // Añadimos datos a las celdas de la tabla detailsJobTable
                            // Dato del centro de costo
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(centrCost + " - " + centroCostName);
                            clCentroCosto = new PdfPCell();
                            clCentroCosto.BorderWidth = 0.75f;
                            clCentroCosto.PaddingTop = -7;
                            clCentroCosto.Colspan = 4;
                            clCentroCosto.AddElement(pr);
                            pr.Clear();
                            detailsJobTable.AddCell(clCentroCosto);
                            // Dato del puesto
                            pr.Font = fontCells;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(jobEmployee);
                            clPuesto = new PdfPCell();
                            clPuesto.BorderWidth = 0.75f;
                            clPuesto.PaddingTop = -7;
                            clPuesto.Colspan = 4;
                            clPuesto.AddElement(pr);
                            pr.Clear();
                            detailsJobTable.AddCell(clPuesto);
                            // Agregamos la tabla al pdf
                            doc.Add(detailsJobTable);
                            doc.Add(new Chunk("\n"));
                            //doc.Add(new Chunk("\n"));
                            // Creamos la ultima tabla para el nombre y firma
                            PdfPTable tableFirm = new PdfPTable(9);
                            tableFirm.WidthPercentage = 100;
                            // Creamos la celda de departamento
                            Font fontFirm = new Font(FontFactory.GetFont("ARIAL", 10, Font.BOLD));
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Departamento");
                            PdfPCell clDepartamento = new PdfPCell();
                            clDepartamento.Border = 0;
                            clDepartamento.PaddingTop = -7;
                            clDepartamento.Colspan = 3;
                            clDepartamento.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de fecha de baja
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Fecha Baja");
                            PdfPCell clFechaBajaTexto = new PdfPCell();
                            clFechaBajaTexto.Border = 0;
                            clFechaBajaTexto.PaddingTop = -7;
                            clFechaBajaTexto.Colspan = 3;
                            clFechaBajaTexto.AddElement(pr);
                            pr.Clear();
                            // Creamos la celda de nombre
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(nameEmployee);
                            PdfPCell clNombreEmpleado = new PdfPCell();
                            clNombreEmpleado.Border = 0;
                            clNombreEmpleado.PaddingTop = -7;
                            clNombreEmpleado.Colspan = 3;
                            clNombreEmpleado.AddElement(pr);
                            pr.Clear();
                            //Agregamos la celda a la tabla
                            tableFirm.AddCell(clDepartamento);
                            tableFirm.AddCell(clFechaBajaTexto);
                            tableFirm.AddCell(clNombreEmpleado);
                            //Agregamos el valor de nombre y firma abajo de la celda de nombre
                            //Dato del departamento
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(departamentName);
                            clDepartamento = new PdfPCell();
                            clDepartamento.BorderWidth = 0;
                            clDepartamento.PaddingTop = 7;
                            clDepartamento.Colspan = 3;
                            clDepartamento.AddElement(pr);
                            pr.Clear();
                            tableFirm.AddCell(clDepartamento);
                            //Dato de la fecha de baja texto
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add(dateDownConvert);
                            clDepartamento = new PdfPCell();
                            clDepartamento.BorderWidth = 0;
                            clDepartamento.PaddingTop = 7;
                            clDepartamento.Colspan = 3;
                            clDepartamento.AddElement(pr);
                            pr.Clear();
                            tableFirm.AddCell(clDepartamento);
                            // Dato de la nomina
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("Nombre y Firma");
                            clNombreEmpleado = new PdfPCell();
                            clNombreEmpleado.BorderWidth = 0;
                            clNombreEmpleado.PaddingTop = 7;
                            clNombreEmpleado.Colspan = 3;
                            clNombreEmpleado.AddElement(pr);
                            tableFirm.AddCell(clNombreEmpleado);
                            pr.Clear();
                            // Agegamos la tabla al pdf
                            doc.Add(tableFirm);
                            // Agregamos el numero de nomina del empleado
                            doc.Add(new Chunk("\n"));
                            PdfPTable tablePayroll = new PdfPTable(1);
                            tablePayroll.WidthPercentage = 100;
                            // Creamos la celda de Nomina
                            Font fontPayroll = new Font(FontFactory.GetFont("ARIAL", 10, Font.BOLD));
                            pr.Font = fontFirm;
                            pr.Alignment = Element.ALIGN_CENTER;
                            pr.Add("NOMINA: " + keyEmployee.ToString());
                            PdfPCell clNominaE = new PdfPCell();
                            clNominaE.Border = 0;
                            clNominaE.PaddingTop = -7;
                            clNominaE.Colspan = 3;
                            clNominaE.AddElement(pr);
                            pr.Clear();
                            tablePayroll.AddCell(clNominaE);
                            doc.Add(tablePayroll);
                            doc.Close();
                            pw.Close();
                        }
                    }
                    else
                    {
                        messageError = "ERRNOTDATA";
                    }
                }
                else
                {
                    messageError = "ERRLOADINFFINIQUITO";
                }
            }
            catch (Exception exc)
            {
                messageError = exc.Message.ToString();
            }
            string nameFileSession = Path.GetFileNameWithoutExtension(nameFilePdf);
           // Session[nameFilePdf] = nameFilePdf;
            return Json(new { Bandera = flag, MensajeError = messageError, NombrePDF = nameFilePdf, InfoFiniquito = dataDownEmployee, NombreFolder = nameFolder });
        }

        [HttpPost]
        public JsonResult AddComplementSettlement(int keySettlement, int type, int keyEmploye)
        {
            Boolean flag         = false;
            String  messageError = "none";
            DatosFiniquito datosFiniquito         = new DatosFiniquito();
            BajasEmpleadosBean bajasEmpleados     = new BajasEmpleadosBean();
            BajasEmpleadosDaoD empleadosDaoD      = new BajasEmpleadosDaoD();
            List<DatosFiniquito> datosFiniquitos  = new List<DatosFiniquito>();
            try {
                int keyBusiness    = Convert.ToInt32(Session["IdEmpresa"].ToString());
                int keyUser = Convert.ToInt32(Session["iIdUsuario"].ToString());
                datosFiniquito = empleadosDaoD.sp_Consulta_Info_Finiquito(keySettlement, keyBusiness, keyEmploye);
                if (datosFiniquito.sMensaje == "SUCCESS") {
                    bajasEmpleados = empleadosDaoD.sp_CNomina_Finiquito(keyBusiness, keyEmploye, datosFiniquito.sFechaAntiguedad, datosFiniquito.iTipoFiniquitoId, datosFiniquito.iMotivoBajaId, datosFiniquito.sFechaBaja, datosFiniquito.sFechaRecibo, datosFiniquito.iBanFechaIngreso, datosFiniquito.iBanCompEspecial, datosFiniquito.iDiasPendientes, datosFiniquito.iAnio, datosFiniquito.iPeriodo, datosFiniquito.sFechaPagoInicio, datosFiniquito.sFechaPFin, 1, 0, 0, keyUser);
                    if (bajasEmpleados.sMensaje == "SUCCESS") {
                        flag = true;
                    }
                }   
            } catch (Exception exc) {
                flag         = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, DatosFiniquito = datosFiniquito });
        } 

        [HttpPost]
        public JsonResult Test(List<ListConcepts> items, int keySettlement)
        {
            Boolean flag = false;
            String messageError = "none"; 
            BajasEmpleadosBean bajasEmpleadosBean  = new BajasEmpleadosBean();
            BajasEmpleadosBean bajasEmpleadosBean1 = new BajasEmpleadosBean();
            BajasEmpleadosDaoD bajasEmpleadosDaoD  = new BajasEmpleadosDaoD();
            PeriodoActualBean periodActBean        = new PeriodoActualBean();
            try {
                int keyBusiness     = Convert.ToInt32(Session["IdEmpresa"].ToString());
                periodActBean       = bajasEmpleadosDaoD.sp_Load_Info_Periodo_Empr(keyBusiness, DateTime.Now.Year);
                bajasEmpleadosBean  = bajasEmpleadosDaoD.sp_Max_Sequence_Number_Complement_Settlement(keySettlement, keyBusiness);
                bajasEmpleadosBean1 = bajasEmpleadosDaoD.sp_Add_Complement_Settlement(items, keySettlement, keyBusiness, bajasEmpleadosBean.iEstatus, periodActBean.iAnio, periodActBean.iPeriodo);
                if (bajasEmpleadosBean1.sMensaje == "success") {
                    flag = true;
                } 
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            }

            return Json(new { Bandera = flag, Datos = items, Finiquito = keySettlement });
        }

        [HttpPost]
        public JsonResult ShowComplementsSettlement(int keySettlement, int keyEmployee)
        {
            Boolean flag         = false;
            String  messageError = "none";
            List<ComplementosFiniquitos> complementos = new List<ComplementosFiniquitos>();
            BajasEmpleadosDaoD bajasEmpleadosDao      = new BajasEmpleadosDaoD();
            int keyBusiness = Convert.ToInt32(Session["IdEmpresa"]);
            try {
                complementos = bajasEmpleadosDao.sp_View_Complement_Settlement(keyBusiness, keySettlement);
                if (complementos.Count > 0) {
                    flag = true;
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = complementos, Empresa = keyBusiness });
        }

        [HttpPost]
        public JsonResult ViewDetailsComplement(int keySettlement, int keyBusiness, int keySeq)
        {
            Boolean flag = false;
            String  messageError = "none";
            List<ComplementosFiniquitos> complementos = new List<ComplementosFiniquitos>();
            BajasEmpleadosDaoD bajasEmpleadosDaoD     = new BajasEmpleadosDaoD();
            decimal totalAmount = 0;
            try {
                complementos = bajasEmpleadosDaoD.sp_View_Details_Complement(keySettlement, keyBusiness, keySeq);
                if (complementos.Count > 0) {
                    flag = true;
                    foreach (ComplementosFiniquitos item in complementos) {
                        if (item.iTipoRenglonId == 1) {
                            totalAmount += item.dImporte;
                        } else if (item.iTipoRenglonId == 2) {
                            totalAmount -= item.dImporte;
                        }
                    }
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = complementos, Total = string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(totalAmount)) });
        }

        [HttpPost]
        public JsonResult CancelComplementSettlement(int keyBusiness, int keySettlement, int keySeq, int keyEmployee)
        {
            Boolean flag = false;
            String  messageError = "none";
            List<ComplementosFiniquitos> complementosDet = new List<ComplementosFiniquitos>();
            ComplementosFiniquitos complementos   = new ComplementosFiniquitos();
            BajasEmpleadosDaoD bajasEmpleadosDaoD = new BajasEmpleadosDaoD();
            PeriodoActualBean periodoActualBean   = new PeriodoActualBean();
            try {
                int periodComplement   = 0;
                int yearComplement     = 0;
                int yearPeriodActually = 0;
                int periodActually     = 0;
                periodoActualBean = bajasEmpleadosDaoD.sp_Load_Info_Periodo_Empr(keyBusiness, DateTime.Now.Year);
                if (periodoActualBean.sMensaje == "success") {
                    periodActually     = periodoActualBean.iPeriodo;
                    yearPeriodActually = periodoActualBean.iAnio;
                }
                complementosDet = bajasEmpleadosDaoD.sp_View_Details_Complement(keySettlement, keyBusiness, keySeq);
                foreach (ComplementosFiniquitos data in complementosDet) {
                    periodComplement = data.iPeriodo;
                    yearComplement   = data.iAnio;
                }
                if (periodComplement != periodActually && yearComplement != yearPeriodActually) {
                    return Json(new { Bandera = false, MensajeError = "NOTCANCEL", Alerta = true });
                }
                complementos    = bajasEmpleadosDaoD.sp_Cancel_Complement_Settlement(keyBusiness, keySettlement, keySeq);
                if (complementos.sMensaje == "SUCCESS") {
                    flag = true;
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }

            return Json(new { Bandera = flag, MensajeError = messageError, Alerta = false });
        }

        [HttpPost]
        public JsonResult SelectRenglonesComplementSettlement()
        {
            Boolean flag = false;
            String messageError = "none";
            BajasEmpleadosDaoD bajasEmpleadosDaoD = new BajasEmpleadosDaoD();
            List<CRenglonesBean> cRenglonesBean   = new List<CRenglonesBean>();
            try {
                int keyBusiness = Convert.ToInt32(Session["IdEmpresa"].ToString());
                cRenglonesBean = bajasEmpleadosDaoD.sp_Select_Renglones_Complement_Settlement(keyBusiness);
                if (cRenglonesBean.Count > 0) {
                    flag = true;
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            } 
            return Json(new { Bandera = flag, MensajeError = messageError, Datos = cRenglonesBean });
        }

        [HttpPost]
        public JsonResult ValidExistsComplementSettlementPeriod(int keySettlement)
        {
            Boolean flag = false;
            String  messageError = "none";
            BajasEmpleadosDaoD bajasEmpleadosDaoD = new BajasEmpleadosDaoD();
            ComplementosFiniquitos complementosFiniquitos = new ComplementosFiniquitos();
            PeriodoActualBean periodoActual = new PeriodoActualBean();
            try {
                int keyBusiness = Convert.ToInt32(Session["IdEmpresa"]);
                periodoActual = bajasEmpleadosDaoD.sp_Load_Info_Periodo_Empr(keyBusiness, DateTime.Now.Year);
                complementosFiniquitos = bajasEmpleadosDaoD.sp_Valid_Exists_Complement_Settlement_Period(keyBusiness, keySettlement, periodoActual.iAnio, periodoActual.iPeriodo);
                if (complementosFiniquitos.sMensaje == "EXISTS") {
                    flag = true;
                } else if (complementosFiniquitos.sMensaje == "NOTEXISTS") {
                    flag = false;
                } else {
                    messageError = complementosFiniquitos.sMensaje;
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError });
        }

    }
}   