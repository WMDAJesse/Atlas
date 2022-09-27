﻿using System.ComponentModel.DataAnnotations;
using Atlas.Common.Sql.BulkInsert;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.MatchPrediction.Test.Validation.Data.Models
{
    public class MatchPredictionRequest : IBulkInsertModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DonorId { get; set; }

        /// <summary>
        /// Id generated by Match Prediction Algorithm. Will be NULL if request failed.
        /// </summary>
        [MaxLength(50)]
        public string? MatchPredictionAlgorithmRequestId { get; set; }

        /// <summary>
        /// Only populated if request failed.
        /// </summary>
        public string? RequestErrors { get; set; }
    }

    internal static class MatchPredictionRequestBuilder
    {
        public static void SetUpModel(this EntityTypeBuilder<MatchPredictionRequest> modelBuilder)
        {
            modelBuilder
                .HasOne<SubjectInfo>()
                .WithMany()
                .HasForeignKey(t => t.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .HasOne<SubjectInfo>()
                .WithMany()
                .HasForeignKey(t => t.DonorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .HasIndex(x => new { x.MatchPredictionAlgorithmRequestId, x.DonorId, x.PatientId });
        }
    }
}
