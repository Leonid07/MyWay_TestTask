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
    public Sprite ButtonBackground { get; private set; }

    private string settingsPath = "JSON/Settings.json";
    private string welcomeMessagePath = "JSON/WelcomeMessage.json";
    private string assetBundlePath = "Assets/AssetBundles/ui_background";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public IEnumerator LoadData(Action<float> onProgress)
    {
        // Load JSON files
        yield return LoadSettings(onProgress, 0.33f);
        yield return LoadMessage(onProgress, 0.66f);

        // Load Asset Bundle
        yield return LoadAssetBundle(onProgress, 1.0f);

        // Artificial delay
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator LoadSettings(Action<float> onProgress, float progressValue)
    {
        string path = $"{Application.streamingAssetsPath}/{settingsPath}";
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var settings = JsonUtility.FromJson<Settings>(request.downloadHandler.text);
            StartingNumber = settings.startingNumber;
            onProgress(progressValue);
        }
        else
        {
            Debug.LogError("Failed to load settings.");
        }
    }

    private IEnumerator LoadMessage(Action<float> onProgress, float progressValue)
    {
        string path = $"{Application.streamingAssetsPath}/{welcomeMessagePath}";
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var message = JsonUtility.FromJson<Message>(request.downloadHandler.text);
            WelcomeMessage = message.message;
            onProgress(progressValue);
        }
        else
        {
            Debug.LogError("Failed to load welcome message.");
        }
    }

    private IEnumerator LoadAssetBundle(Action<float> onProgress, float progressValue)
    {
        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
        yield return bundleRequest;

        AssetBundle bundle = bundleRequest.assetBundle;
        if (bundle != null)
        {
            Texture2D texture = bundle.LoadAsset<Texture2D>("background");
            ButtonBackground = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            bundle.Unload(false);
            onProgress(progressValue);
        }
        else
        {
            Debug.LogError("Failed to load Asset Bundle.");
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