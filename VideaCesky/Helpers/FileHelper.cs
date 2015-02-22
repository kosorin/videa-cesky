using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace VideaCesky.Helpers
{
    public static class FileHelper
    {
        public static async Task<T> ReadFromJson<T>(string fileName) where T : class, new()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            T collection = new T();
            try
            {
                StorageFile sampleFile = await localFolder.GetFileAsync(fileName);
                string data = await FileIO.ReadTextAsync(sampleFile);
                collection = JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception e)
            {
                Debug.WriteLine("ReadFromJson: {0}", e.Message);
            }
            return collection;
        }

        public static async Task WriteToJson<T>(string fileName, T collection) where T : class
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            try
            {
                if (file != null)
                {
                    string data = JsonConvert.SerializeObject(collection, Formatting.Indented);
                    await FileIO.WriteTextAsync(file, data);
                }
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine("WriteToJson: {0}", e.Message);
            }
        }
    }
}
