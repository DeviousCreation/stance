﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation.AspNetCore;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Stance.Queries.OData.Entities;
using Stance.Web.Pages.FirstRun;

namespace Stance.Web.Infrastructure.ServiceConfiguration
{
    public static class WebConfig
    {
        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services)
        {
            services.AddOData();
            services
                .AddMvc().AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<InitialUserSetup.Validator>();
                    fv.ImplicitlyValidateChildProperties = true;
                })
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Add("/{0}.cshtml");
                    options.PageViewLocationFormats.Add("/{0}.cshtml");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson();

            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddControllers(mvc => { mvc.EnableEndpointRouting = false; });
            return services;
        }

        public static IApplicationBuilder AddCustomizedErrorResponse(
            this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            return app;
        }

        public static IApplicationBuilder UseCustomizedEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel());
                routeBuilder.Select().Expand().Filter().OrderBy().MaxTop(1000).Count();
            });

            return app;
        }

        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<User>("User");
            builder.EntitySet<Role>("Role");
            return builder.GetEdmModel();
        }
    }
}