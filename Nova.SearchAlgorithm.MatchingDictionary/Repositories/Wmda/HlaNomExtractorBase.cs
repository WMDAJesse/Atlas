﻿using Nova.SearchAlgorithm.MatchingDictionary.Models.HLATypings;
using Nova.SearchAlgorithm.MatchingDictionary.Models.Wmda;
using System.Text.RegularExpressions;

namespace Nova.SearchAlgorithm.MatchingDictionary.Repositories.Wmda
{
    internal abstract class HlaNomExtractorBase : WmdaDataExtractor<HlaNom>
    {
        private const string FileName = WmdaFilePathPrefix + "hla_nom";
        private const string RegexPattern = @"^(\w+\*{0,1})\;([\w:]+)\;\d+\;(\d*)\;([\w:]*)\;";
        private readonly TypingMethod typingMethod;

        protected HlaNomExtractorBase(TypingMethod typingMethod) : base(FileName)
        {
            this.typingMethod = typingMethod;
        }

        protected override HlaNom MapLineOfFileToWmdaHlaTypingElseNull(string line)
        {
            var regex = new Regex(RegexPattern);

            if (!regex.IsMatch(line))
                return null;

            var extractedData = regex.Match(line).Groups;

            return new HlaNom(
                typingMethod,
                extractedData[1].Value,
                extractedData[2].Value,
                !extractedData[3].Value.Equals(""),
                extractedData[4].Value);
        }
    }
}
