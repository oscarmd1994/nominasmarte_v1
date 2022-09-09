using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;
using System.Diagnostics;
using Hangfire.Storage.Monitoring;
using Payroll.Models.Beans;
using Payroll.Models.Utilerias;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using Payroll.Models.Daos;
using System.Collections.Generic;
using Payroll.Controllers;

[assembly: OwinStartup(typeof(Payroll.Startup))]

namespace Payroll 
{
   
    public class Startup : Conexion
    {
        
        /// <summary>
        ///  configuracion del hangfire 
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            
            
           
            //  GlobalConfiguration.Configuration.UseSqlServerStorage("Data Source = 201.149.34.185,15002; Initial Catalog=IPSNet_original; User ID= IPSNet;Password= IPSNet2;Integrated Security= False");
            
            // Produccion
            GlobalConfiguration.Configuration.UseSqlServerStorage("Data Source = 201.149.34.185,15002; Initial Catalog=IPSNet; User ID= IPSNet_Admin;Password= @Tecas123;Integrated Security= False");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }

       

        // proceso actualiza en BD TpProcesosJons cada 2 min 
        public void ActBDTbJobs() {
             RecurringJob.AddOrUpdate(() => ProcesosContinuos(), "*/2****");
          //  RecurringJob.AddOrUpdate(() => ProcesosContinuos(), Cron.Minutely);
        }

        public void ProcesosContinuos() 
       {
            FuncionesNomina Dao = new FuncionesNomina();          
            Dao.sp_ProcesoJobs_update_TPProcesosJobs();
        }

       

     
    }
}
