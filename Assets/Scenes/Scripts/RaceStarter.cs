using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RaceStarter : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text countdownText;
    public GameObject countdownPanel;

    [Header("Sounds")]
    public AudioClip countdownSound;   // Œƒ»Õ Á‚ÛÍ ‰Îˇ ‚Ò¸Ó„Ó ‚≥‰Î≥ÍÛ

    private bool raceStarted = false;
    private CarController carController;
    private ProgressTracker progressTracker;

    void Start()
    {
        StartCoroutine(FindCarAndStart());
    }

    IEnumerator FindCarAndStart()
    {
        yield return new WaitForSeconds(0.5f);

        carController = FindAnyObjectByType<CarController>();
        progressTracker = FindAnyObjectByType<ProgressTracker>();

        if (carController != null)
            carController.SetControlEnabled(false);

        if (progressTracker != null)
            progressTracker.SetRacing(false);

        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        if (countdownPanel != null)
            countdownPanel.SetActive(true);

        // ========== √–¿™ÃŒ ¬≈—‹ «¬”  ¬≤ƒÀ≤ ” ==========
        if (countdownSound != null)
            AudioSource.PlayClipAtPoint(countdownSound, Camera.main.transform.position);

        int count = 3;

        while (count > 0)
        {
            if (countdownText != null)
                countdownText.text = count.ToString();

            Debug.Log(count);
            count--;
            yield return new WaitForSeconds(1f);
        }

        if (countdownText != null)
            countdownText.text = "—“¿–“!";

        Debug.Log("—“¿–“!");

        yield return new WaitForSeconds(0.5f);

        if (countdownPanel != null)
            countdownPanel.SetActive(false);

        if (carController != null)
            carController.SetControlEnabled(true);

        if (progressTracker != null)
            progressTracker.SetRacing(true);

        raceStarted = true;
    }

    public bool IsRaceStarted()
    {
        return raceStarted;
    }
}