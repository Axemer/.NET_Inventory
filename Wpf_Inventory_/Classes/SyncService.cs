using System.Data.SqlClient;
using System.Data.SQLite;

namespace Wpf_Inventory_.Classes
{
    internal class SyncService
    {
        private const string MSSQL_ConnectionString = "Server=your_server;Database=your_db;User Id=your_user;Password=your_password;";
        private const string SQLite_ConnectionString = "Data Source=cache.db;Version=3;";

        public static void SyncWithRemote()
        {
            using (var sqlConnection = new SqlConnection(MSSQL_ConnectionString))
            using (var sqliteConnection = new SQLiteConnection(SQLite_ConnectionString))
            {
                sqlConnection.Open();
                sqliteConnection.Open();

                using (var sqlCommand = new SqlCommand("SELECT Id, ProductName, Quantity, LastUpdated FROM Inventory", sqlConnection))
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var name = reader.GetString(1);
                        var quantity = reader.GetInt32(2);
                        var lastUpdated = reader.GetDateTime(3);

                        using (var sqliteCommand = new SQLiteCommand(sqliteConnection))
                        {
                            sqliteCommand.CommandText = @"
                            INSERT INTO Inventory (Id, ProductName, Quantity, LastUpdated) 
                            VALUES (@Id, @ProductName, @Quantity, @LastUpdated)
                            ON CONFLICT(Id) DO UPDATE SET
                                ProductName = excluded.ProductName,
                                Quantity = excluded.Quantity,
                                LastUpdated = excluded.LastUpdated;";

                            sqliteCommand.Parameters.AddWithValue("@Id", id);
                            sqliteCommand.Parameters.AddWithValue("@ProductName", name);
                            sqliteCommand.Parameters.AddWithValue("@Quantity", quantity);
                            sqliteCommand.Parameters.AddWithValue("@LastUpdated", lastUpdated);

                            sqliteCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
