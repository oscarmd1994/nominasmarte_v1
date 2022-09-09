using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
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
    public class DispersionGroupsController : Controller
    {

        [HttpPost]
        public JsonResult LoadBanksAvailable(int year, int period, int typePeriod)
        {
            bool flag = false;
            string messageError = "none";
            DataDispersionGroups dataDispersion = new DataDispersionGroups();
            List<DataDepositsBankingBean> dataDeposits = new List<DataDepositsBankingBean>();
            try {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                dataDeposits = dataDispersion.sp_Obtiene_Depositos_GruposBancarios(keyBusiness, year, period, typePeriod);
                if (dataDeposits.Count > 0) {
                    flag = true;
                }
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, Error = messageError, Data = dataDeposits });
        }

        [HttpPost]
        public JsonResult AddRemoveBankGroup(int keyBankBusiness, int keyBank, int status)
        {
            bool flag = false;
            bool save = false;
            string messageError = "none";
            DataDispersionGroups dataDispersion = new DataDispersionGroups();
            try
            {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                save = dataDispersion.sp_Inserta_Banco_GrupoDispersion(keyBusiness, keyBankBusiness, keyBank, status);
                if (save)
                {
                    flag = true;
                }
            }
            catch (Exception exc)
            {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, Guardado = save, Error = messageError });
        }

        [HttpPost]
        public JsonResult ClearSettings()
        {
            bool flag   = false;
            bool delete = false;
            string messageError = "none";
            DataDispersionGroups dataDispersion = new DataDispersionGroups();
            try {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                delete = dataDispersion.sp_Elimina_Configuraciones_GrupoDispersion(keyBusiness);
                if (delete) {
                    flag = true;
                }
            } catch (Exception exc) {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, Eliminado = delete, Error = messageError });
        }

        [HttpPost]
        public JsonResult ViewDetails(int keyBankBusiness, int year, int period, int typePeriod)
        {
            bool flag = false;
            string messageError = "none";
            DataDispersionGroups dataDispersion = new DataDispersionGroups();
            List<DataDepositsBankingBean> dataBanks = new List<DataDepositsBankingBean>();
            double sumaTotalAmount = 0;
            int sumaTotalDeposits = 0;
            string sumaTotalAmountWord = "";
            try
            {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                dataBanks = dataDispersion.sp_Detalles_Configuracion_DispersionGrupos(keyBusiness, year, period, typePeriod, keyBankBusiness);
                if (dataBanks.Count > 0)
                {
                    flag = true;
                    foreach (DataDepositsBankingBean data in dataBanks)
                    {
                        sumaTotalAmount += data.dImporteSF;
                        sumaTotalDeposits += data.iDepositos;
                    }
                    ConversorMoneda conversorMoneda = new ConversorMoneda();
                    sumaTotalAmountWord = conversorMoneda.Convertir(sumaTotalAmount.ToString(), true, "PESOS");
                }
            }
            catch (Exception exc)
            {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, Error = messageError, Data = dataBanks, Total = string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal(sumaTotalAmount)), Depositos = sumaTotalDeposits, TotalLetra = sumaTotalAmountWord });
        }

        [HttpPost]
        public JsonResult DeployDepositsGroup(int keyBankBusiness, int typeDispersion, int keyType, int period, int typePeriod, int year)
        {
            bool flag = false;
            bool generateSuccess = false;
            string messageError = "none";
            DataDispersionGroups dataDispersion = new DataDispersionGroups();
            DataDispersionBusiness dataDispersionBusiness = new DataDispersionBusiness();
            DatosEmpresaBeanDispersion datosEmpresaBeanDispersion = new DatosEmpresaBeanDispersion();
            List<DataDepositsBankingBean> dataBanks = new List<DataDepositsBankingBean>();
            string nameFolder = "";
            string directoryZip = Server.MapPath("/DispersionZIP").ToString();
            string directoryTxt = Server.MapPath("/DispersionTXT").ToString();
            string nameFolderYear = DateTime.Now.Year.ToString();
            DateTime dateGeneration = DateTime.Now;
            bool flagInterbank = false;
            string nameFileError = "";
            string msgEstatusZip = "";
            string msgEstatus = "";
            string nameFolderSave = (keyType == 1) ? "INTERBANCARIOS" : "NOMINAS";
            List<string> nameFiles = new List<string>();
            try
            {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                dataBanks = dataDispersion.sp_Detalles_Configuracion_DispersionGrupos(keyBusiness, year, period, typePeriod, keyBankBusiness);
                if (dataBanks.Count == 0) {
                    return Json(new { Cantidad = 0 });
                }
                datosEmpresaBeanDispersion = dataDispersionBusiness.sp_Datos_Empresa_Dispersion(keyBusiness, typeDispersion);
                nameFolder = "DEPOSITOS" + "E" + keyBusiness.ToString() + "P" + period.ToString() + "A" + dateGeneration.ToString("yyyy").Substring(2, 2);
                if (System.IO.File.Exists(directoryZip + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\" + nameFolder + ".zip"))
                {
                    System.IO.File.Delete(directoryZip + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\" + nameFolder + ".zip");
                }
                if (Directory.Exists(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\" + nameFolder))
                {
                    Directory.Delete(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\" + nameFolder, recursive: true);
                }
                if (!System.IO.Directory.Exists(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\" + nameFolder))
                {
                    System.IO.Directory.CreateDirectory(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\" + nameFolder);
                }
                if (keyType == 1)
                { // INTERBANCARIO
                    flagInterbank = ProcessDepositsInterbank(year, period, typePeriod, datosEmpresaBeanDispersion.iBanco_id, keyBankBusiness, nameFolder, nameFolderYear, nameFolderSave, directoryTxt, directoryZip, datosEmpresaBeanDispersion);
                    if (flagInterbank)
                    {
                        string pathCompleteZip = directoryZip + @"\\" + nameFolderYear + @"\\" + nameFolderSave;
                        if (!System.IO.Directory.Exists(pathCompleteZip))
                        {
                            System.IO.Directory.CreateDirectory(pathCompleteZip);
                        }
                        FileStream stream = new FileStream(directoryZip + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder + ".zip", FileMode.OpenOrCreate);
                        ZipArchive fileZip = new ZipArchive(stream, ZipArchiveMode.Create);
                        System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder);
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
                            using (ZipArchive zipArchive = ZipFile.OpenRead(directoryZip + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder + ".zip"))
                            {
                                foreach (ZipArchiveEntry archiveEntry in zipArchive.Entries)
                                {
                                    using (ZipArchive zipArchives = ZipFile.Open(directoryZip + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder + ".zip", ZipArchiveMode.Read))
                                    {
                                        zEntrys = zipArchives.GetEntry(archiveEntry.ToString());
                                        nameFileError = zEntrys.Name;
                                        using (StreamReader read = new StreamReader(zEntrys.Open()))
                                        {
                                            if (read.ReadLine().Length > 0)
                                            {
                                                msgEstatusZip = "filesuccess";
                                                nameFiles.Add(nameFileError);
                                            }
                                            else
                                            {
                                                msgEstatusZip = "fileerror";
                                            }
                                        }
                                    }
                                }
                            }
                            flag = true;
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
                    }
                }
            }
            catch (Exception exc)
            {
                messageError = exc.Message.ToString();
            }
            return Json(new { Bandera = flag, MensajeError = messageError, Zip = nameFolder, EstadoZip = msgEstatusZip, Estado = msgEstatus, Anio = nameFolderYear, FolderGuardado = nameFolderSave, Cantidad = dataBanks.Count, Archivos = nameFiles, FolderAnio = nameFolderYear, FolderTxt = nameFolderSave });
        }

        public bool ProcessDepositsInterbank(int year, int period, int typePeriod, int bankDispersion, int keyBankBusiness, string nameFolder, string nameFolderYear, string nameFolderSave, string directoryTxt, string directoryZip, DatosEmpresaBeanDispersion datosEmpresaBeanDispersion)
        {
            bool flag = false;
            DatosCuentaClienteBancoEmpresaBean datosBancoEmpresa = new DatosCuentaClienteBancoEmpresaBean();
            DataDispersionBusiness dataDispersionBusiness = new DataDispersionBusiness();
            DataDispersionGroups dataDispersion = new DataDispersionGroups();
            DatosDepositosBancariosBean datosDepositos = new DatosDepositosBancariosBean();
            //List<DatosProcesaChequesNominaBean> listDatosProcesaChequesNominaBean = new List<DatosProcesaChequesNominaBean>();
            List<DatosProcesaChequesNominaBean> datosProcesamiento = new List<DatosProcesaChequesNominaBean>();
            string fileNamePdf = "";
            string fileNameTxt = "";
            try
            {
                int keyBusiness = int.Parse(Session["IdEmpresa"].ToString());
                datosBancoEmpresa = dataDispersionBusiness.sp_Cuenta_Cliente_Banco_Empresa(keyBusiness, bankDispersion);
                datosDepositos = dataDispersion.sp_Procesa_Cheques_Total_DispersionGrupos(keyBusiness, year, period, typePeriod, keyBankBusiness);
                datosProcesamiento = dataDispersion.sp_Obtiene_Depositos_Bancarios_DispersionGrupos(keyBusiness, year, period, typePeriod, keyBankBusiness);
                if (datosProcesamiento.Count > 0)
                {
                    fileNamePdf = "NOMINAS_E" + keyBusiness.ToString() + "A" + year.ToString() + "P" + period.ToString() + "B" + bankDispersion.ToString() + "INTERBANCOS.pdf";
                    fileNameTxt = "NOMINAS_E" + keyBusiness.ToString() + "A" + year.ToString() + "P" + period.ToString() + "B" + bankDispersion.ToString() + "INTERBANCOS.txt";
                    FileStream fs = new FileStream(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder + @"\\" + fileNamePdf, FileMode.Create);
                    Document doc = new Document(iTextSharp.text.PageSize.LETTER, 20, 40, 20, 40);
                    PdfWriter pw = PdfWriter.GetInstance(doc, fs);
                    doc.AddTitle("Reporte de Dispersion");
                    doc.AddAuthor("");
                    doc.Open();
                    iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                    Font fontDefault = new Font(FontFamily.HELVETICA, 10);
                    Paragraph pr = new Paragraph();
                    DateTime datePdf = DateTime.Now;
                    pr.Font = fontDefault;
                    pr.Add("Fecha: " + datePdf.ToString("yyyy-MM-dd") + " Periodo: " + period.ToString());
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
                    int dc = 0;
                    double sumTotal = 0;
                    int cantidadEmpleados = 0;
                    double renglon1481 = 0;
                    foreach (DatosProcesaChequesNominaBean payroll in datosProcesamiento)
                    {
                        dc += 1;
                        // INICIO CODIGO NUEVO (RESTA RENGLON 1481)
                        double restaImporte = 0;
                        double importeFinal = 0;
                        renglon1481 = dataDispersionBusiness.sp_Comprueba_Existencia_Renglon_Vales(keyBusiness,
                                Convert.ToInt32(payroll.sNomina), period, typePeriod, year);
                        if (renglon1481 > 0)
                        {
                            restaImporte = payroll.doImporte - renglon1481;
                            importeFinal = restaImporte;
                        }
                        else
                        {
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
                    DateTime dateGeneration = DateTime.Now;
                    string dateGenerationFormat = dateGeneration.ToString("MMddyyyy");

                    // BANCOMER -> INTERBANCARIO -> OK -> LISTO
                    if (bankDispersion == 12)
                    {
                        if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48)
                        {
                            // CÓDIGO NUEVO INTERBANCARIO BANCOMER \\
                            // Cada archivo debe de separarse mediante 200 registros
                            int contador = 0;
                            int vueltas = 1;
                            List<DatosProcesaChequesNominaBean> bancomerNew = new List<DatosProcesaChequesNominaBean>();
                            foreach (DatosProcesaChequesNominaBean payroll in datosProcesamiento)
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
                                using (StreamWriter fileBancomer = new StreamWriter(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_BBVA_" + keyBusiness.ToString() + "_P" + z.ToString() + ".txt", false, new ASCIIEncoding()))
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
                                            string accountOrigin = datosBancoEmpresa.sNumeroCuenta;
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
                    if (bankDispersion == 14)
                    {
                        // SANTANDER - ARCHIVO OK (INTERBANCARIO)
                        // - DETALLE - \\
                        string campoFijoIntSantanderD1 = "NOMINA";
                        string fillerIntSantanderD3 = "                                                                                                                            ";
                        if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48)
                        {
                            campoFijoIntSantanderD1 = keyBusiness.ToString() + " PAGO POR CUENTA Y ORDEN SUELDO";
                            fillerIntSantanderD3 = "                                                                                                 ";
                        }
                        //if (tipPago == 2)
                        //{
                        //    campoFijoIntSantanderD1 = "HON";
                        //    fillerIntSantanderD3 = "                                                                                                                               ";
                        //}
                        string numCuentaEmpresaSantanderD = datosBancoEmpresa.sNumeroCuenta,
                            fillerIntSantanderD1 = "     ",
                            fillerIntSantanderD2 = "  ",
                            sucursalIntSantanderD1 = "1001",
                            plazaIntSantanderD1 = "01001";
                        int consecutivoIntSantanderD1 = 0;
                        if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48)
                        {
                            // CÓDIGO NUEVO
                            // Separa archivos con 500 registros cada uno
                            List<DatosProcesaChequesNominaBean> santanderNew = new List<DatosProcesaChequesNominaBean>();
                            int contador = 0;
                            int vueltas = 1;
                            foreach (DatosProcesaChequesNominaBean payroll in datosProcesamiento)
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
                                using (StreamWriter fileIntSantander = new StreamWriter(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_SANT_" + keyBusiness.ToString() + "_P" + z.ToString() + ".txt"))
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
                                                    Convert.ToInt32(bank.sNomina), period, typePeriod, year);
                                            if (renglon1481 > 0)
                                            {
                                                importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                    Convert.ToInt32(bank.sNomina), period, typePeriod, year);
                                                importeFinal = importe.decimalTotalDispersion.ToString();
                                            }
                                            else
                                            {
                                                importeFinal = bank.dImporte.ToString();
                                            }
                                            string clave = bank.sCodigo;
                                            if (bank.sCodigo != "")
                                            {
                                                clave = "  " + bank.sCodigo;
                                            }
                                            int longNomEmpIntSantanderResult = longNomEmpIntSan - nombreEmpIntSantander.Length;
                                            int longImpIntSantanderResult = longImpIntSan - importeFinal.ToString().Length;
                                            int longConIntSantanderResult = longConIntSan - consecutivoIntSantanderD1.ToString().Length;
                                            for (var b = 0; b < longNomEmpIntSantanderResult; b++) { espaciosNomEmpIntSantander += " "; }
                                            for (var t = 0; t < longImpIntSantanderResult; t++) { cerosImpIntSantander += "0"; }
                                            for (var p = 0; p < longConIntSantanderResult; p++) { cerosConIntSantander += "0"; }
                                            // + fillerIntSantanderD2 + clave
                                            fileIntSantander.Write(numCuentaEmpresaSantanderD + fillerIntSantanderD1 + cuentaFinal
                                                + clave + nombreEmpIntSantander + espaciosNomEmpIntSantander
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
                        }
                        else
                        {
                            using (StreamWriter fileIntSantander = new StreamWriter(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_SANT_" + keyBusiness.ToString() + ".txt"))
                            {
                                string espaciosNomEmpIntSantander = "",
                                    nombreEmpIntSantander = "",
                                    cerosImpIntSantander = "",
                                    cerosConIntSantander = "";
                                int longNomEmpIntSan = 40,
                                    longImpIntSan = 15,
                                    longConIntSan = 7;
                                foreach (DatosProcesaChequesNominaBean bank in datosProcesamiento)
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
                                            Convert.ToInt32(bank.sNomina), period, typePeriod, year);
                                    if (renglon1481 > 0)
                                    {
                                        importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                            Convert.ToInt32(bank.sNomina), period, typePeriod, year);
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
                                fileIntSantander.Close();
                            }
                        }
                        // # [ FIN -> CREACION DE DISPERSION DE SANTANDER (INTERBANCARIO) ] * \\
                    }

                    // SCOTIABANK -> INTERBANCARIO -> OK -> LISTO
                    if (bankDispersion == 44)
                    {
                        decimal resultadoSuma = 0;
                        // SCOTIABANK -- ARCHIVO OK (INTERBANCARIO)
                        string tipoArchivoIntScotiabank = "EE",
                            tipoRegistroIntScotiabank = "HA",
                            numeroContratoIntScotiabank = "",
                            secuenciaIntScotiabank = "01",
                            fillerIntScotiabankHA1 = "                                                                                                                                                                                                                                                                                                                                                                       ";

                        if (keyBusiness == 8)
                        {
                            numeroContratoIntScotiabank = "88178";
                        }
                        else if (keyBusiness == 10)
                        {
                            numeroContratoIntScotiabank = "47848";
                        }
                        else if (keyBusiness == 7)
                        {
                            numeroContratoIntScotiabank = "48426";
                        }
                        else if (keyBusiness == 15)
                        {
                            numeroContratoIntScotiabank = "85301";
                        }
                        else
                        {
                            numeroContratoIntScotiabank = "00000";
                        }

                        numeroContratoIntScotiabank = datosBancoEmpresa.sNumeroCliente;

                        string headerLayoutAIntScotiabank = tipoArchivoIntScotiabank + tipoRegistroIntScotiabank
                            + numeroContratoIntScotiabank + secuenciaIntScotiabank + fillerIntScotiabankHA1;
                        // - ENCABEZADO BLOQUE - \\
                        string tipoRegistroBIntScotiabank = "HB",
                            monedaCuentaBIntScotiabank = "00",
                            usoFuturoIntScotiabank = "0000",
                            cuentaCargoIntScotiabank = datosBancoEmpresa.sNumeroCuenta,
                            referenciaEmpresaIntScotiabank = "0000000001",
                            codigoStatusIntScotiabank = "000",
                            fillerIntScotiabankHB1 = "                                                                                                                                                                                                                                                                                                                                                ";
                        string headerLayoutBIntScotiabank = tipoArchivoIntScotiabank + tipoRegistroBIntScotiabank
                            + monedaCuentaBIntScotiabank + usoFuturoIntScotiabank + cuentaCargoIntScotiabank
                            + referenciaEmpresaIntScotiabank + codigoStatusIntScotiabank + fillerIntScotiabankHB1;
                        // - DETALLE - \\
                        string fechaIntScotiabankD = dateGeneration.ToString("yyyyMMdd");
                        //if (dateDisC != "")
                        //{
                        //    fechaIntScotiabankD = DateTime.Parse(dateDisC.ToString()).ToString("yyyyMMdd");
                        //}
                        string conceptoPagoIntScotiabankD = "PAGO NOMINA";
                        //if (tipPago == 2)
                        //{
                        //    conceptoPagoIntScotiabankD = "HONORARIOS ";
                        //}
                        string tipoRegistroCIntScotiabankD = "DA",
                            tipoPagoIntScotiabankD = "04",
                            claveMonedaIntScotiabank = "00",
                            servicioIntScotiabankD = "01",
                            fillerIntScotiabankD1 = "                            ",
                            plazaIntScotiabankD = "00000",
                            sucursalIntScotiabankD = "00000",
                            paisIntScotiabankD = "00000",
                            fillerIntScotiabankD2 = "                                        ",
                            tipoCuentaIntScotiabankD1 = "9",
                            digitoIntScotiabankD1 = " ",
                            bancoEmisorIntScotiabankD1 = "044",
                            diasVigenciaIntScotiabankD = "001",
                            fillerIntScotiabankD3 = "                                       ",
                            fillerIntScotiabankD4 = "                                                            ",
                            fillerIntScotiabankD5 = "                      ";
                        int consecutivoIntScotiabankD1 = 0;
                        if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48)
                        {
                            conceptoPagoIntScotiabankD = keyBusiness.ToString() + " PAGO POR CUENTA Y ORDEN SUELDO";
                            fillerIntScotiabankD3 = "                 ";
                        }
                        // - CREACION DE LISTA PARA LLENAR EL DETALLE - \\
                        if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48)
                        {
                            int cont = 0;
                            int vueltas = 1;
                            List<DatosProcesaChequesNominaBean> scotiabankNew = new List<DatosProcesaChequesNominaBean>();
                            foreach (DatosProcesaChequesNominaBean payroll in datosProcesamiento)
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
                                using (StreamWriter fileIntScotiabank = new StreamWriter(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_SCOT_" + keyBusiness.ToString() + "_P" + z.ToString() + ".txt"))
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
                                                if (accortPayroll > 0)
                                                {
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
                                                    Convert.ToInt32(bank.sNomina), period, typePeriod, year);
                                            if (renglon1481 > 0)
                                            {
                                                importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                     Convert.ToInt32(bank.sNomina), period, typePeriod, year);
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
                                z += 1;
                            }
                        }
                        else
                        {
                            using (StreamWriter fileIntScotiabank = new StreamWriter(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_SCOT_" + keyBusiness.ToString() + ".txt"))
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
                                foreach (DatosProcesaChequesNominaBean bank in datosProcesamiento)
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
                                                Convert.ToInt32(bank.sNomina), period, typePeriod, year);
                                        if (renglon1481 > 0)
                                        {
                                            importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                 Convert.ToInt32(bank.sNomina), period, typePeriod, year);
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
                    if (bankDispersion == 72)
                    {
                        // LONGITUDES \\
                        int longitudOperacion = 2;
                        int longitudClaveId = 12;
                        int longitudCuentaOrigen = 20;
                        int longitudCuentaDestino = 20;

                        string tipoOperacion = "04";
                        string cuentaOrigen = "";
                        string cuentaDestino = "";
                        string apartCeros1 = "00000000000";
                        string fillerCuentaOrigen = "";
                        string fillerCuentaDestino = "";
                        string apartCeros2 = "0";
                        string apartCeros3 = "00";
                        string referenceDate = DateTime.Now.ToString("ddMMyyyy");
                        //if (dateDisC != "")
                        //{
                        //    referenceDate = DateTime.Parse(dateDisC.ToString()).ToString("ddMMyyyy");
                        //}
                        string descriptionPd = "PAGO NOMINA                   ";
                        if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48)
                        {
                            descriptionPd = keyBusiness.ToString() + " PAGO POR CUENTA Y ORDEN SUE";
                        }
                        //if (tipPago == 2)
                        //{
                        //    descriptionPd = "HONORARIOS                    ";
                        //}
                        string coinOrigin = "1";
                        string coingDestiny = "1";
                        string ivaBanorte = "00000000000000";
                        string emailBusiness = "";
                        if (keyBusiness == 2074)
                        {
                            emailBusiness = "asesoresimpasrh@gmail.com              ";
                        }
                        else
                        {
                            emailBusiness = "dgarcia@gruposeri.com                  ";
                        }
                        // Longitudes campos
                        int longNumberPayroll = 13;
                        int longNumberADestiny = 10;
                        int longNumberImport = 14;
                        int longNumberBRfc = 13;
                        int longNumberEmail = 70;
                        int longDetailsTotal = 255;
                        if (keyBusiness == 36 || keyBusiness == 37 || keyBusiness == 38 || keyBusiness == 39 || keyBusiness == 40 || keyBusiness == 41 || keyBusiness == 46 || keyBusiness == 47 || keyBusiness == 48)
                        {
                            int contador = 0;
                            int vueltas = 1;
                            List<DatosProcesaChequesNominaBean> banorteNew = new List<DatosProcesaChequesNominaBean>();
                            foreach (DatosProcesaChequesNominaBean payroll in datosProcesamiento)
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
                                using (StreamWriter fileIntBanorte = new StreamWriter(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_BANO_" + keyBusiness.ToString() + "_P" + z.ToString() + ".txt", false, new ASCIIEncoding()))
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
                                                    Convert.ToInt32(data.sNomina), period, typePeriod, year);
                                            if (renglon1481 > 0)
                                            {
                                                importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                    Convert.ToInt32(data.sNomina), period, typePeriod, year);
                                                importeFinal = importe.decimalTotalDispersion.ToString();
                                            }
                                            else
                                            {
                                                importeFinal = data.dImporte.ToString();
                                            }
                                            string payroll = data.sNomina;
                                            int longPayroll = longNumberPayroll - payroll.Length;
                                            string accountOrigin = datosBancoEmpresa.sNumeroCuenta;
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
                        }
                        else
                        {
                            using (StreamWriter fileIntBanorte = new StreamWriter(directoryTxt + @"\\" + nameFolderYear + @"\\" + nameFolderSave + @"\\" + nameFolder + @"\\" + "LOE_INTERNAS_BANO_" + keyBusiness.ToString() + ".txt", false, new ASCIIEncoding()))
                            {
                                foreach (DatosProcesaChequesNominaBean data in datosProcesamiento)
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
                                                Convert.ToInt32(data.sNomina), period, typePeriod, year);
                                        if (renglon1481 > 0)
                                        {
                                            importe = reportDao.sp_Genera_Resta_Importes_Reporte_Dispersion(keyBusiness,
                                                Convert.ToInt32(data.sNomina), period, typePeriod, year);
                                            importeFinal = importe.decimalTotalDispersion.ToString();
                                        }
                                        else
                                        {
                                            importeFinal = data.dImporte.ToString();
                                        }
                                        string payroll = data.sNomina;
                                        int longPayroll = longNumberPayroll - payroll.Length;
                                        string accountOrigin = datosBancoEmpresa.sNumeroCuenta;
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
            catch (Exception exc)
            {

            }
            return flag;
        }

    }
}