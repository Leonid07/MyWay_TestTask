using System.IO;
using UnityEngine;

public static class DataHandler
{
    // Определённый путь для сохранения файла
    private static readonly string saveFilePath = Path.Combine(Application.dataPath, "JSON", "counter_state.json");

    public static void SaveCounterState(int counter)
    {
        var state = new CounterState { CounterValue = counter };
        string json = JsonUtility.ToJson(state, true);

        try
        {
            // Убедимся, что папка существует
            string directoryPath = Path.GetDirectoryName(saveFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Debug.Log($"Директория создана: {directoryPath}");
            }

            // Записываем файл
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"Состояние счетчика успешно сохранено: {saveFilePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Не удалось сохранить состояние счетчика: {ex.Message}");
        }
    }

    public static int LoadCounterState(int defaultValue)
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                var state = JsonUtility.FromJson<CounterState>(json);
                Debug.Log($"Состояние счетчика успешно загружено из {saveFilePath}");
                return state.CounterValue;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Не удалось загрузить состояние счетчика: {ex.Message}");
            }
        }
        else
        {
            Debug.Log("Файл состояния счетчика не найден. Используется значение по умолчанию.");
        }

        return defaultValue;
    }
}