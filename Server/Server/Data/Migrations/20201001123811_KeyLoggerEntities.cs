using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Server.Data.Migrations
{
    public partial class KeyLoggerEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeyPressSets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyPressSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyPressSets_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyPresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SetId = table.Column<int>(nullable: false),
                    From = table.Column<DateTime>(nullable: false),
                    To = table.Column<DateTime>(nullable: false),
                    Code = table.Column<int>(nullable: false),
                    DataType = table.Column<int>(nullable: true),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyPresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyPresses_KeyPressSets_SetId",
                        column: x => x.SetId,
                        principalTable: "KeyPressSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeyPresses_SetId",
                table: "KeyPresses",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyPressSets_MachineId",
                table: "KeyPressSets",
                column: "MachineId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyPresses");

            migrationBuilder.DropTable(
                name: "KeyPressSets");

            migrationBuilder.DropTable(
                name: "Machines");
        }
    }
}
