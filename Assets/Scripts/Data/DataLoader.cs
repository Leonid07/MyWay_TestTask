using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataLoader : MonoBehaviour
{
    public static DataLoader Instance;
    public int StartingNumber { get; private set; }
    public string WelcomeMessage { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public IEnumerator LoadData(Action<float> onProgress)
    {
        // Загрузка JSON-файлов
        yield return LoadSettings(onProgress, 0.33f);
        yield return LoadMessage(onProgress, 0.66f);

        // Искусственная задержка
        yield return new WaitForSeconds(1f);
        onProgress(1f);
    }

    private IEnumerator LoadSettings(Action<float> onProgress, float progressValue)
    {
        string path = $"{Application.streamingAssetsPath}/Settings.json";
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var settings = JsonUtility.FromJson<Settings>(request.downloadHandler.text);
            StartingNumber = settings.startingNumber;
            onProgress(progressValue);
        }
    }

    private IEnumerator LoadMessage(Action<float> onProgress, float progressValue)
    {
        string path = $"{Application.streamingAssetsPath}/WelcomeMessage.json";
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var message = JsonUtility.FromJson<Message>(request.downloadHandler.text);
            WelcomeMessage = message.message;
            onProgress(progressValue);
        }
    }
}

[Serializable]
public class Settings
{
    public int startingNumber;
}

[Serializable]
public class Message
{
    public string message;
}
