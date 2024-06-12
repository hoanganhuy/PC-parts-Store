using Helpers;
using MySql.Data.MySqlClient;
using PC_Part_Store.Implement;

public static class Program
{
    static void Main()
    {
        using(MySqlConnection connection= DBHelper.GetConnection())
        {
            Account account = new Account();
            account.Register(connection);
        }
    }
}