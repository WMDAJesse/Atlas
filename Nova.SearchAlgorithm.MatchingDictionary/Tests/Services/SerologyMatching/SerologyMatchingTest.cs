﻿using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using Nova.SearchAlgorithm.MatchingDictionary.Models.HLATypes;
using Nova.SearchAlgorithm.MatchingDictionary.Models.MatchingTypes;
using NUnit.Framework;

namespace Nova.SearchAlgorithm.MatchingDictionary.Tests.Services.SerologyMatching
{
    [TestFixtureSource(typeof(SerologyMatchingTestFixtureArgs))]
    [UseReporter(typeof(NUnitReporter))]
    public class SerologyMatchingTest : MatchedOnTestBase<IMatchingSerology>
    {
        public SerologyMatchingTest(IEnumerable<IMatchingSerology> matchingSerology) : base(matchingSerology)
        {
        }

        [Test]
        public void ServiceReturnsAllExpectedSerology()
        {
            var str = string.Join("\r\n", MatchingTypes
                .OrderBy(s => s.HlaType.MatchLocus)
                .ThenBy(s => int.Parse(s.HlaType.Name))
                .Select(s => $"{s.HlaType.MatchLocus}\t{s.HlaType.Name}")
                .ToList());
            Approvals.Verify(str);
        }

        [Test]
        public void BroadMatchingSerologiesAreCorrect()
        {
            var broadWhereSplitHasAssociated = new SerologyToSerology
            (
                new Serology("A", "9", SerologySubtype.Broad),
                new Serology("A", "9", SerologySubtype.Broad),
                new List<Serology>
                {
                    new Serology("A", "9", SerologySubtype.Broad),
                    new Serology ("A", "23", SerologySubtype.Split),
                    new Serology ("A", "24", SerologySubtype.Split),
                    new Serology ("A", "2403", SerologySubtype.Associated)
                }
            );

            var broadWhereSplitHasNoAssociated = new SerologyToSerology
            (
                new Serology("DQ", "1", SerologySubtype.Broad),
                new Serology("DQ", "1", SerologySubtype.Broad),
                new List<Serology>
                {
                    new Serology("DQ", "1", SerologySubtype.Broad),
                    new Serology ("DQ", "5", SerologySubtype.Split),
                    new Serology ("DQ", "6", SerologySubtype.Split)
                }
            );

            var broadHasSplitAndAssociated = new SerologyToSerology
            (
                new Serology("B", "21", SerologySubtype.Broad),
                new Serology("B", "21", SerologySubtype.Broad),
                new List<Serology>
                {
                    new Serology("B", "21", SerologySubtype.Broad),
                    new Serology ("B", "4005", SerologySubtype.Associated),
                    new Serology ("B", "49", SerologySubtype.Split),
                    new Serology ("B", "50", SerologySubtype.Split)
                }
            );

            Assert.AreEqual(broadWhereSplitHasAssociated, GetSingleMatchingType("A", "9"));
            Assert.AreEqual(broadWhereSplitHasNoAssociated, GetSingleMatchingType("DQB1", "1"));
            Assert.AreEqual(broadHasSplitAndAssociated, GetSingleMatchingType("B", "21"));
        }

        [Test]
        public void SplitMatchingSerologiesAreCorrect()
        {
            var splitHasAssociated = new SerologyToSerology
            (
                new Serology("B", "51", SerologySubtype.Split),
                new Serology("B", "51", SerologySubtype.Split),
                new List<Serology>
                {
                    new Serology("B", "51", SerologySubtype.Split),
                    new Serology ("B", "5", SerologySubtype.Broad),
                    new Serology ("B", "5102", SerologySubtype.Associated),
                    new Serology ("B", "5103", SerologySubtype.Associated)
                }
            );

            var splitNoAssociated = new SerologyToSerology
            (
                new Serology("Cw", "10", SerologySubtype.Split),
                new Serology("Cw", "10", SerologySubtype.Split),
                new List<Serology>
                {
                    new Serology("Cw", "10", SerologySubtype.Split),
                    new Serology ("Cw", "3", SerologySubtype.Broad)
                }
            );

            Assert.AreEqual(splitHasAssociated, GetSingleMatchingType("B", "51"));
            Assert.AreEqual(splitNoAssociated, GetSingleMatchingType("C", "10"));
        }

