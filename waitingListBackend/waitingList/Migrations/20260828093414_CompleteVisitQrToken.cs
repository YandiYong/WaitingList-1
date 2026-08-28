using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waitingList.Migrations
{
    /// <inheritdoc />
    public partial class CompleteVisitQrToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "qrGeneratedAt",
                table: "visits",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "qrToken",
                table: "visits",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            // Give every existing appointment its own token before adding the unique index.
            migrationBuilder.Sql("""
                UPDATE [visits]
                SET [qrToken] = LOWER(REPLACE(CONVERT(varchar(36), NEWID()), '-', '')),
                    [qrGeneratedAt] = [createdAt];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_visits_qrToken",
                table: "visits",
                column: "qrToken",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_visits_qrToken",
                table: "visits");

            migrationBuilder.DropColumn(
                name: "qrGeneratedAt",
                table: "visits");

            migrationBuilder.DropColumn(
                name: "qrToken",
                table: "visits");
        }
    }
}
