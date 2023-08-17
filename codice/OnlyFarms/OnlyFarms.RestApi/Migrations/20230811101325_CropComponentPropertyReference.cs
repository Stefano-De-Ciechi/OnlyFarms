using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyFarms.RestApi.Migrations
{
    /// <inheritdoc />
    public partial class CropComponentPropertyReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "Reservations",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "ActuatorsCommands",
                newName: "Timestamp");

            migrationBuilder.AddColumn<int>(
                name: "ComponentId",
                table: "SensorsMeasurements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ComponentId",
                table: "ActuatorsCommands",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComponentId",
                table: "SensorsMeasurements");

            migrationBuilder.DropColumn(
                name: "ComponentId",
                table: "ActuatorsCommands");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Reservations",
                newName: "TimeStamp");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "ActuatorsCommands",
                newName: "TimeStamp");
        }
    }
}
