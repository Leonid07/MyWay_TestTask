//#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundle
{
    [MenuItem("Assets/Assign and Build AssetBundles")]
    public static void AssignAndBuildAllAssetBundles()
    {
        // Папка, где хранятся ассеты для назначения
        string assetFolderPath = "Assets/Resurses"; // Укажите путь к папке с ассетами
        string assetBundleDirectory = "Assets/AssetBundles"; // Папка для сохранения AssetBundles

        // Назначение AssetBundle для всех объектов в папке
        AssignAssetBundles(assetFolderPath);

        // Создание папки для AssetBundles, если её нет
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        // Построение всех AssetBundles
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        Debug.Log("AssetBundles успешно созданы!");
    }

    private static void AssignAssetBundles(string folderPath)
    {
        // Получение всех файлов в папке
        string[] assetPaths = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

        foreach (string assetPath in assetPaths)
        {
            // Пропуск метафайлов
            if (assetPath.EndsWith(".meta")) continue;

            // Преобразование абсолютного пути в относительный
            string normalizedPath = Path.GetFullPath(assetPath).Replace('\\', '/');
            string projectPath = Path.GetFullPath(Application.dataPath).Replace('\\', '/');

            if (normalizedPath.StartsWith(projectPath))
            {
                string relativePath = "Assets" + normalizedPath.Substring(projectPath.Length);
                AssetImporter assetImporter = AssetImporter.GetAtPath(relativePath);

                if (assetImporter != null)
                {
                    // Назначаем имя AssetBundle как имя файла без расширения
                    string bundleName = Path.GetFileNameWithoutExtension(assetPath);
                    assetImporter.assetBundleName = bundleName;
                    Debug.Log($"Назначен AssetBundle: {bundleName} для {relativePath}");
                }
                else
                {
                    Debug.LogWarning($"Не удалось получить AssetImporter для {relativePath}");
                }
            }
            else
            {
                Debug.LogError($"Путь ассета {assetPath} не принадлежит папке проекта {Application.dataPath}");
            }
        }
    }
}
//#endif