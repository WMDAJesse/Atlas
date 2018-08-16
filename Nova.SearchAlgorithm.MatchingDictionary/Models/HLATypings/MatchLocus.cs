﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nova.SearchAlgorithm.MatchingDictionary.Models.HLATypings
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MatchLocus
    {
        A = 1,
        B = 2,
        C = 3,
        Dqb1 = 4,
        Drb1 = 5,
        Dpb1 = 6
    }
}