        [Test]
        public void AssociatedMatchingSerologiesAreCorrect()
        {
            var associatedWithSplit = new SerologyToSerology
            (
                new Serology("B", "3902", SerologySubtype.Associated),
                new Serology("B", "3902", SerologySubtype.Associated),
                new List<Serology>
                {
                    new Serology("B", "3902", SerologySubtype.Associated),
                    new Serology ("B", "39", SerologySubtype.Split),
                    new Serology ("B", "16", SerologySubtype.Broad)
                }
            );

            var associatedWithBroad = new SerologyToSerology
            (
                new Serology("B", "4005", SerologySubtype.Associated),
                new Serology("B", "4005", SerologySubtype.Associated),
                new List<Serology>
                {
                    new Serology("B", "4005", SerologySubtype.Associated),
                    new Serology ("B", "21", SerologySubtype.Broad)
                }
            );

            var associatedWithNotSplit = new SerologyToSerology
            (
                new Serology("DR", "103", SerologySubtype.Associated),
                new Serology("DR", "103", SerologySubtype.Associated),
                new List<Serology>
                {
                    new Serology("DR", "103", SerologySubtype.Associated),
                    new Serology ("DR", "1", SerologySubtype.NotSplit)
                }
            );

            Assert.AreEqual(associatedWithSplit, GetSingleMatchingType("B", "3902"));
            Assert.AreEqual(associatedWithBroad, GetSingleMatchingType("B", "4005"));
            Assert.AreEqual(associatedWithNotSplit, GetSingleMatchingType("DRB1", "103"));
        }

        [Test]
        public void NotSplitMatchingSerologiesAreCorrect()
        {
            var notSplitWithAssociated = new SerologyToSerology
            (
                new Serology("A", "2", SerologySubtype.NotSplit),
                new Serology("A", "2", SerologySubtype.NotSplit),
                new List<Serology>
                {
                    new Serology("A", "2", SerologySubtype.NotSplit),
                    new Serology ("A", "203", SerologySubtype.Associated),
                    new Serology ("A", "210", SerologySubtype.Associated)
                }
            );

            var notSplitNoAssociatedA = new SerologyToSerology
            (
                new Serology("A", "1", SerologySubtype.NotSplit),
                new Serology("A", "1", SerologySubtype.NotSplit),
                new List<Serology> { new Serology("A", "1", SerologySubtype.NotSplit) }
            );

            var notSplitNoAssociatedB = new SerologyToSerology
            (
                new Serology("B", "13", SerologySubtype.NotSplit),
                new Serology("B", "13", SerologySubtype.NotSplit),
                new List<Serology> { new Serology("B", "13", SerologySubtype.NotSplit) }
            );

            var notSplitNoAssociatedC = new SerologyToSerology
            (
                new Serology("Cw", "8", SerologySubtype.NotSplit),
                new Serology("Cw", "8", SerologySubtype.NotSplit),
                new List<Serology> { new Serology("Cw", "8", SerologySubtype.NotSplit) }
            );

            var notSplitNoAssociatedDq = new SerologyToSerology
            (
                new Serology("DQ", "4", SerologySubtype.NotSplit),
                new Serology("DQ", "4", SerologySubtype.NotSplit),
                new List<Serology> { new Serology("DQ", "4", SerologySubtype.NotSplit) }
            );

            var notSplitNoAssociatedDr = new SerologyToSerology
            (
                new Serology("DR", "9", SerologySubtype.NotSplit),
                new Serology("DR", "9", SerologySubtype.NotSplit),
                new List<Serology> { new Serology("DR", "9", SerologySubtype.NotSplit) }
            );

            Assert.AreEqual(notSplitWithAssociated, GetSingleMatchingType("A", "2"));
            Assert.AreEqual(notSplitNoAssociatedA, GetSingleMatchingType("A", "1"));
            Assert.AreEqual(notSplitNoAssociatedB, GetSingleMatchingType("B", "13"));
            Assert.AreEqual(notSplitNoAssociatedC, GetSingleMatchingType("C", "8"));
            Assert.AreEqual(notSplitNoAssociatedDq, GetSingleMatchingType("DQB1", "4"));
            Assert.AreEqual(notSplitNoAssociatedDr, GetSingleMatchingType("DRB1", "9"));
        }

