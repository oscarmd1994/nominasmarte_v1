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

namespace Payroll.Models.Utilerias
{
    public class ValidacionesLayout
    {
        public bool ValidateDataDouble(string value)
        {
            Double number;
            bool isDouble = Double.TryParse(value, out number);
            return isDouble;
        }
        public bool ValidateDataInt(string value)
        {
            int number;
            bool isNumber = Int32.TryParse(value, out number);
            return isNumber;
        }
        public bool ValidateDataLong(string value)
        {
            long number;
            bool isNumber = Int64.TryParse(value, out number);
            return isNumber;
        }
        public string ClearStringWordsCharacteres(string value)
        {
            string result = "";
            if (value.Length > 0) {
                result = value.Replace("DELETE", "").Replace("FROM", "").Replace("WHERE", "")
                              .Replace("delete", "").Replace("from", "").Replace("where", "");
                result = Regex.Replace(result, @"[^0-9a-zA-Z- ]+", "");
            }
            return result;
        }

        public FileLayoutValidations ValidationsFileDataPayroll(FileLayout fileLayout)
        {
            FileLayoutValidations validations = new FileLayoutValidations();
            validations.sMensaje  = "none";
            string tableName      = "LayoutDPayroll";
            string codeTableName  = "DATAPAYROLL";
            string typeLayout     = "";
            Boolean flagTableName    = false;
            Boolean flagCodeCorrect  = false;
            Boolean flagNumberTable  = false;
            Boolean flagCodeTpLayout = false;
            Boolean flagValidations  = false;
            validations.bBanderaHoja = false;
            validations.bBandera     = false;
            StringBuilder messageValidations = new StringBuilder();
            int rowRegister      = 0;
            int quantityRegister = 0;
            int quantityCorrect  = 0;
            int quantityError    = 0;
            List<LayoutLog> layoutLog  = new List<LayoutLog>();
            FileLayoutHoja vNombreHoja = new FileLayoutHoja();
            FileLayoutHoja vCantidadRegistros = new FileLayoutHoja();
            FileLayoutHoja vNombreCodigo = new FileLayoutHoja();
            FileLayoutHoja vTipoLayout   = new FileLayoutHoja();
            LayoutsDao layoutsDao        = new LayoutsDao();
            List<BusinessOriginBean> listOrigin = new List<BusinessOriginBean>();
            List<string> listCodes       = new List<string> { "PREMIOS", "RETROACTIVO", "SDI", "TRANSPORTE", "DIFERENCIA", "COMPLEMENTO", "EMPRESAORIGEN", "SALARIO" };
            List<int>    valuesBit       = new List<int> { 0, 1};
            try {
                using (var stream = System.IO.File.Open(fileLayout.sRuta, FileMode.Open, FileAccess.Read)) {
                    using (var reader = ExcelReaderFactory.CreateReader(stream)) {
                        var result      = reader.AsDataSet();
                        DataTable table = result.Tables[0];
                        DataRow row     = table.Rows[0];
                        if (table.TableName == tableName) {
                            vNombreHoja.bBandera = true;
                            vNombreHoja.sMensaje = " [*] El nombre de la hoja " + table.TableName + " es valido. ";
                            flagTableName = true;
                            validations.VNombreHoja = vNombreHoja;
                        } else {
                            vNombreHoja.bBandera = false;
                            vNombreHoja.sMensaje = " [*] El nombre de la hoja " + table.TableName + " no es valido. ";
                            validations.VNombreHoja = vNombreHoja;
                        }
                        if (row[1].ToString() == codeTableName) {
                            vNombreCodigo.bBandera = true;
                            vNombreCodigo.sMensaje = "[*] El código " + row[1].ToString() + " es valido para el layout seleccionado. ";
                            validations.VNombreCodigo = vNombreCodigo;
                            flagCodeCorrect = true;
                        } else {
                            vNombreCodigo.bBandera = false;
                            vNombreCodigo.sMensaje = "[*] El código " + row[1].ToString() + " no es valido para el tipo de layout seleccionado. ";
                            validations.VNombreCodigo = vNombreCodigo;
                        }
                        bool isNumber = ValidateDataInt(row[0].ToString());
                        if (isNumber) {
                            vCantidadRegistros.bBandera = true;
                            vCantidadRegistros.sMensaje = " [*] El valor especificado " + row[0].ToString() + " a la cantidad de datos es valido. ";
                            validations.VCantidadRegistros = vCantidadRegistros;
                            flagNumberTable = true;
                        } else {
                            vCantidadRegistros.bBandera = false;
                            vCantidadRegistros.sMensaje = " [*] El valor especificado " + row[0].ToString() + " a la cantidad de datos no corresponde a un dato númerico. ";
                            validations.VCantidadRegistros = vCantidadRegistros;
                        }
                        if (row[2].ToString().Trim() != "") {
                            if (!listCodes.Contains(row[2].ToString().Trim())) {
                                vTipoLayout.bBandera = false;
                                vTipoLayout.sMensaje = "[*] El valor especificado " + row[2].ToString() + " al tipo de layout no corresponde a un código valido. ";
                                validations.vTipoLayout = vTipoLayout;
                            } else {
                                vTipoLayout.bBandera = true;
                                vTipoLayout.sMensaje = "[*] El valor especificado " + row[2].ToString() + " al tipo de layout es valido. ";
                                validations.vTipoLayout = vTipoLayout;
                                flagCodeTpLayout = true;
                                typeLayout = row[2].ToString().Trim();
                            }
                        } else {
                            vTipoLayout.bBandera = false;
                            vTipoLayout.sMensaje = "[*] El valor especificado " + row[2].ToString() + " al tipo de layout no puede ir vacío. ";
                            validations.vTipoLayout = vTipoLayout;
                        }
                        if (flagTableName && flagCodeCorrect && flagNumberTable && flagCodeTpLayout) {
                            validations.bBanderaHoja = true;
                            quantityRegister = Convert.ToInt32(row[0]);
                            listOrigin = layoutsDao.sp_Obtiene_Empresas_Origen();
                            foreach (DataRow dr in table.Rows) {
                                bool continueBusiness = false;
                                bool continueEmployee = false;
                                if (dr[1].ToString() != codeTableName) {
                                    rowRegister += 1;
                                    if ((quantityRegister + 1) == rowRegister) {
                                        break;
                                    }
                                    flagValidations = false;
                                    /* VALIDACIONES */
                                    // Columna Empresa
                                    bool isNumberBusiness = ValidateDataInt(dr[3].ToString().Trim());
                                    if (isNumberBusiness) {
                                        if (dr[3].ToString().Trim() == "" || Convert.ToInt32(dr[3].ToString().Trim()) == 0) {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[3].ToString() + "</b> en la columna <b>EMPRESA</b> no puede ir vacío ni valor 0.");
                                            flagValidations = true;
                                        } else {
                                            continueBusiness = true;
                                        }
                                    } else {
                                        messageValidations.Append("[*]El valor ingresado <b>" + dr[3].ToString() + "</b> en la columna <b>EMPRESA</b> no es un valor númerico.");
                                        flagValidations = true;
                                    }
                                    // Columna Numero de empleado
                                    bool isNumberPayroll = ValidateDataInt(dr[4].ToString().Trim());
                                    if (isNumberPayroll) {
                                        if (dr[4].ToString().Trim() == "" || Convert.ToInt32(dr[4].ToString().Trim()) == 0) {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[4].ToString() + "</b> en la columna <b>NÓMINA</b> no puede ir vacío ni valor 0.");
                                            flagValidations = true;
                                        } else {
                                            continueEmployee = true;
                                        }
                                    } else {
                                        messageValidations.Append("[*]El valor ingresado <b>" + dr[4].ToString() + "</b> en la columna <b>NÓMINA</b> no es un valor númerico.");
                                        flagValidations = true;
                                    }
                                    // Columnas 
                                    if (typeLayout == "PREMIOS") {
                                        // Columna 5 (F)
                                        if (dr[5].ToString().Trim() == "") {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[5].ToString() + "</b> en la columna <b>CON PREMIOS</b> no puede ir vacío.");
                                            flagValidations = true;
                                        } else {
                                            bool isNumberP = ValidateDataInt(dr[5].ToString().Trim());
                                            if (isNumberP) {
                                                if (!valuesBit.Contains(Convert.ToInt32(dr[5].ToString().Trim()))) {
                                                    messageValidations.Append("[*]El valor ingresado <b>" + dr[5].ToString() + "</b> en la columna <b>CON PREMIOS</b> no pertenece a un valor del catalogo valido (0, 1).");
                                                    flagValidations = true;
                                                }
                                            } else {
                                                messageValidations.Append("[*]El valor ingresado <b>" + dr[5].ToString() + "</b> en la columna <b>CON RETROACTIVO</b> debe de ser un valor númerico (0, 1).");
                                                flagValidations = true;
                                            }
                                        }
                                    } else if (typeLayout == "RETROACTIVO") {
                                        // Columna 6 (G)
                                        if (dr[6].ToString().Trim() == "") {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[6].ToString() + "</b> en la columna <b>CON RETROACTIVO</b> no puede ir vacío.");
                                            flagValidations = true;
                                        } else {
                                            bool isNumberR = ValidateDataInt(dr[6].ToString().Trim());
                                            if (isNumberR) {
                                                if (!valuesBit.Contains(Convert.ToInt32(dr[6].ToString().Trim()))) {
                                                    messageValidations.Append("[*]El valor ingresado <b>" + dr[6].ToString() + "</b> en la columna <b>CON RETROACTIVO</b> no pertenece a un valor del catalogo valido (0, 1).");
                                                    flagValidations = true;
                                                }
                                            } else {
                                                messageValidations.Append("[*]El valor ingresado <b>" + dr[6].ToString() + "</b> en la columna <b>CON RETROACTIVO</b> debe de ser un valor númerico (0, 1).");
                                                flagValidations = true;
                                            }
                                        }
                                    } else if (typeLayout == "SDI") {
                                        // Columna 7 (H)
                                        if (dr[7].ToString().Trim() == "") {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[7].ToString() + "</b> en la columna <b>SDI</b> no puede ir vacío.");
                                            flagValidations = true;
                                        } else {
                                            bool isNumberS = ValidateDataDouble(dr[7].ToString().Trim());
                                            if (!isNumberS) {
                                                messageValidations.Append("[*]El valor ingresado <b>" + dr[7].ToString() + "</b> en la columna <b>SDI</b> debe de ser un valor númerico.");
                                                flagValidations = true;
                                            }
                                             
                                        }
                                    } else if (typeLayout == "TRANSPORTE") {
                                        // Columna 8 (I)
                                        if (dr[8].ToString().Trim() == "") {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[8].ToString() + "</b> en la columna <b>TRANSPORTE</b> no puede ir vacío.");
                                            flagValidations = true;
                                        } else {
                                            bool isNumberT = ValidateDataDouble(dr[8].ToString().Trim());
                                            if (!isNumberT) {
                                                messageValidations.Append("[*]El valor ingresado <b>" + dr[8].ToString() + "</b> en la columna <b>TRANSPORTE</b> debe de ser un valor númerico.");
                                                flagValidations = true;
                                            }

                                        }
                                    } else if (typeLayout == "DIFERENCIA") {
                                        // Columna 9 (J)
                                        if (dr[9].ToString().Trim() == "") {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[9].ToString() + "</b> en la columna <b>DIFERENCIAP</b> no puede ir vacío.");
                                            flagValidations = true;
                                        } else {
                                            bool isNumberD = ValidateDataDouble(dr[9].ToString().Trim());
                                            if (!isNumberD) {
                                                messageValidations.Append("[*]El valor ingresado <b>" + dr[9].ToString() + "</b> en la columna <b>DIFERENCIAP</b> debe de ser un valor númerico.");
                                                flagValidations = true;
                                            }
                                        }
                                    } else if (typeLayout == "COMPLEMENTO") {
                                        // Columna 10 (K)
                                        if (dr[10].ToString().Trim() == "") {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[10].ToString() + "</b> en la columna <b>COMPLEMENTOE</b> no puede ir vacío.");
                                            flagValidations = true;
                                        } else {
                                            bool isNumberC = ValidateDataDouble(dr[10].ToString().Trim());
                                            if (!isNumberC) {
                                                messageValidations.Append("[*]El valor ingresado <b>" + dr[10].ToString() + "</b> en la columna <b>COMPLEMENTOE</b> debe de ser un valor númerico.");
                                                flagValidations = true;
                                            }

                                        }
                                    } else if (typeLayout == "EMPRESAORIGEN") {
                                        // Columna 11 (L)
                                        if (dr[11].ToString().Trim() == "") {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[11].ToString() + "</b> en la columna <b>EMPRESA ORIGEN</b> no puede ir vacío");
                                            flagValidations = true;
                                        } else {
                                            bool isNumberEO = ValidateDataInt(dr[11].ToString().Trim());
                                            if (isNumberEO) {
                                                if (!listOrigin.Exists(x => x.iId == Convert.ToInt32(dr[11].ToString().Trim()))) {
                                                    messageValidations.Append("[*]El valor ingresado <b>" + dr[11].ToString() + "</b> en la columna <b>EMPRESA ORIGEN</b> no pertenece a un valor del catalogo valido de Empresa Origen.");
                                                    flagValidations = true;
                                                }
                                            } else {
                                                messageValidations.Append("[*]El valor ingresado <b> " + dr[11].ToString() + " </b> en la columna <b>EMPRESA ORIGEN</b> debe de ser un valor númerico");
                                                flagValidations = true;
                                            }
                                        }
                                    } else if (typeLayout == "SALARIO") {
                                        // Columna 12 (M)
                                        if (dr[12].ToString().Trim() == "") {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[12].ToString() + "</b> en la columna <b>SALARIO</b> no puede ir vacío");
                                            flagValidations = true;
                                        } else {
                                            bool isDoubleSA = ValidateDataDouble(dr[12].ToString().Trim());
                                            bool isNumberSA = ValidateDataInt(dr[12].ToString().Trim());
                                            if (!isDoubleSA || !isNumber) {
                                                messageValidations.Append("[*]El valor ingresado <b> " + dr[12].ToString() + " </b> en la columna <b>SALARIO</b> debde de ser un valor númerico");
                                                flagValidations = true;
                                            }
                                        }
                                    }
                                    // Comprueba que el empleado pertenezca a la empresa indicada
                                    if (continueBusiness && continueEmployee) {
                                        int keyEmployee = Convert.ToInt32(dr[4].ToString().Trim());
                                        int keyBusiness = Convert.ToInt32(dr[3].ToString().Trim());
                                        Boolean checkCorrectExists = layoutsDao.sp_Comprueba_Empleado_Empresa(keyEmployee, keyBusiness);
                                        if (!checkCorrectExists) {
                                            messageValidations.Append("[*]El empleado indicado <b>" + keyEmployee.ToString() + "</b> no corresponde a la empresa <b>" + keyBusiness.ToString() + "</b>.");
                                            flagValidations = true;
                                        }
                                    }
                                    if (flagValidations) {
                                        layoutLog.Add(new LayoutLog {
                                            iFilaError = rowRegister,
                                            sMensaje = messageValidations.ToString()
                                        });
                                        quantityError += 1;
                                        validations.sHtml += "<tr>"
                                                          + "<td> " + rowRegister.ToString() + "</td>"
                                                          + "<td> " + messageValidations.ToString() + " </td>"
                                                          + "</tr>";
                                        messageValidations.Clear();
                                        continue;
                                    }
                                    quantityCorrect += 1;
                                }
                            }
                            validations.lLogs      = layoutLog;
                            validations.bBandera   = true;
                            validations.iCorrectos = quantityCorrect;
                            validations.iErrores   = quantityError;
                        }
                    }
                }
            } catch (Exception exc) {
                validations.sMensaje = exc.Message.ToString();
            }
            return validations;
        }

        public FileLayoutValidations ValidationsFileAccountBank(FileLayout fileLayout)
        {
            FileLayoutValidations validations = new FileLayoutValidations();
            validations.sMensaje = "none";
            string tableName     = "LayoutAccount";
            string codeTableName = "DATAACCOUNT";
            Boolean flagTableName    = false;
            Boolean flagCodeCorrect  = false;
            Boolean flagNumberTable  = false;
            Boolean flagValidations  = false;
            validations.bBanderaHoja = false;
            validations.bBandera     = false;
            StringBuilder messageValidations = new StringBuilder();
            int rowRegister      = 0;
            int quantityRegister = 0;
            int quantityCorrect  = 0;
            int quantityError    = 0;
            List<LayoutLog> layoutLog  = new List<LayoutLog>();
            FileLayoutHoja vNombreHoja = new FileLayoutHoja();
            FileLayoutHoja vCantidadRegistros = new FileLayoutHoja();
            FileLayoutHoja vNombreCodigo      = new FileLayoutHoja();
            LayoutsDao layoutsDao             = new LayoutsDao();
            List<int> listCodeBanks           = new List<int>();
            try {
                using (var stream = System.IO.File.Open(fileLayout.sRuta, FileMode.Open, FileAccess.Read)) {
                    using (var reader = ExcelReaderFactory.CreateReader(stream)) {
                        listCodeBanks = layoutsDao.sp_Codigos_Bancos();
                        var result = reader.AsDataSet();
                        DataTable table = result.Tables[0];
                        DataRow row     = table.Rows[0];
                        if (table.TableName == tableName) {
                            vNombreHoja.bBandera = true;
                            vNombreHoja.sMensaje = " [*] El nombre de la hoja " + table.TableName + " es valido. ";
                            flagTableName = true;
                            validations.VNombreHoja = vNombreHoja;
                        } else {
                            vNombreHoja.bBandera = false;
                            vNombreHoja.sMensaje = " [*] El nombre de la hoja " + table.TableName + " no es valido. ";
                            validations.VNombreHoja = vNombreHoja;
                        }
                        if (row[1].ToString() == codeTableName) {
                            vNombreCodigo.bBandera = true;
                            vNombreCodigo.sMensaje = "[*] El código " + row[1].ToString() + " es valido para el layout seleccionado. ";
                            validations.VNombreCodigo = vNombreCodigo;
                            flagCodeCorrect = true;
                        } else {
                            vNombreCodigo.bBandera = false;
                            vNombreCodigo.sMensaje = "[*] El código " + row[1].ToString() + " no es valido para el tipo de layout seleccionado. ";
                            validations.VNombreCodigo = vNombreCodigo;
                        }
                        bool isNumber = ValidateDataInt(row[0].ToString());
                        if (isNumber) {
                            vCantidadRegistros.bBandera = true;
                            vCantidadRegistros.sMensaje = " [*] El valor especificado " + row[0].ToString() + " a la cantidad de datos es valido. ";
                            validations.VCantidadRegistros = vCantidadRegistros;
                            flagNumberTable = true;
                        } else {
                            vCantidadRegistros.bBandera = false;
                            vCantidadRegistros.sMensaje = " [*] El valor especificado " + row[0].ToString() + " a la cantidad de datos no corresponde a un dato númerico. ";
                            validations.VCantidadRegistros = vCantidadRegistros;
                        }
                        if (flagTableName && flagCodeCorrect && flagNumberTable) {
                            validations.bBanderaHoja = true;
                            quantityRegister         = Convert.ToInt32(row[0]);
                            foreach (DataRow dr in table.Rows) {
                                bool continueBusiness = false;
                                bool continueEmployee = false;
                                if (dr[1].ToString() != codeTableName) {
                                    rowRegister += 1;
                                    if ((quantityRegister + 1) == rowRegister) {
                                        break;
                                    }
                                    flagValidations = false;
                                    /* VALIDACIONES */
                                    // Columna Empresa
                                    bool isNumberBusiness = ValidateDataInt(dr[2].ToString().Trim());
                                    if (isNumberBusiness) {
                                        if (dr[2].ToString().Trim() == "" || Convert.ToInt32(dr[2].ToString().Trim()) == 0) {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[2].ToString() + "</b> en la columna <b>EMPRESA</b> no puede ir vacío ni valor 0.");
                                            flagValidations = true;
                                        } else {
                                            continueBusiness = true;
                                        }
                                    } else {
                                        messageValidations.Append("[*]El valor ingresado <b>" + dr[2].ToString() + "</b> en la columna <b>EMPRESA</b> no es un valor númerico.");
                                        flagValidations = true;
                                    }
                                    // Columna Numero de empleado
                                    bool isNumberPayroll = ValidateDataInt(dr[3].ToString().Trim());
                                    if (isNumberPayroll) {
                                        if (dr[3].ToString().Trim() == "" || Convert.ToInt32(dr[3].ToString().Trim()) == 0) {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[3].ToString() + "</b> en la columna <b>NÓMINA</b> no puede ir vacío ni valor 0.");
                                            flagValidations = true;
                                        } else {
                                            continueEmployee = true;
                                        }
                                    } else {
                                        messageValidations.Append("[*]El valor ingresado <b>" + dr[3].ToString() + "</b> en la columna <b>NÓMINA</b> no es un valor númerico.");
                                        flagValidations = true;
                                    }
                                    // Columna Banco
                                    if (dr[4].ToString().Trim() == "") {
                                        messageValidations.Append("[*]El valor ingresado <b>" + dr[4].ToString() + "</b> en la columna <b>BANCO</b> no puede ir vacío ni valor 0.");
                                        flagValidations = true;
                                    } else {
                                        bool isNumberBank = ValidateDataInt(dr[4].ToString().Trim());
                                        if (isNumberBank) {
                                            if (!listCodeBanks.Contains(Convert.ToInt32(dr[4].ToString().Trim()))) {
                                                messageValidations.Append("[*]El valor ingresado <b>" + dr[4].ToString() + "</b> en la columna <b>BANCO</b> no pertenece a un valor del catalogo valido.");
                                                flagValidations = true;
                                            }
                                        } else {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[3].ToString() + "</b> en la columna <b>BANCO</b> no es un valor númerico.");
                                            flagValidations = true;
                                        }
                                    }
                                    // Columna Cuenta cheques
                                    if (dr[5].ToString().Trim() == "") {
                                        messageValidations.Append("[*]El valor ingresado <b>" + dr[5].ToString() + "</b> en la columna <b>CUENTA CHEQUES</b> no puede ir vacío ni valor 0.");
                                        flagValidations = true;
                                    } else {
                                        string accountClear = dr[5].ToString().Trim().Replace("Cta_", "").Replace("cta_", "");
                                        bool isNumberAccount = ValidateDataLong(accountClear);
                                        if (isNumberAccount) {
                                            if (accountClear.Length > 18) {
                                                messageValidations.Append("[*]El valor ingresado <b>" + dr[5].ToString() + "</b> en la columna <b>CUENTA CHEQUES</b> excede el número de caracteres permitidos (18).");
                                                flagValidations = true;
                                            }
                                        } else {
                                            messageValidations.Append("[*]El valor ingresado <b>" + accountClear + "</b> en la columna <b>CUENTA CHEQUES</b> no es un valor númerico.");
                                            flagValidations = true;
                                        }
                                    }
                                    // Comprueba que el empleado pertenezca a la empresa indicada
                                    if (continueBusiness && continueEmployee) {
                                        int keyEmployee = Convert.ToInt32(dr[3].ToString().Trim());
                                        int keyBusiness = Convert.ToInt32(dr[2].ToString().Trim());
                                        Boolean checkCorrectExists = layoutsDao.sp_Comprueba_Empleado_Empresa(keyEmployee, keyBusiness);
                                        if (!checkCorrectExists) {
                                            messageValidations.Append("[*]El empleado indicado <b>" + keyEmployee.ToString() + "</b> no corresponde a la empresa <b>" + keyBusiness.ToString() + "</b>.");
                                            flagValidations = true;
                                        }
                                    }
                                    if (flagValidations) {
                                        layoutLog.Add(new LayoutLog {
                                            iFilaError = rowRegister,
                                            sMensaje = messageValidations.ToString()
                                        });
                                        quantityError += 1;
                                        validations.sHtml += "<tr>"
                                                          + "<td> " + rowRegister.ToString() + "</td>"
                                                          + "<td> " + messageValidations.ToString() + " </td>"
                                                          + "</tr>";
                                        messageValidations.Clear();
                                        continue;
                                    }
                                    quantityCorrect += 1;
                                }
                            }
                            validations.lLogs      = layoutLog;
                            validations.bBandera   = true;
                            validations.iCorrectos = quantityCorrect;
                            validations.iErrores   = quantityError;
                        }
                    }
                }
            } catch (Exception exc) {
                validations.sMensaje = exc.Message.ToString();
            }
            return validations;
        }
        public FileLayoutValidations ValidationsFilePosts(FileLayout fileLayout)
        {
            FileLayoutValidations validations = new FileLayoutValidations();
            validations.sMensaje = "none";
            string tableName     = "LayoutPosts";
            string codeTableName = "DATAPOSTS";
            Boolean flagTableName    = false;
            Boolean flagCodeCorrect  = false;
            Boolean flagNumberTable  = false;
            Boolean flagValidations  = false;
            validations.bBanderaHoja = false;
            validations.bBandera     = false;
            StringBuilder messageValidations = new StringBuilder();
            int rowRegister      = 0;
            int quantityRegister = 0;
            int quantityCorrect  = 0;
            int quantityError    = 0;
            List< LayoutLog> layoutLog  = new List<LayoutLog>();
            FileLayoutHoja vNombreHoja  = new FileLayoutHoja();
            FileLayoutHoja vCantidadRegistros = new FileLayoutHoja();
            FileLayoutHoja vNombreCodigo = new FileLayoutHoja();
            List<string> listNivelsJerar = new List<string>() { "OPERATIVOS", "MANDO", "SUPERVISION" };
            LayoutsDao layoutsDao = new LayoutsDao();
            try {
                using (var stream = System.IO.File.Open(fileLayout.sRuta, FileMode.Open, FileAccess.Read)) {
                    using (var reader = ExcelReaderFactory.CreateReader(stream)) {
                        var result = reader.AsDataSet();
                        DataTable table = result.Tables[0];
                        DataRow   row   = table.Rows[0];
                        if (table.TableName == tableName) {
                            vNombreHoja.bBandera = true;
                            vNombreHoja.sMensaje = " [*] El nombre de la hoja " + table.TableName + " es valido. ";
                            flagTableName = true;
                            validations.VNombreHoja = vNombreHoja;
                        } else {
                            vNombreHoja.bBandera = false;
                            vNombreHoja.sMensaje = " [*] El nombre de la hoja " + table.TableName + " no es valido. ";
                            validations.VNombreHoja = vNombreHoja;
                        }
                        if (row[1].ToString() == codeTableName) {
                            vNombreCodigo.bBandera = true;
                            vNombreCodigo.sMensaje = "[*] El código " + row[1].ToString() + " es valido para el layout seleccionado. ";
                            validations.VNombreCodigo = vNombreCodigo;
                            flagCodeCorrect = true;
                        } else {
                            vNombreCodigo.bBandera = false;
                            vNombreCodigo.sMensaje = "[*] El código " + row[1].ToString() + " no es valido para el tipo de layout seleccionado. ";
                            validations.VNombreCodigo = vNombreCodigo;
                        }
                        bool isNumber = ValidateDataInt(row[0].ToString());
                        if (isNumber) {
                            vCantidadRegistros.bBandera = true;
                            vCantidadRegistros.sMensaje = " [*] El valor especificado " + row[0].ToString() + " a la cantidad de datos es valido. ";
                            validations.VCantidadRegistros = vCantidadRegistros;
                            flagNumberTable = true;
                        } else{
                            vCantidadRegistros.bBandera = false;
                            vCantidadRegistros.sMensaje = " [*] El valor especificado " + row[0].ToString() + " a la cantidad de datos no corresponde a un dato númerico. ";
                            validations.VCantidadRegistros = vCantidadRegistros;
                        }
                        if (flagTableName && flagCodeCorrect && flagNumberTable) {
                            validations.bBanderaHoja = true;
                            quantityRegister = Convert.ToInt32(row[0]);
                            foreach (DataRow dr in table.Rows) {
                                bool continueBusiness = false;
                                bool continueEmployee = false;
                                if (dr[1].ToString().Trim() != codeTableName) {
                                    rowRegister += 1;
                                    if ((quantityRegister + 1) == rowRegister) {
                                        break;
                                    }
                                    flagValidations = false;
                                    // Validaciones
                                    // Columna Empresa
                                    bool isNumberBusiness = ValidateDataInt(dr[2].ToString().Trim());
                                    if (isNumberBusiness) {
                                        if (dr[2].ToString().Trim() == "" || Convert.ToInt32(dr[2].ToString().Trim()) == 0) {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[2].ToString() + "</b> en la columna <b>EMPRESA</b> no puede ir vacío ni valor 0." );
                                            flagValidations = true;
                                        } else {
                                            continueBusiness = true;
                                        }
                                    } else {
                                        messageValidations.Append("[*]El valor ingresado <b>" + dr[2].ToString() + "</b> en la columna <b>EMPRESA</b> no es un valor númerico.");
                                        flagValidations = true;
                                    }
                                    // Columna Numero de empleado
                                    bool isNumberPayroll = ValidateDataInt(dr[3].ToString().Trim());
                                    if (isNumberPayroll) {
                                        if (dr[3].ToString().Trim() == "" || Convert.ToInt32(dr[3].ToString().Trim()) == 0) {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[3].ToString() + "</b> en la columna <b>NÚMERO DE EMPLEADO</b> no puede ir vacío ni valor 0.");
                                            flagValidations = true;
                                        } else {
                                            continueEmployee = true;
                                        }
                                    } else {
                                        messageValidations.Append("[*]El valor ingresado <b>" + dr[3].ToString() + "</b> en la columna <b>NÚMERO DE EMPLEADO</b> no es un valor númerico.");
                                        flagValidations = true;
                                    }
                                    // Columna Nuevo puesto
                                    if (dr[4].ToString().Trim() == "") {
                                        messageValidations.Append("[*]El valor ingresado <b>" + dr[4].ToString() + "</b> en la columna <b>NUEVO PUESTO</b> no puede ir vacío ni valor 0.");
                                        flagValidations = true;
                                    }
                                    // Columna Nivel jerarquico
                                    if (dr[5].ToString().Trim() == "") {
                                        messageValidations.Append("[*]El valor ingresado <b>" + dr[5].ToString() + "</b> en la columna <b>NIVEL JERARQUICO</b> no puede ir vacío ni valor 0.");
                                        flagValidations = true;
                                    } else {
                                        if (!listNivelsJerar.Contains(dr[5].ToString().Trim())) {
                                            messageValidations.Append("[*]El valor ingresado <b>" + dr[5].ToString() + "</b> en la columna <b>NIVEL JERARQUICO</b> no pertenece a un valor del catalogo valido.");
                                            flagValidations = true;
                                        }
                                    }
                                    // Comprueba que el empleado pertenezca a la empresa indicada
                                    if (continueBusiness && continueEmployee) {
                                        int keyEmployee = Convert.ToInt32(dr[3].ToString().Trim());
                                        int keyBusiness = Convert.ToInt32(dr[2].ToString().Trim());
                                        Boolean checkCorrectExists = layoutsDao.sp_Comprueba_Empleado_Empresa(keyEmployee, keyBusiness);
                                        if (!checkCorrectExists) {
                                            messageValidations.Append("[*]El empleado indicado <b>" + keyEmployee.ToString() + "</b> no corresponde a la empresa <b>" + keyBusiness.ToString() + "</b>.");
                                            flagValidations = true;
                                        }
                                    }
                                    if (flagValidations) {
                                        layoutLog.Add(new LayoutLog {
                                            iFilaError = rowRegister,
                                            sMensaje   = messageValidations.ToString()
                                        });
                                        quantityError += 1;
                                        validations.sHtml += "<tr>" 
                                                          + "<td> "+ rowRegister.ToString() + "</td>" 
                                                          + "<td> " + messageValidations.ToString() + " </td>"
                                                          + "</tr>";
                                        messageValidations.Clear();
                                        continue;
                                    }
                                    quantityCorrect += 1;
                                }
                            }
                            validations.lLogs        = layoutLog;
                            validations.bBandera     = true;
                            validations.iCorrectos   = quantityCorrect;
                            validations.iErrores     = quantityError;
                        }
                    }
                }
            } catch (Exception exc) {
                validations.sMensaje = exc.Message.ToString();
            }
            return validations;
        }

    }
}