using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyFarms.RestApi.Migrations
{
    /// <inheritdoc />
    public partial class addedWaterLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ConsumedQuantity",
                table: "WaterUsages",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "WaterSupply",
                table: "WaterCompanies",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AddColumn<int>(
                name: "GlobalWaterLimit",
                table: "WaterCompanies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Value",
                table: "SensorsMeasurements",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "BookedQuantity",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "WaterSupply",
                table: "FarmingCompanies",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "WaterNeeds",
                table: "Crops",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "SurfaceArea",
                table: "Crops",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "IdealHumidity",
                table: "Crops",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.CreateTable(
                name: "WaterLimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FarmingCompanyId = table.Column<int>(type: "INTEGER", nullable: false),
                    WaterCompanyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Limit = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterLimits_FarmingCompanies_FarmingCompanyId",
                        column: x => x.FarmingCompanyId,
                        principalTable: "FarmingCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WaterLimits_WaterCompanies_WaterCompanyId",
                        column: x => x.WaterCompanyId,
                        principalTable: "WaterCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaterLimits_FarmingCompanyId",
                table: "WaterLimits",
                column: "FarmingCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterLimits_WaterCompanyId",
                table: "WaterLimits",
                column: "WaterCompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaterLimits");

            migrationBuilder.DropColumn(
                name: "GlobalWaterLimit",
                table: "WaterCompanies");

            migrationBuilder.AlterColumn<float>(
                name: "ConsumedQuantity",
                table: "WaterUsages",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<float>(
                name: "WaterSupply",
                table: "WaterCompanies",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<float>(
                name: "Value",
                table: "SensorsMeasurements",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<float>(
                name: "BookedQuantity",
                table: "Reservations",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<float>(
                name: "WaterSupply",
                table: "FarmingCompanies",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<float>(
                name: "WaterNeeds",
                table: "Crops",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<float>(
                name: "SurfaceArea",
                table: "Crops",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<float>(
                name: "IdealHumidity",
                table: "Crops",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
