﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stenn.EntityFrameworkCore.Data.Main;

namespace Stenn.EntityFrameworkCore.DbContext.Initial.Migrations
{
    [DbContext(typeof(MainDbContext_Step1PlusStep2))]
    [Migration("20220328093627_RoleSoftDelete")]
    partial class RoleSoftDelete
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.15")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Stenn.EntityFrameworkCore.Data.Main.Contact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EMail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte>("Type")
                        .HasColumnType("tinyint");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TypeName2")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Contact");
                });

            modelBuilder.Entity("Stenn.EntityFrameworkCore.Data.Main.Currency", b =>
                {
                    b.Property<string>("Iso3LetterCode")
                        .HasMaxLength(3)
                        .IsUnicode(false)
                        .HasColumnType("char(3)")
                        .IsFixedLength(true);

                    b.Property<byte>("DecimalDigits")
                        .HasColumnType("tinyint");

                    b.Property<string>("Description")
                        .HasMaxLength(150)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("IsoNumericCode")
                        .HasColumnType("int");

                    b.HasKey("Iso3LetterCode");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("Stenn.EntityFrameworkCore.Data.Main.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()")
                        .HasComment("Row creation datetime. Configured by convention 'ICreateAuditedEntity'");

                    b.Property<DateTime?>("Deleted")
                        .HasColumnType("datetime2")
                        .HasComment("Row deleted  datetime. Used for soft delete row. Updated by 'instead of' trigger. Configured by convention 'ISoftDeleteEntity'")
                        .HasAnnotation("ColumnTriggerSoftDelete", true);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()")
                        .HasComment("Row last modified datetime. Updated by trigger. Configured by convention 'IUpdateAuditedEntity'")
                        .HasAnnotation("ColumnTriggerUpdate", "getdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SourceSystemId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasComment("Source system id. Row id for cross services' communication. Uses trigger on row insertion. Configured by convention 'IEntityWithSourceSystemId'");

                    b.HasKey("Id");

                    b.HasIndex("SourceSystemId")
                        .IsUnique();

                    b.ToTable("Role");
                });
#pragma warning restore 612, 618
        }
    }
}
