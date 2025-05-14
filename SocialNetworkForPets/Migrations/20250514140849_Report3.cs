using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetworkForPets.Migrations
{
    /// <inheritdoc />
    public partial class Report3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Report",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "Report");

            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "Report",
                type: "int",
                nullable: false
            )
            .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Report",
                table: "Report",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_PostId",
                table: "Report",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Report",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "Report");

            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "Report",
                type: "int",
                nullable: false
            );

            migrationBuilder.DropIndex(
                name: "IX_Report_PostId",
                table: "Report");

            migrationBuilder.AlterColumn<int>(
                name: "ReportId",
                table: "Report",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Report",
                table: "Report",
                columns: new[] { "PostId", "UserId" });
        }
    }
}
