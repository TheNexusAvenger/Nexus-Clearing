﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nexus.Clearing.Server.Database;

#nullable disable

namespace Nexus.Clearing.Server.Migrations
{
    [DbContext(typeof(SqliteContext))]
    [Migration("20230414041145_SqliteInitialCreate")]
    partial class SqliteInitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("Nexus.Clearing.Server.Database.Model.DataStore", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DataStoreKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DataStoreName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("GameId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("DataStores");
                });

            modelBuilder.Entity("Nexus.Clearing.Server.Database.Model.RobloxGameKey", b =>
                {
                    b.Property<long>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("OpenCloudApiKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("WebHookSecret")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("GameId");

                    b.ToTable("RobloxGameKeys");
                });

            modelBuilder.Entity("Nexus.Clearing.Server.Database.Model.RobloxUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("GameIds")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RequestedTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("RobloxUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
