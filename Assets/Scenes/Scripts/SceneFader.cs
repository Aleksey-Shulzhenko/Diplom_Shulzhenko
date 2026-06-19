using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    public Image fadeImage;
    public float fadeDuration = 1f;

    private bool isFading = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Робимо кореневий об'єкт, щоб DontDestroyOnLoad працював
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (fadeImage == null)
        {
            GameObject panel = GameObject.Find("FadePanel");
            if (panel != null)
                fadeImage = panel.GetComponent<Image>();
        }

        if (fadeImage != null)
        {
            fadeImage.raycastTarget = false;
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void FadeToScene(string sceneName)
    {
        if (!isFading && fadeImage != null)
            StartCoroutine(FadeOutAndLoad(sceneName));
        else if (fadeImage == null)
            SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;

        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 1f);

        SceneManager.LoadScene(sceneName);

        yield return null;

        GameObject panel = GameObject.Find("FadePanel");
        if (panel != null)
            fadeImage = panel.GetComponent<Image>();

        if (fadeImage != null)
        {
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                fadeImage.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }

            fadeImage.color = new Color(color.r, color.g, color.b, 0f);
        }

        isFading = false;
    }
}