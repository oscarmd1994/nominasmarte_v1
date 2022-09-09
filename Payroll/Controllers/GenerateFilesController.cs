﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Payroll.Models.Beans;
using Payroll.Models.Daos;
using Payroll.Models.Utilerias;
using static iTextSharp.text.Font;

namespace Payroll.Controllers
{
    public class GenerateFilesController : Controller
    {
        // GET: GenerateFiles
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public String ConvertDateText(string dateConvert)
        {
            String convertDate = "";
            try
            {
                string year = dateConvert.Substring(0, 4);
                string month = dateConvert.Substring(5, 2);
                string day = dateConvert.Substring(8, 2);
                string[] days = new string[] { "Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sábado" };
                string[] months = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
                convertDate = day + " de " + months[Convert.ToInt32(month) - 1] + " del " + year;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            return convertDate;
        }

        [HttpPost]
        public JsonResult ComplementSettlement(int keyBusiness, int keySettlement, int keySeq, int keyEmployee)
        {
            Boolean flag = false;
            String  messageError = "none";
            string nameFilePdf   = "";
            string nameFolder    = "";
            List<BajasEmpleadosBean> dataDownEmployee = new List<BajasEmpleadosBean>();
            List<DatosFiniquito> listDataDownBean     = new List<DatosFiniquito>();
            BajasEmpleadosDaoD dataDownEmplDaoD       = new BajasEmpleadosDaoD();
            List<ComplementosFiniquitos> listConcepts = new List<ComplementosFiniquitos>();
            string nameEmployee = "";
            try {
                dataDownEmployee = dataDownEmplDaoD.sp_Finiquitos_Empleado(keyEmployee, keyBusiness, keySettlement);
                if (dataDownEmployee.Count > 0) {
                    listConcepts = dataDownEmplDaoD.sp_Info_Complement_Settlement(keySettlement, keyBusiness, keyEmployee, keySeq);
                    if (listConcepts.Count > 0) {
                        flag = true;
                        string pathSaveDocs = Server.MapPath("~/Content/");
                        nameFolder = "DOCSFINIQUITOSCOMPLEMENTOS";
                        string nameFileTest = "test.txt";
                        if (!Directory.Exists(pathSaveDocs + nameFolder)) {
                            Directory.CreateDirectory(pathSaveDocs + nameFolder);
                        }
                        if (!System.IO.File.Exists(pathSaveDocs + nameFolder + @"\\" + nameFileTest)) {
                            using (StreamWriter fileTest = new StreamWriter(pathSaveDocs + nameFolder + @"\\" + nameFileTest, false, Encoding.UTF8)) {
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
                        decimal totalBalance = 0;
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
                        foreach (BajasEmpleadosBean data in dataDownEmployee) {
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
                        decimal totalPer = 0;
                        decimal totalDed = 0;
                        //Foreach para sacar el saldo total
                        foreach (ComplementosFiniquitos data in listConcepts) {
                            nameEmployee =  data.sNombreEmpleado;
                            if (data.iTipoRenglonId == 1) {
                                totalPer += data.dImporte;
                                totalBalance += data.dImporte;
                            }
                            if (data.iTipoRenglonId == 2) {
                                totalDed += data.dImporte;
                                totalBalance -= data.dImporte;
                            }
                        }
                        if (totalBalance == 0) {
                            totalBalance = 0;
                        }
                        ConversorMoneda convertMon = new ConversorMoneda();
                        // Definimos el nombre del archivo pdf
                        nameFilePdf = nameEmployee.Replace(" ", "").ToUpper() + "_" + keyEmployee.ToString() + "_E" + keyBusiness + "_COMPLEMENTO.pdf";
                        if (System.IO.File.Exists(pathSaveDocs + nameFolder + @"\\" + nameFilePdf)) {
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
                        Font fontCells = new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
                        Paragraph pr = new Paragraph();
                        DateTime datePdf = DateTime.Now;
                        string datePrintReceipt = ConvertDateText(DateTime.Parse(dateDown).ToString("yyyy-MM-dd"));
                        string dateAntiquityConvert = ConvertDateText(DateTime.Parse(dateAntiquity).ToString("yyyy-MM-dd"));
                        pr.Font = fontBold;
                        pr.Add(nameBusiness.ToUpper() + ".");
                        doc.Add(pr);
                        pr.Clear();
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
                        string dateDownConvert = ConvertDateText(DateTime.Parse(dateDown).ToString("yyyy-MM-dd"));
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
                        pr.Add("Fecha de pago del " + ConvertDateText(DateTime.Parse(dateStartPay).ToString("yyyy-MM-dd")) + " al " + ConvertDateText(DateTime.Parse(dateEndPay).ToString("yyyy-MM-dd")) + " Periodo " + periodPay);
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
                        foreach (ComplementosFiniquitos data in listConcepts)
                        {
                            int lengthPer = 0;
                            int lengthDed = 0;

                            if (data.iTipoRenglonId == 1) {
                                lengthPer += 1;
                            }
                            if (data.iTipoRenglonId == 2) {
                                lengthDed += 1;
                            }
                            string descPerc = "";
                            string totaPerc = "";
                            string descDedu = "";
                            string totaDedu = "";
                            if (lengthPer > 0) {
                                if (data.iTipoRenglonId == 1) {
                                    descPerc = "[" + data.iRenglonId.ToString() + "] " + data.sNombreRenglon;
                                    totaPerc = "$" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.dImporte));
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
                            if (lengthDed > 0) {
                                if (data.iTipoRenglonId == 2) {
                                    descDedu = "[" + data.iRenglonId.ToString() + "] " + data.sNombreRenglon;
                                    totaDedu = "$" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(data.dImporte));
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

                        // Descripcion de la percepcion
                        clPercepciones = new PdfPCell(new Phrase("     " + "Total percepcion", _standardFont));
                        clPercepciones.Colspan = 2;
                        tableInfo.AddCell(clPercepciones);
                        // Total de la percepcion
                        clPercepciones = new PdfPCell(new Phrase("     $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(totalPer)), _standardFont));
                        clPercepciones.Colspan = 1;
                        tableInfo.AddCell(clPercepciones);
                        // Descripcion de la deduccion
                        clDeducciones = new PdfPCell(new Phrase("     " + "Total deduccion", _standardFont));
                        clDeducciones.Colspan = 2;
                        tableInfo.AddCell(clDeducciones);
                        // Total de la deduccion
                        clDeducciones = new PdfPCell(new Phrase("     $" + string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(totalDed)), _standardFont));
                        clDeducciones.Colspan = 1;
                        tableInfo.AddCell(clDeducciones);
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
                    } else {
                        messageError = "ERRNOTDATA";
                    }
                } else {
                    messageError = "ERRLOADINFFINIQUITO";
                }
            } catch (Exception exc) {
                flag = false;
                messageError = exc.Message.ToString();
            }
            string nameFileSession = Path.GetFileNameWithoutExtension(nameFilePdf);
            Session[nameFilePdf] = nameFilePdf;
            return Json(new { Bandera = flag, MensajeError = messageError, NombrePDF = nameFilePdf, InfoFiniquito = dataDownEmployee, NombreFolder = nameFolder });
        }

    }
}