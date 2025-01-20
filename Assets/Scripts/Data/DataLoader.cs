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

    public Dictionary<string, Sprite> LoadedAssets { get; private set; } = new Dictionary<string, Sprite>();
    private string settingsPath = "JSON/Settings.json";
    private string welcomeMessagePath = "JSON/WelcomeMessage.json";
    private string assetBundlePath = "Assets/AssetBundles/ui_background";
    private float artificialDelay = 1f;

    // Список имён ассетов, которые нужно загрузить
    [SerializeField]
    private List<string> assetsToLoad = new List<string> { "background" }; // Добавьте нужные имена ассетов

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public IEnumerator LoadData(Action<float> onProgress)
    {
        float progress = 0f;

        // Load JSON files
        yield return LoadSettings(value =>
        {
            progress = value;
            onProgress(progress);
        });

        yield return LoadMessage(value =>
        {
            progress = value;
            onProgress(progress);
        });

        // Load Asset Bundle
        yield return LoadAssetBundle(value =>
        {
            progress = value;
            onProgress(progress);
        });

        // Artificial delay
        yield return new WaitForSeconds(artificialDelay);
    }

    private IEnumerator LoadSettings(Action<float> onProgress)
    {
        string path = $"{Application.streamingAssetsPath}/{settingsPath}";
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var settings = JsonUtility.FromJson<Settings>(request.downloadHandler.text);
            StartingNumber = settings.startingNumber;
            onProgress(0.33f);
        }
        else
        {
            Debug.LogError("Failed to load settings.");
        }
    }

    private IEnumerator LoadMessage(Action<float> onProgress)
    {
        string path = $"{Application.streamingAssetsPath}/{welcomeMessagePath}";
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var message = JsonUtility.FromJson<Message>(request.downloadHandler.text);
            WelcomeMessage = message.message;
            onProgress(0.66f);
        }
        else
        {
            Debug.LogError("Failed to load welcome message.");
        }
    }

    private IEnumerator LoadAssetBundle(Action<float> onProgress)
    {
        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
        yield return bundleRequest;

        AssetBundle bundle = bundleRequest.assetBundle;
        if (bundle != null)
        {
            foreach (string assetName in assetsToLoad)
            {
                if (bundle.Contains(assetName))
                {
                    Texture2D texture = bundle.LoadAsset<Texture2D>(assetName);

                    if (texture != null)
                    {
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        LoadedAssets[assetName] = sprite;
                        Debug.Log($"Успешно загружен ассет: {assetName}");
                    }
                    else
                    {
                        Debug.LogError($"Не удалось загрузить текстуру из ассета: {assetName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"AssetBundle не содержит ассета с именем: {assetName}");
                }
            }

            onProgress(1.0f);
            bundle.Unload(false);
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