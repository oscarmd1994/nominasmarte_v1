﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using System.Xml;
using Payroll.Models.Beans;
using Payroll.Models.Utilerias;

namespace Payroll.Models.Daos
{
    public class ListTablesDao { }


    public class ListEmpleadosDao : Conexion
    {

        public List<EmpleadosBean> Sp_Empleados_Retrieve_Search_Empleados(int keyemp, string wordsearch, string filtered)
        {
            List<EmpleadosBean> listEmpleadosBean = new List<EmpleadosBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Empleados_Retrieve_Search_Empleados", this.conexion) {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keyemp));
                cmd.Parameters.Add(new SqlParameter("@ctrlWordSearch", wordsearch));
                cmd.Parameters.Add(new SqlParameter("@ctrlFiltered", filtered));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        EmpleadosBean empleadoBean = new EmpleadosBean();
                        empleadoBean.iIdEmpleado = Convert.ToInt32(data["IdEmpleado"].ToString());
                        //empleadoBean.sNombreEmpleado = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(data["Nombre_Empleado"].ToString() + " " + data["Apellido_Paterno_Empleado"].ToString() + " " + data["Apellido_Materno_Empleado"].ToString());
                        empleadoBean.sNombreEmpleado = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(data["Apellido_Paterno_Empleado"].ToString() + " " + data["Apellido_Materno_Empleado"].ToString() + " " + data["Nombre_Empleado"].ToString());
                        empleadoBean.iNumeroNomina = Convert.ToInt32(data["IdEmpleado"].ToString());
                        listEmpleadosBean.Add(empleadoBean);
                    }
                }
                cmd.Dispose(); cmd.Parameters.Clear(); data.Close(); conexion.Close();
            } catch (Exception exc) {
                string origenerror = "ListTablesdao";
                string mensajeerror = exc.ToString();
                CapturaErroresBean capturaErrorBean = new CapturaErroresBean();
                CapturaErrores capturaErrorDao = new CapturaErrores();
                capturaErrorBean = capturaErrorDao.sp_Errores_Insert_Errores(origenerror, mensajeerror);
                Console.WriteLine(exc);
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return listEmpleadosBean;
        }
        
        public List<EmpleadosBean> sp_Empleados_Retrieve_Search_Empleados_Baja(int keyemp, string wordsearch, string filtered)
        {
            List<EmpleadosBean> empleadosBeans = new List<EmpleadosBean>();
            try {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Empleados_Retrieve_Search_Empleados_Baja", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keyemp));
                cmd.Parameters.Add(new SqlParameter("@ctrlWordSearch", wordsearch));
                cmd.Parameters.Add(new SqlParameter("@ctrlFiltered", filtered));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows) {
                    while (data.Read()) {
                        EmpleadosBean empleadoBean = new EmpleadosBean();
                        empleadoBean.iIdEmpleado = Convert.ToInt32(data["IdEmpleado"].ToString());
                        //empleadoBean.sNombreEmpleado = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(data["Nombre_Empleado"].ToString() + " " + data["Apellido_Paterno_Empleado"].ToString() + " " + data["Apellido_Materno_Empleado"].ToString());
                        empleadoBean.sNombreEmpleado = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(data["Apellido_Paterno_Empleado"].ToString() + " " + data["Apellido_Materno_Empleado"].ToString() + " " + data["Nombre_Empleado"].ToString());
                        empleadoBean.iNumeroNomina = Convert.ToInt32(data["IdEmpleado"].ToString());
                        empleadosBeans.Add(empleadoBean);
                    }
                }
                cmd.Dispose(); cmd.Parameters.Clear(); data.Close(); conexion.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return empleadosBeans;
        }
        
        public List<EmpleadosBean> sp_Empleados_Retrieve_Empleados(int keyemp)
        {
            List<EmpleadosBean> listEmpleadosBean = new List<EmpleadosBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Empleados_Retrieve_Empleados", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keyemp));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        EmpleadosBean empleadoBean = new EmpleadosBean();
                        empleadoBean.iIdEmpleado = Convert.ToInt32(data["IdEmpleado"].ToString());
                        empleadoBean.sNombreEmpleado = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(data["NombreEmpleado"].ToString() + " " + data["ApellidoPaternoEmpleado"].ToString() + " " + data["ApellidoMaternoEmpleado"].ToString());
                        empleadoBean.iNumeroNomina = Convert.ToInt32(data["NumeroNomina"].ToString());
                        listEmpleadosBean.Add(empleadoBean);
                    }
                }
                cmd.Dispose(); cmd.Parameters.Clear(); data.Close(); conexion.Close();
            }
            catch (Exception exc)
            {
                string origenerror = "ListTablesdao";
                string mensajeerror = exc.ToString();
                CapturaErroresBean capturaErrorBean = new CapturaErroresBean();
                CapturaErrores capturaErrorDao = new CapturaErrores();
                capturaErrorBean = capturaErrorDao.sp_Errores_Insert_Errores(origenerror, mensajeerror);
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return listEmpleadosBean;
        }
        public EmpleadosBean sp_Empleados_Retrieve_Empleado(int keyemploye, int keybusiness)
        {
            EmpleadosBean empleadoBean = new EmpleadosBean();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Empleados_Retrieve_Empleado", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpleado", keyemploye));
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keybusiness));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read())
                {
                    empleadoBean.iIdEmpleado = Convert.ToInt32(data["IdEmpleado"].ToString());
                    empleadoBean.sNombreEmpleado = (String.IsNullOrEmpty(data["Nombre_Empleado"].ToString())) ? "" : data["Nombre_Empleado"].ToString();
                    empleadoBean.sApellidoPaterno = (String.IsNullOrEmpty(data["Apellido_Paterno_Empleado"].ToString())) ? "" : data["Apellido_Paterno_Empleado"].ToString();
                    empleadoBean.sApellidoMaterno = (String.IsNullOrEmpty(data["Apellido_Materno_Empleado"].ToString())) ? "" : data["Apellido_Materno_Empleado"].ToString();
                    empleadoBean.sFechaNacimiento = (String.IsNullOrEmpty(data["Fecha_Nacimiento_Empleado"].ToString())) ? "" : DateTime.Parse(data["Fecha_Nacimiento_Empleado"].ToString(), CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd");
                    empleadoBean.sLugarNacimiento = (String.IsNullOrEmpty(data["Lugar_Nacimiento_Empleado"].ToString())) ? "" : data["Lugar_Nacimiento_Empleado"].ToString();
                    if (data["Cg_Titulo_id"].ToString().Length != 0)
                    {
                        empleadoBean.iTitulo_id = Convert.ToInt32(data["Cg_Titulo_id"].ToString());
                    }
                    else
                    {
                        empleadoBean.iTitulo_id = 0;
                    }
                    if (data["Cg_Genero_Empleado_id"].ToString().Length != 0)
                    {
                        empleadoBean.iGeneroEmpleado = Convert.ToInt32(data["Cg_Genero_Empleado_id"].ToString());
                    }
                    else
                    {
                        empleadoBean.iGeneroEmpleado = 0;
                    }
                    if (data["Nacionalidad_id"].ToString().Length != 0)
                    {
                        empleadoBean.iNacionalidad = Convert.ToInt32(data["Nacionalidad_id"].ToString());
                    }
                    else
                    {
                        empleadoBean.iNacionalidad = 0;
                    }
                    if (data["Cg_EstadoCivil_Empleado_id"].ToString().Length != 0)
                    {
                        empleadoBean.iEstadoCivil = Convert.ToInt32(data["Cg_EstadoCivil_Empleado_id"].ToString());
                    }
                    else
                    {
                        empleadoBean.iEstadoCivil = 0;
                    }
                    empleadoBean.sCodigoPostal = (String.IsNullOrEmpty(data["Codigo_Postal"].ToString())) ? "" : data["Codigo_Postal"].ToString();
                    if (data["Cg_Estado_id"].ToString().Length != 0)
                    {
                        empleadoBean.iEstado_id = Convert.ToInt32(data["Cg_Estado_id"].ToString());
                    }
                    else
                    {
                        empleadoBean.iEstado_id = 0;
                    }
                    empleadoBean.sCiudad = (String.IsNullOrEmpty(data["Ciudad"].ToString())) ? "" : data["Ciudad"].ToString();
                    empleadoBean.sColonia = (String.IsNullOrEmpty(data["Colonia"].ToString())) ? "" : data["Colonia"].ToString();
                    empleadoBean.sCalle = (String.IsNullOrEmpty(data["Calle"].ToString())) ? "" : data["Calle"].ToString();
                    empleadoBean.sNumeroCalle = (String.IsNullOrEmpty(data["Numero_Calle"].ToString())) ? "S/N" : data["Numero_Calle"].ToString();
                    empleadoBean.sTelefonoFijo = (String.IsNullOrEmpty(data["Telefono_Fijo"].ToString())) ? "" : data["Telefono_Fijo"].ToString();
                    empleadoBean.sTelefonoMovil = (String.IsNullOrEmpty(data["Telefono_Movil"].ToString())) ? "" : data["Telefono_Movil"].ToString();
                    empleadoBean.sCorreoElectronico = (String.IsNullOrEmpty(data["Correo_Electronico"].ToString())) ? "" : data["Correo_Electronico"].ToString();
                    empleadoBean.sFechaMatrimonio = (String.IsNullOrEmpty(data["Fecha_Matrimonio"].ToString())) ? "" : DateTime.Parse(data["Fecha_Matrimonio"].ToString()).ToString("yyyy-MM-dd");
                    empleadoBean.sTipoSangre = (String.IsNullOrEmpty(data["Tipo_Sangre"].ToString())) ? "" : data["Tipo_Sangre"].ToString();
                    empleadoBean.sMensaje = "success";
                }
                else
                {
                    empleadoBean.sMensaje = "ERRDB";
                }
                cmd.Dispose(); cmd.Parameters.Clear(); data.Close(); conexion.Close();
            }
            catch (Exception exc)
            {
                empleadoBean.sMensaje = exc.Message.ToString();
                string origenerror = "ListTablesdao";
                string mensajeerror = exc.ToString();
                CapturaErroresBean capturaErrorBean = new CapturaErroresBean();
                CapturaErrores capturaErrorDao = new CapturaErrores();
                capturaErrorBean = capturaErrorDao.sp_Errores_Insert_Errores(origenerror, mensajeerror);
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return empleadoBean;
        }
        public ImssBean sp_Imss_Retrieve_ImssEmpleado(int keyemploye, int keyemp)
        {
            ImssBean imssBean = new ImssBean();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Imss_Retrieve_ImssEmpleado", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpleado", keyemploye));
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keyemp));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read()) {
                    imssBean.iIdImss = Convert.ToInt32(data["IdImss"].ToString());
                    imssBean.iEmpleado_id = Convert.ToInt32(data["Empleado_id"].ToString());
                    imssBean.iEmpresa_id = Convert.ToInt32(data["Empresa_id"].ToString());
                    // (String.IsNullOrEmpty(data["Effdt"].ToString())) ? "" : data["Effdt"].ToString();
                    if (data["Effdt"].ToString() != "") {
                        //imssBean.sFechaEfectiva = DateTime.Parse(data["Effdt"].ToString()).ToString("yyyy-MM-dd");
                        imssBean.sFechaEfectiva = DateTime.Parse(data["Effdt"].ToString(), CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd");
                    } else {
                        imssBean.sFechaEfectiva = "";
                    }
                    imssBean.sRegistroImss = (String.IsNullOrEmpty(data["RegistroImss"].ToString())) ? "" : data["RegistroImss"].ToString();
                    imssBean.sRfc = (String.IsNullOrEmpty(data["RFC"].ToString())) ? "" : data["RFC"].ToString();
                    imssBean.sCurp = (String.IsNullOrEmpty(data["CURP"].ToString())) ? "" : data["CURP"].ToString();
                    if (data["Cg_NivelEstudio_id"].ToString().Length != 0) {
                        imssBean.iNivelEstudio_id = Convert.ToInt32(data["Cg_NivelEstudio_id"].ToString());
                    } else {
                        imssBean.iNivelEstudio_id = 0;
                    }
                    if (data["Cg_NivelSocioeconomico_id"].ToString().Length != 0) {
                        imssBean.iNivelSocioeconomico_id = Convert.ToInt32(data["Cg_NivelSocioeconomico_id"].ToString());
                    } else {
                        imssBean.iNivelSocioeconomico_id = 0;
                    }
                    imssBean.sMensaje = "success";
                }
                else {
                    imssBean.sMensaje = "ERRDB";
                }
                cmd.Dispose(); cmd.Parameters.Clear(); data.Close(); conexion.Close();
            } catch (Exception exc) {
                imssBean.sMensaje = exc.Message.ToString();
                string origenerror = "ListTablesdao";
                string mensajeerror = exc.ToString();
                CapturaErroresBean capturaErrorBean = new CapturaErroresBean();
                CapturaErrores capturaErrorDao = new CapturaErrores();
                capturaErrorBean = capturaErrorDao.sp_Errores_Insert_Errores(origenerror, mensajeerror);
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return imssBean;
        }
        public DatosNominaBean sp_Nominas_Retrieve_NominaEmpleado(int keyemploye, int keyemp)
        {
            DatosNominaBean nominaBean = new DatosNominaBean();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Nominas_Retrieve_NominaEmpleado", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpleado", keyemploye));
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keyemp));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read())
                {
                    nominaBean.iIdNomina = Convert.ToInt32(data["IdNomina"].ToString());
                    nominaBean.iEmpleado_id = Convert.ToInt32(data["Empleado_id"].ToString());
                    nominaBean.iEmpresa_id = Convert.ToInt32(data["Empresa_id"].ToString());
                    if (data["Effdt"].ToString() != "") {
                        //nominaBean.sFechaEfectiva = DateTime.Parse(data["Effdt"].ToString()).ToString("yyyy-MM-dd");
                        nominaBean.sFechaEfectiva = DateTime.Parse(data["Effdt"].ToString(), CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd");
                    } else {
                        nominaBean.sFechaEfectiva = "";
                    }
                    if (data["Tipo_Periodo"].ToString().Length != 0) {
                        nominaBean.iTipoPeriodo = Convert.ToInt32(data["Tipo_Periodo"].ToString());
                    } else {
                        nominaBean.iTipoPeriodo = 0;
                    }
                    nominaBean.dSalarioMensual = Convert.ToDouble(data["SalarioMensual"].ToString());
                    if (data["Cg_TipoEmpleado_id"].ToString().Length != 0) {
                        nominaBean.iTipoEmpleado_id = Convert.ToInt32(data["Cg_TipoEmpleado_id"].ToString());
                    } else {
                        nominaBean.iTipoEmpleado_id = 0;
                    }
                    if (data["Cg_NivelEmpleado_id"].ToString().Length != 0) {
                        nominaBean.iNivelEmpleado_id = Convert.ToInt32(data["Cg_NivelEmpleado_id"].ToString());
                    } else {
                        nominaBean.iNivelEmpleado_id = 0;
                    }
                    if (data["Cg_TipoJornada_id"].ToString().Length != 0) {
                        nominaBean.iTipoJornada_id = Convert.ToInt32(data["Cg_TipoJornada_id"].ToString());
                    } else {
                        nominaBean.iTipoJornada_id = 0;
                    }
                    if (data["Cg_TipoContrato_id"].ToString().Length != 0) {
                        nominaBean.iTipoContrato_id = Convert.ToInt32(data["Cg_TipoContrato_id"].ToString());
                    } else {
                        nominaBean.iTipoContrato_id = 0;
                    }
                    if (data["Cg_TipoContratacion_id"].ToString().Length != 0) {
                        nominaBean.iTipoContratacion_id = Convert.ToInt32(data["Cg_TipoContratacion_id"].ToString());
                    } else {
                        nominaBean.iTipoContratacion_id = 0;
                    }
                    if (data["Cg_MotivoIncremento_id"].ToString().Length != 0) {
                        nominaBean.iMotivoIncremento_id = Convert.ToInt32(data["Cg_MotivoIncremento_id"].ToString());
                    } else {
                        nominaBean.iMotivoIncremento_id = 0;
                    }
                    nominaBean.sFechaIngreso = DateTime.Parse(data["FechaIngreso"].ToString(), CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd");
                    nominaBean.sFechaAntiguedad = DateTime.Parse(data["FechaAntiguedad"].ToString(), CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd");
                    if (data["VencimientoContrato"].ToString() != "") {
                        nominaBean.sVencimientoContrato = DateTime.Parse(data["VencimientoContrato"].ToString(), CultureInfo.CreateSpecificCulture("es-MX")).ToString("yyyy-MM-dd");
                    } else {
                        nominaBean.sVencimientoContrato = "";
                    }
                    if (data["Posicion_id"].ToString().Length != 0)
                    {
                        nominaBean.iPosicion_id = Convert.ToInt32(data["Posicion_id"].ToString());
                    }
                    else
                    {
                        nominaBean.iPosicion_id = 0;
                    }
                    if (data["Cg_tipoPago_id"].ToString().Length != 0)
                    {
                        nominaBean.iTipoPago_id = Convert.ToInt32(data["Cg_tipoPago_id"].ToString());
                    }
                    else
                    {
                        nominaBean.iTipoPago_id = 0;
                    }
                    if (data["Banco_id"].ToString().Length != 0)
                    {
                        nominaBean.iBanco_id = Convert.ToInt32(data["Banco_id"].ToString());
                    }
                    else
                    {
                        nominaBean.iBanco_id = 0;
                    }
                    if (data["Cg_Tipo_sueldo"].ToString().Length != 0) {
                        nominaBean.iTipoSueldo_id = Convert.ToInt32(data["Cg_Tipo_sueldo"].ToString());
                    } else {
                        nominaBean.iTipoSueldo_id = 0;
                    }
                    if (data["Politica"].ToString().Length != 0) {
                        nominaBean.iPolitica = Convert.ToInt32(data["Politica"]);
                    } else {
                        nominaBean.iPolitica = 0;
                    }
                    if (data["DiferenciaP"].ToString().Length != 0) {
                        nominaBean.dDiferencia = Convert.ToDouble(data["DiferenciaP"]);
                    } else {
                        nominaBean.dDiferencia = 0.00;
                    }
                    if (data["Transporte"].ToString().Length != 0) {
                        nominaBean.dTransporte = Convert.ToDouble(data["Transporte"]);
                    } else {
                        nominaBean.dTransporte = 0.00;
                    }
                    if (data["Retroactivo"].ToString().Length != 0) {
                        if (data["Retroactivo"].ToString() == "False") {
                            nominaBean.iRetroactivo = 0;
                        } else {
                            nominaBean.iRetroactivo = 1;
                        }
                    } else {
                        nominaBean.iRetroactivo = 0;
                    }

                    if (data["Con_fondo_ahorro"].ToString().Length != 0)
                    {
                        if (data["Con_fondo_ahorro"].ToString() == "False")
                        {
                            nominaBean.iConFondo = 0;
                        }
                        else
                        {
                            nominaBean.iConFondo = 1;
                        }
                    }
                    else
                    {
                        nominaBean.iConFondo = 0;
                    }

                    if (data["Cg_categoria_id"].ToString().Length != 0) {
                        nominaBean.iCategoriaId = Convert.ToInt32(data["Cg_categoria_id"]);
                    } else {
                        nominaBean.iCategoriaId = 0;
                    }
                    if (data["Cg_pago_por"].ToString().Length != 0) {
                        nominaBean.iPagoPor = Convert.ToInt32(data["Cg_pago_por"]);
                    } else {
                        nominaBean.iPagoPor = 0;
                    }
                    if (data["Cg_Clasif"].ToString().Length != 0) {
                        nominaBean.iClasif = Convert.ToInt32(data["Cg_Clasif"]);
                    } else {
                        nominaBean.iClasif = 368;
                    }
                    nominaBean.sUlt_sdi = data["Ult_sdi"].ToString();
                    nominaBean.sCuentaCheques = (String.IsNullOrEmpty(data["Cta_Cheques"].ToString())) ? "" : data["Cta_Cheques"].ToString();
                    nominaBean.iUsuarioAlta_id = Convert.ToInt32(data["Usuario_Alta_id"].ToString());
                    nominaBean.sFechaAlta = data["Fecha_Alta"].ToString();
                    if (data["Prestaciones"].ToString().Length != 0 && data["Prestaciones"].ToString() != "")
                    {
                        nominaBean.sPrestaciones = data["Prestaciones"].ToString();
                        //nominaBean.iPrestaciones = Convert.ToInt32(data["Prestaciones"].ToString());
                    } else {
                        nominaBean.sPrestaciones = "False";
                    }
                    if (data["ComplementoEspecial"].ToString().Length != 0 && data["ComplementoEspecial"].ToString() != "") {
                        nominaBean.dComplementoEspecial = Convert.ToDouble(data["ComplementoEspecial"].ToString());
                    } else {
                        nominaBean.dComplementoEspecial = 0;
                    }
                    nominaBean.sEstatus = data["Estatus"].ToString();
                    nominaBean.sValor   = data["Valor"].ToString();
                    nominaBean.sMensaje = "success";
                }
                else
                {
                    nominaBean.sMensaje = "ERRDB";
                }
                cmd.Dispose(); cmd.Parameters.Clear(); data.Close(); conexion.Close();
            }
            catch (Exception exc)
            {
                nominaBean.sMensaje = exc.Message.ToString();
                string origenerror = "ListTablesdao";
                string mensajeerror = exc.ToString();
                CapturaErroresBean capturaErrorBean = new CapturaErroresBean();
                CapturaErrores capturaErrorDao = new CapturaErrores();
                capturaErrorBean = capturaErrorDao.sp_Errores_Insert_Errores(origenerror, mensajeerror);
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return nominaBean;
        }
        public DatosPosicionesBean sp_Posiciones_Retrieve_PosicionEmpleado(int keyemploye, int keyemp)
        {
            DatosPosicionesBean posicionBean = new DatosPosicionesBean();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Posiciones_Retrieve_PosicionEmpleado", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpleado", keyemploye));
                cmd.Parameters.Add(new SqlParameter("@ctrlIdEmpresa", keyemp));
                SqlDataReader data = cmd.ExecuteReader();
                if (data.Read())
                {
                    posicionBean.iIdPosicion      = (data["IdPosicion"].ToString().Length != 0) ? Convert.ToInt32(data["IdPosicion"].ToString()) : 0;
                    posicionBean.sPosicionCodigo  = (data["PosCode1"].ToString().Length != 0) ? data["PosCode1"].ToString() : "" ;
                    posicionBean.sFechaEffectiva  = (data["Effdt"].ToString().Length != 0) ? DateTime.Parse(data["Effdt"].ToString()).ToString("yyyy-MM-dd") : "";
                    posicionBean.iPuesto_id       = (data["Puesto_id"].ToString().Length != 0) ? Convert.ToInt32(data["Puesto_id"].ToString()) : 0;
                    posicionBean.sNombrePuesto    = (data["NombrePuesto"].ToString().Length != 0) ? data["NombrePuesto"].ToString() : "";
                    posicionBean.sPuestoCodigo    = (data["PuestoCodigo"].ToString().Length != 0) ? data["PuestoCodigo"].ToString() : "";
                    posicionBean.iDepartamento_id = (data["Departamento_id"].ToString().Length != 0) ? Convert.ToInt32(data["Departamento_id"].ToString()) : 0;
                    posicionBean.sDeptoCodigo     = (data["Depto_Codigo"].ToString().Length != 0) ? data["Depto_Codigo"].ToString() : "";
                    posicionBean.sNombreDepartamento = (data["DescripcionDepartamento"].ToString().Length != 0) ? data["DescripcionDepartamento"].ToString() : "";
                    posicionBean.iIdLocalidad = (data["Localidad_id"].ToString().Length != 0) ? Convert.ToInt32(data["Localidad_id"].ToString()) : 0;
                    posicionBean.sLocalidad   = (data["Descripcion"].ToString().Length != 0) ? data["Descripcion"].ToString() : "";
                    posicionBean.iIdReportaAPosicion = (data["Reporta_A_Posicion_id"].ToString().Length != 0) ? Convert.ToInt32(data["Reporta_A_Posicion_id"].ToString()) : 0;
                    posicionBean.iIdRegistroPat = (data["RegPat_id"].ToString().Length != 0) ? Convert.ToInt32(data["RegPat_id"].ToString()) : 0;
                    posicionBean.sRegistroPat   = (data["Afiliacion_IMSS"].ToString().Length != 0) ? data["Afiliacion_IMSS"].ToString() : "";
                    posicionBean.iIdReportaAEmpresa = (data["Reporta_A_Empresa"].ToString().Length != 0) ? Convert.ToInt32(data["Reporta_A_Empresa"].ToString()) : 0;
                    posicionBean.sNombreEmrpesaRepo = (data["NombreEmpresa"].ToString().Length != 0) ? data["NombreEmpresa"].ToString() : "";
                    posicionBean.sMensaje = "success";
                }
                else
                {
                    posicionBean.sMensaje = "error";
                }
                cmd.Dispose(); cmd.Parameters.Clear(); data.Close(); conexion.Close();
            }
            catch (Exception exc)
            {
                string origenerror = "ListTablesdao";
                string mensajeerror = exc.ToString();
                CapturaErroresBean capturaErrorBean = new CapturaErroresBean();
                CapturaErrores capturaErrorDao = new CapturaErrores();
                capturaErrorBean = capturaErrorDao.sp_Errores_Insert_Errores(origenerror, mensajeerror);
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return posicionBean;
        }
        public List<EmpleadosEmpresaBean> sp_EmpleadosDEmpresa_Retrieve_EmpleadosDEmpresa(int CtrliIdEmpresa,int CtrliTipoPeriodo,int CtrliPeriodo,int Ctrlianio)
        {
            List<EmpleadosEmpresaBean> list = new List<EmpleadosEmpresaBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_EmpleadosDEmpresa_Retrieve_EmpleadosDEmpresa", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpresa", CtrliIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@Ctrlianio", Ctrlianio));
                cmd.Parameters.Add(new SqlParameter("@CtrliPeriodo", CtrliPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliTipoPeriodo", CtrliTipoPeriodo));
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        EmpleadosEmpresaBean ls = new EmpleadosEmpresaBean();

                        ls.iIdEmpleado = int.Parse(data["IdEmpleado"].ToString());
                        ls.sNombreCompleto = data["Nombre_Empleado"].ToString();
                        list.Add(ls);
                    }
                }
                else
                {
                    list = null;
                }
                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            return list;
        }
        public List<EmisorReceptorBean> sp_EmisorReceptor_Retrieve_EmisorReceptor(int CrtliIdEmpresa, int CrtliIdEmpleado)
        {
            List<EmisorReceptorBean> list = new List<EmisorReceptorBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_EmisorReceptor_Retrieve_EmisorReceptor", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CrtliIdEmpresa", CrtliIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@CrtliIdEmpleado", CrtliIdEmpleado));

                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        EmisorReceptorBean ls = new EmisorReceptorBean();
                        ls.sMensaje = "success";
                        if (data["RazonSocial"].ToString() == null)
                        {
                            ls.sNombreEmpresa = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sNombreEmpresa = data["RazonSocial"].ToString();
                        }
                        if (data["Calle"].ToString() == null)
                        {
                            ls.sCalle = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCalle = data["Calle"].ToString();
                        }
                        if (data["Colonia"].ToString() == null)
                        {
                            ls.sColonia = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sColonia = data["Colonia"].ToString();
                        }
                        if (data["CodigoPostal"].ToString() == null)
                        {
                            ls.iCP = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iCP = Convert.ToInt32(data["CodigoPostal"].ToString());
                        }
                        if (data["Ciudad"].ToString() == null)
                        {
                            ls.sCiudad = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCiudad = data["Ciudad"].ToString();
                        }
                        if (data["RFC"].ToString() == null)
                        {
                            ls.sRFC = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sRFC = data["RFC"].ToString();
                        }
                        if (data["Representante_legal"].ToString() == null)
                        {
                            ls.sRepresentanteLegal = " ";
                            ls.sMensaje = "error";
                        }   
                        else
                        {
                            ls.sRepresentanteLegal = data["Representante_legal"].ToString();
                        }
                        if (data["Afiliacion_IMSS"].ToString() == null)
                        {
                            ls.sAfiliacionIMSS = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sAfiliacionIMSS = data["Afiliacion_IMSS"].ToString();
                        }
                        if (data["FechaAntiguedad"].ToString() == null)
                        {
                            ls.sFechaAntiguedad = "TempleadosNomina";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sFechaAntiguedad = data["FechaAntiguedad"].ToString();
                        }
                        if (data["Fecha_baja"].ToString() == null)
                        {
                            ls.sFechaBajaEmple = " ";

                        }
                        else
                        {
                            ls.sFechaBajaEmple = data["Fecha_baja"].ToString();
                        }
                        if (data["Lugar_Nacimiento_Empleado"].ToString() == null)
                        {
                            ls.sLugarNacimiento = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sLugarNacimiento = data["Lugar_Nacimiento_Empleado"].ToString();
                        }
                        if (data["Fecha_Nacimiento_Empleado"].ToString() == null)
                        {
                            ls.sFechaNacimiento = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sFechaNacimiento = data["Fecha_Nacimiento_Empleado"].ToString();
                        }
                        if (data["NombreComp"].ToString() == null)
                        {
                            ls.sNombreComp = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sNombreComp = data["NombreComp"].ToString();
                            ls.sNombreemple = data["Nombre_Empleado"].ToString();
                            ls.sApellPatemple = data["Apellido_Paterno_Empleado"].ToString();
                            ls.sApellMatemple = data["Apellido_Materno_Empleado"].ToString();
                        }
                        if (data["Domicilio"].ToString() == null)
                        {
                            ls.sDomiciolioEmple = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sDomiciolioEmple = data["Domicilio"].ToString();
                        }
                        if (data["SEXO"].ToString() == null)
                        {
                            ls.sSexo = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            if (data["SEXO"].ToString()== "FEMENINO") {
                                ls.sSexo ="F" ;
                            }


                            if (data["SEXO"].ToString() == "MASCULINO")
                            {
                                ls.sSexo = "M";
                            }

                        }
                        if (data["ESTADO_CIVIL"].ToString() == null)
                        {
                            ls.sEstadoCivil = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            if (data["ESTADO_CIVIL"].ToString() == "SOLTERO") {
                                ls.sEstadoCivil= "S" ; 
                            }
                            if (data["ESTADO_CIVIL"].ToString() == "CASADO")
                            {
                                ls.sEstadoCivil = "C";
                            }
                            if (data["ESTADO_CIVIL"].ToString() == "DIVORCIADO")
                            {
                                ls.sEstadoCivil = "D";
                            }
                            if (data["ESTADO_CIVIL"].ToString() == "UNION LIBRE")
                            {
                                ls.sEstadoCivil = "U";
                            }
                            if (data["ESTADO_CIVIL"].ToString() == "VIUDO")
                            {
                                ls.sEstadoCivil = "V";
                            }

                        }
                        if (data["Ciud"].ToString() == null)
                        {
                            ls.sCiudadEmple = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCiudadEmple = data["Ciud"].ToString();
                        }
                        if (data["RFCEmpleado"].ToString() == null)
                        {
                            ls.sRFCEmpleado = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sRFCEmpleado = data["RFCEmpleado"].ToString();
                        }
                        if (data["IdEmpleado"].ToString() == null)
                        {
                            ls.iIdEmpleado = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iIdEmpleado = int.Parse(data["IdEmpleado"].ToString());
                        }
                        if (data["DescripcionDepartamento"].ToString() == null)
                        {
                            ls.sDescripcionDepartamento = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sDescripcionDepartamento = data["DescripcionDepartamento"].ToString();
                        }
                        if (data["Localidad"].ToString() == null)
                        {
                            ls.sLocalidademple = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sLocalidademple = data["Localidad"].ToString();
                        }
                        if (data["NombrePuesto"].ToString() == null)
                        {
                            ls.sNombrePuesto = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sNombrePuesto = data["NombrePuesto"].ToString();
                        }
                        if (data["FechaIngreso"].ToString() == null)
                        {
                             ls.sFechaIngreso = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sFechaIngreso = data["FechaIngreso"].ToString();
                        }
                        if (data["TipoContrato"].ToString() == null)
                        {
                            ls.sTipoContrato = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sTipoContrato = data["TipoContrato"].ToString();
                        }
                        if (data["CentroCosto"].ToString() == null)
                        {
                            ls.sCentroCosto = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCentroCosto = data["CentroCosto"].ToString();
                        }
                        if (data["SalarioMensual"].ToString() == null)
                        {
                            ls.dSalarioMensual = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.dSalarioMensual = decimal.Parse(data["SalarioMensual"].ToString());
                        }
                        if (data["RegistroImss"].ToString() == null)
                        {
                            ls.sRegistroImss = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sRegistroImss = data["RegistroImss"].ToString();
                        }
                        if (data["CURP"].ToString() == null)
                        {
                            ls.sCURP = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCURP = data["CURP"].ToString(); ;
                        }
                        if (data["Descripcion"].ToString() == null)
                        {
                            ls.sDescripcion = "CBanco";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sDescripcion = data["Descripcion"].ToString();
                        }
                        if (data["codigo"].ToString() == null)
                        {
                        }
                        else {
                            ls.sCodiBanco = data["codigo"].ToString();
                        }
                        if (data["Cta_Cheques"].ToString() == null)
                        {
                            ls.sCtaCheques = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCtaCheques = data["Cta_Cheques"].ToString();
                        }
                        if (data["Regimen_Fiscal_id"].ToString() == null)
                        {
                            ls.iRegimenFiscal = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iRegimenFiscal = int.Parse(data["Regimen_Fiscal_id"].ToString());
                        }
                        if (data["IdNomina"].ToString() == null)
                        {
                            ls.iIdNomina = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iIdNomina = int.Parse(data["IdNomina"].ToString());
                        }
                        if (data["Ult_sdi"].ToString() == null)
                        {
                            ls.SDINT = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.SDINT = data["Ult_sdi"].ToString();
                        }
                        if (data["ClasesRegPat_id"].ToString() == null)
                        {
                            ls.sRiesgoTrabajo = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sRiesgoTrabajo = data["ClasesRegPat_id"].ToString();
                        }
                        if (data["GrupoEmpresa_Id"].ToString() == null)
                        {
                            ls.GrupoEmpresas = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.GrupoEmpresas = int.Parse(data["GrupoEmpresa_Id"].ToString());
                        }
                        if (data["iTipoJor"].ToString() == null)
                        {
                            ls.iTipoJordana = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iTipoJordana = int.Parse(data["iTipoJor"].ToString());
                        }
                        if (data["sTipoJor"].ToString() == null)
                        {
                            ls.sTipoJornada = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sTipoJornada = data["sTipoJor"].ToString();
                        }
                        if (data["Cg_TipoEmpleado_id"].ToString() == null)
                        {
                            ls.iCgTipoEmpleadoId = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iCgTipoEmpleadoId = int.Parse(data["Cg_TipoEmpleado_id"].ToString());
                        }
                        if (data["ClaveEnt"].ToString() == null)
                        {
                            ls.sClaveEnt = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sClaveEnt = data["ClaveEnt"].ToString();
                        }
                        if (data["Cg_tipoPago_id"].ToString() == null)
                        {
                            ls.iCgTipoPago = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iCgTipoPago =int.Parse(data["Cg_tipoPago_id"].ToString());
                        }
                        if (data["Cg_pago_por"].ToString() == null)
                        {
                            ls.iPagopor = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iPagopor = int.Parse(data["Cg_pago_por"].ToString());
                        }
                        if (data["Ult_sdi"].ToString() == null)
                        {
                            ls.dSalarioInt = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.dSalarioInt =decimal.Parse(data["Ult_sdi"].ToString());
                        }


                        list.Add(ls);
                    }
                }
                else
                {
                    list = null;
                }
                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return list;
        }
        public List<CInicioFechasPeriodoBean> sp_DatosPerido_Retrieve_DatosPerido(int CtrliIdPeriodo)
        {
            List<CInicioFechasPeriodoBean> list = new List<CInicioFechasPeriodoBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_DatosPerido_Retrieve_DatosPerido", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliIdPeriodo", CtrliIdPeriodo));
                //cmd.Parameters.Add(new SqlParameter("@CtrliAnio", CtrliAnio));
                //cmd.Parameters.Add(new SqlParameter("@CtrliTipoPereriodo", CtrliTipoPereriodo));
                //cmd.Parameters.Add(new SqlParameter("@CtrliPeriodo", CtrliPeriodo));
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        CInicioFechasPeriodoBean LP = new CInicioFechasPeriodoBean();
                        {
                            LP.iId = int.Parse(data["Id"].ToString());
                            LP.sNominaCerrada = data["Nomina_Cerrada"].ToString();
                            LP.sFechaInicio = data["Fecha_Inicio"].ToString();
                            LP.sFechaFinal = data["Fecha_Final"].ToString();
                            LP.sFechaProceso = data["Fecha_Proceso"].ToString();
                            LP.sFechaPago = data["Fecha_Pago"].ToString();
                            LP.iDiasEfectivos = int.Parse(data["Dias_Efectivos"].ToString());
                            LP.iPeriodo = int.Parse(data["Periodo"].ToString());
                        };

                        list.Add(LP);
                    }
                }
                else
                {
                    list = null;
                }

                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return list;

        }
        public List<XMLBean> sp_FileCer_Retrieve_CCertificados(string CtrlsRFC)
        {
            List<XMLBean> list = new List<XMLBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_FileCer_Retrieve_CCertificados", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrlsRFC", CtrlsRFC));

                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        XMLBean Lxml = new XMLBean();
                        {
                            Lxml.sfilecer = data["file_cer"].ToString();
                            Lxml.sfilekey = data["file_key"].ToString();
                            Lxml.stransitorio = data["transitorio"].ToString();

                        };

                        list.Add(Lxml);
                    }
                }
                else
                {
                    list = null;
                }

                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }

            return list;

        }
        public List<XMLBean> sp_ObtenFolioCCertificados_RetrieveUpdate_Ccertificados(string rfc)
        {
            List<XMLBean> list = new List<XMLBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_ObtenFolioCCertificados_RetrieveUpdate_Ccertificados", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@rfc", rfc));

                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        XMLBean Lxml = new XMLBean();
                        {
                            Lxml.ifolio = int.Parse(data["regresa"].ToString());

                        };

                        list.Add(Lxml);
                    }
                }
                else
                {
                    list = null;
                }

                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return list;

        }
        public List<ReciboNominaBean> sp_SaldosTotales_Retrieve_TPlantillasCalculos(int CtrlIdEmpresa, int CtrlIdEmpleado, int CtrliAnio,int  CtrlIdTperiodo, int CtrlPeriodo,int CtrliEspejo)
        {
            List<ReciboNominaBean> list = new List<ReciboNominaBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_SaldosTotales_Retrieve_TPlantillasCalculos", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrlIdEmpresa", CtrlIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@CtrlIdEmpleado", CtrlIdEmpleado));
                cmd.Parameters.Add(new SqlParameter("@CtrliAnio", CtrliAnio));
                cmd.Parameters.Add(new SqlParameter("@CtrlIdTperiodo", CtrlIdTperiodo));
                cmd.Parameters.Add(new SqlParameter("@CtrlPeriodo", CtrlPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliEspejo", CtrliEspejo));
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        ReciboNominaBean ls = new ReciboNominaBean();
                        {
                            ls.iIdTipoPeriodo = int.Parse(data["Tipo_Periodo_id"].ToString());
                            ls.iIdCalculoshd = int.Parse(data["Calculos_Hd_id"].ToString());
                            ls.iIdRenglon = int.Parse(data["Renglon_id"].ToString());
                            ls.dSaldo = decimal.Parse(data["Saldo"].ToString());
                            ls.dGravado = decimal.Parse(data["Gravado"].ToString());
                            ls.dExcento = decimal.Parse(data["Excento"].ToString());
                            ls.sEspejo = data["es_espejo"].ToString();
                            ls.iGrupEmpresa = int.Parse(data["GrupoEmpresa_Id"].ToString());
                        }

                        list.Add(ls);
                    }

                }
                else
                {
                    list = null;
                }
                data.Close(); conexion.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            return list;
        }
        public HttpResponse response { get; }

        public List<EmisorReceptorBean> GXMLNOM(int IdEmpresa, string sNombreComple, string path, int Periodo, int anios, int Tipodeperido, int masivo, int Recibo)
        {
            int IdCalcHD, iperiodo;
            int NumEmpleado = 0, NoXmlx = 1, id = 0, row198 = 0, row195 = 0, rowTper = 0, row17 = 0, row113 = 0, row27 = 0, row28 = 0, row29 = 0, row227 = 0, row467 = 0, row1007 = 0, row198Exit = 0, rowExit199=0 , row199 = 0, Recibo2 = 0, FinR = 0, ISREs = 0, PEr0 = 0, deduc0 = 0;
            int PeridoEmple = 0;
            string[] Nombre = sNombreComple.Split(' ');
            string NomEmple = "";
            List<string> NomArchXML = new List<string>();
            List<EmisorReceptorBean> ListDatEmisor = new List<EmisorReceptorBean>();
            List<EmisorReceptorBean> ListErrEmisorRep = new List<EmisorReceptorBean>();
            List<EmpleadosBean> ListEmple = new List<EmpleadosBean>();
            //  List<EmpleadosBean> LisEmpleados = new List<EmpleadosBean>();
            List<CInicioFechasPeriodoBean> LFechaPerido = new List<CInicioFechasPeriodoBean>();
            List<ReciboNominaBean> LisTRecibo = new List<ReciboNominaBean>();
            List<ReciboNominaBean> LisTDiasEmple = new List<ReciboNominaBean>();
            FuncionesNomina Dao = new FuncionesNomina();
            List<ReciboNominaBean> ListTotales = new List<ReciboNominaBean>();


            string Prefijo = "cfdi";
            string Prefijo2 = "nomina12";
            string parametro1 = "http://www.w3.org/2001/XMLSchema-instance";
            string EspacioDeNombreNomina = "http://www.sat.gob.mx/nomina12";
            string parametro3 = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd http://www.sat.gob.mx/nomina12 http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xsd";
            string EspacioDeNombre = "http://www.sat.gob.mx/cfd/3";
            string s_certificadoKey = ""; string s_certificadoCer = ""; string ArchivoXmlFile; string NomArch; string s_transitorio = "";
            string pathCer;
            pathCer = path.Replace("XmlZip\\", "certificados\\");
            string idEmpresapath = Convert.ToString(IdEmpresa);
            pathCer = pathCer.Replace(idEmpresapath + "\\", "");

            int nRfcEmisor = 0;
            //int nImporte = 1;
            int nCurpReceptor = 2;
            int nRfcReceptor = 3;
            int nFechaInicialPago = 4;
            int nFechaFinalPago = 5;
            int nFechaPago = 6;

            string Emisor;
            string EmisorRFC;
            string ReceptorCurp;
            string ReceptorRFC;
            string FileCadenaXslt;
            string sUsoCFDI;

            string sFechaInicialPago;
            string sFechaFinalPago;
            string sFechaPago;
            string sDiasEfectivos;
            string sDiasIncapa;
            string anoarchivo;
            string archivoError;
            string ErrEmiRecep;
            string ErrXml;


            int error = 0, ErroPerce = 0;
            LFechaPerido = sp_DatosPerido_Retrieve_DatosPerido(Periodo);

            if (masivo == 1)
            {

                 PeridoEmple = LFechaPerido[0].iPeriodo;
                ListEmple = Dao.sp_EmpleadosEmpresa_periodo(IdEmpresa, Tipodeperido, PeridoEmple, anios, 1);
                if (ListEmple == null)
                {
                    NoXmlx = 0;
                    EmisorReceptorBean ls = new EmisorReceptorBean();
                    {
                        ls.sMensaje = "Error";
                    }
                    ListDatEmisor.Add(ls);
                    error = 0;
                }
                if (ListEmple != null)
                {
                    NoXmlx = ListEmple.Count - 1;
                    if (NoXmlx < 1)
                    {
                        NoXmlx = 1;
                    }
                    error = 1;
                }
            };
            if (masivo == 0)
            {
                NoXmlx = 1;
                error = 1;
            }
            if (masivo == 3)
            {
                NoXmlx = 1;
                error = 1;
            }
            if (NoXmlx > 0 && error > 0)
            {
                if (masivo == 1)
                {

                    NoXmlx = ListEmple.Count - 1;
                }
                if (masivo == 0)
                {
                    NoXmlx = 0;
                }
                if (masivo == 3)
                {
                    NoXmlx = 0;
                }


                for (int i = 0; i <= NoXmlx; i++)
                {
                    row198 = 0;
                    row195 = 0;
                    ErroPerce = 0;

                    if (masivo == 0 || masivo == 3)
                    {
                        Nombre = sNombreComple.Split(' ');
                        string Idempleado = Nombre[0].ToString();
                        NumEmpleado = Convert.ToInt32(Idempleado.ToString());
                        id = int.Parse(Idempleado);

                        if (Recibo == 1) {
                           
                            ListDatEmisor = sp_EmisorReceptor_Retrieve_EmisorReceptor(IdEmpresa, id);

                        }

                        if (Recibo == 2) {
                            ListDatEmisor = sp_Retrieve_EmisorRecepEmpOrigen(IdEmpresa, anios, Tipodeperido, PeridoEmple, id,1);

                        }

                        if (Recibo == 3)
                        {
                            ListDatEmisor = sp_Retrieve_EmisorRecepEmpOrigen(IdEmpresa, anios, Tipodeperido, PeridoEmple, id, 2);

                        }

                        if (Nombre.Length > 0)
                        {
                            NomEmple = "";
                            for (int a = 0; a < Nombre.Length; a++)
                            {
                                NomEmple = NomEmple + Nombre[a].ToString() + " ";
                            };

                        }
                    }
                    if (masivo == 1)
                    {
                        NomEmple = "";
                        Nombre = ListEmple[i].sNombreEmpleado.Split(' '); //.sNombreCompleto.Split(' ');
                        NumEmpleado = ListEmple[i].iIdEmpleado;   //ListEmple[i].iIdEmpleado;
                        id = ListEmple[i].iIdEmpleado;

                        if (Recibo == 1)
                        {
                            ListDatEmisor = sp_EmisorReceptor_Retrieve_EmisorReceptor(IdEmpresa, id);
                        }

                        if (Recibo == 2)
                        {
                            ListDatEmisor = sp_Retrieve_EmisorRecepEmpOrigen(IdEmpresa, anios, Tipodeperido, PeridoEmple, id,1);
                        }
                        if (Recibo == 3)
                        {
                            ListDatEmisor = sp_Retrieve_EmisorRecepEmpOrigen(IdEmpresa, anios, Tipodeperido, PeridoEmple, id, 2);
                        }


                        ErrEmiRecep = "";
                        /// validacion xml 

                        if (ListDatEmisor != null) {
                            if (ListDatEmisor[0].sMensaje == "error")
                            {

                                if (ListDatEmisor[0].sNombreEmpresa == " ")
                                {
                                    ErrEmiRecep = "Nombre de Empresa";
                                }
                                if (ListDatEmisor[0].sCalle == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", calle de empresa";
                                }
                                if (ListDatEmisor[0].sColonia == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", colonia de empresa";
                                }
                                if (ListDatEmisor[0].iCP == 0)
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Codigo postal empresa";
                                }
                                if (ListDatEmisor[0].sCiudad == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Ciudad de empresa";
                                }
                                if (ListDatEmisor[0].sRFC == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Rfc Empresa";

                                }
                                if (ListDatEmisor[0].sAfiliacionIMSS == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Rfc Empresa";
                                }
                                if (ListDatEmisor[0].sFechaAntiguedad == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Rfc empresa";
                                }
                                if (ListDatEmisor[0].sLugarNacimiento == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", lugar de nacimiento empleado";
                                }
                                if (ListDatEmisor[0].sFechaNacimiento == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", fecha de nacimiento empleado";

                                }
                                if (ListDatEmisor[0].sNombreComp == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Nombre del Empleado";

                                }
                                if (ListDatEmisor[0].sDomiciolioEmple == " ")
                                {

                                    ErrEmiRecep = ErrEmiRecep + ", domiciolio empleado";

                                }
                                if (ListDatEmisor[0].sSexo == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", sexo empleado";
                                }
                                if (ListDatEmisor[0].sEstadoCivil == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", sexo empleado";
                                }
                                if (ListDatEmisor[0].sCiudadEmple == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", sexo empleado";
                                }
                                if (ListDatEmisor[0].sRFCEmpleado == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", RFC de empleado ";
                                }
                                if (ListDatEmisor[0].iIdEmpleado == 0)
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", numero de empleado";

                                }
                                if (ListDatEmisor[0].sDescripcionDepartamento == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", nombre de departamento";

                                }
                                if (ListDatEmisor[0].sLocalidademple == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", localidad del empleado";

                                }
                                if (ListDatEmisor[0].sNombrePuesto == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", nombre puesto ";

                                }
                                if (ListDatEmisor[0].sFechaIngreso == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", nombre puesto ";

                                }
                                if (ListDatEmisor[0].sTipoContrato == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", tipo de contrato";

                                }
                                if (ListDatEmisor[0].sCentroCosto == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", centro de costos";

                                }
                                if (ListDatEmisor[0].dSalarioMensual == 0)
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", solario mensual del empleado";
                                }
                                if (ListDatEmisor[0].sRegistroImss == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Registro de IMSS del empleado";

                                }
                                if (ListDatEmisor[0].sCURP == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Registro de IMSS del empleado";

                                }
                                if (ListDatEmisor[0].sDescripcion == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", banco del empleado";

                                }
                                if (ListDatEmisor[0].sCodiBanco == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Codigo del banco";

                                }
                                if (ListDatEmisor[0].sCtaCheques == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", cuenta de cheques del empleado";

                                }
                                if (ListDatEmisor[0].iRegimenFiscal == 0)
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", cuenta de cheques del empleado";

                                }
                                if (ListDatEmisor[0].iIdNomina == 0)
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", No. Nomina";

                                }
                                if (ListDatEmisor[0].SDINT == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", salario integrado del empleado";
                                }
                                if (ListDatEmisor[0].sRiesgoTrabajo == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Riesgo de Trabajo del empleado";
                                }
                                if (ListDatEmisor[0].iTipoJordana == 0)
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", tipo de jornada del empleado";

                                }
                                if (ListDatEmisor[0].iCgTipoEmpleadoId == 0)
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", tipo de empleado";

                                }
                                if (ListDatEmisor[0].sClaveEnt == " ")
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", clave de entidad federativa del empleado";

                                }
                                if (ListDatEmisor[0].iCgTipoPago == 0)
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Tipo de pago del empleado";

                                }
                                if (ListDatEmisor[0].iPagopor == 0)
                                {
                                    ErrEmiRecep = ErrEmiRecep + ", Tipo de pago del empleado";

                                }


                                EmisorReceptorBean ls2 = new EmisorReceptorBean();
                                ls2.iIdNomina = ListDatEmisor[0].iIdNomina;
                                ls2.iIdEmpresa = IdEmpresa;
                                ls2.camposErr = ErrEmiRecep;
                                ListErrEmisorRep.Add(ls2);
                            }
                            if (Nombre.Length > 0)
                            {
                                NomEmple = "";
                                for (int a = 1; a < Nombre.Length; a++)
                                {
                                    NomEmple = NomEmple + Nombre[a].ToString() + " ";
                                };

                            }

                        }

                    };
                    if (masivo != 3)
                    {
                        if (Recibo == 1) {
                            ListTotales = null;
                            ListTotales = sp_SaldosTotales_Retrieve_TPlantillasCalculos(IdEmpresa, NumEmpleado, anios, Tipodeperido, LFechaPerido[0].iPeriodo, 0);
                            LisTRecibo = null;
                            LisTRecibo = Dao.sp_TpCalculoEmpleado_Retrieve_TpCalculoEmpleado(IdEmpresa, id, LFechaPerido[0].iPeriodo, Tipodeperido, anios, 0);
                        }

                        if (Recibo == 2) {
                            ListTotales = null;
                            ListTotales = sp_SaldosTotales_Retrieve_TPlantillasCalculos(IdEmpresa, NumEmpleado,anios,Tipodeperido, LFechaPerido[0].iPeriodo, 1);
                            LisTRecibo = null;
                            LisTRecibo = Dao.sp_TpCalculoEmpleado_Retrieve_TpCalculoEmpleado(IdEmpresa, id, LFechaPerido[0].iPeriodo, Tipodeperido, anios, 1);
                        }

                        if (Recibo == 3)
                        {
                            ListTotales = null;
                            ListTotales = sp_SaldosTotales_Retrieve_TPlantillasCalculos(IdEmpresa, NumEmpleado,anios,Tipodeperido, LFechaPerido[0].iPeriodo, 2);
                            LisTRecibo = null;
                            LisTRecibo = Dao.sp_TpCalculoEmpleado_Retrieve_TpCalculoEmpleado(IdEmpresa, id, LFechaPerido[0].iPeriodo, Tipodeperido, anios, 1);
                        }




                        LisTDiasEmple = null;
                        LisTDiasEmple = Dao.sp_DiaTrabjEmple_Retrieve_TPlantillaCalculosLn(IdEmpresa, id, LFechaPerido[0].iPeriodo, Tipodeperido, anios);

                    }
                    if (masivo == 3)
                    {

                        ListTotales = null;
                        LisTRecibo = null;
                        ListTotales = Sp_TotalesFiniquito_Retrieve_TFiniquitoLn(IdEmpresa, NumEmpleado, LFechaPerido[0].iPeriodo, anios, 0);
                        LisTRecibo = Sp_TotalesFiniquito_Retrieve_TFiniquitoLn(IdEmpresa, NumEmpleado, LFechaPerido[0].iPeriodo, anios, 1);
                        LisTDiasEmple = null;
                        LisTDiasEmple = Dao.sp_DiaTrabjEmple_Retrieve_TPlantillaCalculosLn(IdEmpresa, id, LFechaPerido[0].iPeriodo, Tipodeperido, anios);


                    };

                    if (ListTotales != null && LisTRecibo != null && ListDatEmisor !=null)
                    {
                        if (ListDatEmisor.Count > 0)
                        {
                            
                            Emisor = ListDatEmisor[0].sNombreEmpresa;
                            EmisorRFC = ListDatEmisor[0].sRFC;
                            ReceptorCurp = ListDatEmisor[0].sCURP;
                            ReceptorRFC = ListDatEmisor[0].sRFCEmpleado;
                            NomArch = "F";
                            ArchivoXmlFile = path + NomArch;
                            archivoError =
                            FileCadenaXslt = pathCer + "cadenaoriginal_3_3.xslt";
                            sUsoCFDI = "P01";
                            var culture = CultureInfo.CreateSpecificCulture("es-MX");
                            var styles = DateTimeStyles.None;
                            DateTime dt1 = DateTime.Now;
                            DateTime dt2 = dt1;
                            DateTime dt3 = dt1;
                            string folio = "";
                            iperiodo = LFechaPerido[0].iPeriodo;
                            if (LFechaPerido[0].sMensaje == null)
                            {

                                bool fechaValida = DateTime.TryParse(LFechaPerido[0].sFechaInicio, culture, styles, out dt1);
                                fechaValida = DateTime.TryParse(LFechaPerido[0].sFechaFinal, culture, styles, out dt2);
                                fechaValida = DateTime.TryParse(LFechaPerido[0].sFechaPago, culture, styles, out dt3);

                                sFechaInicialPago = String.Format("{0:yyyy-MM-dd}", dt1);
                                sFechaFinalPago = String.Format("{0:yyyy-MM-dd}", dt2);
                                sFechaPago = String.Format("{0:yyyy-MM-dd}", dt3);
                                anoarchivo = String.Format("{0:yyyy}", dt2);


                                IdCalcHD = LisTRecibo[0].iIdCalculoshd;
                                //Partidas
                                decimal dTotalPercepciones = 0;
                                decimal dTotaldeducciones = 0;
                                string tipoNom = " ";
                                string TotalPercepciones = " ";
                                string totalDeduciones = " ";
                                string totalRecibo = " ";
                                string SueldoDiario = " ";
                                string SuedoAgravado = " ";

                                if (ListTotales.Count > 0 && masivo < 3)
                                {
                                    for (int a = 0; a < ListTotales.Count; a++)
                                    {
                                        if (ListTotales[a].sEspejo == "False") { tipoNom = "0"; }
                                        if (ListTotales[a].sEspejo == "True") { tipoNom = "1"; }
                                        if (ListTotales[a].iIdRenglon == 990) { TotalPercepciones = string.Format("{0:N2}", ListTotales[a].dSaldo); rowTper = a; dTotalPercepciones = ListTotales[a].dSaldo; }
                                        if (ListTotales[a].iIdRenglon == 1990) { totalDeduciones = string.Format("{0:N2}", ListTotales[a].dSaldo); }
                                        if (ListTotales[a].iIdRenglon == 9999) { totalRecibo = string.Format("{0:N2}", ListTotales[a].dSaldo); }
                                        if (ListTotales[a].iIdRenglon == 9992) { SuedoAgravado = string.Format("{0:N2}", ListTotales[a].dSaldo); }
                                        if (ListTotales[a].iIdRenglon == 9993) { SueldoDiario = string.Format("{0:N2}", ListTotales[a].dSaldo); }
                                    }
                                    TotalPercepciones = TotalPercepciones.Replace(",", "");
                                    totalDeduciones = totalDeduciones.Replace(",", "");
                                    if (totalDeduciones == " " || totalDeduciones == "") { totalDeduciones = "0"; };
                                    totalRecibo = totalRecibo.Replace(",", "");
                                }

                                if (masivo > 2 && FinR < 1)
                                {
                                    if (LisTRecibo.Count > 0)
                                    {
                                        decimal Tpercep = 0, Tdedu = 0, TotRec = 0;
                                        for (int a = 0; a < LisTRecibo.Count; a++)
                                        {

                                            if (LisTRecibo[a].iIdRenglon != 27 || LisTRecibo[a].iIdRenglon != 28 || LisTRecibo[a].iIdRenglon != 29 && LisTRecibo[a].iIdRenglon != 199 && LisTRecibo[a].sValor == "Percepciones")
                                            {
                                                tipoNom = "0";
                                                Tpercep = Tpercep + LisTRecibo[a].dGravado + LisTRecibo[a].dExcento;
                                            }

                                            if (LisTRecibo[a].iIdRenglon != 1005 && LisTRecibo[a].sValor == "Deducciones")
                                            {
                                                Tdedu = Tdedu + LisTRecibo[a].dSaldo;


                                            }
                                            TotRec = Tpercep + Tdedu;


                                        }

                                        TotalPercepciones = string.Format("{0:N2}", Tpercep); dTotalPercepciones = Tpercep;
                                        totalDeduciones = string.Format("{0:N2}", Tdedu);
                                        totalRecibo = string.Format("{0:N2}", TotRec);
                                        TotalPercepciones = TotalPercepciones.Replace(",", "");
                                        totalDeduciones = totalDeduciones.Replace(",", "");
                                        if (totalDeduciones == " " || totalDeduciones == "") { totalDeduciones = "0"; };
                                        totalRecibo = totalRecibo.Replace(",", "");
                                    }
                                }
                                if (masivo > 2 && FinR > 0)
                                {
                                    if (LisTRecibo.Count > 0)
                                    {
                                        decimal Tpercep = 0, Tdedu = 0, TotRec = 0;
                                        for (int a = 0; a < LisTRecibo.Count; a++)
                                        {
                                            if (LisTRecibo[a].iIdRenglon == 27 || LisTRecibo[a].iIdRenglon == 28 || LisTRecibo[a].iIdRenglon == 29 && LisTRecibo[a].sValor == "Percepciones")
                                            {
                                                tipoNom = "0";
                                                Tpercep = Tpercep + LisTRecibo[a].dGravado + LisTRecibo[a].dExcento;
                                            }

                                            if (LisTRecibo[a].iIdRenglon == 1005 && LisTRecibo[a].sValor == "Deducciones")
                                            {
                                                Tdedu = Tdedu + LisTRecibo[a].dSaldo;
                                            }
                                            TotRec = Tpercep + Tdedu;


                                        }

                                        TotalPercepciones = string.Format("{0:N2}", Tpercep); dTotalPercepciones = Tpercep;
                                        totalDeduciones = string.Format("{0:N2}", Tdedu);
                                        totalRecibo = string.Format("{0:N2}", TotRec);
                                        TotalPercepciones = TotalPercepciones.Replace(",", "");
                                        totalDeduciones = totalDeduciones.Replace(",", "");
                                        if (totalDeduciones == " " || totalDeduciones == "") { totalDeduciones = "0"; };
                                        totalRecibo = totalRecibo.Replace(",", "");
                                    }
                                }

                                //Antiguedad 
                                string sAntiguedad = "";
                                List<XMLBean> LisCer = new List<XMLBean>();
                                LisCer = sp_FileCer_Retrieve_CCertificados(EmisorRFC);

                                if (LisCer != null && ListDatEmisor[0].sMensaje != "error")
                                {
                                    if (LisCer.Count > 0)
                                    {
                                        s_certificadoKey = pathCer + LisCer[0].sfilekey;
                                        s_certificadoCer = pathCer + LisCer[0].sfilecer;
                                        s_transitorio = LisCer[0].stransitorio;

                                        if (File.Exists(s_certificadoKey))
                                        {

                                            System.Security.Cryptography.X509Certificates.X509Certificate CerSAT;
                                            CerSAT = System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromCertFile(s_certificadoCer);
                                            byte[] bcert = CerSAT.GetSerialNumber();
                                            string CerNo = LibreriasFacturas.StrReverse((string)Encoding.UTF8.GetString(bcert));
                                            byte[] CERT_SIS = CerSAT.GetRawCertData();

                                            List<XMLBean> LFolio = new List<XMLBean>();
                                            LFolio = sp_ObtenFolioCCertificados_RetrieveUpdate_Ccertificados(EmisorRFC);

                                            if (LFolio != null) folio = LFolio[0].ifolio.ToString();
                                            else ListDatEmisor[0].sMensaje = "Erro en Genera el folio Contacte a sistemas";
                                            string sNombre = ListDatEmisor[0].sNombreComp;
                                            string sRegistroPatronal = ListDatEmisor[0].sAfiliacionIMSS;
                                            string sNumSeguridadSocial = ListDatEmisor[0].sRegistroImss;
                                            sNumSeguridadSocial = sNumSeguridadSocial.Replace("-", "");
                                            fechaValida = DateTime.TryParse(ListDatEmisor[0].sFechaIngreso, culture, styles, out dt3);
                                            string sFechaInicioRelLaboral = String.Format("{0:yyyy-MM-dd}", dt3);
                                            string ticontrato = "0" + ListDatEmisor[0].sTipoContrato;
                                            string[] contrato = ticontrato.Split(' ');
                                            string scontrato = contrato[0].ToString();
                                            string sTipoContrato = scontrato;
                                            string sNumEmpleado = Convert.ToString(ListDatEmisor[0].iIdEmpleado);
                                            string sDepartamento = ListDatEmisor[0].sDescripcionDepartamento;
                                            string sPuesto = ListDatEmisor[0].sNombrePuesto;
                                            string sBanco = ListDatEmisor[0].sDescripcion;
                                            string sCodiBan = ListDatEmisor[0].sCodiBanco;
                                            string sCuentaBancaria = ListDatEmisor[0].sCtaCheques;
                                            string sSalarioDiarioIntegrado = SueldoDiario;
                                            string sNombreEmisor = ListDatEmisor[0].sNombreEmpresa;
                                            string sRegimenFiscal = Convert.ToString(ListDatEmisor[0].iRegimenFiscal);
                                            string sCalveEntidad = ListDatEmisor[0].sClaveEnt;


                                            //Antiguedad

                                            DateTime f1 = DateTime.Parse(sFechaInicioRelLaboral);
                                            DateTime f2 = DateTime.Parse(sFechaFinalPago);
                                            TimeSpan diferencia = f2.Subtract(f1);
                                            sAntiguedad = "P" + ((int)(diferencia.Days / 7)).ToString() + "W";
                                            StreamWriter writer;
                                            XmlTextWriter xmlWriter;

                                            // Nombre del archivo XML
                                            int NoidCalhd = ListTotales[0].iIdCalculoshd;
                                            if (masivo != 3)
                                            {
                                                NomArch = NomArch + NoidCalhd + "_CFDI_E" + IdEmpresa + "_F" + anoarchivo;

                                            }
                                            if (masivo == 3)
                                            {

                                                NomArch = NomArch + NoidCalhd + "_CFDI_E" + IdEmpresa + "_F" + anoarchivo;
                                                if (Recibo2 == 1 && FinR == 1)
                                                {
                                                    NomArch = NomArch + "_R2";
                                                };

                                            }
                                            if (LFechaPerido[0].iPeriodo > 9)
                                            {
                                                NomArch = NomArch + ListTotales[0].iIdTipoPeriodo + "0" + LFechaPerido[0].iPeriodo + tipoNom + "_N" + id;
                                            }
                                            if (LFechaPerido[0].iPeriodo < 10)
                                            {
                                                NomArch = NomArch + ListTotales[0].iIdTipoPeriodo + "00" + LFechaPerido[0].iPeriodo + tipoNom + "_N" + id;
                                            }

                                            ArchivoXmlFile = ArchivoXmlFile + NomArch;
                                            NomArchXML.Add(ArchivoXmlFile);

                                            //Crear archivo XML
                                            writer = File.CreateText(ArchivoXmlFile);
                                            writer.Close();

                                            //Preparar archivo
                                            xmlWriter = new XmlTextWriter(ArchivoXmlFile, System.Text.Encoding.UTF8);
                                            xmlWriter.Formatting = Formatting.Indented;
                                            xmlWriter.WriteStartDocument();


                                            //Insertar elementos
                                            xmlWriter.WriteStartElement(Prefijo, "Comprobante", EspacioDeNombre);
                                            xmlWriter.WriteAttributeString("xmlns", "xsi", null, parametro1);
                                            xmlWriter.WriteAttributeString("xmlns", Prefijo2, null, EspacioDeNombreNomina);
                                            xmlWriter.WriteAttributeString("xsi", "schemaLocation", null, parametro3);
                                            xmlWriter.WriteAttributeString("xmlns", Prefijo, null, EspacioDeNombre);
                                            xmlWriter.WriteAttributeString("Version", "3.3");
                                            xmlWriter.WriteAttributeString("Serie", "NOM");
                                            xmlWriter.WriteAttributeString("Folio", folio);
                                            string FechaEmision = DateTime.Now.ToString("s");
                                            xmlWriter.WriteAttributeString("Fecha", FechaEmision);
                                            xmlWriter.WriteAttributeString("Sello", "");
                                            xmlWriter.WriteAttributeString("FormaPago", "99");
                                            xmlWriter.WriteAttributeString("NoCertificado", CerNo);
                                            string sCertificado = Convert.ToBase64String(CERT_SIS);
                                            xmlWriter.WriteAttributeString("Certificado", sCertificado);
                                            xmlWriter.WriteAttributeString("SubTotal", TotalPercepciones.ToString());
                                            
                                            if (totalDeduciones != "0.00") { xmlWriter.WriteAttributeString("Descuento", totalDeduciones.ToString()); };

                                            xmlWriter.WriteAttributeString("Moneda", "MXN");
                                            xmlWriter.WriteAttributeString("Total", totalRecibo.ToString());
                                            xmlWriter.WriteAttributeString("TipoDeComprobante", "N");
                                            xmlWriter.WriteAttributeString("MetodoPago", "PUE");
                                            xmlWriter.WriteAttributeString("LugarExpedicion", "04600");

                                            xmlWriter.WriteStartElement(Prefijo, "Emisor", EspacioDeNombre);
                                            xmlWriter.WriteAttributeString("Rfc", EmisorRFC);
                                            xmlWriter.WriteAttributeString("Nombre", sNombreEmisor);
                                            xmlWriter.WriteAttributeString("RegimenFiscal", sRegimenFiscal);
                                            xmlWriter.WriteEndElement();

                                            xmlWriter.WriteStartElement(Prefijo, "Receptor", EspacioDeNombre);
                                            xmlWriter.WriteAttributeString("Rfc", ReceptorRFC);
                                            xmlWriter.WriteAttributeString("Nombre", sNombre);
                                            xmlWriter.WriteAttributeString("UsoCFDI", sUsoCFDI);
                                            xmlWriter.WriteEndElement();

                                            xmlWriter.WriteStartElement(Prefijo, "Conceptos", EspacioDeNombre);
                                            xmlWriter.WriteStartElement(Prefijo, "Concepto", EspacioDeNombre);
                                            xmlWriter.WriteAttributeString("ClaveProdServ", "84111505");
                                            xmlWriter.WriteAttributeString("Cantidad", "1");
                                            xmlWriter.WriteAttributeString("ClaveUnidad", "ACT");
                                            xmlWriter.WriteAttributeString("Descripcion", "Pago de nómina");
                                            if (totalDeduciones.ToString() != "0.00")
                                            {
                                                xmlWriter.WriteAttributeString("Descuento", totalDeduciones.ToString());

                                            }

                                            xmlWriter.WriteAttributeString("ValorUnitario", TotalPercepciones.ToString());
                                            xmlWriter.WriteAttributeString("Importe", TotalPercepciones.ToString());
                                            xmlWriter.WriteEndElement();
                                            xmlWriter.WriteEndElement();

                                            xmlWriter.WriteStartElement(Prefijo, "Complemento", EspacioDeNombre);
                                            xmlWriter.WriteStartElement(Prefijo2, "Nomina", EspacioDeNombreNomina);
                                            xmlWriter.WriteAttributeString("Version", "1.2");
                                            string TipoNom = "";
                                            if ((ListDatEmisor[0].iPagopor == 364 && (IdEmpresa == 2075 || IdEmpresa == 2076 || IdEmpresa == 2077 || IdEmpresa == 2078 || IdEmpresa == 2080 || IdEmpresa == 2081)) ||(Tipodeperido==3 && LFechaPerido[0].iPeriodo > 55 && IdEmpresa != 212 || Tipodeperido ==0 && LFechaPerido[0].iPeriodo > 55 && IdEmpresa != 212))
                                            {
                                                TipoNom = "E";
                                            }
                                            else
                                            {

                                                TipoNom = "O";
                                            }

                                            
                                            xmlWriter.WriteAttributeString("TipoNomina", TipoNom);
                                            xmlWriter.WriteAttributeString("FechaPago", sFechaPago);
                                            xmlWriter.WriteAttributeString("FechaInicialPago", sFechaInicialPago);
                                            xmlWriter.WriteAttributeString("FechaFinalPago", sFechaFinalPago);


                                            sDiasEfectivos = "";
                                           
                                            sDiasIncapa = "";
                                            int Dias = 0;
                                            if (LisTDiasEmple != null)
                                            {
                                                for (int c = 0; c < LisTDiasEmple.Count; c++)
                                                {
                                                    if (LisTDiasEmple[c].iIdRenglon == 31)
                                                    {
                                                        string dias = Convert.ToString(LisTDiasEmple[c].iDiasTrab);
                                                        string[] Dias2 = dias.Split('.');
                                                        sDiasEfectivos = Dias2[0];
                                                        Dias = Dias +int.Parse(sDiasEfectivos);

                                                    }
                                                    if (LisTDiasEmple[c].iIdRenglon == 34)
                                                    {
                                                        string dias = Convert.ToString(LisTDiasEmple[c].iDiasTrab);
                                                        string[] Dias2 = dias.Split('.');
                                                        sDiasIncapa = Dias2[0];
                                                        Dias = Dias+ int.Parse(sDiasIncapa);

                                                    }
                                                }
                                                sDiasEfectivos = Dias.ToString();
                                            }
                                            if (LisTDiasEmple == null)
                                            {
                                                sDiasEfectivos = Convert.ToString(LFechaPerido[0].iDiasEfectivos);
                                            }


                                            string Otrospagos = "0.00";
                                            decimal DotrosPagos = 0;
                                            if (LisTRecibo.Count > 0 && FinR == 0)
                                            {
                                                for (int a = 0; a < LisTRecibo.Count; a++)
                                                {

                                                    if (LisTRecibo[a].iIdRenglon == 198)
                                                    {

                                                        DotrosPagos = DotrosPagos + LisTRecibo[a].dSaldo;

                                                    }
                                                    if (LisTRecibo[a].iIdRenglon == 199)
                                                    {

                                                        DotrosPagos = DotrosPagos + LisTRecibo[a].dSaldo;

                                                    }
                                                    if (LisTRecibo[a].iIdRenglon == 17)
                                                    {
                                                        DotrosPagos = DotrosPagos + LisTRecibo[a].dSaldo;
                                                    }
                                                    if (LisTRecibo[a].iIdRenglon == 227)
                                                    {
                                                        DotrosPagos = DotrosPagos + LisTRecibo[a].dSaldo;
                                                    }
                                                    if (LisTRecibo[a].iIdRenglon == 113)
                                                    {
                                                        DotrosPagos = DotrosPagos + LisTRecibo[a].dSaldo;
                                                    }
                                                    if (LisTRecibo[a].iIdRenglon == 467)
                                                    {
                                                        DotrosPagos = DotrosPagos + LisTRecibo[a].dSaldo;
                                                    }

                                                }
                                                TotalPercepciones = string.Format("{0:N2}", ListTotales[rowTper].dSaldo - DotrosPagos);
                                                TotalPercepciones = TotalPercepciones.Replace(",", "");
                                                Otrospagos = string.Format("{0:N2}", DotrosPagos);

                                            }

                                            Otrospagos = Otrospagos.Replace(",", "");

                                            if (masivo != 3)
                                            {
                                                xmlWriter.WriteAttributeString("NumDiasPagados", sDiasEfectivos.ToString());
                                            }

                                            if (TotalPercepciones != "0.00")
                                            {
                                                xmlWriter.WriteAttributeString("TotalPercepciones", TotalPercepciones.ToString());

                                            }
                                            if (totalDeduciones.ToString() != "0.00")
                                            {
                                                xmlWriter.WriteAttributeString("TotalDeducciones", totalDeduciones.ToString());

                                            }

                                            if (ListDatEmisor[0].iPagopor != 364)
                                            {
                                                xmlWriter.WriteAttributeString("TotalOtrosPagos", Otrospagos);
                                            }

                                            xmlWriter.WriteAttributeString("xmlns", Prefijo2, null, EspacioDeNombreNomina);
                                            xmlWriter.WriteStartElement(Prefijo2, "Emisor", EspacioDeNombreNomina);

                                            if (ListDatEmisor[0].iPagopor != 364)
                                            {
                                                xmlWriter.WriteAttributeString("RegistroPatronal", sRegistroPatronal);
                                            }

                                            xmlWriter.WriteEndElement();

                                            string TipoRiesgo = ListDatEmisor[0].sRiesgoTrabajo.ToString();
                                            xmlWriter.WriteStartElement(Prefijo2, "Receptor", EspacioDeNombreNomina);
                                            xmlWriter.WriteAttributeString("Curp", ReceptorCurp);
                                            string TipJordana = "";
                                            if (ListDatEmisor[0].iTipoJordana < 10) { TipJordana = "0" + ListDatEmisor[0].iTipoJordana; };
                                            if (ListDatEmisor[0].iTipoJordana > 9) { TipJordana = Convert.ToString(ListDatEmisor[0].iTipoJordana); };
                                            xmlWriter.WriteAttributeString("NumEmpleado", sNumEmpleado);
                                            xmlWriter.WriteAttributeString("Departamento", sDepartamento);
                                            xmlWriter.WriteAttributeString("Puesto", sPuesto);
                                            if ((IdEmpresa == 2075 || IdEmpresa == 2076 || IdEmpresa == 2077 || IdEmpresa == 2078 || IdEmpresa == 2080 || IdEmpresa ==2081)||(Tipodeperido ==0 && LFechaPerido[0].iPeriodo > 55 && IdEmpresa!=212 )||(Tipodeperido == 3 &&  LFechaPerido[0].iPeriodo > 55 && IdEmpresa != 212))
                                            {
                                                xmlWriter.WriteAttributeString("PeriodicidadPago", "99");
                                            }
                                            else
                                            {
                                                if (Tipodeperido == 0)
                                                {
                                                    xmlWriter.WriteAttributeString("PeriodicidadPago", "02");
                                                }
                                                if (Tipodeperido == 1)
                                                {
                                                    xmlWriter.WriteAttributeString("PeriodicidadPago", "10");
                                                }
                                                if (Tipodeperido == 2)
                                                {
                                                    xmlWriter.WriteAttributeString("PeriodicidadPago", "03");
                                                }
                                                if (Tipodeperido == 3)
                                                {
                                                    xmlWriter.WriteAttributeString("PeriodicidadPago", "04");
                                                }
                                                if (Tipodeperido == 4)
                                                {
                                                    xmlWriter.WriteAttributeString("PeriodicidadPago", "05");
                                                }
                                                if (Tipodeperido == 5)
                                                {
                                                    xmlWriter.WriteAttributeString("PeriodicidadPago", "06");
                                                }

                                            }

                                            if (sCuentaBancaria.Length >= 7 && sCuentaBancaria.Length < 18)
                                            {
                                                if (sBanco.Length > 0)
                                                {
                                                    // xmlWriter.WriteAttributeString("Banco", sBanco);
                                                    xmlWriter.WriteAttributeString("CuentaBancaria", sCuentaBancaria);
                                                    xmlWriter.WriteAttributeString("Banco", sCodiBan);
                                                    //xmlWriter.WriteAttributeString("CalveBanco", );
                                                }
                                                else
                                                {
                                                    //xmlWriter.WriteAttributeString("Banco", "0");
                                                    xmlWriter.WriteAttributeString("CuentaBancaria", sCuentaBancaria);

                                                }

                                            }
                                            if ((sCuentaBancaria.Length == 18))
                                            {

                                                xmlWriter.WriteAttributeString("CuentaBancaria", sCuentaBancaria);
                                            }


                                            if (ListDatEmisor[0].iPagopor != 364)
                                            {
                                                xmlWriter.WriteAttributeString("SalarioBaseCotApor", SuedoAgravado.Replace(",", ""));
                                                xmlWriter.WriteAttributeString("SalarioDiarioIntegrado", SueldoDiario.Replace(",", ""));
                                                xmlWriter.WriteAttributeString("RiesgoPuesto", TipoRiesgo);//1
                                                xmlWriter.WriteAttributeString("TipoJornada", TipJordana);
                                                xmlWriter.WriteAttributeString("TipoRegimen", "02");

                                                if (IdEmpresa == 107)
                                                {
                                                    xmlWriter.WriteAttributeString("Sindicalizado", "Sí");
                                                }
                                                if (IdEmpresa != 107)
                                                {
                                                    xmlWriter.WriteAttributeString("Sindicalizado", "No");
                                                }


                                                xmlWriter.WriteAttributeString("NumSeguridadSocial", sNumSeguridadSocial);
                                                xmlWriter.WriteAttributeString("FechaInicioRelLaboral", sFechaInicioRelLaboral);
                                                xmlWriter.WriteAttributeString("Antigüedad", sAntiguedad);
                                                xmlWriter.WriteAttributeString("TipoContrato", sTipoContrato);

                                            }
                                            if (ListDatEmisor[0].iPagopor == 364)
                                            {
                                                xmlWriter.WriteAttributeString("TipoRegimen", "09");
                                                // xmlWriter.WriteAttributeString("TipoContrato", sTipoContrato);
                                                xmlWriter.WriteAttributeString("TipoContrato", "99");
                                            }
                                            xmlWriter.WriteAttributeString("ClaveEntFed", sCalveEntidad);
                                            xmlWriter.WriteEndElement();

                                            // Percepciones

                                            decimal ExtentoPer = 0;
                                            decimal Perpecio = 0;
                                            if (LisTRecibo.Count > 0)
                                            {
                                                for (int a = 0; a < LisTRecibo.Count; a++)
                                                {
                                                    if (LisTRecibo[a].sValor == "Percepciones")
                                                    {
                                                        if (masivo != 3)
                                                        {

                                                            if (LisTRecibo[a].iIdRenglon != 50 && LisTRecibo[a].iIdRenglon != 51 && LisTRecibo[a].iIdRenglon != 17 && LisTRecibo[a].iIdRenglon != 227 && LisTRecibo[a].iIdRenglon != 198 && LisTRecibo[a].iIdRenglon != 467 && LisTRecibo[a].iIdRenglon != 113 && LisTRecibo[a].iIdRenglon != 199)
                                                            {
                                                                ExtentoPer = ExtentoPer + LisTRecibo[a].dExcento;
                                                                Perpecio = Perpecio + LisTRecibo[a].dGravado;
                                                            }
                                                            if (LisTRecibo[a].iIdRenglon == 50)
                                                            {

                                                                ExtentoPer = ExtentoPer + LisTRecibo[a].dExcento;
                                                                Perpecio = Perpecio + LisTRecibo[a].dGravado;
                                                            }
                                                            if (LisTRecibo[a].iIdRenglon == 51)
                                                            {

                                                                ExtentoPer = ExtentoPer + LisTRecibo[a].dExcento;
                                                                Perpecio = Perpecio + LisTRecibo[a].dGravado;
                                                            }

                                                        }
                                                        if (masivo == 3 && Recibo2 < 1)
                                                        {

                                                            if (LisTRecibo[a].iIdRenglon != 27 && LisTRecibo[a].iIdRenglon != 28 && LisTRecibo[a].iIdRenglon != 29)
                                                            {
                                                                ExtentoPer = ExtentoPer + LisTRecibo[a].dExcento;
                                                                Perpecio = Perpecio + LisTRecibo[a].dGravado;
                                                            }
                                                        }
                                                        if (masivo == 3)
           
                                                        {

                                                            if (LisTRecibo[a].iIdRenglon == 27 || LisTRecibo[a].iIdRenglon == 28 || LisTRecibo[a].iIdRenglon == 29)
                                                            {
                                                                ExtentoPer = ExtentoPer + LisTRecibo[a].dExcento;
                                                                Perpecio = Perpecio + LisTRecibo[a].dGravado;
                                                            }

                                                        }

                                                    }

                                                }

                                            }
                                            if ((ExtentoPer + Perpecio) != 0)
                                            {

                                                //string Totalexetoper = string.Format("{0:N2}", ExtentoPer);
                                                //TotalPercepciones = TotalPercepciones.Replace(",", "");
                                                xmlWriter.WriteStartElement(Prefijo2, "Percepciones", EspacioDeNombreNomina);
                                                xmlWriter.WriteAttributeString("TotalExento", string.Format("{0:N2}", ExtentoPer).Replace(",", ""));
                                                xmlWriter.WriteAttributeString("TotalGravado", string.Format("{0:N2}", Perpecio).Replace(",", ""));
                                                xmlWriter.WriteAttributeString("TotalSueldos", string.Format("{0:N2}", (ExtentoPer + Perpecio)).Replace(",", ""));


                                            }
                                            decimal Isr = 0;
                                            Recibo2 = 0;

                                            if (LisTRecibo.Count > 0)
                                            {
                                                for (int a = 0; a < LisTRecibo.Count; a++)
                                                {
                                                    if (LisTRecibo[a].sValor == "Percepciones")
                                                    {
                                                        if (Recibo2 != 1)
                                                        {
                                                            if (LisTRecibo[a].iIdRenglon == 198)
                                                            {
                                                                if (LisTRecibo[a].dSaldo > 0)
                                                                {
                                                                    row198 = a;
                                                                    row198Exit = 1;
                                                                };

                                                            };
                                                            if (LisTRecibo[a].iIdRenglon == 199)
                                                            {
                                                                if (LisTRecibo[a].dSaldo > 0)
                                                                {
                                                                    row199 = a;
                                                                    rowExit199 = 1;
                                                                };

                                                            };
                                                            if (LisTRecibo[a].iIdRenglon == 195)
                                                            {
                                                                row195 = a;
                                                            };
                                                            if (LisTRecibo[a].iIdRenglon == 17)
                                                            {
                                                                row17 = a;
                                                            };
                                                            if (LisTRecibo[a].iIdRenglon == 113)
                                                            {
                                                                row113 = a;
                                                            };
                                                            if (LisTRecibo[a].iIdRenglon == 227)
                                                            {
                                                                row227 = a;
                                                            };
                                                            if (LisTRecibo[a].iIdRenglon == 467)
                                                            {
                                                                row467 = a;
                                                            };
                                                            if (LisTRecibo[a].iIdRenglon == 27 && masivo == 3)
                                                            {
                                                                row27 = a;
                                                                Recibo2 = 1;
                                                            };
                                                            if (LisTRecibo[a].iIdRenglon == 28 && masivo == 3)
                                                            {
                                                                row28 = a;
                                                                Recibo2 = 1;
                                                            };
                                                            if (LisTRecibo[a].iIdRenglon == 29 && masivo == 3)
                                                            {
                                                                row29 = a;
                                                                Recibo2 = 1;
                                                            };

                                                        }
                                                        sDiasIncapa = "";
                                                        if (LisTDiasEmple != null)
                                                        {
                                                            for (int c = 0; c < LisTDiasEmple.Count; c++)
                                                            {
                                                                if (LisTDiasEmple[c].iIdRenglon == 31)
                                                                {
                                                                    string dias = Convert.ToString(LisTDiasEmple[c].iDiasTrab);
                                                                    string[] Dias2 = dias.Split('.');
                                                                    sDiasEfectivos = Dias2[0];

                                                                }

                                                                if (LisTDiasEmple[c].iIdRenglon == 34)
                                                                {
                                                                    string dias = Convert.ToString(LisTDiasEmple[c].iDiasTrab);
                                                                    string[] Dias2 = dias.Split('.');
                                                                    sDiasIncapa = Dias2[0];

                                                                }
                                                            }

                                                        }
                                                        if (LisTDiasEmple == null)
                                                        {
                                                            sDiasEfectivos = Convert.ToString(LFechaPerido[0].iDiasEfectivos);
                                                        }

                                                        sDiasIncapa = sDiasIncapa;
                                                        string lengRenglon = "";
                                                        string IporPagado = string.Format("{0:N2}", LisTRecibo[a].dSaldo);
                                                        string ImporGra = string.Format("{0:N2}", LisTRecibo[a].dGravado);
                                                        string ImporExt = string.Format("{0:N2}", LisTRecibo[a].dExcento);

                                                        ImporGra = ImporGra.Replace(",", "");
                                                        ImporExt = ImporExt.Replace(",", "");
                                                        IporPagado = IporPagado.Replace(",", "");

                                                        string IdRenglon = Convert.ToString(LisTRecibo[a].iIdRenglon);
                                                        string concepto = LisTRecibo[a].sNombre_Renglon;


                                                        if (Recibo2 != 1)
                                                        {
                                                            if (IdRenglon == "1")
                                                            {
                                                                if (ListDatEmisor[0].iPagopor != 364)
                                                                {

                                                                    concepto = "Sueldo " + "{" + sDiasEfectivos + " Dias}";


                                                                    if (masivo == 3)
                                                                    {
                                                                        concepto = "Dias de Sueldo Pedientes";
                                                                    }

                                                                    lengRenglon = "001";
                                                                }

                                                                if (ListDatEmisor[0].iPagopor == 364)
                                                                {

                                                                    concepto = "Asimilados a salarios {" + sDiasEfectivos + " Dias}";
                                                                    if (IdEmpresa == 2075 || IdEmpresa == 2076 || IdEmpresa == 2077 || IdEmpresa == 2078)
                                                                    {
                                                                        concepto = LisTRecibo[a].sNombre_Renglon;

                                                                    }

                                                                    lengRenglon = "001";
                                                                }

                                                            }
                                                        }

                                                        lengRenglon = Convert.ToString(LisTRecibo[a].sIdSat);

                                                        int idReglontama = IdRenglon.Length;
                                                        if (idReglontama == 1) { IdRenglon = "00" + IdRenglon; };
                                                        if (idReglontama == 2) { IdRenglon = "0" + IdRenglon; };

                                                        int iSatidNum = lengRenglon.Length;
                                                        string idSat = "";

                                                        if (iSatidNum == 1) { idSat = "00" + LisTRecibo[a].sIdSat; };
                                                        if (iSatidNum == 2) { idSat = "0" + LisTRecibo[a].sIdSat; };
                                                        if (LisTRecibo[a].iIdRenglon != 50 && LisTRecibo[a].iIdRenglon != 51 && LisTRecibo[a].iIdRenglon != 17 && LisTRecibo[a].iIdRenglon != 199 && LisTRecibo[a].iIdRenglon != 113 && LisTRecibo[a].iIdRenglon != 227 && LisTRecibo[a].iIdRenglon != 198 && LisTRecibo[a].iIdRenglon != 467 && masivo != 3)
                                                        {

                                                            xmlWriter.WriteStartElement(Prefijo2, "Percepcion", EspacioDeNombreNomina);
                                                            xmlWriter.WriteAttributeString("ImporteExento", ImporExt.ToString());
                                                            xmlWriter.WriteAttributeString("TipoPercepcion", idSat);
                                                            xmlWriter.WriteAttributeString("Clave", IdRenglon);
                                                            xmlWriter.WriteAttributeString("Concepto", concepto.ToString());
                                                            if (LisTRecibo[a].iIdRenglon == 17)
                                                            {
                                                                xmlWriter.WriteAttributeString("ImporteGravado", IporPagado.ToString());
                                                            }
                                                            if (LisTRecibo[a].iIdRenglon != 17)
                                                            {
                                                                xmlWriter.WriteAttributeString("ImporteGravado", ImporGra.ToString());
                                                            }
                                                            xmlWriter.WriteEndElement();
                                                        }
                                                        if (masivo == 3 && FinR == 0 && LisTRecibo[a].iIdRenglon != 27 && LisTRecibo[a].iIdRenglon != 28 && LisTRecibo[a].iIdRenglon != 199 && LisTRecibo[a].iIdRenglon != 29 && LisTRecibo[a].iIdRenglon != 990)
                                                        {

                                                            xmlWriter.WriteStartElement(Prefijo2, "Percepcion", EspacioDeNombreNomina);
                                                            xmlWriter.WriteAttributeString("ImporteExento", ImporExt.ToString());
                                                            xmlWriter.WriteAttributeString("TipoPercepcion", idSat);
                                                            xmlWriter.WriteAttributeString("Clave", IdRenglon);
                                                            xmlWriter.WriteAttributeString("Concepto", concepto.ToString());
                                                            if (LisTRecibo[a].iIdRenglon == 17)
                                                            {
                                                                xmlWriter.WriteAttributeString("ImporteGravado", IporPagado.ToString());
                                                            }
                                                            if (LisTRecibo[a].iIdRenglon != 17)
                                                            {
                                                                xmlWriter.WriteAttributeString("ImporteGravado", ImporGra.ToString());
                                                            }
                                                            xmlWriter.WriteEndElement();


                                                        }
                                                        if (masivo == 3 && FinR == 1 && (LisTRecibo[a].iIdRenglon == 27 || LisTRecibo[a].iIdRenglon == 28 || LisTRecibo[a].iIdRenglon == 29))
                                                        {

                                                            xmlWriter.WriteStartElement(Prefijo2, "Percepcion", EspacioDeNombreNomina);
                                                            xmlWriter.WriteAttributeString("ImporteExento", ImporExt.ToString());
                                                            xmlWriter.WriteAttributeString("TipoPercepcion", idSat);
                                                            xmlWriter.WriteAttributeString("Clave", IdRenglon);
                                                            xmlWriter.WriteAttributeString("Concepto", concepto.ToString());
                                                            if (LisTRecibo[a].iIdRenglon == 17)
                                                            {
                                                                xmlWriter.WriteAttributeString("ImporteGravado", IporPagado.ToString());
                                                            }
                                                            if (LisTRecibo[a].iIdRenglon != 17)
                                                            {
                                                                xmlWriter.WriteAttributeString("ImporteGravado", ImporGra.ToString());
                                                            }
                                                            xmlWriter.WriteEndElement();


                                                        }
                                                        if (LisTRecibo[a].iIdRenglon == 50)
                                                        {
                                                            xmlWriter.WriteStartElement(Prefijo2, "Percepcion", EspacioDeNombreNomina);
                                                            xmlWriter.WriteAttributeString("ImporteExento", ImporExt.ToString());
                                                            xmlWriter.WriteAttributeString("TipoPercepcion", idSat);
                                                            xmlWriter.WriteAttributeString("Clave", IdRenglon);
                                                            xmlWriter.WriteAttributeString("Concepto", concepto.ToString());

                                                            xmlWriter.WriteAttributeString("ImporteGravado", ImporGra.ToString());
                                                            xmlWriter.WriteStartElement(Prefijo2, "HorasExtra", EspacioDeNombreNomina);
                                                            xmlWriter.WriteAttributeString("ImportePagado", IporPagado.ToString());
                                                            int iHoras =  Convert.ToInt32(LisTRecibo[a].dHoras);
                                                            xmlWriter.WriteAttributeString("HorasExtra", Convert.ToString(iHoras));
                                                            if (iHoras > 9)
                                                            {
                                                                xmlWriter.WriteAttributeString("Dias", "9");
                                                            }
                                                            if (iHoras > 0 && iHoras < 10)
                                                            {
                                                                int dias = iHoras;
                                                                int resulado = 0;
                                                                Decimal resultadod = 0;

                                                                resulado = dias / 3;
                                                                resultadod = 0;
                                                                if (resulado == resultadod)
                                                                {
                                                                    resulado = resulado + 1;
                                                                    xmlWriter.WriteAttributeString("Dias", Convert.ToString(resulado));
                                                                }
                                                                if (dias / 3 != resultadod)
                                                                {

                                                                    xmlWriter.WriteAttributeString("Dias", Convert.ToString(resulado));
                                                                }

                                                            }
                                                            xmlWriter.WriteAttributeString("TipoHoras", "01");
                                                            xmlWriter.WriteEndElement();
                                                            xmlWriter.WriteEndElement();
                                                        }
                                                        if (LisTRecibo[a].iIdRenglon == 51)
                                                        {
                                                            xmlWriter.WriteStartElement(Prefijo2, "Percepcion", EspacioDeNombreNomina);
                                                            xmlWriter.WriteAttributeString("ImporteExento", ImporExt.ToString());
                                                            xmlWriter.WriteAttributeString("TipoPercepcion", idSat);
                                                            xmlWriter.WriteAttributeString("Clave", IdRenglon);
                                                            xmlWriter.WriteAttributeString("Concepto", concepto.ToString());
                                                            xmlWriter.WriteAttributeString("ImporteGravado", ImporGra.ToString());
                                                            xmlWriter.WriteStartElement(Prefijo2, "HorasExtra", EspacioDeNombreNomina);
                                                            xmlWriter.WriteAttributeString("ImportePagado", IporPagado.ToString());
                                                            int iHoras = Convert.ToInt32(LisTRecibo[a].dHoras);
                                                            xmlWriter.WriteAttributeString("HorasExtra", Convert.ToString(iHoras));

                                                            if (iHoras > 9)
                                                            {
                                                                xmlWriter.WriteAttributeString("Dias", "9");
                                                            }
                                                            if (iHoras > 0 && iHoras < 10)
                                                            {
                                                                int dias = iHoras;
                                                                int resulado = 0;
                                                                Decimal resultadod = 0;

                                                                resulado = dias / 4;
                                                                resultadod = 0;
                                                                if (resulado == resultadod)
                                                                {
                                                                    resulado = resulado + 1;
                                                                    xmlWriter.WriteAttributeString("Dias", Convert.ToString(resulado));
                                                                }
                                                                if (dias / 4 != resultadod)
                                                                {

                                                                    xmlWriter.WriteAttributeString("Dias", Convert.ToString(resulado));
                                                                }

                                                            }
                                                            xmlWriter.WriteAttributeString("TipoHoras", "01");
                                                            xmlWriter.WriteEndElement();
                                                            xmlWriter.WriteEndElement();
                                                        }

                                                    }
                                                    if (LisTRecibo[a].sValor == "Deducciones")
                                                    {

                                                        string IdRenglon = Convert.ToString(LisTRecibo[a].iIdRenglon);

                                                        if (IdRenglon == "1001")
                                                        {
                                                            Isr = LisTRecibo[a].dSaldo;
                                                            ISREs = 1;
                                                        }

                                                        if (IdRenglon == "1007")
                                                        {
                                                            Isr = Isr + LisTRecibo[a].dSaldo;
                                                            ISREs = 1;
                                                        }

                                                        if (masivo == 1 && FinR == 0)
                                                        {
                                                            if (IdRenglon == "1011")
                                                            {
                                                                Isr = LisTRecibo[a].dSaldo;
                                                                ISREs = 1;
                                                            }

                                                        }

                                                        if (masivo == 3 && FinR == 1)
                                                        {
                                                            if (IdRenglon == "1105")
                                                            {
                                                                Isr = Isr + LisTRecibo[a].dSaldo;
                                                                ISREs = 1;
                                                            }

                                                        }


                                                    }
                                                }

                                            }
                                            if ((ExtentoPer + Perpecio) != 0)
                                            {
                                                xmlWriter.WriteEndElement();

                                            }


                                            decimal Deduciones = 0;
                                            if (totalDeduciones.ToString() != " " || totalDeduciones.ToString() != "") { Deduciones = Convert.ToDecimal(totalDeduciones.ToString()); }
                                            string deduciones = string.Format("{0:N2}", Deduciones - Isr);
                                            string isr = string.Format("{0:N2}", Isr);
                                            deduciones = deduciones.Replace(",", "");
                                            isr = isr.Replace(",", "");


                                            // Deducciones

                                            
                                            if (totalDeduciones.ToString() != "0.00")
                                            {
                                                xmlWriter.WriteStartElement(Prefijo2, "Deducciones", EspacioDeNombreNomina);
                                            }                                       
                                            if (ISREs == 1)
                                            {
                                                string d = Convert.ToString(isr);
                                                if (d != "0.00")
                                                {
                                                    xmlWriter.WriteAttributeString("TotalImpuestosRetenidos", d);

                                                }
                                            }
                                            if (totalDeduciones.ToString() != "0.00")
                                            {
                                                xmlWriter.WriteAttributeString("TotalOtrasDeducciones", deduciones);
                                                if (LisTRecibo.Count > 0)
                                                {
                                                    for (int a = 0; a < LisTRecibo.Count; a++)
                                                    {
                                                        if (totalDeduciones.ToString() != "0.00")
                                                        {
                                                            if (LisTRecibo[a].sValor == "Deducciones")
                                                            {
                                                                string lengRenglon = "";
                                                                string ImporGra = string.Format("{0:N2}", LisTRecibo[a].dSaldo);
                                                                ImporGra = ImporGra.Replace(",", "");
                                                                string IdRenglon = Convert.ToString(LisTRecibo[a].iIdRenglon);


                                                                string concepto = LisTRecibo[a].sNombre_Renglon;

                                                                if (masivo == 3 && LisTRecibo[a].iIdRenglon == 1105 && FinR == 1)
                                                                {

                                                                    concepto = "ISR";
                                                                    IdRenglon = "1001";
                                                                }
                                                                if (masivo == 3 && LisTRecibo[a].iIdRenglon == 1011 && FinR == 0)
                                                                {

                                                                    concepto = "ISR";
                                                                    IdRenglon = "1001";

                                                                }
                                                                lengRenglon = Convert.ToString(LisTRecibo[a].sIdSat);
                                                                int idReglontama = IdRenglon.Length;
                                                                if (idReglontama == 1) { IdRenglon = "00" + IdRenglon; };
                                                                if (idReglontama == 2) { IdRenglon = "0" + IdRenglon; };
                                                                if (idReglontama == 3) { lengRenglon = "100"; };
                                                                string TipoDeduccion = "010";
                                                                if (IdRenglon == "1001") { TipoDeduccion = "002"; }

                                                                int Rangidsatdedu = 0;
                                                                Rangidsatdedu = LisTRecibo[a].sIdSat.ToString().Length;

                                                                if (Rangidsatdedu == 1) { TipoDeduccion = "00" + LisTRecibo[a].sIdSat; };
                                                                if (Rangidsatdedu == 2) { TipoDeduccion = "0" + LisTRecibo[a].sIdSat; };
                                                                if (Rangidsatdedu == 3) { TipoDeduccion = LisTRecibo[a].sIdSat.ToString(); };
                                                                if (IdRenglon == "1001") { TipoDeduccion = "002"; }

                                                                if (masivo != 3)
                                                                {
                                                                    if (LisTRecibo[a].iIdRenglon != 1201 && LisTRecibo[a].iIdRenglon != 1202 && LisTRecibo[a].iIdRenglon != 1203 && LisTRecibo[a].iIdRenglon != 1204)
                                                                    {
                                                                        xmlWriter.WriteStartElement(Prefijo2, "Deduccion", EspacioDeNombreNomina);
                                                                        xmlWriter.WriteAttributeString("Importe", ImporGra.ToString());
                                                                        xmlWriter.WriteAttributeString("TipoDeduccion", TipoDeduccion);
                                                                        xmlWriter.WriteAttributeString("Clave", IdRenglon);
                                                                        xmlWriter.WriteAttributeString("Concepto", concepto.ToString());
                                                                        xmlWriter.WriteEndElement();
                                                                    }
                                                                    if (LisTRecibo[a].iIdRenglon == 1201 && LisTRecibo[a].iIdRenglon == 1202 && LisTRecibo[a].iIdRenglon == 1203 && LisTRecibo[a].iIdRenglon == 1204)
                                                                    {
                                                                        xmlWriter.WriteStartElement(Prefijo2, "Deduccion", EspacioDeNombreNomina);
                                                                        xmlWriter.WriteAttributeString("ImporteExento", ImporGra.ToString());
                                                                        xmlWriter.WriteAttributeString("TipoDeduccion", TipoDeduccion);
                                                                        xmlWriter.WriteAttributeString("Clave", IdRenglon);
                                                                        xmlWriter.WriteAttributeString("Concepto", concepto.ToString());
                                                                        xmlWriter.WriteAttributeString("ImporteGravado", ImporGra.ToString());
                                                                        xmlWriter.WriteStartElement(Prefijo2, "Incapacidad", EspacioDeNombreNomina);
                                                                        int iDias = Convert.ToInt32(LisTRecibo[a].dHoras);

                                                                        xmlWriter.WriteAttributeString("Dias", Convert.ToString(iDias));
                                                                        xmlWriter.WriteEndElement();
                                                                        xmlWriter.WriteEndElement();

                                                                    }

                                                                }
                                                                if (masivo == 3 && FinR == 0)
                                                                {
                                                                    if (LisTRecibo[a].iIdRenglon != 1201 && LisTRecibo[a].iIdRenglon != 1202 && LisTRecibo[a].iIdRenglon != 1203 && LisTRecibo[a].iIdRenglon != 1204 && LisTRecibo[a].iIdRenglon != 1005)
                                                                    {
                                                                        xmlWriter.WriteStartElement(Prefijo2, "Deduccion", EspacioDeNombreNomina);
                                                                        xmlWriter.WriteAttributeString("Importe", ImporGra.ToString());
                                                                        xmlWriter.WriteAttributeString("TipoDeduccion", TipoDeduccion);
                                                                        xmlWriter.WriteAttributeString("Clave", IdRenglon);
                                                                        xmlWriter.WriteAttributeString("Concepto", concepto.ToString());
                                                                        xmlWriter.WriteEndElement();
                                                                    }
                                                                    if (LisTRecibo[a].iIdRenglon == 1201 && LisTRecibo[a].iIdRenglon == 1202 && LisTRecibo[a].iIdRenglon == 1203 && LisTRecibo[a].iIdRenglon == 1204 && LisTRecibo[a].iIdRenglon != 1005)
                                                                    {
                                                                        xmlWriter.WriteStartElement(Prefijo2, "Deduccion", EspacioDeNombreNomina);
                                                                        xmlWriter.WriteAttributeString("ImporteExento", ImporGra.ToString());
                                                                        xmlWriter.WriteAttributeString("TipoDeduccion", TipoDeduccion);
                                                                        xmlWriter.WriteAttributeString("Clave", IdRenglon);
                                                                        xmlWriter.WriteAttributeString("Concepto", concepto.ToString());

                                                                        xmlWriter.WriteAttributeString("ImporteGravado", ImporGra.ToString());

                                                                        xmlWriter.WriteStartElement(Prefijo2, "Incapacidad", EspacioDeNombreNomina);
                                                                        int iDias = Convert.ToInt32(LisTRecibo[a].dHoras);

                                                                        xmlWriter.WriteAttributeString("Dias", Convert.ToString(iDias));
                                                                        xmlWriter.WriteEndElement();
                                                                        xmlWriter.WriteEndElement();

                                                                    }

                                                                }
                                                                if (masivo == 3 && FinR == 1)
                                                                {
                                                                    if (LisTRecibo[a].iIdRenglon == 1005)
                                                                    {
                                                                        xmlWriter.WriteStartElement(Prefijo2, "Deduccion", EspacioDeNombreNomina);
                                                                        xmlWriter.WriteAttributeString("Importe", ImporGra.ToString());
                                                                        xmlWriter.WriteAttributeString("TipoDeduccion", TipoDeduccion);
                                                                        xmlWriter.WriteAttributeString("Clave", IdRenglon);
                                                                        xmlWriter.WriteAttributeString("Concepto", concepto.ToString());
                                                                        xmlWriter.WriteEndElement();
                                                                    }
                                                                }
                                                            }
                                                        }

                                                    }

                                                }

                                            }
                                            if (totalDeduciones.ToString() != "0.00")
                                            {
                                                xmlWriter.WriteEndElement();
                                            }
                                            if (LisTDiasEmple != null)
                                            {
                                                
                                                for (int c = 0; c < LisTDiasEmple.Count; c++)
                                                {
                                                   
                                                    if (LisTDiasEmple[c].iIdRenglon == 31)
                                                    {
                                                        string dias = Convert.ToString(LisTDiasEmple[c].iDiasTrab);
                                                        string[] Dias2 = dias.Split('.');
                                                        sDiasEfectivos = Dias2[0];
                                                    }
                                                    if (LisTDiasEmple[c].iIdRenglon == 34)
                                                    {
                                                        string dias = Convert.ToString(LisTDiasEmple[c].iDiasTrab);
                                                        string[] Dias2 = dias.Split('.');
                                                        sDiasIncapa = Dias2[0];
                                                        
                                                    }
                                                }
                                               
                                            }                                        
                                            if (LisTDiasEmple == null)
                                            {
                                                sDiasEfectivos = Convert.ToString(LFechaPerido[0].iDiasEfectivos);
                                            }

             
                                            if (ListDatEmisor[0].iPagopor != 364)
                                            {
                                                xmlWriter.WriteStartElement(Prefijo2, "OtrosPagos", EspacioDeNombreNomina);
                                                xmlWriter.WriteStartElement(Prefijo2, "OtroPago", EspacioDeNombreNomina);
                                                if (row198 > 0 && row198Exit == 1)
                                                {
                                                    xmlWriter.WriteAttributeString("TipoOtroPago", "002");
                                                    xmlWriter.WriteAttributeString("Clave", "198");

                                                }
                                                if (row198 == 0 && row198Exit == 0)
                                                {
                                                    xmlWriter.WriteAttributeString("TipoOtroPago", "002");
                                                    xmlWriter.WriteAttributeString("Clave", "198");
                                                }
                                                if (row198 == 0 && row198Exit == 1)
                                                {
                                                    xmlWriter.WriteAttributeString("TipoOtroPago", "002");
                                                    if (sDiasEfectivos == "0")
                                                    {
                                                        xmlWriter.WriteAttributeString("Clave", "002");

                                                    }
                                                    if (sDiasEfectivos != "0")
                                                    {
                                                        xmlWriter.WriteAttributeString("Clave", "198");

                                                    }


                                                }
                                                xmlWriter.WriteAttributeString("Concepto", "Subsidio al Empleado");
                                                if (row198 == 0 && row198Exit == 0)
                                                {
                                                    xmlWriter.WriteAttributeString("Importe", string.Format("{0:0.00}", 0));
                                                    xmlWriter.WriteStartElement(Prefijo2, "SubsidioAlEmpleo", EspacioDeNombreNomina);
                                                    xmlWriter.WriteAttributeString("SubsidioCausado", string.Format("{0:0.00}", 0));
                                                    xmlWriter.WriteEndElement();
                                                    xmlWriter.WriteEndElement();
                                                };
                                                if (row198 == 0 && row198Exit == 1)
                                                {
                                                    xmlWriter.WriteAttributeString("Importe", string.Format("{0:0.00}", LisTRecibo[row198].dSaldo));
                                                    xmlWriter.WriteStartElement(Prefijo2, "SubsidioAlEmpleo", EspacioDeNombreNomina);
                                                    xmlWriter.WriteAttributeString("SubsidioCausado", string.Format("{0:0.00}", LisTRecibo[row198].dSaldo + LisTRecibo[row198].dExcento));
                                                    xmlWriter.WriteEndElement();
                                                    xmlWriter.WriteEndElement();
                                                };
                                                if (row198 > 0 && row198Exit == 1)
                                                {
                                                    xmlWriter.WriteAttributeString("Importe", string.Format("{0:0.00}", LisTRecibo[row198].dSaldo));
                                                    xmlWriter.WriteStartElement(Prefijo2, "SubsidioAlEmpleo", EspacioDeNombreNomina);
                                                    xmlWriter.WriteAttributeString("SubsidioCausado", string.Format("{0:0.00}", LisTRecibo[row198].dSaldo + LisTRecibo[row198].dExcento));
                                                    xmlWriter.WriteEndElement();
                                                    xmlWriter.WriteEndElement();
                                                };

                                                row198Exit = 0;
                                                if (row17 > 0)
                                                {
                                                    xmlWriter.WriteStartElement(Prefijo2, "OtroPago", EspacioDeNombreNomina);
                                                    xmlWriter.WriteAttributeString("TipoOtroPago", "999");
                                                    xmlWriter.WriteAttributeString("Clave", "017");
                                                    xmlWriter.WriteAttributeString("Concepto", Convert.ToString(LisTRecibo[row17].sNombre_Renglon));
                                                    xmlWriter.WriteAttributeString("Importe", string.Format("{0:0.00}", LisTRecibo[row17].dSaldo));
                                                    xmlWriter.WriteEndElement();

                                                    row17 = 0;
                                                };
                                                if (row113 > 0)
                                                {
                                                    xmlWriter.WriteStartElement(Prefijo2, "OtroPago", EspacioDeNombreNomina);
                                                    xmlWriter.WriteAttributeString("TipoOtroPago", "999");
                                                    xmlWriter.WriteAttributeString("Clave", "113");
                                                    xmlWriter.WriteAttributeString("Concepto", Convert.ToString(LisTRecibo[row113].sNombre_Renglon));
                                                    xmlWriter.WriteAttributeString("Importe", string.Format("{0:0.00}", LisTRecibo[row113].dSaldo));
                                                    xmlWriter.WriteEndElement();
                                                    row113 = 0;
                                                };
                                                if (row199 > 0 && rowExit199==1)
                                                {
                                                    xmlWriter.WriteStartElement(Prefijo2, "OtroPago", EspacioDeNombreNomina);
                                                    xmlWriter.WriteAttributeString("TipoOtroPago", "001");
                                                    xmlWriter.WriteAttributeString("Clave", "199");
                                                    xmlWriter.WriteAttributeString("Concepto", Convert.ToString(LisTRecibo[row199].sNombre_Renglon));
                                                    xmlWriter.WriteAttributeString("Importe", string.Format("{0:0.00}", LisTRecibo[row199].dSaldo));
                                                    xmlWriter.WriteEndElement();
                                                    row199 = 0;
                                                    rowExit199 = 0;
                                                };
                                                if (row227 > 0)
                                                {
                                                    xmlWriter.WriteStartElement(Prefijo2, "OtroPago", EspacioDeNombreNomina);
                                                    xmlWriter.WriteAttributeString("TipoOtroPago", "999");
                                                    xmlWriter.WriteAttributeString("Clave", "227");
                                                    xmlWriter.WriteAttributeString("Concepto", Convert.ToString(LisTRecibo[row227].sNombre_Renglon));
                                                    xmlWriter.WriteAttributeString("Importe", string.Format("{0:0.00}", LisTRecibo[row227].dSaldo));
                                                    xmlWriter.WriteEndElement();
                                                    row227 = 0;
                                                };
                                                if (row467 > 0)
                                                {
                                                    xmlWriter.WriteStartElement(Prefijo2, "OtroPago", EspacioDeNombreNomina);
                                                    xmlWriter.WriteAttributeString("TipoOtroPago", "999");
                                                    xmlWriter.WriteAttributeString("Clave", "467");
                                                    xmlWriter.WriteAttributeString("Concepto", Convert.ToString(LisTRecibo[row467].sNombre_Renglon));
                                                    xmlWriter.WriteAttributeString("Importe", string.Format("{0:0.00}", LisTRecibo[row467].dSaldo));
                                                    xmlWriter.WriteEndElement();
                                                    row467 = 0;
                                                };

                                                xmlWriter.WriteEndElement();
                                                xmlWriter.WriteEndElement();

                                            };
                                            if ((ExtentoPer + Perpecio) == 0 && sDiasEfectivos == "0")
                                            {
                                                xmlWriter.WriteStartElement(Prefijo2, "Incapacidades", EspacioDeNombreNomina);
                                                xmlWriter.WriteStartElement(Prefijo2, "Incapacidad", EspacioDeNombreNomina);
                                                xmlWriter.WriteAttributeString("ImporteMonetario", "0");
                                                xmlWriter.WriteAttributeString("TipoIncapacidad", "02");
                                                xmlWriter.WriteAttributeString("DiasIncapacidad", sDiasIncapa);
                                                xmlWriter.WriteEndElement();
                                                xmlWriter.WriteEndElement();

                                            }
                                            if ((ExtentoPer + Perpecio) == 0 && sDiasEfectivos == "0")
                                            {
                                                xmlWriter.WriteEndElement();

                                            }
                                            if ((ExtentoPer + Perpecio) != 0 && sDiasEfectivos != "0")
                                            {
                                                xmlWriter.WriteEndElement();

                                            }
                                            xmlWriter.WriteEndElement();
                                            //Cerrar
                                            xmlWriter.Flush();
                                            xmlWriter.Close();

                                            if (Recibo == 1) {
                                                Dao.sp_Tsellos_InsertUPdate_TSellosSat(0, IdCalcHD, IdEmpresa, NumEmpleado, anios, Tipodeperido, iperiodo, "Nomina", " ", " ", " ", " ", " ", " ");

                                            }
                                            if (Recibo == 2) {
                                                Dao.sp_Tsellos_InsertUPdate_TSellosSat(0, IdCalcHD,ListDatEmisor[0].iIdEmpresa, NumEmpleado, anios, Tipodeperido, iperiodo, "Nomina R2", " ", " ", " ", " ", " ", " ");
                                            }

                                            //FileCadenaXslt = path + "cadenaoriginal_3_3.xslt";
                                            string Cadena = LibreriasFacturas.GetCadenaOriginal(ArchivoXmlFile, FileCadenaXslt, pathCer);
                                            string selloDigitalOriginal = LibreriasFacturas.ObtenerSelloDigital(Cadena, s_certificadoKey, s_transitorio);
                                            LibreriasFacturas.AplicarSelloDigital(selloDigitalOriginal, ArchivoXmlFile);

                                            // Quitar el BOM
                                            string line;
                                            StreamReader sr = new StreamReader(ArchivoXmlFile);
                                            StreamWriter sw = new StreamWriter(path + NomArch + ".xml", false, new UTF8Encoding(false));

                                            //Read the first line of text
                                            line = sr.ReadLine();

                                            //Continue to read until you reach end of file
                                            while (line != null)
                                            {
                                                //write the line
                                                //line = line.Replace("xmlns_nomina", "xmlns:nomina");
                                                line = line.Replace("utf-8", "UTF-8");
                                                sw.WriteLine(line);     //Read the next line
                                                line = sr.ReadLine();
                                            }

                                            //close the file
                                            sr.Close();
                                            sw.Close();
                                            File.Delete(ArchivoXmlFile);

                                            if (masivo == 3 && Recibo2 == 1 && FinR == 0)
                                            {
                                                NoXmlx = NoXmlx + 1;
                                                // NomArch = NomArch + "R2";
                                                FinR = 1;
                                            }

                                        }
                                        else
                                        {
                                            //continua mi flujo
                                            ListDatEmisor[0].sMensaje = "NorCert";

                                        }



                                    }
                                    if (LisCer.Count < 1)
                                    {
                                        ListDatEmisor[0].sMensaje = "NorCert";
                                    }
                                }
                                if (LisCer is null)
                                {
                                    ListDatEmisor[0].sMensaje = "NorCert";
                                }


                                //Borra archivo temporal
                                // 1 
                                //DirectoryInfo dir = new DirectoryInfo(@"C:\reportes\");
                                // 2 
                                string nombreArchivoZip = pathCer.Replace("certificados\\", "") + "ZipXML.zip";


                                if (System.IO.File.Exists(nombreArchivoZip))
                                {
                                    System.IO.File.Delete(nombreArchivoZip);
                                    TipoFiniquito ls = new TipoFiniquito();

                                }
                                FileStream stream = new FileStream(nombreArchivoZip, FileMode.OpenOrCreate);
                                // 3 
                                ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create);
                                // 4 
                                System.IO.DirectoryInfo directorio = new System.IO.DirectoryInfo(path);
                                FileInfo[] sourceFiles = directorio.GetFiles("*" + ".xml");
                                foreach (FileInfo sourceFile in sourceFiles)
                                {
                                    // 5 
                                    Stream sourceStream = sourceFile.OpenRead();
                                    // 6 
                                    ZipArchiveEntry entry = archive.CreateEntry(sourceFile.Name);
                                    // 7 
                                    Stream zipStream = entry.Open();
                                    // 8 
                                    sourceStream.CopyTo(zipStream);
                                    // 9 
                                    zipStream.Close();
                                    sourceStream.Close();
                                }

                                // 10 
                                archive.Dispose();
                                stream.Close();
                                // descargar zip         

                            }
                        }

                    };


                };
                string[] xmlList = Directory.GetFiles(path, "*.xml");

                foreach (string f in xmlList)
                {
                    System.IO.File.Delete(f);

                }
            }
          
            return ListDatEmisor;
        }


        public List<string> Archivo()
        {

            List<string> zips = new List<string>();

            return zips;
        }

        /// trae datos de sello del Sat

        public List<SelloSatBean> sp_DatosSat_Retrieve_TSellosSat(int CtrliIdEmpresa, int Ctrlianio, int CtrliTipoPeriodo, int CtrliPeriodo, int CtrliIdEmpleado) {
            List<SelloSatBean> Lsat = new List<SelloSatBean>();

            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_DatosSat_Retrieve_TSellosSat", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpresa", CtrliIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@Ctrlianio", Ctrlianio));
                cmd.Parameters.Add(new SqlParameter("@CtrliTipoPeriodo", CtrliTipoPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliPeriodo", CtrliPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpleado", CtrliIdEmpleado));
            
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        SelloSatBean ls = new SelloSatBean();
                        {
                            ls.sSelloSat = data["Sello_sat"].ToString();
                            ls.Fecha = data["Fecha"].ToString();
                            ls.sUUID = data["UUID"].ToString();
                            ls.sSelloCFD = data["SelloCFD"].ToString();
                            ls.Rfcprov = data["RfcProvCertif"].ToString();
                            ls.sNoCertificado = data["NoCertificadoSAT"].ToString();
                            ls.Fechatimbrado = data["FechaTimbrado"].ToString();
                            
                        }
                        Lsat.Add(ls);
                    }
                }
                else
                {
                    Lsat = null;
                }
                data.Close(); conexion.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }

            return Lsat;
        }

    // 

        /// insertar ccontro de ejecucuionHd
        
        public List<ControlEjecucionBean> ps_ControlEje_Insert_CControlEjecEmpr(int CtrliIdusuario, string CtrlsDescripcion, int CtrliInactivo,int CtrliIdEmpresa, int CtrliAnio, int CtrliIdtipoPeriodo,int CtrliIdPeriodo,int CtrliRecibo,int CtrliNoEje)
        {
            List<ControlEjecucionBean> bean = new List<ControlEjecucionBean>();

            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("ps_ControlEje_Insert_CControlEjecEmpr", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliIdusuario", CtrliIdusuario));
                cmd.Parameters.Add(new SqlParameter("@CtrlsDescripcion", CtrlsDescripcion));
                cmd.Parameters.Add(new SqlParameter("@CtrliInactivo", CtrliInactivo));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpresa", CtrliIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@CtrliAnio", CtrliAnio));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdtipoPeriodo", CtrliIdtipoPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdPeriodo", CtrliIdPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliRecibo", CtrliRecibo));
                cmd.Parameters.Add(new SqlParameter("@CtrliNoEje", @CtrliNoEje));
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        ControlEjecucionBean ls = new ControlEjecucionBean();

                        ls.iIdContro = int.Parse(data["Control_id"].ToString());
                        ls.sMensaje = "succes";
                        bean.Add(ls);
                    }
                }
                else
                {
                    bean = null;
                }


                cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return bean;
        }

        /// consulta los emplados que tengan finiquito
        public List<EmpleadosEmpresaBean> sp_EmpleadosFiniquito_Retrieve_Tfiniquito_hst(int CtrliIdEmpresa, int CtrliPeriodo, int Ctrlianio,int CtrliIdEmpleado, int CtrliTipoperiodo)
        {
            List<EmpleadosEmpresaBean> list = new List<EmpleadosEmpresaBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_EmpleadosFiniquito_Retrieve_Tfiniquito_hst", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpresa", CtrliIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@Ctrlianio", Ctrlianio));
                cmd.Parameters.Add(new SqlParameter("@CtrliTipoperiodo", CtrliTipoperiodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliPeriodo", CtrliPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpleado", CtrliIdEmpleado));
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        EmpleadosEmpresaBean ls = new EmpleadosEmpresaBean();

                        ls.iIdEmpleado = int.Parse(data["Exite"].ToString());
                        list.Add(ls);
                    }
                }
                else
                {
                    EmpleadosEmpresaBean ls = new EmpleadosEmpresaBean();

                    ls.iIdEmpleado = 0;
                    list.Add(ls);
                }
                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return list;
        }

        /// consulta llista de finiquito 
        /// 
        /// consulta los emplados que tengan finiquito
        public List<TipoFiniquito> sp_TpFiniquitosEmpleado_Retrieve_TFiniquito(int CtrliIdEmpresa,int CtrliIdEmpleado,  int CtrliAnio, int CtrliPeriodo)
        {
            List<TipoFiniquito> list = new List<TipoFiniquito>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_TpFiniquitosEmpleado_Retrieve_TFiniquito", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpresa", CtrliIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpleado", CtrliIdEmpleado));
                cmd.Parameters.Add(new SqlParameter("@CtrliAnio", CtrliAnio));
                cmd.Parameters.Add(new SqlParameter("@CtrliPeriodo", CtrliPeriodo));
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        TipoFiniquito ls = new TipoFiniquito();

                        ls.iIdTipoFiniquito = int.Parse(data["Tipo_finiquito_id"].ToString());
                        ls.sNombreFiniquito = data["Descripcion"].ToString();
                        ls.sMensaje= "success";
                        list.Add(ls);
                    }
                }
                else
                {
                    TipoFiniquito ls = new TipoFiniquito();
                    ls.sMensaje = "error";
                    list.Add(ls);
                }
                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return list;
        }

        // datos del perido de la empresa 
        public List<CInicioFechasPeriodoBean> sp_DatPeridoEmpresa(int CtrliIdEmpresa, int CtrliIdTipoPeriodo, int CtrliIdAnio, int CtrliIdPeriodo)
        {
            List<CInicioFechasPeriodoBean> list = new List<CInicioFechasPeriodoBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_DatPeridoEmpresa", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpresa", CtrliIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdTipoPeriodo", CtrliIdTipoPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdAnio", CtrliIdAnio));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdPeriodo", CtrliIdPeriodo));
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        CInicioFechasPeriodoBean LP = new CInicioFechasPeriodoBean();
                        {
                            LP.iId = int.Parse(data["Id"].ToString());
                            LP.sNominaCerrada = data["Nomina_Cerrada"].ToString();
                            LP.sFechaInicio = data["Fecha_Inicio"].ToString();
                            LP.sFechaFinal = data["Fecha_Final"].ToString();
                            LP.sFechaProceso = data["Fecha_Proceso"].ToString();
                            LP.sFechaPago = data["Fecha_Pago"].ToString();
                            LP.iDiasEfectivos = int.Parse(data["Dias_Efectivos"].ToString());
                            LP.iPeriodo = int.Parse(data["Periodo"].ToString());
                        };

                        list.Add(LP);
                    }
                }
                else
                {
                    list = null;
                }

                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return list;

        }

        /// inserta datos en la tabla de CControl_ejecionLn 

        public ControlEjecucionBean sp_CControlEjeLn_insert_CControlEjeLn(int CtrliIdControl , int CtrliIdEmpresa, int CtrliInactivo, int CtrliAnio, int CtrliIdTipoPeriodo,int CtrliIdPeriodo, int CtrliRecibo)
        {
            ControlEjecucionBean bean = new ControlEjecucionBean();

            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_CControlEjeLn_insert_CControlEjeLn", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliIdControl", CtrliIdControl));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpresa", CtrliIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@CtrliInactivo", CtrliInactivo));
                cmd.Parameters.Add(new SqlParameter("@CtrliAnio", CtrliAnio));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdTipoPeriodo", CtrliIdTipoPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdPeriodo", CtrliIdPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliRecibo", CtrliRecibo));
                if (cmd.ExecuteNonQuery() > 0)
                {
                    bean.sMensaje = "success";
                }
                else
                {
                    bean.sMensaje = "error";
                }
                cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return bean;
        }

        /// inserta datos en la tabla de CControl_ejecionLn 

        public SelloSatBean  sp_CCejecucionAndSen_update_TsellosSat(int CtrliIdEmpresa,int CtrliIdEmpleado, int CtrliAnio, int CtrliTipoperido, int CtrliPeriodo, int CtriliOpcione,string CtrlsUurl)
        {
            SelloSatBean bean = new SelloSatBean();

            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_CCejecucionAndSen_update_TsellosSat", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpresa", CtrliIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@CtrliIdEmpleado", CtrliIdEmpleado)); 
                cmd.Parameters.Add(new SqlParameter("@CtrliAnio", CtrliAnio));
                cmd.Parameters.Add(new SqlParameter("@CtrliTipoperido", CtrliTipoperido));
                cmd.Parameters.Add(new SqlParameter("@CtrliPeriodo", CtrliPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtriliOpcione", CtriliOpcione));
                cmd.Parameters.Add(new SqlParameter("@CtrlsUurl", CtrlsUurl));

                if (cmd.ExecuteNonQuery() > 0)
                {
                    bean.sMensaje = "success";
                }
                else
                {
                    bean.sMensaje = "error";
                }
                cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return bean;
        }

        // tra los datos de la fecha mayo de la tabla ejecc

        public List<ControlEjecucionBean> sp_UltimaEje_Retrieve_CControlejecEmpr()
        {
            List<ControlEjecucionBean> list = new List<ControlEjecucionBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_UltimaEje_Retrieve_CControlejecEmpr", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //cmd.Parameters.Add(new SqlParameter("@ctrlNombreEmpresa", txt));
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        ControlEjecucionBean ls = new ControlEjecucionBean();
                        {
                            ls.iIdContro = int.Parse(data["IdControl"].ToString()); 
                            ls.sDescripcion = data["Descripcion"].ToString();
                            ls.iIdempresa = int.Parse(data["Empresa_id"].ToString());
                            ls.iTipoPeriodo = int.Parse(data["TipoPerido_id"].ToString());
                            ls.iPeriodo = int.Parse(data["Perido_id"].ToString());
                            ls.iAnio = int.Parse(data["Anio"].ToString());
                            ls.iRecibo = int.Parse(data["Recibo"].ToString());
                            ls.iNoEje = int.Parse(data["NoEjecutados"].ToString());
                            ls.sMensaje = "succes";
                        };
                        list.Add(ls);
                    }
                }
                else
                {
                    ControlEjecucionBean ls = new ControlEjecucionBean();
                    {
                        ls.sMensaje = "Not Dat";
                    };
                    list.Add(ls);
                }
                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return list;
        }

        /// Consulta los pdfconsellos ejecutados y enviados 

        public  List<SelloSatBean> sp_EjectadosAndSend_Retrieve_TSelloSat(int CtrliIdempresa, int CtrliAnio, int CtrliTipoPeriodo, int CtrliPeriodo, int CtrliEjecutado, int CtrlImensaje,int CtrliOpc,int CtrliRecibo)
        {
           List<SelloSatBean> bean = new List<SelloSatBean>();

            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_EjectadosAndSend_Retrieve_TSelloSat", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliIdempresa", CtrliIdempresa));
                cmd.Parameters.Add(new SqlParameter("@CtrliAnio", CtrliAnio));
                cmd.Parameters.Add(new SqlParameter("@CtrliTipoPeriodo", CtrliTipoPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliPeriodo", CtrliPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliEjecutado", CtrliEjecutado));
                cmd.Parameters.Add(new SqlParameter("@CtrliRecibo", CtrliRecibo));
                
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        SelloSatBean LP = new SelloSatBean();
                        {
                            LP.iIdEmpresa = int.Parse(data["Empresa_id"].ToString());
                            LP.iIdEmpleado = int.Parse(data["Empleado_id"].ToString());
                            LP.sNomEmpleado = data["Empleado_id"].ToString() + " " + data["Empleado"].ToString();
                            LP.sNombre = data["Nombre_Empleado"].ToString();
                            LP.ianio = int.Parse(data["Anio"].ToString());
                            LP.iTipoPeriodo = int.Parse(data["Tipo_Periodo_id"].ToString());
                            LP.iPeriodo = int.Parse(data["Periodo"].ToString());
                            LP.bEmailSent = "";
                            if (data["Email_Sent"].ToString() == "True")
                            {
                                LP.bEmailSent = "Enviado";
                            }
                            if (data["Email_Sent"].ToString() == "False")
                            {
                                LP.bEmailSent = "No enviado";
                            }


                            if (data["Correo_Electronico"].ToString() != "" || data["Correo_Electronico"].ToString() != " ")
                            {
                                LP.sEmailPErsona = data["Correo_Electronico"].ToString();
                            }
                            else { LP.sEmailSent = ""; }
                            if (data["Recibo_Simple"].ToString() != null) { LP.sUurReciboSim = data["Recibo_Simple"].ToString(); }
                            else { LP.sUurReciboSim = " "; };
                            if (data["Recibo_Fiscal"].ToString() != null) { LP.sUrllReciboFis = data["Recibo_Fiscal"].ToString(); } else { LP.sUrllReciboFis = " "; };
                            if (data["Recibo2"].ToString() != null) { LP.sUrllRecibo2 = data["Recibo2"].ToString(); } else { LP.sUrllRecibo2 = " "; };

                            LP.sEmailSendSim = "";
                            if (data["Email_Sent_simple"].ToString() == "True")
                            {
                                LP.sEmailSendSim = "Enviado";
                            }
                            if (data["Email_Sent_simple"].ToString() == "False")
                            {
                                LP.sEmailSendSim = "No enviado";
                            }
                            if (data["Email_sent_Recibo2"].ToString() == "False")
                            {
                                LP.sEmailSendRecibo2 = "No enviado";
                            }
                            if (data["Email_sent_Recibo2"].ToString() == "True")
                            {
                                LP.sEmailSendRecibo2 = "Enviado";
                            }
                            LP.sEmailEmpresa = data["EmailEmpresa"].ToString();
                            LP.sPassword = data["Password"].ToString();
                            LP.sMensaje = "Succes";
          
                        };

                        bean.Add(LP);
                    }
                }
                else
                {
                    SelloSatBean LP = new SelloSatBean();
                    {
                        
                        LP.sMensaje = "NoDat";

                    };

                    bean.Add(LP);
                };
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return bean;
        }

        /// Empleados Finiquitos
        public List<EmpleadosEmpresaBean> sp_EmpledoFi_Retrieve_TFiniquito(int CtrliIdEmpresa, int CtrliPeriodo, int Ctrlianio)
        {
            List<EmpleadosEmpresaBean> list = new List<EmpleadosEmpresaBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_EmpledoFi_Retrieve_TFiniquito", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliEmpleado", CtrliIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@CtrliAnio", Ctrlianio));
                cmd.Parameters.Add(new SqlParameter("@CtrliPeriodo", CtrliPeriodo));
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        EmpleadosEmpresaBean ls = new EmpleadosEmpresaBean();

                        ls.iIdEmpleado = int.Parse(data["Empleado_id"].ToString());
                        ls.sNombreCompleto = int.Parse(data["Empleado_id"].ToString()) +" "+  data["NombreCompleto"].ToString();
                        list.Add(ls);
                    }
                }
                else
                {
                    list = null;
                }
                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            return list;
        }

        /// totales de finituito
        /// 
        public List<ReciboNominaBean> Sp_TotalesFiniquito_Retrieve_TFiniquitoLn(int CtrlIdEmpresa, int CtrlIdEmpleado, int CtrlPeriodo, int CtrliAnio, int CtrliOpcio)
        {
            List<ReciboNominaBean> list = new List<ReciboNominaBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("Sp_TotalesFiniquito_Retrieve_TFiniquitoLn", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliEmpresaid", CtrlIdEmpresa));
                cmd.Parameters.Add(new SqlParameter("@CtrliAnio",CtrliAnio));
                cmd.Parameters.Add(new SqlParameter("@CtrliEmpleado",CtrlIdEmpleado));
                cmd.Parameters.Add(new SqlParameter("@CtrliPeriodo",CtrlPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliOpcio",CtrliOpcio));
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        ReciboNominaBean ls = new ReciboNominaBean();
                        {
                           
                            ls.iIdCalculoshd = int.Parse(data["Finiquito_id"].ToString());
                            ls.iIdRenglon = int.Parse(data["Renglon_id"].ToString());
                            ls.dSaldo = decimal.Parse(data["Saldo"].ToString());
                            ls.dGravado = decimal.Parse(data["Gravado"].ToString());
                            ls.dExcento = decimal.Parse(data["Excento"].ToString());
                            if (CtrliOpcio==1) { ls.sValor = data["Valor"].ToString();
                                ls.sNombre_Renglon = data["Nombre_Renglon"].ToString();
                                ls.sIdSat = int.Parse(data["Codigo"].ToString());
                            }
                                         
                        }

                        list.Add(ls);
                    }

                }
                else
                {
                    list = null;
                }
                data.Close(); conexion.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CrtliIdEmpresa"></param>
        /// <param name="CrtliIdEmpleado"></param>
        /// <returns></returns>
        public List<EmisorReceptorBean> Sp_EmpresaDir_Retrieve(string CtrlsRFCEmp)
        {
            List<EmisorReceptorBean> list = new List<EmisorReceptorBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("Sp_EmpresaDir_Retrieve", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrlsRFCEmp", CtrlsRFCEmp));
                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        EmisorReceptorBean ls = new EmisorReceptorBean();
                        ls.sMensaje = "success";
                        if (data["Direc"].ToString() == null)
                        {
                            ls.sDomiciolioEmple = "CEmpresas";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sDomiciolioEmple = data["Direc"].ToString();
                        }

                        if (data["Direc2"].ToString() == null)
                        {
                            ls.sDomiciolioEmpre = "CEmpresas";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sDomiciolioEmpre = data["Direc2"].ToString();
                        }



                        list.Add(ls);
                    }
                }
                else
                {
                    list = null;
                }
                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            return list;
        }

        // sp de Archivo TRecibosSat
        public List<TsellosBean> sp_Recibos_Retrieve_TsellosSat(int CtrliIdempresa, int EmpleadoID, int CtrliAnio, int CtrliTipoPeriodo, int CtrliPeriodo, int CtrliRecibo)
        {
            List<TsellosBean> bean = new List<TsellosBean>();

            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Recibos_Retrieve_TsellosSat", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliEmpresa", CtrliIdempresa));
                cmd.Parameters.Add(new SqlParameter("@CtrliEmple", EmpleadoID));
                cmd.Parameters.Add(new SqlParameter("@CtrliAnio", CtrliAnio));
                cmd.Parameters.Add(new SqlParameter("@CtrliTipoPerio", CtrliTipoPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliPeriodo", CtrliPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliRecibo", CtrliRecibo));

                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        TsellosBean LP = new TsellosBean();
                        {


                            LP.sMensaje = "Succes";
                            if (data["Recibo_Simple"].ToString() == "" || data["Recibo_Simple"].ToString() == " " || data["Recibo_Simple"].ToString() == null)
                            {
                                if (CtrliRecibo == 1)
                                {
                                    LP.sMensaje = "NoDat";
                                }

                            }
                            if (data["Recibo_Simple"].ToString() != "" || data["Recibo_Simple"].ToString() != " " || data["Recibo_Simple"].ToString() != null)
                            {
                                LP.sURreciboSimple = data["Recibo_Simple"].ToString();
                            }
                            if (data["Recibo_Fiscal"].ToString() == "" || data["Recibo_Fiscal"].ToString() == " " || data["Recibo_Fiscal"].ToString() == null)
                            {

                                if (CtrliRecibo == 2)
                                {
                                    LP.sMensaje = "NoDat";
                                }
                            }
                            if (data["Recibo_Fiscal"].ToString() != "" || data["Recibo_Fiscal"].ToString() != " " || data["Recibo_Fiscal"].ToString() != null)
                            {
                                LP.sURreciboFiscal = data["Recibo_Fiscal"].ToString();
                            }

                            if (data["Recibo2"].ToString() == "" || data["Recibo2"].ToString() == " " || data["Recibo2"].ToString() == null)
                            {

                                if (CtrliRecibo == 3)
                                {
                                    LP.sMensaje = "NoDat";
                                }
                            }

                            if (data["Recibo2"].ToString() != "" || data["Recibo2"].ToString() != " " || data["Recibo2"].ToString() != null)
                            {
                                LP.sURrecibo2 = data["Recibo2"].ToString();
                            }


                        };

                        bean.Add(LP);
                    }
                }
                else
                {
                    TsellosBean LP = new TsellosBean();
                    {

                        LP.sMensaje = "NoDat";

                    };

                    bean.Add(LP);
                };
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            return bean;
        }

        // ps Emisor Recep Empresa destino Recibo2

        public List<EmisorReceptorBean> sp_Retrieve_EmisorRecepEmpOrigen(int CtrliEmpresa, int CtrliAnio,int CtrliTiPeriodo,int CtrliPeriodo, int CtrliEmpleado, int CtrliEspejo)
        {
            List<EmisorReceptorBean> list = new List<EmisorReceptorBean>();
            try
            {
                this.Conectar();
                SqlCommand cmd = new SqlCommand("sp_Retrieve_EmisorRecepEmpOrigen", this.conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@CtrliEmpresa", CtrliEmpresa));
                cmd.Parameters.Add(new SqlParameter("@CtrliAnio", CtrliAnio));
                cmd.Parameters.Add(new SqlParameter("@CtrliTiPeriodo", CtrliTiPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliPeriodo", CtrliPeriodo));
                cmd.Parameters.Add(new SqlParameter("@CtrliEmpleado", CtrliEmpleado));
                cmd.Parameters.Add(new SqlParameter("@CtrliEmpleado", CtrliEmpleado));
                cmd.Parameters.Add(new SqlParameter("@CtrliEspejo", CtrliEspejo));



                SqlDataReader data = cmd.ExecuteReader();
                cmd.Dispose();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        EmisorReceptorBean ls = new EmisorReceptorBean();
                        ls.sMensaje = "success";

                        if (data["IdEmpresa"].ToString() == null)
                        {
                            ls.iIdEmpresa = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iIdEmpresa =int.Parse(data["IdEmpresa"].ToString());
                        }
                        if (data["RazonSocial"].ToString() == null)
                        {
                            ls.sNombreEmpresa = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sNombreEmpresa = data["RazonSocial"].ToString();
                        }
                        if (data["Calle"].ToString() == null)
                        {
                            ls.sCalle = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCalle = data["Calle"].ToString();
                        }
                        if (data["Colonia"].ToString() == null)
                        {
                            ls.sColonia = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sColonia = data["Colonia"].ToString();
                        }
                        if (data["CodigoPostal"].ToString() == null)
                        {
                            ls.iCP = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iCP = Convert.ToInt32(data["CodigoPostal"].ToString());
                        }
                        if (data["Ciudad"].ToString() == null)
                        {
                            ls.sCiudad = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCiudad = data["Ciudad"].ToString();
                        }
                        if (data["RFC"].ToString() == null)
                        {
                            ls.sRFC = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sRFC = data["RFC"].ToString();
                        }
                        if (data["Representante_legal"].ToString() == null)
                        {
                            ls.sRepresentanteLegal = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sRepresentanteLegal = data["Representante_legal"].ToString();
                        }
                        if (data["Afiliacion_IMSS"].ToString() == null)
                        {
                            ls.sAfiliacionIMSS = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sAfiliacionIMSS = data["Afiliacion_IMSS"].ToString();
                        }
                        if (data["FechaAntiguedad"].ToString() == null)
                        {
                            ls.sFechaAntiguedad = "TempleadosNomina";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sFechaAntiguedad = data["FechaAntiguedad"].ToString();
                        }
                        if (data["Fecha_baja"].ToString() == null)
                        {
                            ls.sFechaBajaEmple = " ";

                        }
                        else
                        {
                            ls.sFechaBajaEmple = data["Fecha_baja"].ToString();
                        }
                        if (data["Lugar_Nacimiento_Empleado"].ToString() == null)
                        {
                            ls.sLugarNacimiento = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sLugarNacimiento = data["Lugar_Nacimiento_Empleado"].ToString();
                        }
                        if (data["Fecha_Nacimiento_Empleado"].ToString() == null)
                        {
                            ls.sFechaNacimiento = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sFechaNacimiento = data["Fecha_Nacimiento_Empleado"].ToString();
                        }
                        if (data["NombreComp"].ToString() == null)
                        {
                            ls.sNombreComp = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sNombreComp = data["NombreComp"].ToString();
                            ls.sNombreemple = data["Nombre_Empleado"].ToString();
                            ls.sApellPatemple = data["Apellido_Paterno_Empleado"].ToString();
                            ls.sApellMatemple = data["Apellido_Materno_Empleado"].ToString();
                        }
                        if (data["Domicilio"].ToString() == null)
                        {
                            ls.sDomiciolioEmple = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sDomiciolioEmple = data["Domicilio"].ToString();
                        }
                        if (data["SEXO"].ToString() == null)
                        {
                            ls.sSexo = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            if (data["SEXO"].ToString() == "FEMENINO")
                            {
                                ls.sSexo = "F";
                            }


                            if (data["SEXO"].ToString() == "MASCULINO")
                            {
                                ls.sSexo = "M";
                            }

                        }
                        if (data["ESTADO_CIVIL"].ToString() == null)
                        {
                            ls.sEstadoCivil = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            if (data["ESTADO_CIVIL"].ToString() == "SOLTERO")
                            {
                                ls.sEstadoCivil = "S";
                            }
                            if (data["ESTADO_CIVIL"].ToString() == "CASADO")
                            {
                                ls.sEstadoCivil = "C";
                            }
                            if (data["ESTADO_CIVIL"].ToString() == "DIVORCIADO")
                            {
                                ls.sEstadoCivil = "D";
                            }
                            if (data["ESTADO_CIVIL"].ToString() == "UNION LIBRE")
                            {
                                ls.sEstadoCivil = "U";
                            }
                            if (data["ESTADO_CIVIL"].ToString() == "VIUDO")
                            {
                                ls.sEstadoCivil = "V";
                            }

                        }
                        if (data["Ciud"].ToString() == null)
                        {
                            ls.sCiudadEmple = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCiudadEmple = data["Ciud"].ToString();
                        }
                        if (data["RFCEmpleado"].ToString() == null)
                        {
                            ls.sRFCEmpleado = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sRFCEmpleado = data["RFCEmpleado"].ToString();
                        }
                        if (data["IdEmpleado"].ToString() == null)
                        {
                            ls.iIdEmpleado = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iIdEmpleado = int.Parse(data["IdEmpleado"].ToString());
                        }
                        if (data["DescripcionDepartamento"].ToString() == null)
                        {
                            ls.sDescripcionDepartamento = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sDescripcionDepartamento = data["DescripcionDepartamento"].ToString();
                        }
                        if (data["Localidad"].ToString() == null)
                        {
                            ls.sLocalidademple = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sLocalidademple = data["Localidad"].ToString();
                        }
                        if (data["NombrePuesto"].ToString() == null)
                        {
                            ls.sNombrePuesto = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sNombrePuesto = data["NombrePuesto"].ToString();
                        }
                        if (data["FechaIngreso"].ToString() == null)
                        {
                            ls.sFechaIngreso = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sFechaIngreso = data["FechaIngreso"].ToString();
                        }
                        if (data["TipoContrato"].ToString() == null)
                        {
                            ls.sTipoContrato = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sTipoContrato = data["TipoContrato"].ToString();
                        }
                        if (data["CentroCosto"].ToString() == null)
                        {
                            ls.sCentroCosto = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCentroCosto = data["CentroCosto"].ToString();
                        }
                        if (data["SalarioMensual"].ToString() == null)
                        {
                            ls.dSalarioMensual = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.dSalarioMensual = decimal.Parse(data["SalarioMensual"].ToString());
                        }
                        if (data["RegistroImss"].ToString() == null)
                        {
                            ls.sRegistroImss = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sRegistroImss = data["RegistroImss"].ToString();
                        }
                        if (data["CURP"].ToString() == null)
                        {
                            ls.sCURP = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCURP = data["CURP"].ToString(); ;
                        }
                        if (data["Descripcion"].ToString() == null)
                        {
                            ls.sDescripcion = "CBanco";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sDescripcion = data["Descripcion"].ToString();
                        }
                        if (data["codigo"].ToString() == null)
                        {
                        }
                        else
                        {
                            ls.sCodiBanco = data["codigo"].ToString();
                        }
                        if (data["Cta_Cheques"].ToString() == null)
                        {
                            ls.sCtaCheques = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sCtaCheques = data["Cta_Cheques"].ToString();
                        }
                        if (data["Regimen_Fiscal_id"].ToString() == null)
                        {
                            ls.iRegimenFiscal = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iRegimenFiscal = int.Parse(data["Regimen_Fiscal_id"].ToString());
                        }
                        if (data["IdNomina"].ToString() == null)
                        {
                            ls.iIdNomina = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iIdNomina = int.Parse(data["IdNomina"].ToString());
                        }
                        if (data["Ult_sdi"].ToString() == null)
                        {
                            ls.SDINT = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.SDINT = data["Ult_sdi"].ToString();
                        }
                        if (data["ClasesRegPat_id"].ToString() == null)
                        {
                            ls.sRiesgoTrabajo = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sRiesgoTrabajo = data["ClasesRegPat_id"].ToString();
                        }
                        if (data["GrupoEmpresa_Id"].ToString() == null)
                        {
                            ls.GrupoEmpresas = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.GrupoEmpresas = int.Parse(data["GrupoEmpresa_Id"].ToString());
                        }
                        if (data["iTipoJor"].ToString() == null)
                        {
                            ls.iTipoJordana = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iTipoJordana = int.Parse(data["iTipoJor"].ToString());
                        }
                        if (data["sTipoJor"].ToString() == null)
                        {
                            ls.sTipoJornada = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sTipoJornada = data["sTipoJor"].ToString();
                        }
                        if (data["Cg_TipoEmpleado_id"].ToString() == null)
                        {
                            ls.iCgTipoEmpleadoId = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iCgTipoEmpleadoId = int.Parse(data["Cg_TipoEmpleado_id"].ToString());
                        }
                        if (data["ClaveEnt"].ToString() == null)
                        {
                            ls.sClaveEnt = " ";
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.sClaveEnt = data["ClaveEnt"].ToString();
                        }
                        if (data["Cg_tipoPago_id"].ToString() == null)
                        {
                            ls.iCgTipoPago = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iCgTipoPago = int.Parse(data["Cg_tipoPago_id"].ToString());
                        }
                        if (data["Cg_pago_por"].ToString() == null)
                        {
                            ls.iPagopor = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.iPagopor = int.Parse(data["Cg_pago_por"].ToString());
                        }
                        if (data["Ult_sdi"].ToString() == null)
                        {
                            ls.dSalarioInt = 0;
                            ls.sMensaje = "error";
                        }
                        else
                        {
                            ls.dSalarioInt = decimal.Parse(data["Ult_sdi"].ToString());
                        }


                        list.Add(ls);
                    }
                }
                else
                {
                    list = null;
                }
                data.Close(); cmd.Dispose(); conexion.Close(); cmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                this.conexion.Close();
                this.Conectar().Close();
            }
            return list;
        }







    }
}