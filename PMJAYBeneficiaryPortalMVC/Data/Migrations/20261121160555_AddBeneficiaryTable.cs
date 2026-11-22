using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMJAYBeneficiaryPortalMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBeneficiaryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beneficiaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BeneficiaryCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BeneficiaryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AadhaarLast4 = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FamilyId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beneficiaries");
        }
    }
}
