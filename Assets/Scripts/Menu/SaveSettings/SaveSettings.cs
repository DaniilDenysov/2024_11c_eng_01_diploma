using System;
using System.IO;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// Static class for saving and loading player settings.
    /// </summary>
    public static class SaveSettings // TODO: add better comments
    {
        private static readonly string path = Path.Combine(Application.dataPath, "Resources", "PlayerSettings");

        public static void Save(string key, Tuple<string, object> data)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string json = JsonUtility.ToJson(new SerializableTuple(data.Item1, data.Item2));
            File.WriteAllText(Path.Combine(path, $"{key}.json"), json);
        }

        public static Tuple<string, object> Load(string key)
        {
            string filePath = Path.Combine(path, $"{key}.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                SerializableTuple data = JsonUtility.FromJson<SerializableTuple>(json);
                return new Tuple<string, object>(data.Item1, data.Item2);
            }
            return null;
        }

        [Serializable]
        private struct SerializableTuple
        {
            public string Item1;
            public object Item2;

            public SerializableTuple(string item1, object item2)
            {
                Item1 = item1;
                Item2 = item2;
            }
        }
    }
}