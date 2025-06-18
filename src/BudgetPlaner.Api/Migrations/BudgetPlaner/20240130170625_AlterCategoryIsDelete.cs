using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetPlaner.Api.Migrations.BudgetPlaner
{
    /// <inheritdoc />
    public partial class AlterCategoryIsDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CategoryEntity",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CategoryEntity");
        }
    }
}
