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
        }
    }
}
