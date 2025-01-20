using System.Collections;
using System.Collections.Generic;
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
            // ������������� ������ �� DataLoader
            SetData();

            // ������������� �� ������� ������
            increaseCounterButton.onClick.AddListener(OnIncreaseCounter);
            refreshContentButton.onClick.AddListener(OnRefreshContent);
        }
        else
        {
            Debug.LogError("DataLoader.Instance is not initialized.");
        }
    }

    private void OnIncreaseCounter()
    {
        counter++;
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        counterText.text = $"Counter: {counter}";
    }

    private void SetData()
    {
        // ������������� �������������� ���������
        welcomeMessageText.text = DataLoader.Instance.WelcomeMessage;

        // ������������� ��� ������ �� ���������� ������
        if (DataLoader.Instance.ButtonBackground != null)
        {
            var buttonImage = increaseCounterButton.GetComponent<Image>();
            buttonImage.sprite = DataLoader.Instance.ButtonBackground;
            Debug.Log("��� ������ �������.");
        }
        else
        {
            Debug.LogWarning("ButtonBackground �� �������. ��������� �������� ������.");
        }

        // ������������� ��������� �������� ��������
        counter = DataLoader.Instance.StartingNumber;
        UpdateCounterText();
    }

    private void OnRefreshContent()
    {
        StartCoroutine(RefreshContent());
    }

    private IEnumerator RefreshContent()
    {
        Debug.Log("Refreshing content...");

        // ��������� ����� ������
        yield return DataLoader.Instance.LoadData(progress =>
        {
            Debug.Log($"Progress: {progress * 100}%");
        });

        // ��������� ������ �� ������
        SetData();

        Debug.Log("Content refreshed successfully.");
    }
}
