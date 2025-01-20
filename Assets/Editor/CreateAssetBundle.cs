//#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundle
{
    [MenuItem("Assets/Assign and Build AssetBundles")]
    public static void AssignAndBuildAllAssetBundles()
    {
        // �����, ��� �������� ������ ��� ����������
        string assetFolderPath = "Assets/Resurses"; // ������� ���� � ����� � ��������
        string assetBundleDirectory = "Assets/AssetBundles"; // ����� ��� ���������� AssetBundles

        // ���������� AssetBundle ��� ���� �������� � �����
        AssignAssetBundles(assetFolderPath);

        // �������� ����� ��� AssetBundles, ���� � ���
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        // ���������� ���� AssetBundles
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        Debug.Log("AssetBundles ������� �������!");
    }

    private static void AssignAssetBundles(string folderPath)
    {
        // ��������� ���� ������ � �����
        string[] assetPaths = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

        foreach (string assetPath in assetPaths)
        {
            // ������� ����������
            if (assetPath.EndsWith(".meta")) continue;

            // �������������� ����������� ���� � �������������
            string normalizedPath = Path.GetFullPath(assetPath).Replace('\\', '/');
            string projectPath = Path.GetFullPath(Application.dataPath).Replace('\\', '/');

            if (normalizedPath.StartsWith(projectPath))
            {
                string relativePath = "Assets" + normalizedPath.Substring(projectPath.Length);
                AssetImporter assetImporter = AssetImporter.GetAtPath(relativePath);

                if (assetImporter != null)
                {
                    // ��������� ��� AssetBundle ��� ��� ����� ��� ����������
                    string bundleName = Path.GetFileNameWithoutExtension(assetPath);
                    assetImporter.assetBundleName = bundleName;
                    Debug.Log($"�������� AssetBundle: {bundleName} ��� {relativePath}");
                }
                else
                {
                    Debug.LogWarning($"�� ������� �������� AssetImporter ��� {relativePath}");
                }
            }
            else
            {
                Debug.LogError($"���� ������ {assetPath} �� ����������� ����� ������� {Application.dataPath}");
            }
        }
    }
}
//#endif