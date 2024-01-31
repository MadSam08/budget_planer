using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetPlaner.Api.Migrations.BudgetPlaner
{
    /// <inheritdoc />
    public partial class AlterCurrencyIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InterestRateType",
                table: "CreditInterestRate",
                newName: "InterestPayType");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CurrencyEntity",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CurrencyEntity");

            migrationBuilder.RenameColumn(
                name: "InterestPayType",
                table: "CreditInterestRate",
                newName: "InterestRateType");
        }
    }
}
