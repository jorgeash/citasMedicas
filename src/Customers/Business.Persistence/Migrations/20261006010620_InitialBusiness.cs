using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Business.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialBusiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Business_Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sigla = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MainEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Business_CustomerType",
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
                    table.PrimaryKey("PK_Business_CustomerType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Business_Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerTypeId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Business_Customer_Business_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Business_Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Business_Customer_Business_CustomerType_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "Business_CustomerType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Business_CustomerContact",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone1 = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Phone2 = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_CustomerContact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Business_CustomerContact_Business_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Business_Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Business_Company",
                columns: new[] { "Id", "CreatedAt", "IsActive", "MainEmail", "Name", "Sigla", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "contacto@clinicavidasalud.example", "Clinica Vida Salud S.A.", "CVS", null },
                    { 2, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "compras@distribuidoraandina.example", "Distribuidora Andina CIA LTDA", "DAN", null },
                    { 3, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "servicios.cliente@epstotal.example", "EPS-total Salud S.A.", "EPST", null }
                });

            migrationBuilder.InsertData(
                table: "Business_CustomerType",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "Empresa", null },
                    { 2, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "Persona Natural", null },
                    { 3, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "EPS / Aseguradora", null }
                });

            migrationBuilder.InsertData(
                table: "Business_Customer",
                columns: new[] { "Id", "Code", "CompanyId", "CreatedAt", "CustomerTypeId", "Email", "IsActive", "LocationId", "Name", "UpdatedAt" },
                values: new object[] { 1, "CLI-0001", 2, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), 1, "compras@distribuidoraandina.example", true, 2, "Distribuidora Andina CIA LTDA", null });

            migrationBuilder.InsertData(
                table: "Business_CustomerContact",
                columns: new[] { "Id", "CreatedAt", "CustomerId", "Email", "FirstName", "IsActive", "LastName", "Phone1", "Phone2", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), 1, "a.torres@distribuidoraandina.example", "Ana", true, "Torres", "+593 2 2345678", "+593 99 1234567", null });

            migrationBuilder.CreateIndex(
                name: "IX_Business_Company_Sigla",
                table: "Business_Company",
                column: "Sigla",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Business_Customer_Code",
                table: "Business_Customer",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Business_Customer_CompanyId",
                table: "Business_Customer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Customer_CustomerTypeId",
                table: "Business_Customer",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Customer_LocationId",
                table: "Business_Customer",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Business_CustomerContact_CustomerId",
                table: "Business_CustomerContact",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Business_CustomerType_Name",
                table: "Business_CustomerType",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Business_CustomerContact");

            migrationBuilder.DropTable(
                name: "Business_Customer");

            migrationBuilder.DropTable(
                name: "Business_Company");

            migrationBuilder.DropTable(
                name: "Business_CustomerType");
        }
    }
}
