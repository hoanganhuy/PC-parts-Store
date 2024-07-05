using MySql.Data.MySqlClient;

namespace Helpers
{
    public static class DBHelper
    {
#if DEBUG
        private static string defaultConnectionString = "Server=localhost;Database=pcshop;User ID=root;Password=Huyat1529#;";
#else
        private static string defaultConnectionString = "Your_Release_Mode_Connection_String_Here";
#endif

        public static string DefaultConnectionString => defaultConnectionString;

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(defaultConnectionString);
        }
    }
}
