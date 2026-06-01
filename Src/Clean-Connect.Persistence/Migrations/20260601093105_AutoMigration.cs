using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clean_Connect.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AutoMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DatePaidOut",
                table: "Escrows",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaystackTransferCode",
                table: "Escrows",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuccessfulReferralCount",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatePaidOut",
                table: "Escrows");

            migrationBuilder.DropColumn(
                name: "PaystackTransferCode",
                table: "Escrows");

            migrationBuilder.DropColumn(
                name: "SuccessfulReferralCount",
                table: "Clients");
        }
    }
}
