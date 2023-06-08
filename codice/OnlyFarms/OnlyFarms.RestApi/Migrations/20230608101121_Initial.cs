using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyFarms.RestApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FarmingCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    WaterSupply = table.Column<float>(type: "REAL", nullable: false),
                    UniqueCompanyCode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmingCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaterCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    WaterSupply = table.Column<float>(type: "REAL", nullable: false),
                    UniqueCompanyCode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Crops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurfaceArea = table.Column<float>(type: "REAL", nullable: false),
                    IrrigationType = table.Column<int>(type: "INTEGER", nullable: false),
                    WaterNeeds = table.Column<float>(type: "REAL", nullable: false),
                    IdealHumidity = table.Column<float>(type: "REAL", nullable: false),
                    FarmingCompanyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Crops_FarmingCompanies_FarmingCompanyId",
                        column: x => x.FarmingCompanyId,
                        principalTable: "FarmingCompanies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WaterUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConsumedQuantity = table.Column<float>(type: "REAL", nullable: false),
                    FarmingCompanyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterUsages_FarmingCompanies_FarmingCompanyId",
                        column: x => x.FarmingCompanyId,
                        principalTable: "FarmingCompanies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BookedQuantity = table.Column<float>(type: "REAL", nullable: false),
                    Price = table.Column<float>(type: "REAL", nullable: false),
                    OnGoing = table.Column<bool>(type: "INTEGER", nullable: false),
                    FarmingCompanyId = table.Column<int>(type: "INTEGER", nullable: true),
                    WaterCompanyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_FarmingCompanies_FarmingCompanyId",
                        column: x => x.FarmingCompanyId,
                        principalTable: "FarmingCompanies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_WaterCompanies_WaterCompanyId",
                        column: x => x.WaterCompanyId,
                        principalTable: "WaterCompanies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Actuators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CropId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actuators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actuators_Crops_CropId",
                        column: x => x.CropId,
                        principalTable: "Crops",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CropId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sensors_Crops_CropId",
                        column: x => x.CropId,
                        principalTable: "Crops",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActuatorsCommands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    ActuatorId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActuatorsCommands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActuatorsCommands_Actuators_ActuatorId",
                        column: x => x.ActuatorId,
                        principalTable: "Actuators",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SensorsMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Value = table.Column<float>(type: "REAL", nullable: false),
                    MeasuringUnit = table.Column<string>(type: "TEXT", nullable: false),
                    SensorId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorsMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorsMeasurements_Sensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actuators_CropId",
                table: "Actuators",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_ActuatorsCommands_ActuatorId",
                table: "ActuatorsCommands",
                column: "ActuatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Crops_FarmingCompanyId",
                table: "Crops",
                column: "FarmingCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_FarmingCompanyId",
                table: "Reservations",
                column: "FarmingCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_WaterCompanyId",
                table: "Reservations",
                column: "WaterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_CropId",
                table: "Sensors",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorsMeasurements_SensorId",
                table: "SensorsMeasurements",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterUsages_FarmingCompanyId",
                table: "WaterUsages",
                column: "FarmingCompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActuatorsCommands");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "SensorsMeasurements");

            migrationBuilder.DropTable(
                name: "WaterUsages");

            migrationBuilder.DropTable(
                name: "Actuators");

            migrationBuilder.DropTable(
                name: "WaterCompanies");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "Crops");

            migrationBuilder.DropTable(
                name: "FarmingCompanies");
        }
    }
}
