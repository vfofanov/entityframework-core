namespace Stenn.EntityFrameworkCore.SqlServer.Tests
{
        public class TestBase
        {
#if NET6_0
                protected const string DBName = "stenn_efcore_tests_net6";
#elif NET7_0
        protected const string DBName = "stenn_efcore_tests_net7";
#endif

                protected static string GetConnectionString(string dbName)
                {
                        return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI;Encrypt=False";
                }
        }
}