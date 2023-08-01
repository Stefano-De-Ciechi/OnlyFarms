using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyFarms.RestApi.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSomeReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Actuators");

            migrationBuilder.AddColumn<int>(
                name: "SensorType",
                table: "Sensors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IrrigationType",
                table: "Crops",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActuatorType",
                table: "Actuators",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SensorType",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "IrrigationType",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "ActuatorType",
                table: "Actuators");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Sensors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Crops",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Actuators",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
