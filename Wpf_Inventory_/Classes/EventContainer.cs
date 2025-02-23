using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Inventory_.Classes
{
    /// <summary>
    /// Класс-контейнер для управления собитиями
    /// </summary>
    class EventContainer
    {
        /// <summary>
        /// Событие для оповещения о сохранении устройства
        /// </summary>
        public event EventHandler<string> DeviceSavedEvent;

        /// <summary>
        /// Метод для подписки на событие
        /// </summary>
        /// <param name="handler"></param>
        public void SubscribeToDeviceSaved(EventHandler<string> handler)
        {
            DeviceSavedEvent += handler;
        }

        /// <summary>
        /// Метод для отписки от события
        /// </summary>
        /// <param name="handler"></param>
        public void UnsubscribeFromDeviceSaved(EventHandler<string> handler)
        {
            DeviceSavedEvent -= handler;
        }

        /// <summary>
        /// Безопасный вызов события
        /// </summary>
        /// <param name="deviceName"></param>
        public void TriggerDeviceSaved(string deviceName)
        {
            DeviceSavedEvent?.Invoke(this, deviceName);
        }
    }
}
