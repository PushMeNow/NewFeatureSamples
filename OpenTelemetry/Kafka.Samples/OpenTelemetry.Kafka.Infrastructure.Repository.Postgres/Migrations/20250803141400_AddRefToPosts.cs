using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddRefToPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql("DELETE FROM \"PostHistoryRecords\";");

            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "PostHistoryRecords",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PostHistoryRecords_PostId",
                table: "PostHistoryRecords",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostHistoryRecords_Posts_PostId",
                table: "PostHistoryRecords",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostHistoryRecords_Posts_PostId",
                table: "PostHistoryRecords");

            migrationBuilder.DropIndex(
                name: "IX_PostHistoryRecords_PostId",
                table: "PostHistoryRecords");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "PostHistoryRecords");
        }
    }
}
