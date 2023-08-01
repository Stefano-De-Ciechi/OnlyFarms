using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyFarms.RestApi.Migrations
{
    /// <inheritdoc />
    public partial class BetterReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actuators_Crops_CropId",
                table: "Actuators");

            migrationBuilder.DropForeignKey(
                name: "FK_ActuatorsCommands_Actuators_ActuatorId",
                table: "ActuatorsCommands");

            migrationBuilder.DropForeignKey(
                name: "FK_Crops_FarmingCompanies_FarmingCompanyId",
                table: "Crops");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_FarmingCompanies_FarmingCompanyId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_WaterCompanies_WaterCompanyId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_Crops_CropId",
                table: "Sensors");

            migrationBuilder.DropForeignKey(
                name: "FK_SensorsMeasurements_Sensors_SensorId",
                table: "SensorsMeasurements");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterUsages_FarmingCompanies_FarmingCompanyId",
                table: "WaterUsages");

            migrationBuilder.DropColumn(
                name: "IrrigationType",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ActuatorsCommands");

            migrationBuilder.AlterColumn<int>(
                name: "FarmingCompanyId",
                table: "WaterUsages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "WaterCompanies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "SensorId",
                table: "SensorsMeasurements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Sensors",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "CropId",
                table: "Sensors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WaterCompanyId",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FarmingCompanyId",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "FarmingCompanies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "FarmingCompanyId",
                table: "Crops",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Crops",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Crops",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "ActuatorId",
                table: "ActuatorsCommands",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CropId",
                table: "Actuators",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Actuators",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Actuators_Crops_CropId",
                table: "Actuators",
                column: "CropId",
                principalTable: "Crops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActuatorsCommands_Actuators_ActuatorId",
                table: "ActuatorsCommands",
                column: "ActuatorId",
                principalTable: "Actuators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Crops_FarmingCompanies_FarmingCompanyId",
                table: "Crops",
                column: "FarmingCompanyId",
                principalTable: "FarmingCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_FarmingCompanies_FarmingCompanyId",
                table: "Reservations",
                column: "FarmingCompanyId",
                principalTable: "FarmingCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_WaterCompanies_WaterCompanyId",
                table: "Reservations",
                column: "WaterCompanyId",
                principalTable: "WaterCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_Crops_CropId",
                table: "Sensors",
                column: "CropId",
                principalTable: "Crops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SensorsMeasurements_Sensors_SensorId",
                table: "SensorsMeasurements",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterUsages_FarmingCompanies_FarmingCompanyId",
                table: "WaterUsages",
                column: "FarmingCompanyId",
                principalTable: "FarmingCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actuators_Crops_CropId",
                table: "Actuators");

            migrationBuilder.DropForeignKey(
                name: "FK_ActuatorsCommands_Actuators_ActuatorId",
                table: "ActuatorsCommands");

            migrationBuilder.DropForeignKey(
                name: "FK_Crops_FarmingCompanies_FarmingCompanyId",
                table: "Crops");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_FarmingCompanies_FarmingCompanyId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_WaterCompanies_WaterCompanyId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_Crops_CropId",
                table: "Sensors");

            migrationBuilder.DropForeignKey(
                name: "FK_SensorsMeasurements_Sensors_SensorId",
                table: "SensorsMeasurements");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterUsages_FarmingCompanies_FarmingCompanyId",
                table: "WaterUsages");

            migrationBuilder.DropColumn(
                name: "City",
                table: "WaterCompanies");

            migrationBuilder.DropColumn(
                name: "City",
                table: "FarmingCompanies");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Actuators");

            migrationBuilder.AlterColumn<int>(
                name: "FarmingCompanyId",
                table: "WaterUsages",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "SensorId",
                table: "SensorsMeasurements",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Sensors",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "CropId",
                table: "Sensors",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "WaterCompanyId",
                table: "Reservations",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "FarmingCompanyId",
                table: "Reservations",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "FarmingCompanyId",
                table: "Crops",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "IrrigationType",
                table: "Crops",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ActuatorId",
                table: "ActuatorsCommands",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ActuatorsCommands",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CropId",
                table: "Actuators",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Actuators_Crops_CropId",
                table: "Actuators",
                column: "CropId",
                principalTable: "Crops",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActuatorsCommands_Actuators_ActuatorId",
                table: "ActuatorsCommands",
                column: "ActuatorId",
                principalTable: "Actuators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Crops_FarmingCompanies_FarmingCompanyId",
                table: "Crops",
                column: "FarmingCompanyId",
                principalTable: "FarmingCompanies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_FarmingCompanies_FarmingCompanyId",
                table: "Reservations",
                column: "FarmingCompanyId",
                principalTable: "FarmingCompanies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_WaterCompanies_WaterCompanyId",
                table: "Reservations",
                column: "WaterCompanyId",
                principalTable: "WaterCompanies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_Crops_CropId",
                table: "Sensors",
                column: "CropId",
                principalTable: "Crops",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorsMeasurements_Sensors_SensorId",
                table: "SensorsMeasurements",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WaterUsages_FarmingCompanies_FarmingCompanyId",
                table: "WaterUsages",
                column: "FarmingCompanyId",
                principalTable: "FarmingCompanies",
                principalColumn: "Id");
        }
    }
}
