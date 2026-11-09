using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Facturas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    FacturaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PacienteID = table.Column<int>(type: "int", nullable: false),
                    PacienteNombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CitaID = table.Column<int>(type: "int", nullable: true),
                    AseguradoraID = table.Column<int>(type: "int", nullable: true),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Impuesto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.FacturaID);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    ServicioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TipoServicio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.ServicioID);
                });

            migrationBuilder.CreateTable(
                name: "Tarifas",
                columns: table => new
                {
                    TarifaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioID = table.Column<int>(type: "int", nullable: false),
                    AseguradoraID = table.Column<int>(type: "int", nullable: true),
                    NombreTarifa = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: true),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarifas", x => x.TarifaID);
                    table.ForeignKey(
                        name: "FK_Tarifas_Servicios_ServicioID",
                        column: x => x.ServicioID,
                        principalTable: "Servicios",
                        principalColumn: "ServicioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Factura_Detalle",
                columns: table => new
                {
                    FacturaDetalleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaID = table.Column<int>(type: "int", nullable: false),
                    ServicioID = table.Column<int>(type: "int", nullable: false),
                    TarifaID = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factura_Detalle", x => x.FacturaDetalleID);
                    table.ForeignKey(
                        name: "FK_Factura_Detalle_Facturas_FacturaID",
                        column: x => x.FacturaID,
                        principalTable: "Facturas",
                        principalColumn: "FacturaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Factura_Detalle_Servicios_ServicioID",
                        column: x => x.ServicioID,
                        principalTable: "Servicios",
                        principalColumn: "ServicioID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Factura_Detalle_Tarifas_TarifaID",
                        column: x => x.TarifaID,
                        principalTable: "Tarifas",
                        principalColumn: "TarifaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Factura_Detalle_FacturaID",
                table: "Factura_Detalle",
                column: "FacturaID");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_Detalle_ServicioID",
                table: "Factura_Detalle",
                column: "ServicioID");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_Detalle_TarifaID",
                table: "Factura_Detalle",
                column: "TarifaID");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_Numero",
                table: "Facturas",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_Codigo",
                table: "Servicios",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tarifas_ServicioID",
                table: "Tarifas",
                column: "ServicioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Factura_Detalle");

            migrationBuilder.DropTable(
                name: "Facturas");

            migrationBuilder.DropTable(
                name: "Tarifas");

            migrationBuilder.DropTable(
                name: "Servicios");
        }
    }
}
