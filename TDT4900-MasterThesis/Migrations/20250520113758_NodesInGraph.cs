using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TDT4900_MasterThesis.Migrations
{
    /// <inheritdoc />
    public partial class NodesInGraph : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GraphId",
                table: "Node",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Node_GraphId",
                table: "Node",
                column: "GraphId");

            migrationBuilder.AddForeignKey(
                name: "FK_Node_Graphs_GraphId",
                table: "Node",
                column: "GraphId",
                principalTable: "Graphs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Node_Graphs_GraphId",
                table: "Node");

            migrationBuilder.DropIndex(
                name: "IX_Node_GraphId",
                table: "Node");

            migrationBuilder.DropColumn(
                name: "GraphId",
                table: "Node");
        }
    }
}
