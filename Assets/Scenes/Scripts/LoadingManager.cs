using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider progressBar;
    public TMP_Text progressText;
    public TMP_Text loadingText;

    [Header("Settings")]
    public float loadDuration = 2f;
    public string nextSceneName = "MainMenu";

    void Start()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        // Плавне заповнення прогресу
        float timer = 0f;

        while (timer < loadDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / loadDuration;

            if (progressBar != null)
                progressBar.value = progress;
            if (progressText != null)
                progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
            if (loadingText != null)
            {
                int dots = Mathf.FloorToInt(Time.time * 1.5f) % 4;
                loadingText.text = "ЗАВАНТАЖЕННЯ" + new string('.', dots);
            }

            yield return null;
        }

        if (progressBar != null)
            progressBar.value = 1f;
        if (progressText != null)
            progressText.text = "100%";

        yield return new WaitForSeconds(0.2f);

        // Використовуємо плавний перехід
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }
}