using System;

using System.Linq;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.Classes
{
    internal class LocalDataBase
    {
        

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

            //context.SaveChanges();
        }

    }
}
