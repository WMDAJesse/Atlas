﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Autofac;
using Nova.Utils.Reflection;
using Owin;

namespace Nova.TemplateService.Config
{
    public static class AutofacConfig
    {
        [ExcludeFromCodeCoverage]
        public static IContainer ConfigureAutofac(this IAppBuilder app, IContainer container = null)
        {
            if (container == null)
            {
                var builder = new ContainerBuilder();

                var reportAssemblies = Assembly.GetExecutingAssembly().LoadNovaAssemblies().ToArray();
                builder.RegisterAssemblyModules(reportAssemblies);

                container = builder.Build();
            }
            app.UseAutofacLifetimeScopeInjector(container);
            return container;
        }
    }
}