using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;

namespace SaveSystem
{
    /// <summary>
    /// Manages saving and loading of game settings.
    /// </summary>
    public class SaveManager : MonoBehaviour // TODO: add better comments
    {
        private Dictionary<string, Saver> _savableComponents = new Dictionary<string, Saver>();
        [SerializeField] private string _fileName = "SaveSettings.json";
        [SerializeField] private string _filePath = "Saves";
        [SerializeField] private string _fullPath;

        private void Start()
        {
            _filePath = Path.Combine(Application.dataPath, _filePath);
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }
            _fullPath = Path.Combine(_filePath, _fileName);
            FindAndRegisterSavableComponents();
            LoadSettings();
           // SceneManager.sceneLoaded += SaveSettings;
        }

        private void FindAndRegisterSavableComponents()
        {
            foreach (var saver in FindObjectsOfType<Saver>(true)) // TODO: would be better to use FindObjectsByType
            {
                _savableComponents.Add(saver.GetID(), saver);
            }
        }

        private void SaveSettings(Scene scene, LoadSceneMode mode)
        {
            SaveSettings();
        }

        public void SaveSettings()
        {
            var dataList = new SavableData
            {
                data = new List<Tuple<string, object>>()
            };

            foreach (var savableComponent in _savableComponents.Values)
            {
                dataList.data.Add(savableComponent.Save());
            }

            string jsonData = JsonConvert.SerializeObject(dataList, Formatting.Indented);
            File.WriteAllText(_fullPath, jsonData);
        }

        public void LoadSettings()
        {
            if (!File.Exists(_fullPath)) return;

            string jsonData = File.ReadAllText(_fullPath);
            SavableData save = JsonConvert.DeserializeObject<SavableData>(jsonData);

            if (save?.data == null) return;

            foreach (var item in save.data)
            {
                if (_savableComponents.TryGetValue(item.Item1, out Saver savableComponent))
                {
                    savableComponent.Load(item);
                }
            }
        }
    }

    [Serializable]
    public class SavableData
    {
        public List<Tuple<string, object>> data;
    }
}