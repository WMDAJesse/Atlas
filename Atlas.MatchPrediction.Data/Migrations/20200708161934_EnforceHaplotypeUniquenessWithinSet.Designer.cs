﻿// <auto-generated />
using System;
using Atlas.MatchPrediction.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Atlas.MatchPrediction.Data.Migrations
{
    [DbContext(typeof(MatchPredictionContext))]
    [Migration("20200708161934_EnforceHaplotypeUniquenessWithinSet")]
    partial class EnforceHaplotypeUniquenessWithinSet
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Atlas.MatchPrediction.Data.Models.HaplotypeFrequency", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("A")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("B")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("C")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("DQB1")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("DRB1")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<decimal>("Frequency")
                        .HasColumnType("decimal(20,20)");

                    b.Property<int>("SetId")
                        .HasColumnName("Set_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SetId");

                    b.HasIndex("A", "B", "C", "DQB1", "DRB1", "SetId")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Include", new[] { "Frequency" });

                    b.ToTable("HaplotypeFrequencies");
                });

            modelBuilder.Entity("Atlas.MatchPrediction.Data.Models.HaplotypeFrequencySet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("DateTimeAdded")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("EthnicityCode")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistryCode")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("EthnicityCode", "RegistryCode")
                        .IsUnique()
                        .HasName("IX_RegistryCode_And_EthnicityCode")
                        .HasFilter("[Active] = 'True'");

                    b.ToTable("HaplotypeFrequencySets");
                });

            modelBuilder.Entity("Atlas.MatchPrediction.Data.Models.HaplotypeFrequency", b =>
                {
                    b.HasOne("Atlas.MatchPrediction.Data.Models.HaplotypeFrequencySet", "Set")
                        .WithMany()
                        .HasForeignKey("SetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
