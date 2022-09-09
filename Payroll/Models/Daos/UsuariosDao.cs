using Payroll.Models.Beans;
using Payroll.Models.Utilerias;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Payroll.Models.Daos
{
    public class UsuariosDao : Conexion
    {
        public Boolean sp_Consulta_Permiso_Consulta(int keyUser)
        {
            Boolean flag = false;
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Consulta_Permiso_Consulta", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Usuario", keyUser));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read()) {
                    flag = (data["Consulta"].ToString() == "1") ? true : false;
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
            }
            return flag;
        }

        public UsuariosBean sp_Login_Retrieve_Usuario_Inicia_Sesion(string username, string password)
        {
            UsuariosBean usuBean = new UsuariosBean();
            string encryptPassword = "";
            encryptPassword = Encriptamiento.SHA512(password);
            Mail mail = new Mail();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Login_Retrieve_Usuario_Inicia_Sesion", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@ctrlsUsuario", username));
                cmd.Parameters.Add(new SqlParameter("@ctrlsPassword", encryptPassword));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read()) {
                    if (data["sRespuesta"].ToString() == "") {
                        usuBean.iIdUsuario = int.Parse(data["iIdUsuario"].ToString());
                        usuBean.iPerfil = int.Parse(data["iPerfil"].ToString());
                        usuBean.sUsuario = data["sUsuario"].ToString();
                        usuBean.bPassword_d = data["bPassword_d"].ToString();
                        usuBean.bConsulta = data["bConsulta"].ToString();
                        usuBean.sMensaje = "success";
                    } else {
                        usuBean.sMensaje = data["sRespuesta"].ToString();
                    }
                }
                else
                {
                    usuBean.sMensaje = data["sRespuesta"].ToString();
                }
                cmd.Dispose();
                data.Close();
                conexion.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                usuBean.sMensaje = "Error - Catch " + exc.ToString();
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return usuBean;
        }
        public List<string> sp_CUsuarios_chagePassword(int iduser, string username, string password)
        {
            List<string> list = new List<string>();
            string encryptPassword = "";
            encryptPassword = Encriptamiento.SHA512(password);
            this.Conectar();
            SqlCommand cmd = new SqlCommand("sp_CUsuarios_chagePassword", this.conexion)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@ctrlUsuario_id", iduser));
            cmd.Parameters.Add(new SqlParameter("@ctrlUsuario", username));
            cmd.Parameters.Add(new SqlParameter("@ctrlPassword", encryptPassword));
            SqlDataReader data = cmd.ExecuteReader();
            if (data.Read())
            {
                list.Add(data["iFlag"].ToString());
                list.Add(data["sRespuesta"].ToString());
            }
            else
            {
                list.Add("1");
                list.Add("Error de conexion con el servidor, revisa tu conexión a internet");
            }
            cmd.Dispose(); data.Close(); conexion.Close();
            return list;
        }
        public void sp_Registro_de_bitacora(int iduser, int idprofile, string reference)
        {
            this.Conectar();
            SqlCommand cmd = new SqlCommand("sp_Registro_de_bitacora", this.conexion)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@ctrlUsuario_id", iduser));
            cmd.Parameters.Add(new SqlParameter("@ctrlPerfil_id", idprofile));
            cmd.Parameters.Add(new SqlParameter("@ctrlReferencia", reference));
            SqlDataReader data = cmd.ExecuteReader();
            if (data.Read())
            { }
            cmd.Dispose(); data.Close(); conexion.Close();
        }

    }
}