﻿using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.ApplicationInsights;
using Nova.Utils.ApplicationInsights;
using Nova.Utils.Solar;
using Nova.Utils.WebApi.ApplicationInsights;
using Nova.SearchAlgorithm.Config;
using Nova.SearchAlgorithm.Data;
using Nova.SearchAlgorithm.Repositories;
using Nova.SearchAlgorithm.Repositories.Donors;
using Nova.SearchAlgorithm.Repositories.Hla;
using NUnit.Framework;
using Autofac;
using System.Collections;

namespace Nova.SearchAlgorithm.Test.Integration
{
    [TestFixture(DonorStorageImplementation.CloudTable)]
    [TestFixture(DonorStorageImplementation.SQL)]
    public abstract class IntegrationTestBase
    {
        private StorageEmulator emulator = new StorageEmulator();
        private readonly DonorStorageImplementation donorStorageImplementation;
        protected IContainer container;

        public IntegrationTestBase(DonorStorageImplementation input)
        {
            this.donorStorageImplementation = input;
        }

        [OneTimeSetUp]
        public void Setup()
        {
            container = CreateContainer();

            if (container.TryResolve(out SearchAlgorithmContext context))
            {
                context.Database.Delete();
            }

            // Starting and stopping the emulator is managed in the setup fixture StorageSetup.cs
            emulator.Clear();
        }

        // This is almost a duplicate of the container in 
        // Nova.SearchAlgorithm.Config.Modules.ServiceModule
        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(AutomapperConfig.CreateMapper())
                .SingleInstance()
                .AsImplementedInterfaces();

            // Switch between testing different implementations
            if (donorStorageImplementation == DonorStorageImplementation.CloudTable)
            {
                builder.RegisterType<DonorCloudTables>().AsImplementedInterfaces().InstancePerLifetimeScope();
                builder.RegisterType<CloudStorageDonorMatchRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<SearchAlgorithmContext>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<Data.Repositories.SqlDonorMatchRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            }

            builder.RegisterType<HlaRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SolarDonorRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<Services.SearchRequestService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<Services.SearchService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<Services.DonorImportService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<CloudTableFactory>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SolarConnectionFactory>().AsImplementedInterfaces().SingleInstance();

            // Tests should not use Solar, so don't provide an actual connection string.
            var solarSettings = new SolarConnectionSettings();
            builder.RegisterInstance(solarSettings).AsSelf().SingleInstance();

            var logger = new RequestAwareLogger(new TelemetryClient(),
                ConfigurationManager.AppSettings["insights.logLevel"].ToLogLevel());
            builder.RegisterInstance(logger).AsImplementedInterfaces().SingleInstance();

            return builder.Build();
        }
    }

    internal class DonorStorageImplementationData
    {
        public static IEnumerable FixtureParms
        {
            get
            {
                //yield return new TestFixtureData(DonorStorageImplementation.SQL);
                //yield return new TestFixtureData(DonorStorageImplementation.CloudTable);
                yield return new TestFixtureData(true);
                yield return new TestFixtureData(false);
            }
        }
    }

    public enum DonorStorageImplementation
    {
        SQL,
        CloudTable
    }
}
