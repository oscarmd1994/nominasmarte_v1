using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Payroll.Models.Utilerias;
using Payroll.Models.Daos;
using Payroll.Models.Beans;
using System.Data.SqlClient;
using System.Data;

namespace Payroll.Models.Daos
{
    public class LayoutsDao : Conexion
    {
        public List<BusinessOriginBean> sp_Obtiene_Empresas_Origen()
        {
            List<BusinessOriginBean> business = new List<BusinessOriginBean>();
            try {
                this.Conectar();
                SqlCommand command = new SqlCommand("sp_Obtiene_Empresas_Origen", this.conexion) { CommandType = CommandType.StoredProcedure };
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows) {
                    while (dataReader.Read()) {
                        BusinessOriginBean bean = new BusinessOriginBean();
                        bean.iId    = Convert.ToInt32(dataReader["id"]);
                        bean.sValor = dataReader["Valor"].ToString();
                        business.Add(bean);
                    }
                }
                command.Parameters.Clear();
                command.Dispose();
                dataReader.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.Conectar().Close();
                this.conexion.Close();
            }
            return business;
        }
        public Boolean sp_Guarda_Historia_Layouts(int keyUser, string typeLayout, string routeFile, string nameFile)
        {
            Boolean flag = false;
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Guarda_Historia_Layouts", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdUsuario", keyUser));
                cmd.Parameters.Add(new SqlParameter("@TipoLayout", typeLayout));
                cmd.Parameters.Add(new SqlParameter("@RutaArchivo", routeFile));
                cmd.Parameters.Add(new SqlParameter("@NombreArchivo", nameFile));
                cmd.Parameters.Add(new SqlParameter("@VersionArchivo", 1));
                if (cmd.ExecuteNonQuery() > 0) {
                    flag = true;
                }
                cmd.Parameters.Clear(); cmd.Dispose();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return flag;
        }
        public List<int> sp_Codigos_Bancos()
        {
            List<int> code = new List<int>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Codigos_Bancos", this.conexion) { CommandType = CommandType.StoredProcedure };
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        int codeB = Convert.ToInt32(data["IdBanco"]);
                        code.Add(codeB);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return code;
        }
        public LayoutResult sp_Actualiza_Puestos_Empleados(int keyBusiness, int keyPayroll, string newPost, string nivelJe)
        {
            LayoutResult layout = new LayoutResult();
            layout.iBandera     = 0;
            layout.sMensaje     = "none";
            layout.sMensajeError = "none";
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Actualiza_Puestos_Empleados", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Empresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Nomina", keyPayroll));
                cmd.Parameters.Add(new SqlParameter("@Puesto", newPost));
                cmd.Parameters.Add(new SqlParameter("@NivelJerarquico", nivelJe));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read()) {
                    if (dataReader["Validacion"].ToString() == "1") {
                        layout.iBandera = Convert.ToInt32(dataReader["Validacion"]);
                        layout.sMensaje = "success";
                    } else {
                        layout.iEmpresa = keyBusiness;
                        layout.iNomina  = keyPayroll;
                        layout.sPuesto  = newPost;
                        layout.sNivelJ  = nivelJe;
                        layout.sMensaje = dataReader["Mensaje"].ToString();
                    }
                    layout.sStoredProcedure = "EXEC sp_Actualiza_Puestos_Empleados " + keyBusiness.ToString() + " , " + keyPayroll.ToString() + " , '" + newPost + "' , '" + nivelJe + "'";
                }
                cmd.Parameters.Clear(); cmd.Dispose(); dataReader.Close();
            } catch (Exception exc) {
                layout.sMensajeError = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return layout;
        }

        public LayoutResult sp_Actualiza_Datos_Bancarios(int keyBusiness, int keyPayroll, int keyBank, string account, int keyUser)
        {
            LayoutResult layout  = new LayoutResult();
            layout.iBandera      = 0;
            layout.sMensaje      = "none";
            layout.sMensajeError = "none";
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Actualiza_Datos_Bancarios", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@IdEmpleado", keyPayroll));
                cmd.Parameters.Add(new SqlParameter("@IdBanco", keyBank));
                cmd.Parameters.Add(new SqlParameter("@Cuenta", account));
                cmd.Parameters.Add(new SqlParameter("@UsuarioModifica", keyUser));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read()) {
                    if (dataReader["Validacion"].ToString() == "1") {
                        layout.iBandera = Convert.ToInt32(dataReader["Validacion"]);
                        layout.sMensaje = "success";
                    } else {
                        layout.iEmpresa = keyBusiness;
                        layout.iNomina  = keyPayroll;
                    }
                    layout.sStoredProcedure = "EXEC sp_Actualiza_Datos_Bancarios " + keyBusiness.ToString() + " , " + keyPayroll.ToString() + " , " + keyBank.ToString() + " , '" + account + "'";
                }
                cmd.Parameters.Clear(); cmd.Dispose(); dataReader.Close();
            } catch (Exception exc) {
                layout.sMensajeError = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return layout;
        }

        public LayoutResult sp_Actualiza_Datos_Diversos_Nominas(int keyBusiness, int keyPayroll, string value, string type, int keyUser)
        {
            LayoutResult layout = new LayoutResult();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Actualiza_Datos_Diversos_Nominas", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@IdEmpleado", keyPayroll));
                cmd.Parameters.Add(new SqlParameter("@Valor", value));
                cmd.Parameters.Add(new SqlParameter("@Tipo", type));
                cmd.Parameters.Add(new SqlParameter("@UsuarioModifica", keyUser));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read()) {
                    if (dataReader["Validacion"].ToString() == "1") {
                        layout.iBandera = Convert.ToInt32(dataReader["Validacion"]);
                        layout.sMensaje = "success";
                    } else {
                        layout.iEmpresa = keyBusiness;
                        layout.iNomina  = keyPayroll;
                    }
                    layout.sStoredProcedure = "EXEC sp_Actualiza_Datos_Bancarios " + keyBusiness.ToString() + " , " + keyPayroll.ToString() + " , '" + value + "' , '" + type + "'";
                }
                cmd.Parameters.Clear(); cmd.Dispose(); dataReader.Close();
            } catch (Exception exc) {
                layout.sMensajeError = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return layout;
        }

        public Boolean sp_Comprueba_Empleado_Empresa(int keyEmployee, int keyBusiness)
        {
            Boolean flag = false;
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Comprueba_Empleado_Empresa", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpleado", keyEmployee));
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read()) {
                    if (data["Validacion"].ToString() == "1") {
                        flag = true;
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                flag = false;
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return flag;
        }

        public LayoutSalarioMasivoBean sp_Actualiza_Salario_Carga_Masiva(int IdEmpresa, int IdEmpleado, int TipoMovimiento, string Salario, string FechaMovimiento, int UsuarioId, int PeriodoId, int Periodo, int Anio)
        {
            LayoutSalarioMasivoBean bean = new LayoutSalarioMasivoBean();
            bean.iBandera1 = 0;
            bean.iBandera2 = 0;
            bean.iBandera3 = 0;
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Actualiza_Salario_Carga_Masiva", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", IdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@IdEmpleado", IdEmpleado));
                cmd.Parameters.Add(new SqlParameter("@TipoMovimiento", TipoMovimiento));
                cmd.Parameters.Add(new SqlParameter("@Salario", Salario));
                cmd.Parameters.Add(new SqlParameter("@FechaMovimiento", FechaMovimiento));
                cmd.Parameters.Add(new SqlParameter("@UsuarioId", UsuarioId));
                cmd.Parameters.Add(new SqlParameter("@PeriodoId", PeriodoId));
                cmd.Parameters.Add(new SqlParameter("@Periodo", Periodo));
                cmd.Parameters.Add(new SqlParameter("@Anio", Anio));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read()) {
                    if (dataReader["Existe"].ToString() == "1") {
                        if (dataReader["Coincidencia"].ToString() == "1" && dataReader["Nomina"].ToString() == "1" &&
                        dataReader["Historial"].ToString() == "1") {
                            bean.sMensaje = "SUCCESS";
                            bean.iBanderaFecha = Convert.ToInt32(dataReader["Fecha"].ToString());
                        } else {
                            bean.sMensaje = (dataReader["Mensaje"].ToString() == "NONE") ? "ERROR" : dataReader["Mensaje"].ToString();
                        }
                    } else {
                        bean.sMensaje = "El empleado no existe para la empresa indicada";
                    }
                    if (dataReader["Coincidencia"].ToString() == "1") {
                        bean.iBandera1 = 1;
                    }
                    if (dataReader["Nomina"].ToString() == "1") {
                        bean.iBandera2 = 1;
                    }
                    if (dataReader["Historial"].ToString() == "1") {
                        bean.iBandera3 = 1;
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); dataReader.Close();
            } catch (Exception exc) {
                bean.sMensaje = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return bean;
        }

    }
}