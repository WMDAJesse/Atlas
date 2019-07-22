﻿using Nova.SearchAlgorithm.Client.Models;
using Nova.SearchAlgorithm.Exceptions;
using System;

namespace Nova.SearchAlgorithm.Helpers
{
    public static class DonorInfoHelper
    {
        public static RegistryCode RegistryCodeFromString(string input)
        {
            if (Enum.TryParse(input, out RegistryCode code))
            {
                return code;
            }
            throw new DonorImportException($"Could not understand registry code {input}");
        }

        public static DonorType DonorTypeFromString(string input)
        {
            switch (input.ToLower())
            {
                case "adult":
                case "a":
                    return DonorType.Adult;
                case "cord":
                case "c":
                    return DonorType.Cord;
                default:
                    throw new DonorImportException($"Could not understand donor type {input}");
            }
        }
    }
}