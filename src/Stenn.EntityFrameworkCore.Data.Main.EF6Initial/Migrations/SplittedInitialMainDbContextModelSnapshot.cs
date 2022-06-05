﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stenn.EntityFrameworkCore.Data.Main.EF6Initial;
using Stenn.EntityFrameworkCore.Data.Main.HistoricalInitial;

namespace Stenn.EntityFrameworkCore.Data.Main.HistoricalInitial.Migrations
{
    [DbContext(typeof(EF6InitialMainDbContext))]
    partial class HistoricalInitialMainDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.15")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Stenn.EntityFrameworkCore.Data.Main.Animal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()")
                        .HasComment("Row creation datetime. Configured by convention 'ICreateAuditedEntity'");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .IsFixedLength(false)
                        .HasComment("Discriminator. Configured by convention 'IEntityWithDiscriminator<>'");

                    b.Property<DateTime>("ModifiedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()")
                        .HasComment("Row last modified datetime. Updated by trigger. Configured by convention 'IUpdateAuditedEntity'")
                        .HasAnnotation("ColumnTriggerUpdate", "getdate()");

                    b.HasKey("Id");

                    b.HasIndex("Discriminator");

                    b.ToTable("Animal");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Animal");
                });

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

                    b.Property<byte?>("TypeNameNullable")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.ToTable("Contact");
                });

            modelBuilder.Entity("Stenn.EntityFrameworkCore.Data.Main.Contact2", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()")
                        .HasComment("Row creation datetime. Configured by convention 'ICreateAuditedEntity'");

                    b.Property<string>("EMail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SourceSystemId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)")
                        .IsFixedLength(false)
                        .HasComment("Source system id. Row id for cross services' communication. Configured by convention 'IEntityWithSourceSystemId'");

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

                    b.Property<byte?>("TypeNameNullable")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.ToTable("Contact2");
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

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Iso3LetterCode");

                    b.ToTable("Currency");

                    b.HasData(
                        new
                        {
                            Iso3LetterCode = "TST",
                            DecimalDigits = (byte)1,
                            Description = "Test currency Changed",
                            IsoNumericCode = 1,
                            Type = 2
                        });
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
                        .HasComment("Row deleted  datetime. Used for soft delete row. Updated by 'instead of' trigger. Configured by convention 'ISoftDeleteEntity'");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasComment("Row deleted flag. Used for soft delete row. Updated by 'instead of' trigger. Configured by convention 'ISoftDeleteEntity'");

                    b.Property<DateTime>("ModifiedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()")
                        .HasComment("Row last modified datetime. Updated by trigger. Configured by convention 'IUpdateAuditedEntity'")
                        .HasAnnotation("ColumnTriggerUpdate", "getdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IsDeleted");

                    b.ToTable("Role");

                    b
                        .HasAnnotation("ColumnTriggerSoftDelete", true);

                    b.HasData(
                        new
                        {
                            Id = new Guid("e017806b-2682-439d-9486-8c13b9f8246a"),
                            Description = "Admin desc",
                            Name = "Admin"
                        },
                        new
                        {
                            Id = new Guid("acadf8c5-dcc8-4f11-aa77-62e2a792f62c"),
                            Description = "Customer desc",
                            Name = "Customer"
                        },
                        new
                        {
                            Id = new Guid("2edf50ca-771c-4002-8704-f75e2255d393"),
                            Description = "Support desc",
                            Name = "Support"
                        });
                });

            modelBuilder.Entity("Stenn.EntityFrameworkCore.Data.Main.Cat", b =>
                {
                    b.HasBaseType("Stenn.EntityFrameworkCore.Data.Main.Animal");

                    b.HasDiscriminator().HasValue("Cat");
                });

            modelBuilder.Entity("Stenn.EntityFrameworkCore.Data.Main.Elefant", b =>
                {
                    b.HasBaseType("Stenn.EntityFrameworkCore.Data.Main.Animal");

                    b.HasDiscriminator().HasValue("Elefant");
                });
#pragma warning restore 612, 618
        }
    }
}