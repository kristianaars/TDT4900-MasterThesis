using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TDT4900_MasterThesis.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlgorithmSpecs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AlgorithmType = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    DeltaTExcitatory = table.Column<int>(type: "INTEGER", nullable: true),
                    DeltaTInhibitory = table.Column<int>(type: "INTEGER", nullable: true),
                    RefractoryPeriod = table.Column<int>(type: "INTEGER", nullable: true),
                    TauPlus = table.Column<int>(type: "INTEGER", nullable: true),
                    TauZero = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorithmSpecs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Graphs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Graphs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimulationBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Edge",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GraphId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Edge_Graphs_GraphId",
                        column: x => x.GraphId,
                        principalTable: "Graphs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NodeEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tick = table.Column<long>(type: "INTEGER", nullable: false),
                    EventType = table.Column<int>(type: "INTEGER", nullable: false),
                    SimulationId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    X = table.Column<int>(type: "INTEGER", nullable: false),
                    Y = table.Column<int>(type: "INTEGER", nullable: false),
                    GraphId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SimulationId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nodes_Graphs_GraphId",
                        column: x => x.GraphId,
                        principalTable: "Graphs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Simulations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GraphId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AlgorithmSpecId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartNodeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetNodeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SimulationBatchId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Simulations_AlgorithmSpecs_AlgorithmSpecId",
                        column: x => x.AlgorithmSpecId,
                        principalTable: "AlgorithmSpecs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Simulations_Graphs_GraphId",
                        column: x => x.GraphId,
                        principalTable: "Graphs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Simulations_Nodes_StartNodeId",
                        column: x => x.StartNodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Simulations_Nodes_TargetNodeId",
                        column: x => x.TargetNodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Simulations_SimulationBatches_SimulationBatchId",
                        column: x => x.SimulationBatchId,
                        principalTable: "SimulationBatches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Edge_GraphId",
                table: "Edge",
                column: "GraphId");

            migrationBuilder.CreateIndex(
                name: "IX_Edge_SourceId",
                table: "Edge",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Edge_TargetId",
                table: "Edge",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_NodeEvent_SimulationId",
                table: "NodeEvent",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_GraphId",
                table: "Nodes",
                column: "GraphId");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_SimulationId",
                table: "Nodes",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulations_AlgorithmSpecId",
                table: "Simulations",
                column: "AlgorithmSpecId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulations_GraphId",
                table: "Simulations",
                column: "GraphId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulations_SimulationBatchId",
                table: "Simulations",
                column: "SimulationBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulations_StartNodeId",
                table: "Simulations",
                column: "StartNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulations_TargetNodeId",
                table: "Simulations",
                column: "TargetNodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Edge_Nodes_SourceId",
                table: "Edge",
                column: "SourceId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Edge_Nodes_TargetId",
                table: "Edge",
                column: "TargetId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NodeEvent_Simulations_SimulationId",
                table: "NodeEvent",
                column: "SimulationId",
                principalTable: "Simulations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Simulations_SimulationId",
                table: "Nodes",
                column: "SimulationId",
                principalTable: "Simulations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Graphs_GraphId",
                table: "Nodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Simulations_Graphs_GraphId",
                table: "Simulations");

            migrationBuilder.DropForeignKey(
                name: "FK_Simulations_Nodes_StartNodeId",
                table: "Simulations");

            migrationBuilder.DropForeignKey(
                name: "FK_Simulations_Nodes_TargetNodeId",
                table: "Simulations");

            migrationBuilder.DropTable(
                name: "Edge");

            migrationBuilder.DropTable(
                name: "NodeEvent");

            migrationBuilder.DropTable(
                name: "Graphs");

            migrationBuilder.DropTable(
                name: "Nodes");

            migrationBuilder.DropTable(
                name: "Simulations");

            migrationBuilder.DropTable(
                name: "AlgorithmSpecs");

            migrationBuilder.DropTable(
                name: "SimulationBatches");
        }
    }
}
