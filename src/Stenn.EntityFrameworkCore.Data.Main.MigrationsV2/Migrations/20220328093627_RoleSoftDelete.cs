using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Stenn.EntityFrameworkCore.Data.Main;
using Stenn.EntityFrameworkCore.HistoricalMigrations;

namespace Stenn.EntityFrameworkCore.DbContext.Initial.Migrations
{
    [HistoricalMigration(typeof(MainDbContext_Step1))]
    public partial class RoleSoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted",
                table: "Role",
                type: "datetime2",
                nullable: true,
                comment: "Row deleted  datetime. Used for soft delete row. Updated by 'instead of' trigger. Configured by convention 'ISoftDeleteEntity'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Role");
        }
    }
}
