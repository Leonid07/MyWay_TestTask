using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private TMP_Text welcomeMessageText;
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private Button increaseCounterButton;
    [SerializeField] private Button refreshContentButton;

    private int counter;
    private string saveFilePath;

    private void Start()
    {
        saveFilePath = $"Assets/JSON/counter_state.json";

        if (DataLoader.Instance != null)
        {
            LoadState();
            SetData();

            increaseCounterButton.onClick.AddListener(OnIncreaseCounter);
            refreshContentButton.onClick.AddListener(OnRefreshContent);
        }
        else
        {
            Debug.LogError("DataLoader.Instance не инициализирован.");
        }
    }

    private void OnIncreaseCounter()
    {
        counter++;
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        counterText.text = $"Счёт: {counter}";
    }

    private void SetData()
    {
        if (DataLoader.Instance != null)
        {
            welcomeMessageText.text = DataLoader.Instance.WelcomeMessage;

            if (DataLoader.Instance.ButtonBackground != null)
            {
                var buttonImage = increaseCounterButton.GetComponent<Image>();
                buttonImage.sprite = DataLoader.Instance.ButtonBackground;
            }
        }
        else
        {
            Debug.LogError("DataLoader.Instance не инициализирован.");
        }

        UpdateCounterText();
    }

    private void OnRefreshContent()
    {
        StartCoroutine(RefreshContent());
    }

    private IEnumerator RefreshContent()
    {
        Debug.Log("Освежающий контент...");

        // Загружаем новые данные
        yield return DataLoader.Instance.LoadData(progress =>
        {
            Debug.Log($"Прогресс: {progress * 100}%");
        });

        // Если файла состояния нет, устанавливаем начальное значение из настроек
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("Файл состояния отсутствует. Повторная инициализация счетчика из настроек...");
            LoadDefaultCounter();
        }

        // Применяем обновленные данные на экран
        SetData();

        Debug.Log("Контент успешно обновлен.");
    }

    private void OnApplicationQuit()
    {
        SaveState();
    }

    private void SaveState()
    {
        var state = new CounterState { CounterValue = counter };
        string json = JsonUtility.ToJson(state, true);

        try
        {
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"Состояние счетчика успешно сохранено {saveFilePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Не удалось сохранить состояние счетчика.: {ex.Message}");
        }
    }

    private void LoadState()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                var state = JsonUtility.FromJson<CounterState>(json);
                counter = state.CounterValue;
                Debug.Log($"Состояние счетчика успешно загружено из {saveFilePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Не удалось загрузить состояние счетчика: {ex.Message}");
                LoadDefaultCounter(); // Используем начальное значение
            }
        }
        else
        {
            Debug.Log("Файл состояния счетчика не найден. Загрузка настроек по умолчанию...");
            LoadDefaultCounter(); // Используем начальное значение
        }
    }

    private void LoadDefaultCounter()
    {
        if (DataLoader.Instance != null)
        {
            counter = DataLoader.Instance.StartingNumber;
            Debug.Log($"Счетчик инициализирован начальным числом по умолчанию: {counter}");
        }
        else
        {
            Debug.LogError("DataLoader.Instance не инициализирован.");
        }
    }
}

[System.Serializable]
public class CounterState
{
    public int CounterValue;
}