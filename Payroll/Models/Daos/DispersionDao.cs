﻿using Payroll.Models.Beans;
using Payroll.Models.Utilerias;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Payroll.Models.Daos
{
    public class DispersionDao { }

    public class LoadTypePeriodPayrollDaoD : Conexion
    {
        public LoadTypePeriodPayrollBean sp_Load_Info_Periodo_Empr(int keyBusiness, int year)
        {
            LoadTypePeriodPayrollBean periodBean = new LoadTypePeriodPayrollBean();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Load_Info_Periodo_Empr", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@ctrlAnio", year));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read())
                {
                    periodBean.iEmpresa_id = Convert.ToInt32(data["Empresa_id"].ToString());
                    periodBean.iAnio = Convert.ToInt32(data["Anio"].ToString());
                    periodBean.iTipoPeriodo = Convert.ToInt32(data["Tipo_Periodo_id"].ToString());
                    periodBean.iPeriodo = Convert.ToInt32(data["Periodo"].ToString());
                    periodBean.sFechaInicio = data["Fecha_Inicio"].ToString();
                    periodBean.sFechaFinal  = data["Fecha_Final"].ToString();
                    periodBean.sMensaje = "success";
                }
                else
                {
                    periodBean.sMensaje = "NODATA";
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return periodBean;
        }
    }

    public class PayrollRetainedEmployeesDaoD : Conexion
    {
        public List<int> sp_Periodos_Retenidos_A_Empleados(int IdEmpresa, int Anio)
        {
            List<int> periodos = new List<int>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Periodos_Retenidos_A_Empleados", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", IdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@Anio", Anio));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        int periodo = Convert.ToInt32(data["Periodo"]);
                        periodos.Add(periodo);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return periodos;
        }

        public List<PayrollRetainedEmployeesBean> sp_Retrieve_NominasRetenidas(int keyBusiness)
        {
            List<PayrollRetainedEmployeesBean> listPayRetained = new List<PayrollRetainedEmployeesBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Retrieve_NominasRetenidas", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keyBusiness));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        PayrollRetainedEmployeesBean payRetained = new PayrollRetainedEmployeesBean();
                        payRetained.iIdNominaRetenida = Convert.ToInt32(data["IdNominaRetenida"].ToString());
                        payRetained.iIdEmpleado = Convert.ToInt32(data["Empleado_id"].ToString());
                        payRetained.sNombreEmpleado = Convert.ToInt32(data["Empleado_id"].ToString()).ToString() + " - " +
                                                data["Nombre_Empleado"].ToString() + " " +
                                                data["Apellido_Paterno_Empleado"].ToString() + " " +
                                                data["Apellido_Materno_Empleado"].ToString();
                        payRetained.sDescripcion = data["Descripcion"].ToString();
                        payRetained.iPeriodo = Convert.ToInt32(data["Periodo"]);
                        listPayRetained.Add(payRetained);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return listPayRetained;
        }

        public PayrollRetainedEmployeesBean sp_Insert_Empleado_Retenida_Nomina(int keyBusiness, int keyEmployee, int typePeriod, int periodPayroll, int yearRetained, string descriptionRetained, int keyUser)
        {
            PayrollRetainedEmployeesBean payRetEmployee = new PayrollRetainedEmployeesBean();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Insert_Empleado_Retenida_Nomina", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpleado", keyEmployee));
                cmd.Parameters.Add(new SqlParameter("@ctrlTipoPeriId", typePeriod));
                cmd.Parameters.Add(new SqlParameter("@ctrlPeriodo", periodPayroll));
                cmd.Parameters.Add(new SqlParameter("@ctrlAnio", yearRetained));
                cmd.Parameters.Add(new SqlParameter("@ctrlDescripcion", descriptionRetained));
                cmd.Parameters.Add(new SqlParameter("@ctrlUsuarioId", keyUser));
                if (cmd.ExecuteNonQuery() > 0)
                {
                    payRetEmployee.sMensaje = "success";
                }
                else
                {
                    payRetEmployee.sMensaje = "error";
                }
                cmd.Parameters.Clear(); cmd.Dispose();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return payRetEmployee;
        }

        public PayrollRetainedEmployeesBean sp_Update_Remove_Nomina_Retenida(int keyPayrollRetained)
        {
            PayrollRetainedEmployeesBean payRetEmployee = new PayrollRetainedEmployeesBean();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Update_Remove_Nomina_Retenida", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdNominaRetenida", keyPayrollRetained));
                if (cmd.ExecuteNonQuery() > 0)
                {
                    payRetEmployee.sMensaje = "success";
                }
                else
                {
                    payRetEmployee.sMensaje = "error";
                }
                cmd.Parameters.Clear(); cmd.Dispose();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return payRetEmployee;
        }

    }

    public class SearchEmployeePayRetainedDaoD : Conexion
    {

        public List<SearchEmployeePayRetainedBean> sp_SearchEmploye_Ret_Nomina(int keyBusiness, string search, string filter)
        {
            List<SearchEmployeePayRetainedBean> listEmployeePayRet = new List<SearchEmployeePayRetainedBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_SearchEmploye_Ret_Nomina", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@ctrlSearchEmp", search));
                cmd.Parameters.Add(new SqlParameter("@ctrlFiltered", filter));
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keyBusiness));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        SearchEmployeePayRetainedBean employee = new SearchEmployeePayRetainedBean();
                        employee.iIdEmpleado = Convert.ToInt32(data["IdEmpleado"].ToString());
                        employee.sNombreEmpleado = data["Nombre_Empleado"].ToString() + " " +
                                                data["Apellido_Paterno_Empleado"].ToString() + " " +
                                                data["Apellido_Materno_Empleado"].ToString();
                        employee.iTipoPeriodo = 3;
                        listEmployeePayRet.Add(employee);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return listEmployeePayRet;
        }

    }

    public class LoadTypePeriodDaoD : Conexion
    {

        public LoadTypePeriodBean sp_Load_Type_Period_Empresa(int keyBusiness, int year, int typePeriod)
        {
            LoadTypePeriodBean loadTypePerBean = new LoadTypePeriodBean();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Load_Type_Period_Empresa", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@ctrlAnio", year));
                cmd.Parameters.Add(new SqlParameter("@ctrlTipoPeriodo", typePeriod));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read())
                {
                    loadTypePerBean.iEmpresa_id = Convert.ToInt32(data["Empresa_id"].ToString());
                    loadTypePerBean.iAnio = Convert.ToInt32(data["Anio"].ToString());
                    loadTypePerBean.iTipoPeriodo = Convert.ToInt32(data["Tipo_Periodo_id"].ToString());
                    loadTypePerBean.iPeriodo = Convert.ToInt32(data["Periodo"].ToString());
                    loadTypePerBean.sFechaInicio = DateTime.Parse(data["Fecha_Inicio"].ToString()).ToString("yyyy-MM-dd");
                    loadTypePerBean.sFechaFinal = DateTime.Parse(data["Fecha_Final"].ToString()).ToString("yyyy-MM-dd");
                    loadTypePerBean.sMensaje = "success";
                }
                else
                {
                    loadTypePerBean.sMensaje = "NODATA";
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return loadTypePerBean;
        }

    }

    public class DataDispersionBusiness : Conexion
    {

        public decimal sp_Totales_Dispersion_Especial (int Anio, int Periodo, int TipoPeriodoId, int GrupoId, int ConfiguracionId)
        {
            decimal resultado = 0;
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Totales_Dispersion_Especial", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Anio", Anio));
                cmd.Parameters.Add(new SqlParameter("@Periodo", Periodo));
                cmd.Parameters.Add(new SqlParameter("@TipoPeriodoId", TipoPeriodoId));
                cmd.Parameters.Add(new SqlParameter("@GrupoId", GrupoId));
                cmd.Parameters.Add(new SqlParameter("@ConfiguracionId", ConfiguracionId));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read()) {
                    resultado = Convert.ToDecimal(dataReader["TOTAL"].ToString());
                }
                cmd.Parameters.Clear(); cmd.Dispose(); dataReader.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return resultado;
        }

        public double sp_Datos_Totales_Resta_Importe_Bancos_Dispersion(int IdEmpresa, int Periodo, int TipoPeriodo, int BancoId, int yearDispersion)
        {
            double result = 0;
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Datos_Totales_Resta_Importe_Bancos_Dispersion", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", IdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@Periodo", Periodo));
                cmd.Parameters.Add(new SqlParameter("@TipoPeriodo", TipoPeriodo));
                cmd.Parameters.Add(new SqlParameter("@BancoId", BancoId));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearDispersion));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read()) { 
                    result = Convert.ToDouble(dataReader["TotalVales"].ToString()); 
                }
                cmd.Parameters.Clear(); cmd.Dispose(); dataReader.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return result;
        }

        public double sp_Comprueba_Existencia_Renglon_Vales(int IdEmpresa, int IdEmpleado, int Periodo, int TipoPeriodoId, int Anio)
        {
            double result = 0;
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Comprueba_Existencia_Renglon_Vales", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", IdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@IdEmpleado", IdEmpleado));
                cmd.Parameters.Add(new SqlParameter("@Periodo", Periodo));
                cmd.Parameters.Add(new SqlParameter("@TipoPeriodoId", TipoPeriodoId));
                cmd.Parameters.Add(new SqlParameter("@Anio", Anio));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read()) {
                    if (dataReader["Bandera"].ToString() == "1") {
                        result = Convert.ToDouble(dataReader["Total"].ToString());
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); dataReader.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return result;
        }
        public List<GroupBusinessDispersionBean> sp_Load_Group_Business_Dispersion ()
        {
            List<GroupBusinessDispersionBean> groupBusinesses = new List<GroupBusinessDispersionBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Load_Group_Business_Dispersion", this.conexion) { CommandType = CommandType.StoredProcedure };
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.HasRows) {
                    while (dataReader.Read()) {
                        GroupBusinessDispersionBean group = new GroupBusinessDispersionBean();
                        group.iIdGrupoEmpresa = Convert.ToInt32(dataReader["IdGrupoEmpresa"]);
                        group.sNombreGrupo    = dataReader["Nombre_Grupo"].ToString();
                        groupBusinesses.Add(group);
                    }
                }
                cmd.Dispose();
                cmd.Parameters.Clear();
                dataReader.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return groupBusinesses;
        }

        public GroupBusinessDispersionBean sp_Save_New_Group_Business_Dispersion (string name, int user)
        {
            GroupBusinessDispersionBean groupBusiness = new GroupBusinessDispersionBean();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Save_New_Group_Business_Dispersion", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Nombre", name));
                cmd.Parameters.Add(new SqlParameter("@Usuario", user));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read()) {
                    if (dataReader["Bandera"].ToString() == "2") {
                        groupBusiness.sMensaje = "SUCCESS";
                    } else if (dataReader["Bandera"].ToString() == "1") {
                        groupBusiness.sMensaje = "EXISTS";
                    } else {
                        groupBusiness.sMensaje = "ERROR";
                    }
                } else {
                    groupBusiness.sMensaje = "ERROR";
                }
                cmd.Dispose();
                cmd.Parameters.Clear();
                dataReader.Close();
            } catch (Exception exc) {
                groupBusiness.sMensaje = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return groupBusiness;
        }

        public List<EmpresasBean> sp_Load_Business_Not_In_Groups_Dispersion()
        {
            List<EmpresasBean> empresasBeans = new List<EmpresasBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Load_Business_Not_In_Groups_Dispersion", this.conexion) { CommandType = CommandType.StoredProcedure };
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.HasRows) {
                    while (dataReader.Read()) {
                        EmpresasBean empresas = new EmpresasBean();
                        empresas.iIdEmpresa   = Convert.ToInt32(dataReader["IdEmpresa"]);
                        empresas.sNombreEmpresa = dataReader["NombreEmpresa"].ToString();
                        empresas.sRazonSocial   = dataReader["RazonSocial"].ToString();
                        empresasBeans.Add(empresas);
                    }
                }
                cmd.Dispose(); 
                cmd.Parameters.Clear();
                dataReader.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return empresasBeans;
        }

        public GroupBusinessDispersionBean sp_Save_Asign_Group_Business(int group, int business)
        {
            GroupBusinessDispersionBean groupBusiness = new GroupBusinessDispersionBean();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Save_Asign_Group_Business", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdGrupo", group));
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", business));
                if (cmd.ExecuteNonQuery() > 0) {
                    groupBusiness.sMensaje = "INSERT";
                } else {
                    groupBusiness.sMensaje = "NOTINSERT";
                }
                cmd.Dispose();
                cmd.Parameters.Clear();
            } catch (Exception exc) {
                groupBusiness.sMensaje = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return groupBusiness;
        }

        public List<EmpresasBean> sp_View_Business_Group_Dispersion(int keyGroup)
        {
            List<EmpresasBean> empresas = new List<EmpresasBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_View_Business_Group_Dispersion", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdGrupo",keyGroup));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.HasRows) {
                    while (dataReader.Read()) {
                        EmpresasBean bean    = new EmpresasBean();
                        bean.iIdDetalleGrupo = Convert.ToInt32(dataReader["IdDetalle"]);
                        bean.iIdEmpresa      = Convert.ToInt32(dataReader["IdEmpresa"]);
                        bean.sNombreEmpresa  = dataReader["NombreEmpresa"].ToString();
                        bean.sRazonSocial    = dataReader["RazonSocial"].ToString();
                        bean.fRfc            = dataReader["RFC"].ToString();
                        empresas.Add(bean);
                    }
                }
                cmd.Dispose();
                cmd.Parameters.Clear();
                dataReader.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return empresas;
        }

        public EmpresasBean sp_Remove_Business_Group (int keyBusinessGroup)
        {
            EmpresasBean empresas = new EmpresasBean();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Remove_Business_Group", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdDetalle", keyBusinessGroup));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read()) {
                    if (dataReader["Bandera"].ToString() == "1") {
                        empresas.sMensaje = "SUCCESS";
                    } else {
                        empresas.sMensaje = "NOTDELETE";
                    }
                } else {
                    empresas.sMensaje = "ERROR";
                }
                cmd.Parameters.Clear();
                cmd.Dispose();
                dataReader.Close();
            } catch (Exception exc) {
                empresas.sMensaje = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return empresas;
        }

        public List<BancosBean> sp_View_Banks_Group_Business_Dispersion(int keyGroup)
        {
            List<BancosBean> bancosBeans = new List<BancosBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_View_Banks_Group_Business_Dispersion", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdGrupo", keyGroup));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.HasRows) {
                    while (dataReader.Read()) {
                        BancosBean bancos   = new BancosBean();
                        bancos.iIdBanco     = Convert.ToInt32(dataReader["IdBanco"]);
                        bancos.sNombreBanco = dataReader["Descripcion"].ToString();
                        bancosBeans.Add(bancos);
                    }
                }
                cmd.Dispose(); cmd.Parameters.Clear(); dataReader.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return bancosBeans;
        }

        public BancosBean sp_Save_Banks_Group_Interbank (int keyGroup, int user, List<BankInt> bankInts, string type)
        {
            BancosBean bancos = new BancosBean();
            int quantity = 0;
            int longList = bankInts.Count;
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Save_Banks_Group_Interbank", this.conexion) { CommandType = CommandType.StoredProcedure };
                foreach (BankInt bank in bankInts) {
                    cmd.Parameters.Add(new SqlParameter("@Grupo_id", keyGroup));
                    cmd.Parameters.Add(new SqlParameter("@Nombre", bank.sNombre));
                    cmd.Parameters.Add(new SqlParameter("@Banco_id", bank.iBanco));
                    cmd.Parameters.Add(new SqlParameter("@Activo", bank.iActivo));
                    cmd.Parameters.Add(new SqlParameter("@Tipo", type));
                    cmd.Parameters.Add(new SqlParameter("@Usuario_id", user));
                    if (cmd.ExecuteNonQuery() > 0) {
                        quantity += 1;
                    }
                    cmd.Parameters.Clear(); cmd.Dispose();
                }
                if (quantity == longList) {
                    bancos.sMensaje = "SUCCESS";
                }
            } catch (Exception exc) {
                bancos.sMensaje = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return bancos;
        }

        public List<BankInt> sp_View_Config_Banks(int keyGroup, string type, string option)
        {
            List<BankInt> bankInts = new List<BankInt>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_View_Config_Banks", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Grupo_id", keyGroup));
                cmd.Parameters.Add(new SqlParameter("@Tipo", type));
                cmd.Parameters.Add(new SqlParameter("@Opcion", option));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.HasRows) {
                    while (dataReader.Read()) {
                        BankInt bank = new BankInt();
                        bank.iIdConfiguracion = Convert.ToInt32(dataReader["IdConfiguracion"]);
                        bank.iGrupoId = Convert.ToInt32(dataReader["Grupo_id"]);
                        bank.sNombre  = dataReader["Nombre"].ToString();
                        bank.iBanco   = Convert.ToInt32(dataReader["Banco_id"]);
                        bank.iActivo  = Convert.ToInt32(dataReader["Activo"]);
                        bank.sTipo    = dataReader["Tipo"].ToString();
                        bankInts.Add(bank);
                    }
                }
                cmd.Dispose(); cmd.Parameters.Clear(); dataReader.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return bankInts;
        }

        public List<BancosBean> sp_View_Banks_Available_Dispersion (int keyGroup, string type, string option, int keyConfig)
        {
            List<BancosBean> bancosBeans = new List<BancosBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_View_Banks_Available_Dispersion", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Grupo_id", keyGroup));
                cmd.Parameters.Add(new SqlParameter("@Tipo", type));
                cmd.Parameters.Add(new SqlParameter("@Opcion", option));
                cmd.Parameters.Add(new SqlParameter("@Configuracion", keyConfig));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.HasRows) {
                    while (dataReader.Read()) {
                        BancosBean bancos     = new BancosBean();
                        bancos.iIdBanco       = Convert.ToInt32(dataReader["IdBanco"]);
                        bancos.sNombreBanco   = dataReader["Descripcion"].ToString();
                        bancos.iConfiguracion = Convert.ToInt32(dataReader["IdConfiguracionDetalle"]);
                        bancos.iGrupoId       = Convert.ToInt32(dataReader["Grupo_id"]);
                        bancosBeans.Add(bancos);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); dataReader.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return bancosBeans;
        }

        public BancosBean sp_Save_Config_Details_Banks(int keyGroup, string type, int keyConfig, int bankId, int keyUser)
        {
            BancosBean bancosBean = new BancosBean();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Save_Config_Details_Banks", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@GrupoId", keyGroup));
                cmd.Parameters.Add(new SqlParameter("@ConfiguracionId", keyConfig));
                cmd.Parameters.Add(new SqlParameter("@BancoId", bankId));
                cmd.Parameters.Add(new SqlParameter("@TipoDispersion", type));
                cmd.Parameters.Add(new SqlParameter("@UsuarioId", keyUser));
                if (cmd.ExecuteNonQuery() > 0) {
                    bancosBean.sMensaje = "success";
                } else {
                    bancosBean.sMensaje = "error";
                }
                cmd.Parameters.Clear(); cmd.Dispose();
            } catch (Exception exc) {
                bancosBean.sMensaje = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return bancosBean;
        }

        public BancosBean sp_Remove_Bank_Details(int keyConfigDetail, int keyConfig)
        {
            BancosBean bancosBean = new BancosBean();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Remove_Bank_Details", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdConfiguracionDetalle", keyConfigDetail));
                cmd.Parameters.Add(new SqlParameter("@ConfiguracionId", keyConfig));
                if (cmd.ExecuteNonQuery() > 0) {
                    bancosBean.sMensaje = "success";
                } else {
                    bancosBean.sMensaje = "error";
                }
                cmd.Parameters.Clear(); cmd.Dispose();
            } catch (Exception exc) {
                bancosBean.sMensaje = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return bancosBean;
        }

        public DatosCuentaClienteBancoEmpresaBean sp_View_Config_Data_Account_Bank(int keyConfig)
        {
            DatosCuentaClienteBancoEmpresaBean datosCuenta = new DatosCuentaClienteBancoEmpresaBean();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_View_Config_Data_Account_Bank", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdConfiguracion", keyConfig));
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read()) {
                    if (dataReader["Bandera"].ToString() == "1") {
                        datosCuenta.sNumeroCliente = dataReader["NCliente"].ToString();
                        datosCuenta.sNumeroCuenta  = dataReader["NCuenta"].ToString();
                        datosCuenta.sClabe         = dataReader["NClabe"].ToString();
                        datosCuenta.iPlaza         = (dataReader["Plaza"].ToString() != "") ? Convert.ToInt32(dataReader["Plaza"]) : 0;
                        datosCuenta.sRFC           = dataReader["RFC"].ToString();
                        datosCuenta.sMensaje       = "SUCCESS";
                    } else {
                        datosCuenta.sMensaje = "NOTFOUND";
                    }
                } else {
                    datosCuenta.sMensaje = "ERROR";
                }
                cmd.Parameters.Clear(); cmd.Dispose(); dataReader.Close();
            } catch (Exception exc) {
                datosCuenta.sMensaje = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosCuenta;
        }
        public DatosCuentaClienteBancoEmpresaBean sp_Save_Config_Data_Account_Bank(string nClient, string nAccount, string nClabe, string nSquare, int keyConfig, string rfc)
        {
            DatosCuentaClienteBancoEmpresaBean datosCuenta = new DatosCuentaClienteBancoEmpresaBean();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Save_Config_Data_Account_Bank", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@Cliente", nClient));
                cmd.Parameters.Add(new SqlParameter("@Cuenta", nAccount));
                cmd.Parameters.Add(new SqlParameter("@Clabe", nClabe));
                cmd.Parameters.Add(new SqlParameter("@Plaza", nSquare));
                cmd.Parameters.Add(new SqlParameter("@IdConfiguracion", keyConfig));
                cmd.Parameters.Add(new SqlParameter("@RFC", rfc));
                if (cmd.ExecuteNonQuery() > 0) {
                    datosCuenta.sMensaje = "SUCCESS";
                } else {
                    datosCuenta.sMensaje = "ERROR";
                }
                cmd.Parameters.Clear(); cmd.Dispose();
            } catch (Exception exc) {
                datosCuenta.sMensaje = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosCuenta;
        }

        public List<DataDepositsBankingBean> sp_Obtiene_Depositos_Bancarios_Especial(int keyGroup, int yearDispersion, int typePeriod, int period, string type)
        {
            List<DataDepositsBankingBean> dataDeposits = new List<DataDepositsBankingBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Obtiene_Depositos_Bancarios_Especial", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdGrupo", keyGroup));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearDispersion));
                cmd.Parameters.Add(new SqlParameter("@Periodo", period));
                cmd.Parameters.Add(new SqlParameter("@IdPeriodo", typePeriod));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        dataDeposits.Add(new DataDepositsBankingBean {
                            iIdEmpresa = keyGroup,
                            iIdBanco   = Convert.ToInt32(data["banco"].ToString()),
                            iIdRenglon = Convert.ToInt32(data["Renglon_id"].ToString()),
                            iDepositos = Convert.ToInt32(data["depositos"].ToString()),
                            sImporte   = string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal((data["importe"])))
                        });
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return dataDeposits;
        }

        public List<DataDepositsBankingBean> sp_Depositos_Nominas_Retenidas(int keyBusiness, int yearDispersion, int typePeriodDisp, int periodDispersion, string type)
        {
            List<DataDepositsBankingBean> list = new List<DataDepositsBankingBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Depositos_Nominas_Retenidas", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Periodo", periodDispersion));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearDispersion));
                cmd.Parameters.Add(new SqlParameter("@TipoPeriodo", typePeriodDisp));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        DataDepositsBankingBean bean = new DataDepositsBankingBean();
                        bean.iIdBanco = Convert.ToInt32(data["IdBanco"]);
                        bean.sBanco   = data["Descripcion"].ToString();
                        bean.iDepositos = Convert.ToInt32(data["Depositos"]);
                        bean.sImporte   = string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal((data["Saldo"])));
                        list.Add(bean);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return list;
        }

        public List<DataDepositsBankingBean> sp_Obtiene_Depositos_Bancarios(int keyBusiness, int yearDispersion, int typePeriodDisp, int periodDispersion, string type)
        {
            List<DataDepositsBankingBean> listDaDepBankingBean = new List<DataDepositsBankingBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Obtiene_Depositos_Bancarios", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@AnioAct", yearDispersion));
                cmd.Parameters.Add(new SqlParameter("@Periodo", periodDispersion));
                cmd.Parameters.Add(new SqlParameter("@IdPeriodo", typePeriodDisp));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        listDaDepBankingBean.Add(new DataDepositsBankingBean
                        {
                            iIdEmpresa = keyBusiness,
                            iIdBanco = Convert.ToInt32(data["banco"].ToString()),
                            iIdRenglon = Convert.ToInt32(data["Renglon_id"].ToString()),
                            iDepositos = Convert.ToInt32(data["depositos"].ToString()),
                            sImporte = string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal((data["importe"]))),
                            dImporteSF = Convert.ToDouble(data["importe"])
                        });
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return listDaDepBankingBean;
        }

        public List<BankDetailsBean> sp_Datos_Banco(List<DataDepositsBankingBean> listData)
        {
            List<BankDetailsBean> listBankDetailsBean = new List<BankDetailsBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Datos_Banco", this.conexion) { CommandType = CommandType.StoredProcedure };
                foreach (DataDepositsBankingBean data in listData)
                {
                    cmd.Parameters.Add(new SqlParameter("@IdBanco", Convert.ToInt32(data.iIdBanco.ToString())));
                    SqlDataReader dataBank = cmd.ExecuteReader();
                    if (dataBank.Read())
                    {
                        listBankDetailsBean.Add(new BankDetailsBean
                        {
                            iIdBanco = Convert.ToInt32(dataBank["IdBanco"].ToString()),
                            sNombreBanco = dataBank["Descripcion"].ToString(),
                            sSufijo = (Convert.ToInt32(data.iIdBanco.ToString()) != 0) ? "Nómina" : "Efectivo"
                        });
                    }
                    cmd.Parameters.Clear(); cmd.Dispose(); dataBank.Close();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return listBankDetailsBean;
        }

        public DatosEmpresaBeanDispersion sp_Datos_Empresa_Dispersion(int keyBusiness, int type)
        {
            DatosEmpresaBeanDispersion datosEmpresaBeanDispersion = new DatosEmpresaBeanDispersion();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Datos_Empresa_Dispersion", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Tipo", type));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read()) {
                    datosEmpresaBeanDispersion.sNombreEmpresa = data["NombreEmpresa"].ToString();
                    datosEmpresaBeanDispersion.sCalle         = data["Calle"].ToString();
                    datosEmpresaBeanDispersion.sColonia       = data["Colonia"].ToString();
                    datosEmpresaBeanDispersion.sCodigoPostal  = data["CodigoPostal"].ToString();
                    datosEmpresaBeanDispersion.sCiudad        = data["Ciudad"].ToString();
                    datosEmpresaBeanDispersion.sRfc           = data["RFC"].ToString();
                    datosEmpresaBeanDispersion.iRegimen_Fiscal_id = Convert.ToInt32(data["Regimen_Fiscal_id"].ToString());
                    datosEmpresaBeanDispersion.sDelegacion    = data["Delegacion"].ToString();
                    datosEmpresaBeanDispersion.iBanco_id      = Convert.ToInt32(data["Banco_id"].ToString());
                    datosEmpresaBeanDispersion.sDescripcion   = data["Descripcion"].ToString();
                    datosEmpresaBeanDispersion.sMensaje       = "SUCCESS";
                } else {
                    datosEmpresaBeanDispersion.sMensaje = "NOTDATA";
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
                datosEmpresaBeanDispersion.sMensaje = exc.Message.ToString();
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosEmpresaBeanDispersion;
        }

        public DatosEmpresaBeanDispersion sp_Datos_Empresa_Dispersion_Grupos(int keyGroup)
        {
            DatosEmpresaBeanDispersion datosEmpresaBeanDispersion = new DatosEmpresaBeanDispersion();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Datos_Empresa_Dispersion_Grupos", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdGrupo", keyGroup));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read())
                {
                    datosEmpresaBeanDispersion.sNombreEmpresa = data["NombreEmpresa"].ToString();
                    datosEmpresaBeanDispersion.sCalle = data["Calle"].ToString();
                    datosEmpresaBeanDispersion.sColonia = data["Colonia"].ToString();
                    datosEmpresaBeanDispersion.sCodigoPostal = data["CodigoPostal"].ToString();
                    datosEmpresaBeanDispersion.sCiudad = data["Ciudad"].ToString();
                    datosEmpresaBeanDispersion.sRfc = data["RFC"].ToString();
                    datosEmpresaBeanDispersion.iRegimen_Fiscal_id = Convert.ToInt32(data["Regimen_Fiscal_id"].ToString());
                    datosEmpresaBeanDispersion.sDelegacion = data["Delegacion"].ToString();
                    datosEmpresaBeanDispersion.iBanco_id = Convert.ToInt32(data["Banco_id"].ToString());
                    datosEmpresaBeanDispersion.sDescripcion = data["Descripcion"].ToString();
                    datosEmpresaBeanDispersion.sMensaje = "SUCCESS";
                }
                else
                {
                    datosEmpresaBeanDispersion.sMensaje = "NOTDATA";
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
                datosEmpresaBeanDispersion.sMensaje = exc.Message.ToString();
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosEmpresaBeanDispersion;
        }

        public List<DatosDepositosBancariosBean> sp_Procesa_Cheques_Total_Nomina(int keyBusiness, int typePeriod, int numberPeriod, int yearPeriod)
        {
            List<DatosDepositosBancariosBean> datosDepositosBancariosBean = new List<DatosDepositosBancariosBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Procesa_Cheques_Total_Nomina", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Tipo_periodo_id", typePeriod));
                cmd.Parameters.Add(new SqlParameter("@Periodo", numberPeriod));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearPeriod));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        DatosDepositosBancariosBean bancariosBean = new DatosDepositosBancariosBean();
                        bancariosBean.iIdBanco   = Convert.ToInt32(data["Banco"].ToString());
                        bancariosBean.iCantidad  = Convert.ToInt32(data["Cantidad"].ToString());
                        bancariosBean.sImporte   = data["Importe"].ToString().Replace(",","");
                        datosDepositosBancariosBean.Add(bancariosBean);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosDepositosBancariosBean;
        }

        public List<DatosDepositosBancariosBean> sp_Procesa_Cheques_Total_Nomina_Espejo(int keyBusiness, int typePeriod, int numberPeriod, int yearPeriod)
        {
            List<DatosDepositosBancariosBean> datosDepositosBancariosBean = new List<DatosDepositosBancariosBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Procesa_Cheques_Total_Nomina_Espejo", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Tipo_periodo_id", typePeriod));
                cmd.Parameters.Add(new SqlParameter("@Periodo", numberPeriod));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearPeriod));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        DatosDepositosBancariosBean bancariosBean = new DatosDepositosBancariosBean();
                        bancariosBean.iIdBanco  = Convert.ToInt32(data["Banco"].ToString());
                        bancariosBean.iCantidad = Convert.ToInt32(data["Cantidad"].ToString());
                        bancariosBean.sImporte  = data["Importe"].ToString().Replace(",", "");
                        datosDepositosBancariosBean.Add(bancariosBean);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosDepositosBancariosBean;
        }

        public List<DatosProcesaChequesNominaBean> sp_Procesa_Cheques_Nomina(int keyBusiness, int typePeriod, int numberPeriod, int yearPeriod)
        {
            List<DatosProcesaChequesNominaBean> datosProcesaChequesNominaBeans = new List<DatosProcesaChequesNominaBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Procesa_Cheques_Nomina", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Tipo_periodo_id", typePeriod));
                cmd.Parameters.Add(new SqlParameter("@Periodo", numberPeriod));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearPeriod));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        DatosProcesaChequesNominaBean datosProcesaCheques = new DatosProcesaChequesNominaBean();
                        datosProcesaCheques.iIdBanco   = Convert.ToInt32(data["IdBanco"].ToString());
                        datosProcesaCheques.sBanco     = data["Banco"].ToString();
                        datosProcesaCheques.iIdEmpresa = Convert.ToInt32(data["Empresa"].ToString());
                        datosProcesaCheques.sNomina    = data["Nomina"].ToString();
                        datosProcesaCheques.sCuenta    = data["Cuenta"].ToString();
                        datosProcesaCheques.sImporte   = data["Saldo"].ToString();
                        datosProcesaCheques.dImporte   = Convert.ToDecimal(data["Importe"].ToString());
                        datosProcesaCheques.doImporte  = Convert.ToDouble(data["ImporteDo"]);
                        datosProcesaCheques.sNombre    = data["Nombre"].ToString();
                        datosProcesaCheques.sPaterno   = data["Paterno"].ToString();
                        datosProcesaCheques.sMaterno   = data["Materno"].ToString();
                        datosProcesaCheques.sRfc       = data["RFC"].ToString();
                        datosProcesaCheques.iTipoPago  = Convert.ToInt32(data["TipoPago"].ToString());
                        datosProcesaChequesNominaBeans.Add(datosProcesaCheques);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosProcesaChequesNominaBeans;
        }

        public List<DatosProcesaChequesNominaBean> sp_Procesa_Cheques_Nomina_Espejo(int keyBusiness, int typePeriod, int numberPeriod, int yearPeriod)
        {
            List<DatosProcesaChequesNominaBean> datosProcesaChequesNominaBeans = new List<DatosProcesaChequesNominaBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Procesa_Cheques_Nomina_Espejo", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Tipo_periodo_id", typePeriod));
                cmd.Parameters.Add(new SqlParameter("@Periodo", numberPeriod));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearPeriod));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        DatosProcesaChequesNominaBean datosProcesaCheques = new DatosProcesaChequesNominaBean();
                        datosProcesaCheques.iIdBanco = Convert.ToInt32(data["IdBanco"].ToString());
                        datosProcesaCheques.sBanco = data["Banco"].ToString();
                        datosProcesaCheques.iIdEmpresa = Convert.ToInt32(data["Empresa"].ToString());
                        datosProcesaCheques.sNomina = data["Nomina"].ToString();
                        datosProcesaCheques.sCuenta = data["Cuenta"].ToString();
                        datosProcesaCheques.dImporte = Convert.ToDecimal(data["Importe"].ToString());
                        datosProcesaCheques.doImporte = Convert.ToDouble(data["ImporteDo"]);
                        datosProcesaCheques.sNombre = data["Nombre"].ToString();
                        datosProcesaCheques.sPaterno = data["Paterno"].ToString();
                        datosProcesaCheques.sMaterno = data["Materno"].ToString();
                        datosProcesaCheques.sRfc = data["RFC"].ToString();
                        datosProcesaCheques.iTipoPago = Convert.ToInt32(data["TipoPago"].ToString());
                        datosProcesaChequesNominaBeans.Add(datosProcesaCheques);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosProcesaChequesNominaBeans;
        }

        public DatosCuentaClienteBancoEmpresaBean sp_Cuenta_Cliente_Banco_Empresa(int keyBusiness, int bankResult)
        {
            DatosCuentaClienteBancoEmpresaBean datosCuentaCliente = new DatosCuentaClienteBancoEmpresaBean();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Cuenta_Cliente_Banco_Empresa", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@IdBanco", bankResult));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read()) {
                    datosCuentaCliente.sNumeroCliente = data["NumeroCliente"].ToString();
                    datosCuentaCliente.sNumeroCuenta  = data["NumeroCuenta"].ToString();
                    datosCuentaCliente.sVacio         = data["Vacio"].ToString();
                    datosCuentaCliente.iPlaza   = Convert.ToInt32(data["Plaza"].ToString());
                    datosCuentaCliente.sClabe   = data["Clabe"].ToString();
                    datosCuentaCliente.iTipo    = Convert.ToInt32(data["Tipo"].ToString());
                    datosCuentaCliente.iCodigo  = Convert.ToInt32(data["Codigo"].ToString());
                    datosCuentaCliente.sMensaje = "SUCCESS";
                } else {
                    datosCuentaCliente.sMensaje = "NOTDATA";
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosCuentaCliente;
        }

        // INTERBANCARIOS

        public List<DatosDepositosBancariosBean> sp_Procesa_Cheques_Total_Interbancarios(int keyBusiness, int typePeriod, int numberPeriod, int yearPeriod)
        {
            List<DatosDepositosBancariosBean> datosDepositosBancariosBean = new List<DatosDepositosBancariosBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Procesa_Cheques_Total_Interbancarios", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Tipo_periodo_id", typePeriod));
                cmd.Parameters.Add(new SqlParameter("@Periodo", numberPeriod));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearPeriod));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        DatosDepositosBancariosBean bancariosBean = new DatosDepositosBancariosBean();
                        bancariosBean.iCantidad = Convert.ToInt32(data["Cantidad"].ToString());
                        bancariosBean.sImporte  = data["Importe"].ToString().Replace(",", "");
                        datosDepositosBancariosBean.Add(bancariosBean);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosDepositosBancariosBean;
        }

        public List<DatosDepositosBancariosBean> sp_Procesa_Cheques_Total_Interbancarios_Espejo(int keyBusiness, int typePeriod, int numberPeriod, int yearPeriod)
        {
            List<DatosDepositosBancariosBean> datosDepositosBancarios = new List<DatosDepositosBancariosBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Procesa_Cheques_Total_Interbancarios_Espejo", this.conexion)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Tipo_periodo_id", typePeriod));
                cmd.Parameters.Add(new SqlParameter("@Periodo", numberPeriod));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearPeriod));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        DatosDepositosBancariosBean bancariosBean = new DatosDepositosBancariosBean();
                        bancariosBean.iCantidad = Convert.ToInt32(data["Cantidad"].ToString());
                        bancariosBean.sImporte = data["Importe"].ToString().Replace(",", "");
                        datosDepositosBancarios.Add(bancariosBean);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosDepositosBancarios;
        }

        public List<DatosProcesaChequesNominaBean> sp_Procesa_Cheques_Interbancarios(int keyBusiness, int typePeriod, int numberPeriod, int yearPeriod)
        {
            List<DatosProcesaChequesNominaBean> datosProcesaChequesNominaBeans = new List<DatosProcesaChequesNominaBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Procesa_Cheques_Interbancarios", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Tipo_periodo_id", typePeriod));
                cmd.Parameters.Add(new SqlParameter("@Periodo", numberPeriod));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearPeriod));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        DatosProcesaChequesNominaBean datosProcesaCheques = new DatosProcesaChequesNominaBean();
                        datosProcesaCheques.iIdBanco = Convert.ToInt32(data["IdBanco"].ToString());
                        datosProcesaCheques.sBanco = data["Banco"].ToString();
                        datosProcesaCheques.sCodigo = data["Codigo"].ToString();
                        datosProcesaCheques.iIdEmpresa = Convert.ToInt32(data["Empresa"].ToString());
                        datosProcesaCheques.sNomina = data["Nomina"].ToString();
                        datosProcesaCheques.sCuenta = data["Cuenta"].ToString();
                        datosProcesaCheques.dImporte = Convert.ToDecimal(data["Importe"].ToString());
                        datosProcesaCheques.doImporte = Convert.ToDouble(data["ImporteDo"]);
                        datosProcesaCheques.sNombre = data["Nombre"].ToString();
                        datosProcesaCheques.sPaterno = data["Paterno"].ToString();
                        datosProcesaCheques.sMaterno = data["Materno"].ToString();
                        datosProcesaCheques.sRfc = data["RFC"].ToString();
                        datosProcesaCheques.iTipoPago = Convert.ToInt32(data["TipoPago"].ToString());
                        datosProcesaCheques.sImporte = data["Saldo"].ToString();
                        datosProcesaChequesNominaBeans.Add(datosProcesaCheques);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosProcesaChequesNominaBeans;
        }

        public List<DatosProcesaChequesNominaBean> sp_Procesa_Cheques_Interbancarios_Espejo(int keyBusiness, int typePeriod, int numberPeriod, int yearPeriod)
        {
            List<DatosProcesaChequesNominaBean> datosProcesaChequesNominaBeans = new List<DatosProcesaChequesNominaBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Procesa_Cheques_Interbancarios_Espejo", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Tipo_periodo_id", typePeriod));
                cmd.Parameters.Add(new SqlParameter("@Periodo", numberPeriod));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearPeriod));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        DatosProcesaChequesNominaBean datosProcesaCheques = new DatosProcesaChequesNominaBean();
                        datosProcesaCheques.iIdBanco = Convert.ToInt32(data["IdBanco"].ToString());
                        datosProcesaCheques.sBanco = data["Banco"].ToString();
                        datosProcesaCheques.iIdEmpresa = Convert.ToInt32(data["Empresa"].ToString());
                        datosProcesaCheques.sNomina = data["Nomina"].ToString();
                        datosProcesaCheques.sCuenta = data["Cuenta"].ToString();
                        datosProcesaCheques.dImporte = Convert.ToDecimal(data["Importe"].ToString());
                        datosProcesaCheques.sNombre = data["Nombre"].ToString();
                        datosProcesaCheques.sPaterno = data["Paterno"].ToString();
                        datosProcesaCheques.sMaterno = data["Materno"].ToString();
                        datosProcesaCheques.sRfc = data["RFC"].ToString();
                        datosProcesaCheques.iTipoPago = Convert.ToInt32(data["TipoPago"].ToString());
                        datosProcesaChequesNominaBeans.Add(datosProcesaCheques);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosProcesaChequesNominaBeans;
        }

    }

    public class DataDispersionGroups : Conexion
    {

        public List<DataDepositsBankingBean> sp_Obtiene_Depositos_GruposBancarios(int keyBusiness, int yearDispersion, int periodDispersion, int typePeriodDisp)
        {
            List<DataDepositsBankingBean> dataDeposits = new List<DataDepositsBankingBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Obtiene_Depositos_GruposBancarios", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearDispersion));
                cmd.Parameters.Add(new SqlParameter("@Periodo", periodDispersion));
                cmd.Parameters.Add(new SqlParameter("@TipoPeriodo", typePeriodDisp));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        dataDeposits.Add(new DataDepositsBankingBean
                        {
                            iIdEmpresa = keyBusiness,
                            iIdBanco = Convert.ToInt32(data["banco"].ToString()),
                            sBanco = data["NombreBanco"].ToString(),
                            iIdRenglon = Convert.ToInt32(data["Renglon_id"].ToString()),
                            iDepositos = Convert.ToInt32(data["depositos"].ToString()),
                            sImporte = string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal((data["importe"]))),
                            dImporteSF = Convert.ToDouble(data["importe"])
                        });
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return dataDeposits;
        }

        public bool sp_Inserta_Banco_GrupoDispersion(int keyBusiness, int keyBankBusiness, int keyBank, int status)
        {
            bool save = false;
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Inserta_Banco_GrupoDispersion", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@BancoEmpresa", keyBankBusiness));
                cmd.Parameters.Add(new SqlParameter("@Banco", keyBank));
                cmd.Parameters.Add(new SqlParameter("@Estatus", status));
                if (cmd.ExecuteNonQuery() > 0) {
                    save = true;
                }
                cmd.Parameters.Clear(); cmd.Dispose();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return save;
        }

        public bool sp_Elimina_Configuraciones_GrupoDispersion(int keyBusiness)
        {
            bool delete = false;
            try {
                this.Conectar();
                SqlCommand command = new SqlCommand("sp_Elimina_Configuraciones_GrupoDispersion", this.conexion) { CommandType = CommandType.StoredProcedure };
                command.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                if (command.ExecuteNonQuery() > 0) {
                    delete = true;
                }
                command.Parameters.Clear(); command.Dispose();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return delete;
        }

        public List<DataDepositsBankingBean> sp_Detalles_Configuracion_DispersionGrupos(int keyBusiness, int yearDispersion, int periodDispersion, int typePeriodDisp, int keyBankBusiness)
        {
            List<DataDepositsBankingBean> dataDeposits = new List<DataDepositsBankingBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Detalles_Configuracion_DispersionGrupos", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Anio", yearDispersion));
                cmd.Parameters.Add(new SqlParameter("@Periodo", periodDispersion));
                cmd.Parameters.Add(new SqlParameter("@TipoPeriodo", typePeriodDisp));
                cmd.Parameters.Add(new SqlParameter("@BancoEmpresa", keyBankBusiness));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        dataDeposits.Add(new DataDepositsBankingBean {
                            iIdEmpresa = keyBusiness,
                            iIdBanco = Convert.ToInt32(data["banco"].ToString()),
                            sBanco = data["NombreBanco"].ToString(),
                            iIdRenglon = Convert.ToInt32(data["Renglon_id"].ToString()),
                            iDepositos = Convert.ToInt32(data["depositos"].ToString()),
                            sImporte = string.Format(CultureInfo.InvariantCulture, "{0:#,###,##0.00}", Convert.ToDecimal((data["importe"]))),
                            dImporteSF = Convert.ToDouble(data["importe"])
                        });
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return dataDeposits;
        }

        public DatosDepositosBancariosBean sp_Procesa_Cheques_Total_DispersionGrupos(int keyBusiness, int year, int period, int typePeriod, int keyBankBusiness)
        {
            DatosDepositosBancariosBean datosDepositos = new DatosDepositosBancariosBean();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Procesa_Cheques_Total_DispersionGrupos", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Anio", year));
                cmd.Parameters.Add(new SqlParameter("@Periodo", period));
                cmd.Parameters.Add(new SqlParameter("@TipoPeriodo", typePeriod));
                cmd.Parameters.Add(new SqlParameter("@BancoEmpresa", keyBankBusiness));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read()) {
                    if (data["Cantidad"].ToString() != "") {
                        datosDepositos.iCantidad = Convert.ToInt32(data["Cantidad"]);
                        datosDepositos.sImporte  = data["Importe"].ToString();
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datosDepositos;
        }

        public List<DatosProcesaChequesNominaBean> sp_Obtiene_Depositos_Bancarios_DispersionGrupos(int keyBusiness, int year, int period, int typePeriod, int keyBankBusiness)
        {
            List<DatosProcesaChequesNominaBean> datos = new List<DatosProcesaChequesNominaBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Obtiene_Depositos_Bancarios_DispersionGrupos", this.conexion) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@IdEmpresa", keyBusiness));
                cmd.Parameters.Add(new SqlParameter("@Anio", year));
                cmd.Parameters.Add(new SqlParameter("@Periodo", period));
                cmd.Parameters.Add(new SqlParameter("@TipoPeriodo", typePeriod));
                cmd.Parameters.Add(new SqlParameter("@BancoEmpresa", keyBankBusiness));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        DatosProcesaChequesNominaBean datosProcesaCheques = new DatosProcesaChequesNominaBean();
                        datosProcesaCheques.iIdBanco = Convert.ToInt32(data["IdBanco"].ToString());
                        datosProcesaCheques.sBanco = data["Banco"].ToString();
                        //datosProcesaCheques.sCodigo = data["Codigo"].ToString();
                        datosProcesaCheques.sCodigo = data["Codigo_Dispersion"].ToString();
                        datosProcesaCheques.iIdEmpresa = Convert.ToInt32(data["Empresa"].ToString());
                        datosProcesaCheques.sNomina = data["Nomina"].ToString();
                        datosProcesaCheques.sCuenta = data["Cuenta"].ToString();
                        datosProcesaCheques.sImporte = data["Saldo"].ToString();
                        datosProcesaCheques.dImporte = Convert.ToDecimal(data["Importe"].ToString());
                        datosProcesaCheques.doImporte = Convert.ToDouble(data["ImporteDo"]);
                        datosProcesaCheques.sNombre = data["Nombre"].ToString();
                        datosProcesaCheques.sPaterno = data["Paterno"].ToString();
                        datosProcesaCheques.sMaterno = data["Materno"].ToString();
                        datosProcesaCheques.sRfc = data["RFC"].ToString();
                        datosProcesaCheques.iTipoPago = Convert.ToInt32(data["TipoPago"].ToString());
                        datos.Add(datosProcesaCheques);
                    }
                }
                cmd.Parameters.Clear(); cmd.Dispose(); data.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return datos;
        }

    }
}