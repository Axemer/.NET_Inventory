using Wpf_Inventory_.dbo;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.Classes
{
    public class DB_Connection
    {
        /// <summary>
        /// Догадайся
        /// </summary>
        private static InventoryRegistryDataBaseEntities3 s_connect;

        /// <summary>
        /// Подключаеся к базе и возвращаем с нее данные
        /// </summary>
        /// <returns></returns>
        internal static InventoryRegistryDataBaseEntities3 GetDataBase()
        {
            if (s_connect == null)
                s_connect = new InventoryRegistryDataBaseEntities3();
            return s_connect;
        }


        /// <summary>
        /// Догадайся
        /// </summary>
        private static InventoryDataBaseContext pgs_connect;

        /// <summary>
        /// Подключаеся к базе и возвращаем с нее данные
        /// </summary>
        /// <returns></returns>
        internal static InventoryDataBaseContext GetDB()
        {
            if (pgs_connect == null)
                pgs_connect = new InventoryDataBaseContext();
            return pgs_connect;
        }
    }
}