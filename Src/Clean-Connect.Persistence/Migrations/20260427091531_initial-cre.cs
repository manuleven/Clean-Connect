using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Clean_Connect.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class initialcre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "LocationPoint",
                table: "Workers",
                type: "geography",
                nullable: false);

            migrationBuilder.AddColumn<Point>(
                name: "LocationPoint",
                table: "Bookings",
                type: "geography",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationPoint",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "LocationPoint",
                table: "Bookings");
        }
    }
}
