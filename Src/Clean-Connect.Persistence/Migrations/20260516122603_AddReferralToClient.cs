using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clean_Connect.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReferralToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferralCode",
                table: "Clients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ReferredById",
                table: "Clients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ReferralCode",
                table: "Clients",
                column: "ReferralCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ReferredById",
                table: "Clients",
                column: "ReferredById");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Clients_ReferredById",
                table: "Clients",
                column: "ReferredById",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Clients_ReferredById",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_ReferralCode",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_ReferredById",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ReferralCode",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ReferredById",
                table: "Clients");
        }
    }
}
