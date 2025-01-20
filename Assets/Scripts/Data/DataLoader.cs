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
    private string settingsPath = "Assets/JSON/Settings.json";
    private string welcomeMessagePath = "Assets/JSON/WelcomeMessage.json";
    private string assetBundlePath = "Assets/AssetBundles/ui_background";
    private float artificialDelay = 1f;

    // Список имён ассетов, которые нужно загрузить
    [SerializeField]
    private List<string> assetsToLoad = new List<string> { "ui_background" }; // Добавьте нужные имена ассетов

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Объект не уничтожается при переходе между сценами
        }
        else
        {
            Destroy(gameObject); // Уничтожаем дублирующийся объект
        }
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
        string path = $"{Application.dataPath}/JSON/Settings.json";
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError($"Файл настроек не найден по пути: {path}");
            yield break;
        }

        string json = System.IO.File.ReadAllText(path);
        try
        {
            var settings = JsonUtility.FromJson<Settings>(json);
            StartingNumber = settings.startingNumber;
            onProgress(0.33f);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при разборе JSON файла настроек: {ex.Message}");
        }
    }

    private IEnumerator LoadMessage(Action<float> onProgress)
    {
        string path = $"{Application.dataPath}/JSON/WelcomeMessage.json";
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError($"Файл приветственного сообщения не найден по пути: {path}");
            yield break;
        }

        string json = System.IO.File.ReadAllText(path);
        try
        {
            var message = JsonUtility.FromJson<Message>(json);
            WelcomeMessage = message.message;
            onProgress(0.66f);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при разборе JSON файла приветственного сообщения: {ex.Message}");
        }
    }

    private IEnumerator LoadAssetBundle(Action<float> onProgress)
    {
        // Освобождаем старый AssetBundle (если есть)
        ButtonBackground = null;
        LoadedAssets.Clear();

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

                        // Устанавливаем ButtonBackground только для первого ассета
                        if (assetName == assetsToLoad[0])
                        {
                            ButtonBackground = sprite;
                        }

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
            bundle.Unload(false); // Уничтожаем AssetBundle, оставляя только загруженные ассеты
        }
        else
        {
            Debug.LogError("Failed to load AssetBundle.");
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