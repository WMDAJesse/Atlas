﻿using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.DonorImport.FileSchema.Models;
using Atlas.DonorImport.Test.TestHelpers.Builders;
using Atlas.DonorImport.Validators;
using FluentAssertions;
using NUnit.Framework;

namespace Atlas.DonorImport.Test.Validators
{
    [TestFixture]
    internal class SearchableDonorValidatorTests
    {
        private static readonly IEnumerable<ImportDonorChangeType> NonDeleteChangeTypes = Enum.GetValues<ImportDonorChangeType>()
            .Except(new[] { ImportDonorChangeType.Delete });

        private static readonly IEnumerable<ImportedHla> EmptyHla = new[]
        {
            null,
            new ImportedHla()
        };

        private SearchableDonorValidator validator;

        [SetUp]
        public void SetUp()
        {
            validator = new SearchableDonorValidator();
        }

        [TestCaseSource(nameof(EmptyHla))]
        public void Validate_ChangeTypeIsDelete_AndHlaIsEmpty_ReturnsValid(ImportedHla emptyHla)
        {
            var donorUpdate = DonorUpdateBuilder.NoHla
                .With(x => x.ChangeType, ImportDonorChangeType.Delete)
                .WithHla(emptyHla)
                .Build();

            var result = validator.Validate(donorUpdate);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Validate_ChangeTypeIsDelete_AndHlaIsNotEmpty_ReturnsValid()
        {
            var donorUpdate = DonorUpdateBuilder.New
                .With(x => x.ChangeType, ImportDonorChangeType.Delete)
                .Build();

            var result = validator.Validate(donorUpdate);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Validate_ChangeTypeIsNotDelete_AndHlaIsEmpty_ReturnsInvalid(
            [ValueSource(nameof(NonDeleteChangeTypes))] ImportDonorChangeType changeType,
            [ValueSource(nameof(EmptyHla))] ImportedHla emptyHla)
        {
            var donorUpdate = DonorUpdateBuilder.NoHla
                .With(x => x.ChangeType, changeType)
                .WithHla(emptyHla)
                .Build();

            var result = validator.Validate(donorUpdate);

            result.IsValid.Should().BeFalse();
        }

        [TestCaseSource(nameof(NonDeleteChangeTypes))]
        public void Validate_ChangeTypeIsNotDelete_AndHlaIsNotEmpty_ReturnsValid(ImportDonorChangeType changeType)
        {
            var donorUpdate = DonorUpdateBuilder.New
                .With(x => x.ChangeType, changeType)
                .Build();

            var result = validator.Validate(donorUpdate);

            result.IsValid.Should().BeTrue();
        }
    }
}