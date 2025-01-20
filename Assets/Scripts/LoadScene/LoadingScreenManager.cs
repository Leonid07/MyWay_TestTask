using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    void Start()
    {
        StartCoroutine(LoadGameData());
    }

    private IEnumerator LoadGameData()
    {
        yield return DataLoader.Instance.LoadData((progress) =>
        {
            progressBar.value = progress; // Обновление прогресс-бара
        });

        SceneManager.LoadScene("MainScene"); // Переход на основной экран
    }
}
