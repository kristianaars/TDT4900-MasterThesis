using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TDT4900_MasterThesis.Migrations
{
    /// <inheritdoc />
    public partial class EventLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SimulationId",
                table: "NodeEvents",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NodeEvents_SimulationId",
                table: "NodeEvents",
                column: "SimulationId");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeEvents_Simulations_SimulationId",
                table: "NodeEvents",
                column: "SimulationId",
                principalTable: "Simulations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeEvents_Simulations_SimulationId",
                table: "NodeEvents");

            migrationBuilder.DropIndex(
                name: "IX_NodeEvents_SimulationId",
                table: "NodeEvents");

            migrationBuilder.DropColumn(
                name: "SimulationId",
                table: "NodeEvents");
        }
    }
}
