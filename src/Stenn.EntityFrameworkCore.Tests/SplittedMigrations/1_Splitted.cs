using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Stenn.EntityFrameworkCore.SplittedMigrations;

namespace Stenn.EntityFrameworkCore.Tests.SplittedMigrations
{
    public class DbContext1 : Microsoft.EntityFrameworkCore.DbContext
    {
        /// <inheritdoc />
        protected DbContext1()
        {
        }

        /// <inheritdoc />
        public DbContext1(DbContextOptions<DbContext1> options)
            : base(options)
        {
        }
    }

    [SplittedMigration(typeof(DbContext0))]
    [DbContext(typeof(DbContext1))]
    [Migration("10_SplittedMigration10")]
    public class SplittedMigration10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }
    }

    [DbContext(typeof(DbContext1))]
    [Migration("11_Migration11")]
    public class Migration11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }
    }

    [DbContext(typeof(DbContext1))]
    [Migration("12_Migration12")]
    public class Migration12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }
    }
}