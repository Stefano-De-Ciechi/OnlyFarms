﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlyFarms.Core.Data;

#nullable disable

namespace OnlyFarms.RestApi.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Actuator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ActuatorType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CropId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FarmingCompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CropId");

                    b.ToTable("Actuators");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CompanyName")
                        .HasColumnType("TEXT");

                    b.Property<int?>("CompanyType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Command", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ActuatorId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ComponentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
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

                    b.Property<int>("IdealHumidity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IrrigationType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SurfaceArea")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("WaterNeeds")
                        .HasColumnType("INTEGER");

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

                    b.Property<int>("WaterSupply")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("FarmingCompanies");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Measurement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ComponentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MeasuringUnit")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SensorId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SensorId");

                    b.ToTable("SensorsMeasurements");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Reservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Accepted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookedQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FarmingCompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("OnGoing")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Price")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Timestamp")
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

                    b.Property<int>("FarmingCompanyId")
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

                    b.Property<int>("GlobalWaterLimit")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UniqueCompanyCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("WaterSupply")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("WaterCompanies");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.WaterLimit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("FarmingCompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Limit")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WaterCompanyId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FarmingCompanyId");

                    b.HasIndex("WaterCompanyId");

                    b.ToTable("WaterLimits");
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.WaterUsage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ConsumedQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CropId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FarmingCompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FarmingCompanyId");

                    b.ToTable("WaterUsages");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OnlyFarms.Core.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Actuator", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.Crop", null)
                        .WithMany("Actuators")
                        .HasForeignKey("CropId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlyFarms.Core.Models.Command", b =>
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

            modelBuilder.Entity("OnlyFarms.Core.Models.WaterLimit", b =>
                {
                    b.HasOne("OnlyFarms.Core.Models.FarmingCompany", "FarmingCompany")
                        .WithMany()
                        .HasForeignKey("FarmingCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OnlyFarms.Core.Models.WaterCompany", "WaterCompany")
                        .WithMany()
                        .HasForeignKey("WaterCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FarmingCompany");

                    b.Navigation("WaterCompany");
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
