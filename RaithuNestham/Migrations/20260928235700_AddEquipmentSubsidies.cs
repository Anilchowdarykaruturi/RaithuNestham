using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaithuNestham.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentSubsidies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentSubsidies",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    EquipmentName = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    Category = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    SchemeName = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    EligibleFarmers = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    SubsidyDetails = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    MaximumSubsidy = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    ApplicationProcess = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    OfficialWebsite = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    State = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    LastVerified = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_EquipmentSubsidies",
                        x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentSubsidies");
        }
    }
}