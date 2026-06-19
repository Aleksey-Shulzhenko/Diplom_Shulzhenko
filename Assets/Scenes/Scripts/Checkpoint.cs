using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointNumber;
    public GameObject nextCheckpoint;  // Наступна контрольна точка

    private bool isPassed = false;
    private MeshRenderer meshRenderer;
    private Collider checkpointCollider;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        checkpointCollider = GetComponent<Collider>();

        // Якщо це не перша точка (номер > 1) — ховаємо її спочатку
        if (checkpointNumber > 1)
        {
            Hide();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPassed)
        {
            isPassed = true;

            if (ProgressTracker.Instance != null)
            {
                ProgressTracker.Instance.PassCheckpoint(checkpointNumber, transform.position);
            }

            // Ховаємо поточну точку
            Hide();

            // Показуємо наступну точку (якщо вона існує)
            if (nextCheckpoint != null)
            {
                Checkpoint next = nextCheckpoint.GetComponent<Checkpoint>();
                if (next != null)
                {
                    next.Show();
                }
            }
        }
    }

    void Hide()
    {
        if (meshRenderer != null) meshRenderer.enabled = false;
        if (checkpointCollider != null) checkpointCollider.enabled = false;
    }

    void Show()
    {
        if (meshRenderer != null) meshRenderer.enabled = true;
        if (checkpointCollider != null) checkpointCollider.enabled = true;
    }
}