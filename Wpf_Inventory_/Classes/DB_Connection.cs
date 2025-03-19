using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.Classes
{
    public class DB_Connection
    {
        /// <summary>
        /// Догадайся
        /// </summary>
        private static InventoryDataBaseContext pgs_connect;

        /// <summary>
        /// Подключаеся к базе и возвращаем с нее данные
        /// </summary>
        /// <returns></returns>
        internal static InventoryDataBaseContext GetDataBase()
        {
            if (pgs_connect == null)
                pgs_connect = new InventoryDataBaseContext();
            return pgs_connect;
        }
    }
}