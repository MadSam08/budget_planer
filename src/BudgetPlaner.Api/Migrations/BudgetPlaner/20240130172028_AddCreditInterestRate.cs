using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BudgetPlaner.Api.Migrations.BudgetPlaner
{
    /// <inheritdoc />
    public partial class AddCreditInterestRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditInterestRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreditId = table.Column<int>(type: "integer", nullable: false),
                    InterestRateType = table.Column<int>(type: "integer", nullable: false),
                    PrincipalValue = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestValue = table.Column<decimal>(type: "numeric", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "IX_CreditInterestRate_CreditId",
                table: "CreditInterestRate",
                column: "CreditId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditInterestRate");
        }
    }
}
