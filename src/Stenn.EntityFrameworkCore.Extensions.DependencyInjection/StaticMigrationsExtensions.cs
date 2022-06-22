using System.Reflection;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    /// <summary>
    ///  Standard extensions for static sql migrations based on guidlines that sql migrations stored in folder '\Migrations.Static\Sql\'  
    /// </summary>
    public static class CommonStaticMigrationsExtensions
    {
        /// <summary>
        /// Gets standard path to resource file '\Migrations.Static\Sql\{filePath}{extension}' 
        /// </summary>
        /// <param name="filePath">File name or path with name without extension </param>
        /// <param name="extension">Resource extension</param>
        /// <returns></returns>
        public static string GetFilePath(string filePath, string extension = ".sql")
        {
            return $@"\Migrations.Static\Sql\{filePath}{extension}";
        }

        /// <summary>
        ///  Gets standard path to resource Apply file '\Migrations.Static\Sql\{filePath}.Apply.sql'
        /// </summary>
        /// <param name="mainFileName">File name or path with name without extension</param>
        /// <returns></returns>
        public static string GetApplyFilePath(string mainFileName)
        {
            return GetFilePath($@"{mainFileName}.Apply");
        }

        /// <summary>
        ///  Gets standard path to resource Revert file '\Migrations.Static\Sql\{filePath}.Revert.sql'
        /// </summary>
        /// <param name="mainFileName">File name or path with name without extension</param>
        /// <returns></returns>
        public static string GetRevertFilePath(string mainFileName)
        {
            return GetFilePath($@"{mainFileName}.Revert");
        }

        /// <summary>
        /// Adds sql res Initial static migration. Initial static migrations will be run before all other migrations(Static or classic EF)
        /// </summary>
        /// <param name="migrations">Static migrations' builder</param>
        /// <param name="mainFileName">File name or path with name without extension</param>
        /// <param name="suppressTransaction">Suppress transaction during run migration or not</param>
        public static void AddInitialSqlResFile(this StaticMigrationBuilder migrations, string mainFileName,
            bool suppressTransaction = false)
        {
            migrations.AddInitResSql(mainFileName, GetFilePath(mainFileName), Assembly.GetCallingAssembly(), suppressTransaction: suppressTransaction);
        }

        /// <summary>
        /// Adds sql res static migration
        /// </summary>
        /// <param name="migrations">Static migrations' builder</param>
        /// <param name="mainFileName">File name or path with name without extension</param>
        /// <param name="type">Type of added resources</param>
        public static void AddSqlResFile(this StaticMigrationBuilder migrations, string mainFileName, ResSqlFile type = ResSqlFile.All)
        {
            migrations.AddResSql(mainFileName,
                type.HasFlag(ResSqlFile.Apply) ? GetApplyFilePath(mainFileName) : null,
                type.HasFlag(ResSqlFile.Revert) ? GetRevertFilePath(mainFileName) : null,
                Assembly.GetCallingAssembly());
        }
    }
}