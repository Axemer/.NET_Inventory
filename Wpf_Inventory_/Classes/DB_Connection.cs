using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf_Inventory_.dbo;

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
    }
}
