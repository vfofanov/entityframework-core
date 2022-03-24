#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore.StaticMigrations.Enums
{
    public abstract class EnumsStaticMigration : StaticMigration, IStaticSqlMigration
    {
        protected const string KeyColumnName = "Id";
        
        protected readonly DbContext Context;
        protected readonly string SchemaName;

        public EnumsStaticMigration(DbContext context, string schemaName = "enum")
        {
            Context = context;
            SchemaName = schemaName;
        }

        /// <inheritdoc />
        protected override byte[] GetHashInternal()
        {
            var items = GetEnumTables().Select(x => new
            {
                Schema = SchemaName,
                Table = new
                {
                    Name = x.Table.TableName,
                    EnumType = x.Table.EnumType.FullName,
                    ValueType = x.Table.ValueType.FullName,
                    x.Table.Rows
                },
                Properties = x.Properties.Select(p => new
                {
                    Schema = p.DeclaringEntityType.GetSchema(),
                    Table = p.DeclaringEntityType.GetTableName(),
                    p.Name
                })
            });
            return GetHash(items);
        }

        /// <inheritdoc />
        public abstract IEnumerable<MigrationOperation> GetRevertOperations();

        /// <inheritdoc />
        public virtual IEnumerable<MigrationOperation> GetApplyOperations(bool isNew)
        {
            var enumTables = GetEnumTables();
            var duplicates = enumTables.GroupBy(t => t.Table.TableName).Where(g => g.Count() > 1).ToList();
            if (duplicates.Count > 0)
            {

                var duplicatesStr = string.Join(", ", duplicates.Select(g => g.Key));
                throw new EnumStaticMigrationException($"Enum tables duplicates:'{duplicatesStr}'. Use EnumTableAttribute to specify different name");
            }

            if (NeedCreateSchema)
            {
                yield return new EnsureSchemaOperation { Name = SchemaName };
            }
            
            foreach (var enumTable in enumTables)
            {
                #region Create table
                var tableOp = GetCreateTableOperation(enumTable);

                var enumTableSuffix = GetTableNameSuffix(tableOp);

                var prop = enumTable.Properties.First();
                var tableQuilifier = StoreObjectIdentifier.Table(prop.DeclaringEntityType.GetTableName(), prop.DeclaringEntityType.GetSchema());
                //TODO: Check that all properties have the same type

                tableOp.Columns.Add(new AddColumnOperation
                {
                    Schema = tableOp.Schema,
                    Table = tableOp.Name,
                    Name = KeyColumnName,
                    ClrType = prop.ClrType,
                    ColumnType = prop.GetColumnType(tableQuilifier),
                    Scale = prop.GetScale(tableQuilifier),
                    Precision = prop.GetPrecision(tableQuilifier),
                    MaxLength = prop.GetMaxLength(tableQuilifier),
                    IsNullable = prop.IsNullable,
                    IsUnicode = prop.IsUnicode(tableQuilifier),
                    IsFixedLength = prop.IsFixedLength(tableQuilifier)
                });

                tableOp.Columns.Add(new AddColumnOperation
                {
                    Schema = tableOp.Schema,
                    Table = tableOp.Name,
                    Name = "Name",
                    ClrType = typeof(string),
                    MaxLength = 256,
                    IsNullable = false,
                    IsUnicode = true,
                    IsFixedLength = false
                });
                tableOp.Columns.Add(new AddColumnOperation
                {
                    Schema = tableOp.Schema,
                    Table = tableOp.Name,
                    Name = "DisplayName",
                    ClrType = typeof(string),
                    MaxLength = 256,
                    IsNullable = false,
                    IsUnicode = true,
                    IsFixedLength = false
                });
                tableOp.Columns.Add(new AddColumnOperation
                {
                    Schema = tableOp.Schema,
                    Table = tableOp.Name,
                    Name = "Description",
                    ClrType = typeof(string),
                    MaxLength = 256,
                    IsNullable = false,
                    IsUnicode = true,
                    IsFixedLength = false
                });

                tableOp.PrimaryKey = new AddPrimaryKeyOperation
                {
                    Schema = tableOp.Schema,
                    Table = tableOp.Name,
                    Name = $"PK_{enumTableSuffix}",
                    Columns = new[] { KeyColumnName }
                };
                yield return tableOp;
                #endregion

                #region Insert data
                var insertOp = new InsertDataOperation
                {
                    Schema = tableOp.Schema,
                    Table = tableOp.Name,
                    Columns = tableOp.Columns.Select(c => c.Name).ToArray(),
                    ColumnTypes = tableOp.Columns.Select(c => c.ColumnType).ToArray()
                };

                var convert = prop.GetValueConverter()?.ConvertToProvider;
                var providerClrType = prop.GetProviderClrType();
                
                
                var values = new object?[enumTable.Table.Rows.Count, 4];
                for (var i = 0; i < enumTable.Table.Rows.Count; i++)
                {
                    var row = enumTable.Table.Rows[i];
                    values[i, 0] = convert != null
                        ? convert(row.RawValue)
                        : providerClrType != null
                            ? Convert.ChangeType(row.RawValue, providerClrType)
                            : row.Value;
                    values[i, 1] = row.Name;
                    values[i, 2] = row.DisplayName;
                    values[i, 3] = row.Description;
                }

                insertOp.Values = values;
                yield return insertOp;
                #endregion

                #region Foreign keys
                foreach (var property in enumTable.Properties)
                {
                    var schema = property.DeclaringEntityType.GetSchema();
                    var table = property.DeclaringEntityType.GetTableName();
                    var columnName = property.GetColumnName(StoreObjectIdentifier.Table(table, schema))
                                     ?? property.GetColumnBaseName();

                    yield return new AddForeignKeyOperation
                    {
                        Name = GetFKName(table, enumTableSuffix, columnName),
                        Schema = schema,
                        Table = table,
                        PrincipalSchema = tableOp.Schema,
                        PrincipalTable = tableOp.Name,
                        Columns = new[] { columnName },
                        PrincipalColumns = new[] { KeyColumnName },
                        OnDelete = ReferentialAction.NoAction,
                        OnUpdate = ReferentialAction.NoAction
                    };
                }
                #endregion
            }


        }

        protected bool NeedCreateSchema => true;
        
        /// <summary>
        /// Names' suffix for keys
        /// </summary>
        /// <param name="tableOp"></param>
        /// <returns></returns>
        protected virtual string GetTableNameSuffix(CreateTableOperation tableOp)
        {
            return $"{tableOp.Schema}_{tableOp.Name}";
        }

        protected virtual string GetFKName(string hostTable, string enumTable, string columnName)
        {
            return $"FK_{hostTable}_{enumTable}_{columnName}";
        }

        protected virtual CreateTableOperation GetCreateTableOperation(ModelEnumTable enumTable)
        {
            var tableOp = new CreateTableOperation
            {
                Schema = SchemaName,
                Name = enumTable.Table.TableName
            };
            return tableOp;
        }


        private List<ModelEnumTable> GetEnumTables()
        {
            return Context.Model.ExtractEnumTables().ToList();
        }
    }
}