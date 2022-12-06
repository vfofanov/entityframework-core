﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Stenn.EntityFrameworkCore.DbContext.Initial.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Iso3LetterCode = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false),
                    IsoNumericCode = table.Column<int>(type: "int", nullable: false),
                    DecimalDigits = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Iso3LetterCode);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currency");
        }
    }
}
