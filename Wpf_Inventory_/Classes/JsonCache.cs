using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Linq;

namespace Wpf_Inventory_.Classes
{
    /// <summary>
    /// Класс кешированния любых переменных в формате JSON
    /// Можно использовать с любыми типами, включая пользовательские. 
    /// Главное — извлекать тем же типом, которым сохранял.
    /// </summary>
    public class JsonCache
    {
        /// <summary>
        /// Временное хранилище для данных
        /// </summary>
        private readonly ConcurrentDictionary<Guid, (object Value, Type Type)> _storage = new();

        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };

        /// <summary>
        /// Сохраняет данные и возвращает идентификатор
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public Guid Store<T>(T data)
        {
            var id = Guid.NewGuid();
            _storage[id] = (data!, typeof(T));
            return id;
        }

        /// <summary>
        /// Возвращает данные по идентификатору в том же типе, в котором они были сохранены
        /// </summary>
        /// <typeparam name="T">Тип данных которым сохранимый был ранее</typeparam>
        /// <param name="id">Индентификатор по которому будет получена переменная</param>
        /// <returns>Данны соотвествовашие ID</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T Retrieve<T>(Guid id)
        {
            var pair = _storage.Values.FirstOrDefault(x => x.Type == typeof(T));
            if (pair.Value == null)
                throw new InvalidOperationException("Тип не совпадает или данные не найдены");

            return (T)pair.Value;
        }

        /// <summary>
        /// Сохраняет все данные в файл (только сериализуемые объекты)
        /// </summary>
        /// <param name="path">Место сохранения и назвавние файла</param>
        public void SaveToFile(string path)
        {
            var data = new Dictionary<Guid, StoredItem>();

            foreach (var kv in _storage)
            {
                var type = kv.Value.Type;
                var json = JsonSerializer.Serialize(kv.Value.Value, type, options);

                data[kv.Key] = new StoredItem
                {
                    TypeName = type.AssemblyQualifiedName!,
                    Json = json
                };
            }

            File.WriteAllText(path, JsonSerializer.Serialize(data, options));
        }

        /// <summary>
        /// Загружает данные из файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void LoadFromFile(string path)
        {
            if (!File.Exists(path)) return;

            var content = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<Dictionary<Guid, StoredItem>>(content, options);

            if (data == null) return;

            _storage.Clear();

            foreach (var kv in data)
            {
                if (string.IsNullOrWhiteSpace(kv.Value.TypeName)) continue;

                var type = Type.GetType(kv.Value.TypeName);
                if (type == null) continue;

                var value = JsonSerializer.Deserialize(kv.Value.Json, type, options);
                if (value != null)
                    _storage[kv.Key] = (value, type);
            }
        }
    }
}
