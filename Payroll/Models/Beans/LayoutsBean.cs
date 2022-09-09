using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Payroll.Models.Beans
{
    public class BusinessOriginBean
    {
        public int iId { get; set; }
        public string sValor { get; set; }
    }
    public class LayoutResult
    {
        public int iEmpresa { get; set; }
        public int iNomina  { get; set; }
        public int iBandera { get; set; }
        public string sPuesto { get; set; }
        public string sNivelJ { get; set; }
        public string sMensaje { get; set; }
        public string sMensajeError { get; set; }
        public string sStoredProcedure { get; set; }
    }
    public class LayoutLog
    {
        public int iFilaError { get; set; }
        public string sMensaje { get; set; }
    }
    public class FileLayoutHoja
    {
        public Boolean bBandera { get; set; }
        public string  sMensaje { get; set; }
    }
    public class FileLayoutValidations
    {
        public List<LayoutLog> lLogs { get; set; }
        public int iCorrectos { get; set; }
        public int iErrores { get; set; }
        public string sRutaCorrectos { get; set; }
        public string sRutaErrores { get; set; }
        public string sMensaje { get; set; }
        public Boolean bBandera { get; set; }
        public Boolean bBanderaHoja { get; set; }
        public Boolean bBanderaRegistros { get; set; }
        public Boolean bBanderaCodigo { get; set; }
        public Boolean bBanderaNombreHoja { get; set; }
        public FileLayoutHoja VNombreHoja { get; set; }
        public FileLayoutHoja VNombreCodigo { get; set; }
        public FileLayoutHoja VCantidadRegistros { get; set; }
        public FileLayoutHoja vTipoLayout { get; set; }
        public string sHtml { get; set; }
    }
    public class FileLayout
    {
        public Boolean bBandera { get; set; }
        public string sNombre { get; set; }
        public string sRuta { get; set; }
        public string sMensaje { get; set; }
    }
    public class ErroresLayoutBean
    {
        public string sNomina            { get; set; }
        public string sEmpresa           { get; set; }
        public string sErroresInsercion  { get; set; }
        public string sErroresValidacion { get; set; }
        public int iFila                 { get; set; }
        public string sTipoInsercion      { get; set; }
        public string sMensaje { get; set; }
    }
    public class LayoutSalarioMasivoBean
    {
        public int iBanderaFecha { get; set; }
        public int iBandera1     { get; set; }
        public int iBandera2     { get; set; }
        public int iBandera3     { get; set; }
        public string sMensaje   { get; set; }
        public int iCantidad     { get; set; }
    }

}