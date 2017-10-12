﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.Owin;
using Nova.TemplateService;
using Nova.TemplateService.Config;
using Nova.Utils.WebApi.Owin.Middleware;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Nova.TemplateService
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = app.ConfigureAutofac();
            app.HandleAllExceptions(JsonConfig.GlobalSettings);
            app.ConfigureLogging();
            app.ConfigureWebApi(container);
        }
    }
}