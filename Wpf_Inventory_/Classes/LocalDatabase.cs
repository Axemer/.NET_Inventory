using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Inventory_.Classes
{
    internal class LocalDatabase
    {
        private const string DbPath = "cache.db";

        public static void Initialize()
        {
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
                
                using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Inventory (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ProductName TEXT,
                            Quantity INTEGER,
                            LastUpdated DATETIME
                        );";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

    }
}
