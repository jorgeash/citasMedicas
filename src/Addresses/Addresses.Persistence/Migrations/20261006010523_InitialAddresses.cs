using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Addresses.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses_Country",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses_City",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses_City", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_City_Addresses_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Addresses_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses_State",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses_State", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_State_Addresses_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Addresses_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses_Position",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses_Position", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Position_Addresses_City_CityId",
                        column: x => x.CityId,
                        principalTable: "Addresses_City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Addresses_Country",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "Ecuador", null },
                    { 2, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "Colombia", null }
                });

            migrationBuilder.InsertData(
                table: "Addresses_City",
                columns: new[] { "Id", "CountryId", "CreatedAt", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "Quito", null },
                    { 2, 1, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "Guayaquil", null },
                    { 3, 2, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "Bogota", null }
                });

            migrationBuilder.InsertData(
                table: "Addresses_State",
                columns: new[] { "Id", "CountryId", "CreatedAt", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "Pichincha", null },
                    { 2, 1, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "Guayas", null },
                    { 3, 2, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "Cundinamarca", null }
                });

            migrationBuilder.InsertData(
                table: "Addresses_Position",
                columns: new[] { "Id", "Address", "CityId", "CreatedAt", "CustomerId", "IsActive", "Latitude", "Longitude", "UpdatedAt", "ZipCode" },
                values: new object[,]
                {
                    { 1, "Av. Amazonas N34-1234, Edificio Clinica Norte", 1, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, true, -0.1807, -78.467799999999997, null, "EC010101" },
                    { 2, "Av. Francisco de Orellana EO7-22, Piso 3", 1, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, true, -0.17000000000000001, -78.450000000000003, null, "EC010201" },
                    { 3, "Av. 9 de Octubre y Los Rios, Torre Norte", 2, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, true, -2.1894, -79.889099999999999, null, "EC090100" },
                    { 4, "Calle 100 # 11-60, Local 204", 3, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, true, 4.7110000000000003, -74.072100000000006, null, "CO110111" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_City_CountryId_Name",
                table: "Addresses_City",
                columns: new[] { "CountryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Country_Name",
                table: "Addresses_Country",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Position_CityId",
                table: "Addresses_Position",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_State_CountryId_Name",
                table: "Addresses_State",
                columns: new[] { "CountryId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses_Position");

            migrationBuilder.DropTable(
                name: "Addresses_State");

            migrationBuilder.DropTable(
                name: "Addresses_City");

            migrationBuilder.DropTable(
                name: "Addresses_Country");
        }
    }
}
