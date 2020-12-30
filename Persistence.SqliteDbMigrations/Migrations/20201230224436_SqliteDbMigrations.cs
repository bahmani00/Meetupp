using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.SqliteDbMigrations.Migrations
{
    public partial class SqliteDbMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2020, 12, 30, 17, 44, 35, 681, DateTimeKind.Local).AddTicks(1442));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2020, 12, 31, 17, 44, 35, 686, DateTimeKind.Local).AddTicks(635));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 3,
                column: "Date",
                value: new DateTime(2021, 1, 1, 17, 44, 35, 686, DateTimeKind.Local).AddTicks(745));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 4,
                column: "Date",
                value: new DateTime(2021, 1, 2, 17, 44, 35, 686, DateTimeKind.Local).AddTicks(753));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 5,
                column: "Date",
                value: new DateTime(2021, 1, 3, 17, 44, 35, 686, DateTimeKind.Local).AddTicks(757));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 6,
                column: "Date",
                value: new DateTime(2021, 1, 4, 17, 44, 35, 686, DateTimeKind.Local).AddTicks(762));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 7,
                column: "Date",
                value: new DateTime(2021, 1, 5, 17, 44, 35, 686, DateTimeKind.Local).AddTicks(766));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2020, 12, 27, 21, 32, 31, 466, DateTimeKind.Local).AddTicks(6525));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2020, 12, 28, 21, 32, 31, 470, DateTimeKind.Local).AddTicks(3598));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 3,
                column: "Date",
                value: new DateTime(2020, 12, 29, 21, 32, 31, 470, DateTimeKind.Local).AddTicks(3692));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 4,
                column: "Date",
                value: new DateTime(2020, 12, 30, 21, 32, 31, 470, DateTimeKind.Local).AddTicks(3700));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 5,
                column: "Date",
                value: new DateTime(2020, 12, 31, 21, 32, 31, 470, DateTimeKind.Local).AddTicks(3705));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 6,
                column: "Date",
                value: new DateTime(2021, 1, 1, 21, 32, 31, 470, DateTimeKind.Local).AddTicks(3708));

            migrationBuilder.UpdateData(
                table: "WeatherForecasts",
                keyColumn: "Id",
                keyValue: 7,
                column: "Date",
                value: new DateTime(2021, 1, 2, 21, 32, 31, 470, DateTimeKind.Local).AddTicks(3712));
        }
    }
}
