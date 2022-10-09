using System;
using System.Data.SqlClient;

namespace Payroll.Models.Utilerias
{
    public class Conexion :  Encrypt
    {
        static readonly string Server = "162.215.13.173";
        //static readonly string Server = "162.215.13.173:4489";
        //static readonly string Server = "DEDI-849133";
        //static readonly string Db = "IPSNet_Copia";
        static readonly string Db = "PayrollM";
        //static readonly string User = "IPSNet";
        static readonly string User = "martesql";
        //static readonly string User = "NMarte";
        static readonly string Pass = "h6L*31^w9bHZZi*S5fg5";
        //static readonly string Pass = "M@rteN@mina";
        protected SqlConnection conexion { get; set; }
        protected SqlConnectionStringBuilder connection { get; set; }
        protected SqlConnection Conectar()
        {
            try
            {
                conexion = new SqlConnection("Data Source=" + Server + ";Initial Catalog=" + Db + ";User ID=" + User + ";Password=" + Pass + ";Integrated Security=False;Language=Spanish");
                //conexion = new SqlConnection("Data Source=" + Server + ";Initial Catalog=" + Db + ";Integrated Security=True");
                //conexion = new SqlConnection("Server=(local);Database=PayrollM;Integrated Security=true");
                //conexion = new SqlConnection("Server=" + Server + ";Database=PayrollM;Integrated Security=true;Language=Spanish");
                conexion.Open();
                return conexion;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return null;
            }
        }

    }
}