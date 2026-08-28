using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waitingList.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    clientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    cellNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.clientId);
                });

            migrationBuilder.CreateTable(
                name: "unitCentres",
                columns: table => new
                {
                    centreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    externalCentreId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    centreName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    allowedRadiusMetres = table.Column<int>(type: "int", nullable: false, defaultValue: 150),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    lastSyncedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unitCentres", x => x.centreId);
                });

            migrationBuilder.CreateTable(
                name: "visits",
                columns: table => new
                {
                    visitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    clientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    centreId = table.Column<int>(type: "int", nullable: false),
                    visitDate = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    checkedInAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    cancelledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visits", x => x.visitId);
                    table.ForeignKey(
                        name: "FK_visits_clients_clientId",
                        column: x => x.clientId,
                        principalTable: "clients",
                        principalColumn: "clientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_visits_unitCentres_centreId",
                        column: x => x.centreId,
                        principalTable: "unitCentres",
                        principalColumn: "centreId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_clients_accNumber",
                table: "clients",
                column: "accNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_unitCentres_externalCentreId",
                table: "unitCentres",
                column: "externalCentreId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_visits_centreId",
                table: "visits",
                column: "centreId");

            migrationBuilder.CreateIndex(
                name: "IX_visits_clientId_centreId_visitDate",
                table: "visits",
                columns: new[] { "clientId", "centreId", "visitDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "visits");

            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.DropTable(
                name: "unitCentres");
        }
    }
}
