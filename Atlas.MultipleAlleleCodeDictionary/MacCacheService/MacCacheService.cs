﻿using System;
using System.Threading.Tasks;
using Atlas.Common.ApplicationInsights;
using Atlas.Common.Caching;
using Atlas.Common.GeneticData.Hla.Models.MolecularHlaTyping;
using Atlas.MultipleAlleleCodeDictionary.AzureStorage.Repositories;
using Atlas.MultipleAlleleCodeDictionary.ExternalInterface.Models;
using Atlas.MultipleAlleleCodeDictionary.MacImportServices;
using Atlas.MultipleAlleleCodeDictionary.utils;
using LazyCache;

namespace Atlas.MultipleAlleleCodeDictionary.MacCacheService
{
    public interface IMacCacheService
    {
        Task<MolecularAlleleDetails> GetHlaFromMac(string macCode, string firstField);
        Task<MultipleAlleleCode> GetMacCode(string macCode);
        Task GenerateMacCache();
    }
    
    internal class MacCacheService: IMacCacheService
    {
        private readonly IAppCache cache;
        private readonly ILogger logger;
        private readonly IMacRepository macRepository;
        private readonly IMacExpander macExpander;

        public MacCache(ILogger logger, IPersistentCacheProvider cacheProvider, IMacRepository macRepository, IMacExpander macExpander)
        {
            this.logger = logger;
            this.cache = cacheProvider.Cache;
            this.macRepository = macRepository;
            this.macExpander = macExpander;
        }

        // TODO Atlas-54 ensure all new MAC functionality is adequately tested
        public async Task<MolecularAlleleDetails> GetHlaFromMac(string macCode, string firstField)
        {
            var mac = await GetMacCode(macCode);
            logger.SendTrace($"Attempting to expand Hla for Mac: {mac.Mac}", LogLevel.Info);
            return macExpander.ExpandMac(mac, firstField);
        }

        public async Task<MultipleAlleleCode> GetMacCode(string macCode)
        {
            return await cache.GetOrAddAsync(macCode, () => macRepository.GetMac(macCode));
        }
        
        public async Task GenerateMacCache()
        {
            var macs = await macRepository.GetAllMacs();
            foreach (var mac in macs)
            {
                cache.GetOrAdd(mac.Mac, () => mac);
            }
        }
        
        private static string GetExpandedHla(Mac mac)
        {
            // TODO: Atlas-384: handle generic Mac.
            return mac.IsGeneric ? throw new NotImplementedException() : mac.Hla;
        }
    }
}