using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ThousandSchnapsen.CRM.Utils
{
    public class StrategyDatabase<T, TK>
    {
        private readonly Dictionary<T, Dictionary<TK, StrategyData>> _data =
            new Dictionary<T, Dictionary<TK, StrategyData>>();

        public bool TryGetValue((T, TK) key, out StrategyData value)
        {
            var (primaryKey, secondaryKey) = key;
            if (_data.TryGetValue(primaryKey, out var innerData))
            {
                return innerData.TryGetValue(secondaryKey, out value);
            }

            value = null;
            return false;
        }

        public void AddValue((T, TK) key, StrategyData value)
        {
            var (primaryKey, secondaryKey) = key;
            if (!_data.ContainsKey(primaryKey))
                _data.Add(primaryKey, new Dictionary<TK, StrategyData>());
            _data[primaryKey].Add(secondaryKey, value);
        }

        public void Save(string dataDirectory)
        {
            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);

            foreach (var (key, value) in _data)
            {
                var path = $"{dataDirectory}/{key}.dat";
                var fs = new FileStream(path, FileMode.Create);

                var formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(fs, value);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }

            ;
        }
    }
}