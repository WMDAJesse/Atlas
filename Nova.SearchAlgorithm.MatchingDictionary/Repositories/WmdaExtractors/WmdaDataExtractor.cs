﻿using Nova.SearchAlgorithm.MatchingDictionary.Data;
using Nova.SearchAlgorithm.MatchingDictionary.Models.Wmda;
using System.Collections.Generic;
using System.Linq;

namespace Nova.SearchAlgorithm.MatchingDictionary.Repositories.WmdaExtractors
{
    internal abstract class WmdaDataExtractor<TWmdaHlaTyping> where TWmdaHlaTyping : IWmdaHlaTyping
    {
        protected const string WmdaFilePathPrefix = "wmda/";

        private readonly string fileName;

        protected WmdaDataExtractor(string fileName)
        {
            this.fileName = fileName;
        }

        public IEnumerable<TWmdaHlaTyping> GetWmdaHlaTypingsForMatchingDictionaryLoci(IWmdaFileReader fileReader, string hlaDatabaseVersion)
        {
            var fileContents = fileReader.GetFileContentsWithoutHeader(hlaDatabaseVersion, fileName).ToList();
            ExtractHeaders(fileContents.First());
            return ExtractWmdaHlaTypingsForMatchingDictionaryLoci(fileContents);
        }

        /// <returns>
        /// The information contained in the line, mapped to the appropriate type
        /// Returns null if a line cannot be parsed
        /// </returns>
        protected abstract TWmdaHlaTyping MapLineOfFileContentsToWmdaHlaTyping(string line);

        /// <summary>
        /// In some cases, the header information from the file is necessary to parse the remaining lines correctly
        /// </summary>
        protected virtual void ExtractHeaders(string headersLine)
        {
            // Do nothing by default
        }

        private IEnumerable<TWmdaHlaTyping> ExtractWmdaHlaTypingsForMatchingDictionaryLoci(IEnumerable<string> wmdaFileContents)
        {
            return 
                wmdaFileContents
                    .Select(line => line.Trim())
                    .Select(MapLineOfFileContentsToWmdaHlaTyping)
                    .Where(typing => typing != null && typing.IsMatchingDictionaryLocusTyping());
        }
    }
}
