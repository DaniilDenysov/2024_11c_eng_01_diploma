/* using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    // Словарь для хранения всех сохраняемых компонентов по id:
    private Dictionary<string, ISavable> savableComponents = new Dictionary<string, ISavable>();
    
    // Путь к файлу для сохранения:
    private string saveFilePath;

    // Метод вызывается при старте UI:
    public string saveFolderPath = @"C:\Users\sofii\AppData\LocalLow\DefaultCompany\Salvager\SavedSettings\";

    private void Start()
    {
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
            Debug.Log("Создана папка для сохранения данных: " + saveFolderPath);
        }
        saveFilePath = Path.Combine(saveFolderPath, "saveData.json");

        FindSavableComponents();
        LoadSettings();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Метод вызывается при загрузке сцены:
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadSettings();
    }

    // Поиск всех компонентов, которые можно сохранить на сццене:
    private void FindSavableComponents()
    {
        ISavable[] components = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToArray();
        foreach (var component in components)
        {
            // Если компонент уже имеет идентификатор, добавляем его в словарь:
            if (!string.IsNullOrEmpty(component.GetID()))
            {
                if (!savableComponents.ContainsKey(component.GetID()))
                {
                    savableComponents.Add(component.GetID(), component);
                }
            }
            // Если компонент не имеет идентификатора, генерируем новый и добавляем его:
            else
            {
                string newID = Guid.NewGuid().ToString();
                component.SetID(newID);
                savableComponents.Add(newID, component);
            }

            Debug.Log("Найден компонент для сохранения с ID: " + component.GetID());
        }
    }

    // Сохранение всех данных в файл джсон:
    public void SaveSettings()
    {
        List<Tuple<string, object>> dataToSave = new List<Tuple<string, object>>();

        foreach (var component in savableComponents.Values)
        {
            dataToSave.Add(component.Save());
        }

        string json = JsonUtility.ToJson(new Serialization<List<Tuple<string, object>>>(dataToSave));
        try
        {
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Данные успешно сохранены в " + saveFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Ошибка при сохранении данных: " + e.Message);
        }
    }

    // Загрузка всех данных из файла джсон:
    public void LoadSettings()
    {
        if (!File.Exists(saveFilePath)) return;

        string json = File.ReadAllText(saveFilePath);
        var dataToLoad = JsonUtility.FromJson<Serialization<List<Tuple<string, object>>>>(json).ToList();

        foreach (var data in dataToLoad)
        {
            if (savableComponents.TryGetValue(data.Item1, out ISavable component))
            {
                component.Load(data);
            }
        }
        Debug.Log("Данные успешно загружены из " + saveFilePath);
    }
}

// Класс для сериализации кортежей:
[Serializable]
public class Serialization<T>
{
    [SerializeField]
    private T target;
    public Serialization(T target)
    {
        this.target = target;
    }

    public T ToList()
    {
        return target;
    }
}
*/