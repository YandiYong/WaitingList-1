using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waitingList.Migrations
{
    /// <inheritdoc />
    public partial class AddQueueCheckIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "queueEntries",
                columns: table => new
                {
                    queueEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    visitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    centreId = table.Column<int>(type: "int", nullable: false),
                    queueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    queueNumber = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    joinedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_queueEntries", x => x.queueEntryId);
                    table.ForeignKey(
                        name: "FK_queueEntries_unitCentres_centreId",
                        column: x => x.centreId,
                        principalTable: "unitCentres",
                        principalColumn: "centreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_queueEntries_visits_visitId",
                        column: x => x.visitId,
                        principalTable: "visits",
                        principalColumn: "visitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_queueEntries_centreId_queueDate_queueNumber",
                table: "queueEntries",
                columns: new[] { "centreId", "queueDate", "queueNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_queueEntries_visitId",
                table: "queueEntries",
                column: "visitId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "queueEntries");
        }
    }
}
