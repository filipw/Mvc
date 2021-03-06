// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;

namespace WebApiCompatShimWebSite
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC services to the services container
            services.AddMvc().AddWebApiConventions();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCultureReplacer();

            app.UseErrorReporter();

            app.UseMvc(routes =>
            {
                // This route can't access any of our webapi controllers
                routes.MapRoute("default", "{controller}/{action}/{id?}");

                // Tests include different styles of WebAPI conventional routing and action selection - the prefix keeps
                // them from matching too eagerly.
                routes.MapWebApiRoute("named-action", "api/Blog/{controller}/{action}/{id?}");
                routes.MapWebApiRoute("unnamed-action", "api/Admin/{controller}/{id?}");
                routes.MapWebApiRoute("name-as-parameter", "api/Store/{controller}/{name?}");
                routes.MapWebApiRoute("extra-parameter", "api/Support/{extra}/{controller}/{id?}");
            });
        }
    }
}
