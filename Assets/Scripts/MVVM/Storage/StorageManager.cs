using System.IO;
using UnityEngine;

namespace MVVM.Storage
{
    public class StorageManager
    {
        public void CacheTexture(string url, byte[] data)
        {
            
            // fileName = разложить на путь и имя файла 
            var fileName = Path.GetFileName(url);
            var path = Path.GetDirectoryName(url) + "/";
            Debug.Log(path);
            Debug.Log(fileName);
            Directory.CreateDirectory(Application.persistentDataPath + path);
                
            var cacheFilePath = Path.Combine(Application.persistentDataPath + path, $"{fileName}.texture");
           

            File.WriteAllBytes(cacheFilePath, data);
        }
        
        public Texture2D GetTextureFromCache(string url)
        {
            var fileName = Path.GetFileName(url);
            var path = Path.GetDirectoryName(url) + "/";
            var cacheFilePath = Path.Combine(Application.persistentDataPath + path, $"{fileName}.texture");

            Texture2D texture = null;

            if (File.Exists(cacheFilePath))
            {
                var data = File.ReadAllBytes(cacheFilePath);

                texture = new Texture2D(1, 1);
                texture.LoadImage(data, true);
            }

            return texture;
        }
    }
}