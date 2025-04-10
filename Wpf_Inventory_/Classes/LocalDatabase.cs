using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.Classes
{
    internal class LocalDataBase
    {
        //private InventoryDataBaseContext _dbo = DB_Connection.GetDataBase();

        /// <summary>
        /// Класс для кеширования данных в формате JSON
        /// </summary>
        //private static JsonCache _storage = new JsonCache();

        /// <summary>
        /// Идентификатор базы данных чтоб найти его в кэше
        /// </summary>
        //private static Guid dboID;

        /// <summary>
        /// Обобщающий словарь для хранения всех данных из базы данных
        /// </summary>
        //private Dictionary<Type, object> dump;

        //private LocalDataBase(InventoryDataBaseContext _dbo)
        //{
        //    dump = new Dictionary<Type, object>
        //    {
        //        { typeof(Device), _dbo.Device.ToList() },
        //        { typeof(Devicetype), _dbo.Devicetype.ToList() },
        //        { typeof(Deviceparts), _dbo.Deviceparts.ToList() },
        //        { typeof(Office), _dbo.Office.ToList() },
        //        { typeof(DeviceWorkplace), _dbo.DeviceWorkplace.ToList() },
        //        { typeof(DevicepartsDevice), _dbo.DevicepartsDevice.ToList() },
        //        { typeof(Model.Model), _dbo.Model.ToList() },
        //        { typeof(Workplace), _dbo.Workplace.ToList() }
        //    };
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbo">Базу данных сююда</param>
        //public static void SaveDataBase(InventoryDataBaseContext _dbo)
        //{
        //    dboID = _storage.Store(_dbo);

        //    var dump = new Dictionary<Type, object>
        //    {
        //        { typeof(Device), _dbo.Device.ToList() },
        //        { typeof(Devicetype), _dbo.Devicetype.ToList() },
        //        { typeof(Deviceparts), _dbo.Deviceparts.ToList() },
        //        { typeof(Office), _dbo.Office.ToList() },
        //        { typeof(DeviceWorkplace), _dbo.DeviceWorkplace.ToList() },
        //        { typeof(DevicepartsDevice), _dbo.DevicepartsDevice.ToList() },
        //        { typeof(Model.Model), _dbo.Model.ToList() },
        //        { typeof(Workplace), _dbo.Workplace.ToList() }
        //    };

        //    _storage = new JsonCache(); // если нужно обнулять перед сохранением

        //    foreach (var kv in dump)
        //    {
        //        _storage.Store(kv.Value); // сохраняем каждую таблицу по типу
        //    }

        //    _storage.SaveToFile("Cache.json");
        //}

        ///// <summary>
        ///// Получает базу данных из кэша
        ///// </summary>
        ///// <returns> Возвращает базу данных в виде списка </returns>
        //public static InventoryDataBaseContext GetDataBase()
        //{
        //    _storage.LoadFromFile("Cache.json");
        //    return _storage.Retrieve<InventoryDataBaseContext>(dboID);
        //}

        private static JsonCache _storage = new JsonCache();
        private static Guid _snapshotId;

        public static void SaveDataBase(InventoryDataBaseContext _dbo)
        {
            var snapshot = new DatabaseSnapshot
            {
                Devices = _dbo.Device.ToList(),
                Devicetypes = _dbo.Devicetype.ToList(),
                Deviceparts = _dbo.Deviceparts.ToList(),
                Offices = _dbo.Office.ToList(),
                DeviceWorkplaces = _dbo.DeviceWorkplace.ToList(),
                DevicepartsDevices = _dbo.DevicepartsDevice.ToList(),
                Models = _dbo.Model.ToList(),
                Workplaces = _dbo.Workplace.ToList()
            };

            _snapshotId = _storage.Store(snapshot);
            _storage.SaveToFile("Cache.json");
        }

        public static DatabaseSnapshot LoadDataBase()
        {
            _storage.LoadFromFile("Cache.json");
            return _storage.Retrieve<DatabaseSnapshot>(_snapshotId);
        }

        public static void LoadToContext(InventoryDataBaseContext context, DatabaseSnapshot snapshot)
        {
            context.Device.AddRange(snapshot.Devices);
            context.Devicetype.AddRange(snapshot.Devicetypes);
            context.Deviceparts.AddRange(snapshot.Deviceparts);
            context.Office.AddRange(snapshot.Offices);
            context.DeviceWorkplace.AddRange(snapshot.DeviceWorkplaces);
            context.DevicepartsDevice.AddRange(snapshot.DevicepartsDevices);
            context.Model.AddRange(snapshot.Models);
            context.Workplace.AddRange(snapshot.Workplaces);

            context.SaveChanges();
        }

    }
}
