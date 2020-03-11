using Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;
using MusicShop.Infrastructure;

[assembly: OwinStartupAttribute(typeof(MusicShop.Startup))]
namespace MusicShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("StoreContext");

            app.UseHangfireServer();

            ConfigureAuth(app);

            var options = new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            };
            app.UseHangfireDashboard("/hangfire", options);
        }
    }
}