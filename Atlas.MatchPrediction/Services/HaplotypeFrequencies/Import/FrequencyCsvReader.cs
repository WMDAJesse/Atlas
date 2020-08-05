﻿using System;
using System.Collections.Generic;
using System.IO;
using Atlas.MatchPrediction.ExternalInterface.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace Atlas.MatchPrediction.Services.HaplotypeFrequencies.Import
{
    public interface IFrequencyCsvReader
    {
        IEnumerable<HaplotypeFrequency> GetFrequencies(Stream stream);
    }

    internal class FrequencyCsvReader : IFrequencyCsvReader
    {
        public IEnumerable<HaplotypeFrequency> GetFrequencies(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }
            
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                ConfigureCsvReader(csv);
                while (csv.Read())
                {
                    var frequency = csv.GetRecord<HaplotypeFrequency>();
                    yield return frequency;
                }
            }
        }

        private static void ConfigureCsvReader(IReaderRow csvReader)
        {
            csvReader.Configuration.Delimiter = ";";
            csvReader.Configuration.PrepareHeaderForMatch = (header, index) => header.ToUpper();
            csvReader.Configuration.RegisterClassMap<HaplotypeFrequencyMap>();
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class HaplotypeFrequencyMap : ClassMap<HaplotypeFrequency>
        {
            public HaplotypeFrequencyMap()
            {
                Map(m => m.A);
                Map(m => m.B);
                Map(m => m.C);
                Map(m => m.Dqb1);
                Map(m => m.Drb1);
                Map(m => m.Frequency).Name("freq");
            }
        }
    }
}
