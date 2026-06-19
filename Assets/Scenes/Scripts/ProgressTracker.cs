using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ProgressTracker : MonoBehaviour
{
    public static ProgressTracker Instance;

    [Header("UI")]
    public Slider progressBar;
    public TMP_Text progressText;
    public TMP_Text checkpointTimeText;
    public TMP_Text raceTimerText;
    public TMP_Text speedText;
    public GameObject resultPanel;
    public TMP_Text finalTimeText;
    public TMP_Text statsText;

    [Header("Checkpoints")]
    private int totalCheckpoints;
    private int passedCheckpoints = 0;
    private Vector3 lastCheckpointPosition;

    [Header("Timer")]
    private float raceTime = 0f;
    private bool isRacing = false;
    private float lastCheckpointTime;
    private List<float> checkpointTimes = new List<float>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Checkpoint[] checkpoints = FindObjectsByType<Checkpoint>();
        totalCheckpoints = checkpoints.Length;

        lastCheckpointTime = 0f;
        raceTime = 0f;
        isRacing = false;
        checkpointTimes.Clear();

        UpdateUI();

        if (resultPanel != null)
            resultPanel.SetActive(false);

        Debug.Log("Знайдено контрольних точок: " + totalCheckpoints);
    }

    void Update()
    {
        if (isRacing)
        {
            raceTime += Time.deltaTime;
            UpdateTimerUI();
        }

        UpdateSpeedometer();
    }

    void UpdateTimerUI()
    {
        if (raceTimerText != null)
        {
            System.TimeSpan t = System.TimeSpan.FromSeconds(raceTime);
            raceTimerText.text = string.Format("{0:D2}:{1:D2}:{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
        }
    }

    void UpdateSpeedometer()
    {
        if (speedText == null) return;

        CarController car = FindAnyObjectByType<CarController>();
        if (car != null)
        {
            float speedKMH = car.GetCurrentSpeed() * 3.6f;
            speedText.text = Mathf.RoundToInt(speedKMH) + " km/h";
        }
    }

    public void SetRacing(bool racing)
    {
        isRacing = racing;
        if (racing)
        {
            lastCheckpointTime = 0f;
            checkpointTimes.Clear();
        }
    }

    public void PassCheckpoint(int number, Vector3 position)
    {
        if (number == passedCheckpoints + 1)
        {
            passedCheckpoints = number;
            lastCheckpointPosition = position;

            float currentTime = raceTime;
            float segmentTime = currentTime - lastCheckpointTime;
            lastCheckpointTime = currentTime;
            checkpointTimes.Add(segmentTime);

            UpdateUI();

            if (checkpointTimeText != null)
            {
                checkpointTimeText.text = $"Чекпоінт {number}: {segmentTime:F2} сек";
                Invoke(nameof(ClearCheckpointMessage), 2f);
            }

            Debug.Log($"Чекпоінт {number} пройдено за {segmentTime:F2} сек");

            if (passedCheckpoints >= totalCheckpoints)
            {
                Debug.Log("ВСІ ЧЕКПОІНТИ ПРОЙДЕНО! ФІНІШ!");
                ShowResult();
            }
        }
    }

    void ClearCheckpointMessage()
    {
        if (checkpointTimeText != null)
            checkpointTimeText.text = "";
    }

    void UpdateUI()
    {
        if (totalCheckpoints == 0) return;

        float progress = (float)passedCheckpoints / totalCheckpoints;

        if (progressBar != null)
            progressBar.value = progress;

        if (progressText != null)
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
    }

    public Vector3 GetLastCheckpointPosition()
    {
        return lastCheckpointPosition;
    }

    public bool IsFinished()
    {
        return passedCheckpoints >= totalCheckpoints;
    }

    public int GetPassedCount()
    {
        return passedCheckpoints;
    }

    public int GetTotalCheckpoints()
    {
        return totalCheckpoints;
    }

    public void ShowResult()
    {
        isRacing = false;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CarController car = FindAnyObjectByType<CarController>();
        if (car != null)
        {
            car.SetControlEnabled(false);
        }

        System.TimeSpan t = System.TimeSpan.FromSeconds(raceTime);
        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);

        string stats = $"═══════════════════════════════\n";
        stats += $"        РЕЗУЛЬТАТИ ЗАЇЗДУ        \n";
        stats += $"═══════════════════════════════\n\n";
        stats += $"Загальний час: {formattedTime}\n\n";
        stats += $"Час проходження чекпоінтів:\n";

        float sum = 0f;
        for (int i = 0; i < checkpointTimes.Count; i++)
        {
            stats += $"  Чекпоінт {i + 1}: {checkpointTimes[i]:F2} сек\n";
            sum += checkpointTimes[i];
        }

        float average = sum / checkpointTimes.Count;
        stats += $"\nСередній час на чекпоінт: {average:F2} сек\n";

        float best = PlayerPrefs.GetFloat("BestTime_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 999f);
        if (raceTime < best)
        {
            PlayerPrefs.SetFloat("BestTime_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, raceTime);
            PlayerPrefs.Save();
            stats += $"\n⭐ НОВИЙ РЕКОРД! ⭐\n";
        }
        else if (best < 999f)
        {
            System.TimeSpan b = System.TimeSpan.FromSeconds(best);
            string bestFormatted = string.Format("{0:D2}:{1:D2}:{2:D3}", b.Minutes, b.Seconds, b.Milliseconds);
            stats += $"\nРекорд траси: {bestFormatted}\n";
        }

        if (finalTimeText != null)
            finalTimeText.text = $"Ваш час: {formattedTime}";

        if (statsText != null)
            statsText.text = stats;

        if (resultPanel != null)
            resultPanel.SetActive(true);

        Debug.Log($"Гру завершено! Загальний час: {formattedTime}");
    }

    public void RestartRace()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}