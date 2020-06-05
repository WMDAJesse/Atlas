using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Atlas.Common.Test.SharedTestHelpers;
using Atlas.MultipleAlleleCodeDictionary.AzureStorage.Models;

namespace Atlas.MultipleAlleleCodeDictionary.Test.TestHelpers.Builders
{
    internal static class MacSourceFileBuilder
    {
        // TODO: ATLAS-47: Replace with internal ATLAS MAC model
        public static Stream BuildMacFile(params MultipleAlleleCodeEntity[] macEntities)
        {
            return BuildMacFile(macEntities.ToList());
        }
        
        // TODO: ATLAS-47: Replace with internal ATLAS MAC model
        public static Stream BuildMacFile(IEnumerable<MultipleAlleleCodeEntity> macEntities)
        {
            var file = BuildFileAsString(macEntities);
            return file.ToStream();
        }

        private static string BuildFileAsString(IEnumerable<MultipleAlleleCodeEntity> macEntities)
        {
            var fileBuilder = new StringBuilder();
            fileBuilder.AppendLine("LAST UPDATED: 10/03/19");
            fileBuilder.AppendLine("*    CODE    SUBTYPE");
            fileBuilder.Append(Environment.NewLine);
            foreach (var macEntity in macEntities)
            {
                if (!macEntity.IsGeneric)
                {
                    fileBuilder.Append("*");
                }

                fileBuilder.Append("\t");
                fileBuilder.Append(macEntity.RowKey);
                fileBuilder.Append("\t");
                fileBuilder.Append(macEntity.HLA);
                fileBuilder.Append(Environment.NewLine);
            }
            return fileBuilder.ToString();
        }
    }
}