        [Test]
        public void DeletedMatchingSerologiesAreCorrect()
        {
            var deletedSerology = new SerologyToSerology
            (
                new Serology("Cw", "11", SerologySubtype.NotSplit, true),
                new Serology("Cw", "1", SerologySubtype.NotSplit),
                new List<Serology>
                {
                    new Serology("Cw", "11", SerologySubtype.NotSplit, true),
                    new Serology("Cw", "1", SerologySubtype.NotSplit)
                }
            );

            Assert.AreEqual(deletedSerology, GetSingleMatchingType("C", "11"));
        }

        [Test]
        public void MatchingSerologyOnlyContainsValidRelationships()
        {
            var groupBySubtype = MatchingTypes
                .Where(m => !m.HlaType.IsDeleted)
                .Select(m => new
                {
                    MatchedType = (Serology)m.HlaType,
                    SubtypeCounts = m.MatchingSerologies
                        .Where(s => !s.Equals(m.HlaType))
                        .GroupBy(s => s.SerologySubtype)
                        .Select(s => new { Subtype = s.Key, Count = s.Count() })
                }).ToList();

            var broads = groupBySubtype.Where(s => s.MatchedType.SerologySubtype == SerologySubtype.Broad).ToList();
            var splits = groupBySubtype.Where(s => s.MatchedType.SerologySubtype == SerologySubtype.Split).ToList();
            var associated = groupBySubtype.Where(s => s.MatchedType.SerologySubtype == SerologySubtype.Associated).ToList();
            var notSplits = groupBySubtype.Where(s => s.MatchedType.SerologySubtype == SerologySubtype.NotSplit).ToList();

            // Matching list should not contain the subtype of the Matched type
            Assert.IsFalse(
                groupBySubtype.Any(s => s.SubtypeCounts.Any(sc => sc.Subtype == s.MatchedType.SerologySubtype)));

            // Broads cannot be matched to NotSplit, and must have at least two Splits
            Assert.IsFalse(
                broads.Any(b => b.SubtypeCounts.Any(sc => sc.Subtype == SerologySubtype.NotSplit)));
            Assert.IsFalse(
                broads.Any(b => b.SubtypeCounts.Single(sc => sc.Subtype == SerologySubtype.Split).Count < 2));

            // Splits cannot be matched to NotSplit, and must have one Broad
            Assert.IsFalse(
                splits.Any(s => s.SubtypeCounts.Any(sc => sc.Subtype == SerologySubtype.NotSplit)));
            Assert.IsFalse(
                splits.Any(s => s.SubtypeCounts.Single(sc => sc.Subtype == SerologySubtype.Broad).Count != 1));

            // Associated can only have:
            //      * 1 x NotSplit, or;
            //      * 1 x Broad, or;
            //      * 1 x Split and 1 x Broad
            Assert.IsFalse(
                associated.Any(a => a.SubtypeCounts.Any(sc => sc.Count != 1)));
            Assert.IsFalse(
                associated.Any(a =>
                    a.SubtypeCounts.Any(sc => sc.Subtype == SerologySubtype.NotSplit)
                    && a.SubtypeCounts.Any(sc => sc.Subtype != SerologySubtype.NotSplit)));
            Assert.IsFalse(
                associated
                .Where(a => a.SubtypeCounts.Any(sc => sc.Subtype == SerologySubtype.Split))
                .Any(a => a.SubtypeCounts.All(sc => sc.Subtype != SerologySubtype.Broad)));

            // NotSplits can only be matched to Associated
            Assert.IsFalse(
                notSplits.Any(n => n.SubtypeCounts.Any(sc => sc.Subtype != SerologySubtype.Associated)));
        }
    }
}
