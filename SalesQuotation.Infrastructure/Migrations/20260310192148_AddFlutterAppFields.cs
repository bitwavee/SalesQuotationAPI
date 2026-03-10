using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesQuotation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFlutterAppFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "FileUploads",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "FileUploads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "FileUploads",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldPermissions",
                table: "EnquiryStatusConfigs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredFields",
                table: "EnquiryStatusConfigs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PackageTitle",
                table: "Enquiries",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "FieldPermissions",
                table: "EnquiryStatusConfigs");

            migrationBuilder.DropColumn(
                name: "RequiredFields",
                table: "EnquiryStatusConfigs");

            migrationBuilder.DropColumn(
                name: "PackageTitle",
                table: "Enquiries");
        }
    }
}
