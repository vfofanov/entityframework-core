﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Stenn.EntityFrameworkCore.SplittedMigrations;

namespace Stenn.EntityFrameworkCore.Tests.SplittedMigrations
{
    public class DbContext2 : Microsoft.EntityFrameworkCore.DbContext
    {
        /// <inheritdoc />
        protected DbContext2()
        {
        }

        /// <inheritdoc />
        public DbContext2(DbContextOptions<DbContext2> options)
            : base(options)
        {
        }
    }

    [SplittedMigration(typeof(DbContext1), Initial = true)]
    [DbContext(typeof(DbContext2))]
    [Migration("20_SplittedInitialMigration20")]
    public class SplittedInitialMigration20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }
    }

    [DbContext(typeof(DbContext2))]
    [Migration("21_Migration21")]
    public class Migration21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }
    }

    [DbContext(typeof(DbContext2))]
    [Migration("22_Migration22")]
    public class Migration22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }
    }
}