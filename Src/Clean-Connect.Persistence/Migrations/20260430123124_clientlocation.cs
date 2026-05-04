using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Clean_Connect.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class clientlocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookingId1",
                table: "Ratings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Clients",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Point>(
                name: "LocationPoint",
                table: "Clients",
                type: "geography",
                nullable: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Clients",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            // 🔥 ADD SPATIAL INDEX HERE
            migrationBuilder.Sql(
                @"CREATE SPATIAL INDEX IX_Clients_LocationPoint
          ON Clients(LocationPoint)"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_BookingId1",
                table: "Ratings",
                column: "BookingId1",
                unique: true,
                filter: "[BookingId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Bookings_BookingId1",
                table: "Ratings",
                column: "BookingId1",
                principalTable: "Bookings",
                principalColumn: "Id");
        }
    }
}
