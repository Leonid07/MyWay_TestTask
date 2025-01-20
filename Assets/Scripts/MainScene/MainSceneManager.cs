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

    private void Start()
    {
        if (DataLoader.Instance != null)
        {
            counter = DataHandler.LoadCounterState(DataLoader.Instance.StartingNumber);
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

        yield return DataLoader.Instance.LoadData(progress =>
        {
            Debug.Log($"Прогресс: {progress * 100}%");
        });

        counter = DataHandler.LoadCounterState(DataLoader.Instance.StartingNumber);
        SetData();

        Debug.Log("Контент успешно обновлен.");
    }

    private void OnApplicationQuit()
    {
        DataHandler.SaveCounterState(counter);
    }
}