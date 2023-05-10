﻿// <auto-generated />
using Bakalarska_prace.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Bakalarska_prace.Migrations
{
    [DbContext(typeof(AutosalonDbContext))]
    [Migration("20230406121601_MySql_Add_Cars")]
    partial class MySql_Add_Cars
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Bakalarska_prace.Models.Entities.Cars", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CreateYear")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<double>("Price")
                        .HasColumnType("double");

                    b.Property<string>("VehicleBrand")
                        .IsRequired()
                        .HasMaxLength(56)
                        .HasColumnType("varchar(56)");

                    b.HasKey("Id");

                    b.ToTable("Cars");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreateYear = 2014,
                            Description = "Auto je v pořádku",
                            Model = "škoda",
                            Price = 250000.0,
                            VehicleBrand = "BMH0342"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}