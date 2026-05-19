using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employees_Salary_Hub.Migrations
{
    /// <inheritdoc />
    public partial class updatedviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "SalarySlips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAllowances",
                table: "SalarySlips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDeductions",
                table: "SalarySlips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "SalarySlips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfJoining",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "SalarySlips");

            migrationBuilder.DropColumn(
                name: "TotalAllowances",
                table: "SalarySlips");

            migrationBuilder.DropColumn(
                name: "TotalDeductions",
                table: "SalarySlips");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "SalarySlips");

            migrationBuilder.DropColumn(
                name: "DateOfJoining",
                table: "AspNetUsers");
        }
    }
}
