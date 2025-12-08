using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlgorithmType = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    DeltaTExcitatory = table.Column<int>(type: "integer", nullable: true),
                    DeltaTInhibitory = table.Column<int>(type: "integer", nullable: true),
                    RefractoryPeriod = table.Column<int>(type: "integer", nullable: true),
                    TauPlus = table.Column<int>(type: "integer", nullable: true),
                    TauZero = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorithmSpecs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Graphs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDirected = table.Column<bool>(type: "boolean", nullable: false),
                    Levels = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Graphs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GraphSpecs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NodeCount = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    Radius = table.Column<double>(type: "double precision", nullable: true),
                    Distance = table.Column<int>(type: "integer", nullable: true),
                    Noise = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphSpecs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Node",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NodeId = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Node", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimulationBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlgorithmSpecId = table.Column<int>(type: "integer", nullable: false),
                    GraphSpecId = table.Column<int>(type: "integer", nullable: false),
                    RandomizeStartAndTarget = table.Column<bool>(type: "boolean", nullable: false),
                    StartNodeId = table.Column<int>(type: "integer", nullable: false),
                    TargetNodeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimulationBatches_AlgorithmSpecs_AlgorithmSpecId",
                        column: x => x.AlgorithmSpecId,
                        principalTable: "AlgorithmSpecs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimulationBatches_GraphSpecs_GraphSpecId",
                        column: x => x.GraphSpecId,
                        principalTable: "GraphSpecs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Simulations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SimulationBatchId = table.Column<int>(type: "integer", nullable: false),
                    GraphId = table.Column<int>(type: "integer", nullable: true),
                    StartNodeId = table.Column<int>(type: "integer", nullable: true),
                    TargetNodeId = table.Column<int>(type: "integer", nullable: true),
                    AlgorithmTaggedNodes = table.Column<List<int>>(type: "integer[]", nullable: true),
                    ShortestPathTaggedNodes = table.Column<List<int>>(type: "integer[]", nullable: true),
                    AlgorithmShortestPathLength = table.Column<int>(type: "integer", nullable: false),
                    ShortestPathLength = table.Column<int>(type: "integer", nullable: false),
                    AlgorithmExecutionTime = table.Column<long>(type: "bigint", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Simulations_Graphs_GraphId",
                        column: x => x.GraphId,
                        principalTable: "Graphs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Simulations_Node_StartNodeId",
                        column: x => x.StartNodeId,
                        principalTable: "Node",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Simulations_Node_TargetNodeId",
                        column: x => x.TargetNodeId,
                        principalTable: "Node",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Simulations_SimulationBatches_SimulationBatchId",
                        column: x => x.SimulationBatchId,
                        principalTable: "SimulationBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NodeEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NodeId = table.Column<int>(type: "integer", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    SimulationId = table.Column<int>(type: "integer", nullable: true),
                    Tick = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NodeEvents_Simulations_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NodeEvents_SimulationId",
                table: "NodeEvents",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationBatches_AlgorithmSpecId",
                table: "SimulationBatches",
                column: "AlgorithmSpecId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationBatches_GraphSpecId",
                table: "SimulationBatches",
                column: "GraphSpecId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeEvents");

            migrationBuilder.DropTable(
                name: "Simulations");

            migrationBuilder.DropTable(
                name: "Graphs");

            migrationBuilder.DropTable(
                name: "Node");

            migrationBuilder.DropTable(
                name: "SimulationBatches");

            migrationBuilder.DropTable(
                name: "AlgorithmSpecs");

            migrationBuilder.DropTable(
                name: "GraphSpecs");
        }
    }
}
