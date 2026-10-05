using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaithuNestham.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Crops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeluguName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Season = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fertilizers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedFor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsageInstructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsOrganic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fertilizers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GovernmentSchemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Eligibility = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Benefits = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationProcess = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfficialWebsite = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GovernmentSchemes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pesticides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetPest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedCrop = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsageInstructions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pesticides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Village = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Humidity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rainfall = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeatherCondition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Farmers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Village = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mandal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farmers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Farmers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AIChats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FarmerId = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIChats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIChats_Farmers_FarmerId",
                        column: x => x.FarmerId,
                        principalTable: "Farmers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FarmerId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AreaInAcres = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoilType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IrrigationType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fields_Farmers_FarmerId",
                        column: x => x.FarmerId,
                        principalTable: "Farmers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CropRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    CropId = table.Column<int>(type: "int", nullable: false),
                    PlantingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HarvestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedYield = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CropRecords_Crops_CropId",
                        column: x => x.CropId,
                        principalTable: "Crops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CropRecords_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIChats_FarmerId",
                table: "AIChats",
                column: "FarmerId");

            migrationBuilder.CreateIndex(
                name: "IX_CropRecords_CropId",
                table: "CropRecords",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_CropRecords_FieldId",
                table: "CropRecords",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Farmers_UserId",
                table: "Farmers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fields_FarmerId",
                table: "Fields",
                column: "FarmerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIChats");

            migrationBuilder.DropTable(
                name: "CropRecords");

            migrationBuilder.DropTable(
                name: "Fertilizers");

            migrationBuilder.DropTable(
                name: "GovernmentSchemes");

            migrationBuilder.DropTable(
                name: "Pesticides");

            migrationBuilder.DropTable(
                name: "WeatherLogs");

            migrationBuilder.DropTable(
                name: "Crops");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "Farmers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
