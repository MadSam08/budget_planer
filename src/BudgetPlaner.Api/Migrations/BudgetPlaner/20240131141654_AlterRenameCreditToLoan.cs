using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BudgetPlaner.Api.Migrations.BudgetPlaner
{
    /// <inheritdoc />
    public partial class AlterRenameCreditToLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomeEntity_CategoryEntity_CategoryId",
                table: "IncomeEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomeEntity_CurrencyEntity_CurrencyId",
                table: "IncomeEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_SpendingEntity_CategoryEntity_CategoryId",
                table: "SpendingEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_SpendingEntity_CurrencyEntity_CurrencyId",
                table: "SpendingEntity");

            migrationBuilder.DropTable(
                name: "CreditInterestRate");

            migrationBuilder.DropTable(
                name: "CreditEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpendingEntity",
                table: "SpendingEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IncomeEntity",
                table: "IncomeEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CurrencyEntity",
                table: "CurrencyEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryEntity",
                table: "CategoryEntity");

            migrationBuilder.RenameTable(
                name: "SpendingEntity",
                newName: "Spending");

            migrationBuilder.RenameTable(
                name: "IncomeEntity",
                newName: "Income");

            migrationBuilder.RenameTable(
                name: "CurrencyEntity",
                newName: "Currency");

            migrationBuilder.RenameTable(
                name: "CategoryEntity",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_SpendingEntity_CurrencyId",
                table: "Spending",
                newName: "IX_Spending_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_SpendingEntity_CategoryId",
                table: "Spending",
                newName: "IX_Spending_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_IncomeEntity_CurrencyId",
                table: "Income",
                newName: "IX_Income_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_IncomeEntity_CategoryId",
                table: "Income",
                newName: "IX_Income_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Spending",
                table: "Spending",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Income",
                table: "Income",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currency",
                table: "Currency",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Loan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Principal = table.Column<decimal>(type: "numeric", nullable: false),
                    Interest = table.Column<decimal>(type: "numeric", nullable: false),
                    AnnualRate = table.Column<decimal>(type: "numeric", nullable: false),
                    CreditStatus = table.Column<int>(type: "integer", nullable: false),
                    APR = table.Column<decimal>(type: "numeric", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loan_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanInterestRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LoanId = table.Column<int>(type: "integer", nullable: false),
                    InterestPayType = table.Column<int>(type: "integer", nullable: false),
                    PrincipalValue = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestValue = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanInterestRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanInterestRate_Loan_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loan_CurrencyId",
                table: "Loan",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanInterestRate_LoanId",
                table: "LoanInterestRate",
                column: "LoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Income_Category_CategoryId",
                table: "Income",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Income_Currency_CurrencyId",
                table: "Income",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Spending_Category_CategoryId",
                table: "Spending",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Spending_Currency_CurrencyId",
                table: "Spending",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Income_Category_CategoryId",
                table: "Income");

            migrationBuilder.DropForeignKey(
                name: "FK_Income_Currency_CurrencyId",
                table: "Income");

            migrationBuilder.DropForeignKey(
                name: "FK_Spending_Category_CategoryId",
                table: "Spending");

            migrationBuilder.DropForeignKey(
                name: "FK_Spending_Currency_CurrencyId",
                table: "Spending");

            migrationBuilder.DropTable(
                name: "LoanInterestRate");

            migrationBuilder.DropTable(
                name: "Loan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Spending",
                table: "Spending");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Income",
                table: "Income");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currency",
                table: "Currency");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "Spending",
                newName: "SpendingEntity");

            migrationBuilder.RenameTable(
                name: "Income",
                newName: "IncomeEntity");

            migrationBuilder.RenameTable(
                name: "Currency",
                newName: "CurrencyEntity");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "CategoryEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Spending_CurrencyId",
                table: "SpendingEntity",
                newName: "IX_SpendingEntity_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Spending_CategoryId",
                table: "SpendingEntity",
                newName: "IX_SpendingEntity_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Income_CurrencyId",
                table: "IncomeEntity",
                newName: "IX_IncomeEntity_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Income_CategoryId",
                table: "IncomeEntity",
                newName: "IX_IncomeEntity_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpendingEntity",
                table: "SpendingEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IncomeEntity",
                table: "IncomeEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CurrencyEntity",
                table: "CurrencyEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryEntity",
                table: "CategoryEntity",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CreditEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    APR = table.Column<decimal>(type: "numeric", nullable: false),
                    AnnualRate = table.Column<decimal>(type: "numeric", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Interest = table.Column<decimal>(type: "numeric", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalValue = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditEntity_CurrencyEntity_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CurrencyEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditInterestRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreditId = table.Column<int>(type: "integer", nullable: false),
                    InterestPayType = table.Column<int>(type: "integer", nullable: false),
                    InterestValue = table.Column<decimal>(type: "numeric", nullable: false),
                    PrincipalValue = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditInterestRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditInterestRate_CreditEntity_CreditId",
                        column: x => x.CreditId,
                        principalTable: "CreditEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditEntity_CurrencyId",
                table: "CreditEntity",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditInterestRate_CreditId",
                table: "CreditInterestRate",
                column: "CreditId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeEntity_CategoryEntity_CategoryId",
                table: "IncomeEntity",
                column: "CategoryId",
                principalTable: "CategoryEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeEntity_CurrencyEntity_CurrencyId",
                table: "IncomeEntity",
                column: "CurrencyId",
                principalTable: "CurrencyEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpendingEntity_CategoryEntity_CategoryId",
                table: "SpendingEntity",
                column: "CategoryId",
                principalTable: "CategoryEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpendingEntity_CurrencyEntity_CurrencyId",
                table: "SpendingEntity",
                column: "CurrencyId",
                principalTable: "CurrencyEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
