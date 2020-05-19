﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Common.GeneticData;
using Atlas.Common.GeneticData.PhenotypeInfo;
using Atlas.HlaMetadataDictionary.ExternalInterface;
using Atlas.HlaMetadataDictionary.Models.Lookups.ScoringLookup;
using Atlas.MatchingAlgorithm.Client.Models.SearchResults.PerLocus;
using Atlas.MatchingAlgorithm.Common.Models.Scoring;
using Atlas.MatchingAlgorithm.Common.Models.SearchResults;
using Atlas.MatchingAlgorithm.Data.Models.SearchResults;
using Atlas.MatchingAlgorithm.Services.ConfigurationProviders;
using Atlas.MatchingAlgorithm.Services.Search.Scoring.Aggregation;
using Atlas.MatchingAlgorithm.Services.Search.Scoring.Confidence;
using Atlas.MatchingAlgorithm.Services.Search.Scoring.Grading;
using Atlas.MatchingAlgorithm.Services.Search.Scoring.Ranking;
using static EnumStringValues.EnumExtensions;

namespace Atlas.MatchingAlgorithm.Services.Search.Scoring
{
    public interface IDonorScoringService
    {
        Task<IEnumerable<MatchAndScoreResult>> ScoreMatchesAgainstHla(
            IEnumerable<MatchResult> matchResults,
            PhenotypeInfo<string> patientHla,
            IReadOnlyCollection<Locus> lociToExcludeFromAggregateScoring = null);

        Task<ScoreResult> ScoreDonorHlaAgainstPatientHla(
            PhenotypeInfo<string> donorHla,
            PhenotypeInfo<string> patientHla,
            IReadOnlyCollection<Locus> lociToExcludeFromAggregateScoring = null);
    }

    public class DonorScoringService : IDonorScoringService
    {
        private readonly IHlaMetadataDictionary hlaMetadataDictionary;
        private readonly IGradingService gradingService;
        private readonly IConfidenceService confidenceService;
        private readonly IRankingService rankingService;
        private readonly IMatchScoreCalculator matchScoreCalculator;
        private readonly IScoreResultAggregator scoreResultAggregator;

        public DonorScoringService(
            IHlaMetadataDictionaryFactory factory,
            IActiveHlaVersionAccessor hlaVersionAccessor,
            IGradingService gradingService,
            IConfidenceService confidenceService,
            IRankingService rankingService,
            IMatchScoreCalculator matchScoreCalculator,
            IScoreResultAggregator scoreResultAggregator)
        {
            this.hlaMetadataDictionary = factory.BuildDictionary(hlaVersionAccessor.GetActiveHlaDatabaseVersion());
            this.gradingService = gradingService;
            this.confidenceService = confidenceService;
            this.rankingService = rankingService;
            this.matchScoreCalculator = matchScoreCalculator;
            this.scoreResultAggregator = scoreResultAggregator;
        }

        public async Task<IEnumerable<MatchAndScoreResult>> ScoreMatchesAgainstHla(
            IEnumerable<MatchResult> matchResults,
            PhenotypeInfo<string> patientHla,
            IReadOnlyCollection<Locus> lociToExcludeFromAggregateScoring = null)
        {
            var patientScoringLookupResult = await GetHlaScoringResults(patientHla);

            var matchAndScoreResults = new List<MatchAndScoreResult>();

            foreach (var matchResult in matchResults)
            {
                var lookupResult = await GetHlaScoringResults(matchResult.DonorInfo.HlaNames);

                var scoreResult = ScoreDonorAndPatient(lookupResult, patientScoringLookupResult, lociToExcludeFromAggregateScoring);

                matchAndScoreResults.Add(CombineMatchAndScoreResults(matchResult, scoreResult));
            }

            return rankingService.RankSearchResults(matchAndScoreResults);
        }

        public async Task<ScoreResult> ScoreDonorHlaAgainstPatientHla(
            PhenotypeInfo<string> donorHla,
            PhenotypeInfo<string> patientHla,
            IReadOnlyCollection<Locus> lociToExcludeFromAggregateScoring)
        {
            var patientLookupResult = await GetHlaScoringResults(patientHla);
            var donorLookupResult = await GetHlaScoringResults(donorHla);
            return ScoreDonorAndPatient(donorLookupResult, patientLookupResult, lociToExcludeFromAggregateScoring);
        }

