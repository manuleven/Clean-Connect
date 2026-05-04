using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clean_Connect.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addAddSpatialIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE SPATIAL INDEX IX_Workers_LocationPoint
          ON Workers(LocationPoint)"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"DROP INDEX IX_Workers_LocationPoint ON Workers"
            );
        }
    }
}
