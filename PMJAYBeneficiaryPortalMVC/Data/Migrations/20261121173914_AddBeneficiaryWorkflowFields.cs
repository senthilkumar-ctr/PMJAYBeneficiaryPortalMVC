using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMJAYBeneficiaryPortalMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBeneficiaryWorkflowFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Beneficiaries",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalRemarks",
                table: "Beneficiaries",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Beneficiaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Beneficiaries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectionRemarks",
                table: "Beneficiaries",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectionRequestedBy",
                table: "Beneficiaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CorrectionRequestedDate",
                table: "Beneficiaries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResubmittedDate",
                table: "Beneficiaries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationRemarks",
                table: "Beneficiaries",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedBy",
                table: "Beneficiaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedDate",
                table: "Beneficiaries",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalRemarks",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "CorrectionRemarks",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "CorrectionRequestedBy",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "CorrectionRequestedDate",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "ResubmittedDate",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "VerificationRemarks",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "VerifiedDate",
                table: "Beneficiaries");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Beneficiaries",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