        private static MatchAndScoreResult CombineMatchAndScoreResults(MatchResult matchResult, ScoreResult scoreResult)
        {
            return new MatchAndScoreResult
            {
                MatchResult = matchResult,
                ScoreResult = scoreResult
            };
        }

        private ScoreResult ScoreDonorAndPatient(
            PhenotypeInfo<IHlaScoringLookupResult> donorScoringLookupResult,
            PhenotypeInfo<IHlaScoringLookupResult> patientScoringLookupResult,
            IReadOnlyCollection<Locus> lociToExcludeFromAggregateScoring)
        {
            var grades = gradingService.CalculateGrades(patientScoringLookupResult, donorScoringLookupResult);
            var confidences = confidenceService.CalculateMatchConfidences(patientScoringLookupResult, donorScoringLookupResult, grades);

            var locusTypingInformation = donorScoringLookupResult.Map((l, p, result) => result != null);

            var scoreResult = BuildScoreResult(grades, confidences, locusTypingInformation, lociToExcludeFromAggregateScoring);
            return scoreResult;
        }

        private ScoreResult BuildScoreResult(
            PhenotypeInfo<MatchGradeResult> grades,
            PhenotypeInfo<MatchConfidence> confidences,
            PhenotypeInfo<bool> locusTypingInformation,
            IReadOnlyCollection<Locus> lociToExcludeFromAggregateScoring)
        {
            var scoreResult = new ScoreResult();
            var scoredLoci = EnumerateValues<Locus>();

            foreach (var locus in scoredLoci)
            {
                var gradeResultAtPosition1 = grades.GetPosition(locus, LocusPosition.One).GradeResult;
                var confidenceAtPosition1 = confidences.GetPosition(locus, LocusPosition.One);
                var gradeResultAtPosition2 = grades.GetPosition(locus, LocusPosition.Two).GradeResult;
                var confidenceAtPosition2 = confidences.GetPosition(locus, LocusPosition.Two);

                var scoreDetails = new LocusScoreDetails
                {
                    // Either position can be used here, as the locus will either be typed at both positions or neither
                    IsLocusTyped = locusTypingInformation.GetPosition(locus, LocusPosition.One),
                    ScoreDetailsAtPosition1 = BuildScoreDetailsForPosition(gradeResultAtPosition1, confidenceAtPosition1),
                    ScoreDetailsAtPosition2 = BuildScoreDetailsForPosition(gradeResultAtPosition2, confidenceAtPosition2)
                };
                scoreResult.SetScoreDetailsForLocus(locus, scoreDetails);
            }

            scoreResult.AggregateScoreDetails = scoreResultAggregator.AggregateScoreDetails(scoreResult, lociToExcludeFromAggregateScoring);
            return scoreResult;
        }

        private LocusPositionScoreDetails BuildScoreDetailsForPosition(MatchGrade matchGrade, MatchConfidence matchConfidence)
        {
            return new LocusPositionScoreDetails
            {
                MatchGrade = matchGrade,
                MatchGradeScore = matchScoreCalculator.CalculateScoreForMatchGrade(matchGrade),
                MatchConfidence = matchConfidence,
                MatchConfidenceScore = matchScoreCalculator.CalculateScoreForMatchConfidence(matchConfidence),
            };
        }

        private async Task<PhenotypeInfo<IHlaScoringLookupResult>> GetHlaScoringResults(PhenotypeInfo<string> hlaNames)
        {
            return await hlaNames.MapAsync(
                async (locus, position, hla) => await GetHlaScoringResultsForLocus(locus, hla)
            );
        }

        private async Task<IHlaScoringLookupResult> GetHlaScoringResultsForLocus(Locus locus, string hla)
        {
            return hla != null
                ? await hlaMetadataDictionary.GetHlaScoringLookupResult(locus, hla)
                : null;
        }
    }
}