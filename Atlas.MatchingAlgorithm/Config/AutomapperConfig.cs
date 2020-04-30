﻿using System.Linq;
using System.Reflection;
using AutoMapper;
using Nova.Utils.Reflection;

namespace Atlas.MatchingAlgorithm.Config
{
    public class AutomapperConfig
    {
        public static IMapper CreateMapper()
        {
            var assemblyNames = Assembly.GetExecutingAssembly()
                .LoadNovaAssemblies()
                .Select(a => a.GetName().Name)
                .ToArray();

            var config = new MapperConfiguration(cfg => { cfg.AddMaps(assemblyNames); });
            return config.CreateMapper();
        }
    }
}