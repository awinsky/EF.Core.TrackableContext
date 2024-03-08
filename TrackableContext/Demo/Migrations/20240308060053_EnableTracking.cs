using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations
{
    /// <inheritdoc />
    public partial class EnableTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER DATABASE TrackableDemo
                    SET CHANGE_TRACKING = ON  
                    (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)", true);

            migrationBuilder.Sql(@"ALTER TABLE Movies ENABLE CHANGE_TRACKING  
                        WITH (TRACK_COLUMNS_UPDATED = ON) ", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
