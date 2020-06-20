﻿// <auto-generated />
using System;
using Atlas.MatchingAlgorithm.Data.Persistent.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Atlas.MatchingAlgorithm.Data.Persistent.Migrations
{
    [DbContext(typeof(SearchAlgorithmPersistentContext))]
    partial class SearchAlgorithmPersistentContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Atlas.MatchingAlgorithm.Data.Persistent.Models.DataRefreshRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DataDeletionCompleted")
                        .HasColumnType("datetime2");

                    b.Property<string>("Database")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DatabaseScalingSetupCompleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DatabaseScalingTearDownCompleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DonorHlaProcessingCompleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DonorImportCompleted")
                        .HasColumnType("datetime2");

                    b.Property<string>("HlaNomenclatureVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("IndexDeletionCompleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("IndexRecreationCompleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("MetadataDictionaryRefreshCompleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("QueuedDonorUpdatesCompleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("RefreshBeginUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RefreshContinueUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RefreshEndUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("SupportComments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("WasSuccessful")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("DataRefreshHistory");
                });

            modelBuilder.Entity("Atlas.MatchingAlgorithm.Data.Persistent.Models.ScoringWeightings.ConfidenceWeighting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<int>("Weight")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("ConfidenceWeightings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Mismatch",
                            Weight = 0
                        },
                        new
                        {
                            Id = 2,
                            Name = "Potential",
                            Weight = 0
                        },
                        new
                        {
                            Id = 3,
                            Name = "Exact",
                            Weight = 0
                        },
                        new
                        {
                            Id = 4,
                            Name = "Definite",
                            Weight = 0
                        });
                });

            modelBuilder.Entity("Atlas.MatchingAlgorithm.Data.Persistent.Models.ScoringWeightings.GradeWeighting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<int>("Weight")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("GradeWeightings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Mismatch",
                            Weight = 0
                        },
                        new
                        {
                            Id = 2,
                            Name = "PermissiveMismatch",
                            Weight = 0
                        },
                        new
                        {
                            Id = 3,
                            Name = "Broad",
                            Weight = 0
                        },
                        new
                        {
                            Id = 4,
                            Name = "Split",
                            Weight = 0
                        },
                        new
                        {
                            Id = 5,
                            Name = "Associated",
                            Weight = 0
                        },
                        new
                        {
                            Id = 6,
                            Name = "NullMismatch",
                            Weight = 0
                        },
                        new
                        {
                            Id = 7,
                            Name = "NullPartial",
                            Weight = 0
                        },
                        new
                        {
                            Id = 8,
                            Name = "NullCDna",
                            Weight = 0
                        },
                        new
                        {
                            Id = 9,
                            Name = "NullGDna",
                            Weight = 0
                        },
                        new
                        {
                            Id = 10,
                            Name = "PGroup",
                            Weight = 0
                        },
                        new
                        {
                            Id = 11,
                            Name = "GGroup",
                            Weight = 0
                        },
                        new
                        {
                            Id = 12,
                            Name = "Protein",
                            Weight = 0
                        },
                        new
                        {
                            Id = 13,
                            Name = "CDna",
                            Weight = 0
                        },
                        new
                        {
                            Id = 14,
                            Name = "GDna",
                            Weight = 0
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
