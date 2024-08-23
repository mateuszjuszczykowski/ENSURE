using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATABASE_library.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Measurements",
                columns: table => new
                {
                    _id = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurements", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "RawData",
                columns: table => new
                {
                    _id = table.Column<string>(type: "text", nullable: false),
                    Payload_Time = table.Column<string>(type: "text", nullable: false),
                    Payload_ENERGY_TotalStartTime = table.Column<string>(type: "text", nullable: false),
                    Payload_ENERGY_Total = table.Column<double>(type: "double precision", nullable: false),
                    Payload_ENERGY_Yesterday = table.Column<double>(type: "double precision", nullable: false),
                    Payload_ENERGY_Today = table.Column<double>(type: "double precision", nullable: false),
                    Payload_ENERGY_Period = table.Column<int>(type: "integer", nullable: false),
                    Payload_ENERGY_Power = table.Column<int>(type: "integer", nullable: false),
                    Payload_ENERGY_ApparentPower = table.Column<int>(type: "integer", nullable: false),
                    Payload_ENERGY_ReactivePower = table.Column<int>(type: "integer", nullable: false),
                    Payload_ENERGY_Factor = table.Column<double>(type: "double precision", nullable: false),
                    Payload_ENERGY_Voltage = table.Column<int>(type: "integer", nullable: false),
                    Payload_ENERGY_Current = table.Column<double>(type: "double precision", nullable: false),
                    DeviceID = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawData", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    _id = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    TokenExpiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EnergyPrice = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    _id = table.Column<string>(type: "text", nullable: false),
                    deviceID = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Total = table.Column<double>(type: "double precision", nullable: false),
                    Today = table.Column<double>(type: "double precision", nullable: false),
                    Power = table.Column<int>(type: "integer", nullable: false),
                    ApparentPower = table.Column<int>(type: "integer", nullable: false),
                    ReactivePower = table.Column<int>(type: "integer", nullable: false),
                    Factor = table.Column<double>(type: "double precision", nullable: false),
                    Voltage = table.Column<int>(type: "integer", nullable: false),
                    Current = table.Column<double>(type: "double precision", nullable: false),
                    MeasurementId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x._id);
                    table.ForeignKey(
                        name: "FK_Data_Measurements_MeasurementId",
                        column: x => x.MeasurementId,
                        principalTable: "Measurements",
                        principalColumn: "_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Data_MeasurementId",
                table: "Data",
                column: "MeasurementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Data");

            migrationBuilder.DropTable(
                name: "RawData");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Measurements");
        }
    }
}
