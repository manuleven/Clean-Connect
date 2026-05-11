using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clean_Connect.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAndBookingSpatialIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Bookings_BookingId1",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_BookingId1",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "BookingId1",
                table: "Ratings");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AuthorizationCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId_Status",
                table: "Payments",
                columns: new[] { "BookingId", "Status" },
                unique: true,
                filter: "[Status] = 'Successful'");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentReference",
                table: "Payments",
                column: "PaymentReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments",
                column: "TransactionId");

            migrationBuilder.Sql(
                @"IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Bookings_LocationPoint'
      AND object_id = OBJECT_ID(N'[Bookings]')
)
BEGIN
    CREATE SPATIAL INDEX IX_Bookings_LocationPoint
    ON Bookings(LocationPoint)
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Bookings_LocationPoint'
      AND object_id = OBJECT_ID(N'[Bookings]')
)
BEGIN
    DROP INDEX IX_Bookings_LocationPoint ON Bookings
END");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Ratings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId1",
                table: "Ratings",
                type: "uniqueidentifier",
                nullable: true);

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
