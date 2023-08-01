﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlyFarms.RestApi.Data;

#nullable disable

namespace OnlyFarms.RestApi.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230801223008_RemovedSomeReferences")]
    partial class RemovedSomeReferences
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("OnlyFarms.Core.Models.Actuator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ActuatorType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CropId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CropId");

                    b.ToTable("Actuators");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.ActuatorCommand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ActuatorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ActuatorId");

                    b.ToTable("ActuatorsCommands");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Crop", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("FarmingCompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("IdealHumidity")
                        .HasColumnType("REAL");

                    b.Property<int>("IrrigationType")
                        .HasColumnType("INTEGER");

                    b.Property<float>("SurfaceArea")
                        .HasColumnType("REAL");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<float>("WaterNeeds")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("FarmingCompanyId");

                    b.ToTable("Crops");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.FarmingCompany", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UniqueCompanyCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<float>("WaterSupply")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("FarmingCompanies");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Measurement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("MeasuringUnit")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SensorId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<float>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("SensorId");

                    b.ToTable("SensorsMeasurements");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Reservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("BookedQuantity")
                        .HasColumnType("REAL");

                    b.Property<int>("FarmingCompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("OnGoing")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Price")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.Property<int>("WaterCompanyId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FarmingCompanyId");

                    b.HasIndex("WaterCompanyId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Sensor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CropId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SensorType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CropId");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.WaterCompany", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UniqueCompanyCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<float>("WaterSupply")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("WaterCompanies");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.WaterUsage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("ConsumedQuantity")
                        .HasColumnType("REAL");

                    b.Property<int>("FarmingCompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FarmingCompanyId");

                    b.ToTable("WaterUsages");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Actuator", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.Crop", null)
                        .WithMany("Actuators")
                        .HasForeignKey("CropId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.ActuatorCommand", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.Actuator", null)
                        .WithMany("Commands")
                        .HasForeignKey("ActuatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Crop", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.FarmingCompany", null)
                        .WithMany("Crops")
                        .HasForeignKey("FarmingCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Measurement", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.Sensor", null)
                        .WithMany("Measurements")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Reservation", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.FarmingCompany", null)
                        .WithMany("Reservations")
                        .HasForeignKey("FarmingCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OnlyFarms.Core.Models.WaterCompany", null)
                        .WithMany("Reservations")
                        .HasForeignKey("WaterCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Sensor", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.Crop", null)
                        .WithMany("Sensors")
                        .HasForeignKey("CropId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.WaterUsage", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.FarmingCompany", null)
                        .WithMany("WaterUsages")
                        .HasForeignKey("FarmingCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Actuator", b =>
                {
                    b.Navigation("Commands");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Crop", b =>
                {
                    b.Navigation("Actuators");

                    b.Navigation("Sensors");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.FarmingCompany", b =>
                {
                    b.Navigation("Crops");

                    b.Navigation("Reservations");

                    b.Navigation("WaterUsages");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Sensor", b =>
                {
                    b.Navigation("Measurements");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.WaterCompany", b =>
                {
                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}
