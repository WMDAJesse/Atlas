using System;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using Nova.SearchAlgorithm.Client.Models.SearchResults;
using Nova.SearchAlgorithm.Common.Models;
using Nova.SearchAlgorithm.Services.ConfigurationProviders;

namespace Nova.SearchAlgorithm.Services.Search.Scoring.Grading
{
    /// <summary>
    /// Wraps a cache provided by LazyCache
    /// Used to ensure consistent key creation for cached items
    /// </summary>
    public interface IScoringCache
    {
        MatchGrade GetOrAddMatchGrade(Locus locus, string patientHlaName, string donorHlaName, Func<ICacheEntry, MatchGrade> func);
        MatchConfidence GetOrAddMatchConfidence(Locus? locus, string patientHlaName, string donorHlaName, Func<ICacheEntry, MatchConfidence> func);
    }

    public class ScoringCache : IScoringCache
    {
        private readonly IAppCache cache;
        private readonly IWmdaHlaVersionProvider wmdaHlaVersionProvider;

        public ScoringCache(IAppCache cache, 
            IWmdaHlaVersionProvider wmdaHlaVersionProvider)
        {
            this.cache = cache;
            this.wmdaHlaVersionProvider = wmdaHlaVersionProvider;
        }

        public MatchGrade GetOrAddMatchGrade(Locus locus, string patientHlaName, string donorHlaName, Func<ICacheEntry, MatchGrade> func)
        {
            var cacheKey = $"MatchGrade:v{wmdaHlaVersionProvider.GetActiveHlaDatabaseVersion()};l{locus};d{donorHlaName};p{patientHlaName}";
            return cache.GetOrAdd(cacheKey, func);
        }

        public MatchConfidence GetOrAddMatchConfidence(
            Locus? locus,
            string patientHlaName,
            string donorHlaName,
            Func<ICacheEntry, MatchConfidence> func)
        {
            var cacheKey = $"MatchConfidence:v{wmdaHlaVersionProvider.GetActiveHlaDatabaseVersion()};l{locus};d{donorHlaName};p{patientHlaName}";
            return cache.GetOrAdd(cacheKey, func);
        }
    }
}