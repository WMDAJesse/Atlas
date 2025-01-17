﻿using System.Collections.Generic;
using Atlas.Common.Public.Models.GeneticData;

namespace Atlas.MatchPrediction.Functions.Models.Debug
{
    public class GenotypeImputationRequest
    {
        public SubjectInfo SubjectInfo { get; set; }
        public IEnumerable<Locus> AllowedLoci { get; set; }
    }
}