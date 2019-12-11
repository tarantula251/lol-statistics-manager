using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace LOLStatisticsManager.Model
{
    class Cache
    {
        private string cachePath;

        public Cache()
        {
            cachePath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            cachePath += @"\cache";
            Directory.CreateDirectory(cachePath);
            cachePath += @"\";
        }

        /// <summary>
        /// Stores object with given name in cache. Note: object must be Serializable.
        /// </summary>
        /// <param name="obj">Object to be stored in cache.</param>
        /// <param name="name">Name of object in cache</param>
        /// <param name="subdirectory">Name of cache subdirectory. Note: separate directories in path using '\'.</param>
        public void Store(object obj, string name, string subdirectory = "")
        {
            Directory.CreateDirectory(cachePath + subdirectory);
            if (subdirectory.Length > 0 && subdirectory[subdirectory.Length - 1] != '\\') subdirectory += '\\';
            FileStream fileStream = new FileStream(cachePath + subdirectory + name, FileMode.CreateNew);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, obj);
        }

        /// <summary>
        /// Loads object of type T with given name from cache.
        /// </summary>
        /// <typeparam name="T">Type of object to be loaded from cache. Note: T must be Serializable.</typeparam>
        /// <param name="name">Name of object in cache</param>
        /// <param name="subdirectory">Name of cache subdirectory. Note: separate directories in path using '\'.</param>
        /// <returns>Object of type T from cache.</returns>
        public T Load<T>(string name, string subdirectory = "") where T: class
        {
            if (subdirectory.Length > 0 && subdirectory[subdirectory.Length - 1] != '\\') subdirectory += '\\';
            if(!File.Exists(cachePath + subdirectory + name)) return null;
            FileStream fileStream = new FileStream(cachePath + subdirectory + name, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            T obj = (T)binaryFormatter.Deserialize(fileStream);
            return obj;
        }
    }
}
