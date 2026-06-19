using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("FINISH TRIGGER: OnTriggerEnter спрацював! Об'єкт: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("FINISH TRIGGER: Це гравець!");

            if (ProgressTracker.Instance != null && ProgressTracker.Instance.IsFinished())
            {
                Debug.Log("FINISH TRIGGER: Викликаємо ShowResult()");
                ProgressTracker.Instance.ShowResult();
            }
            else
            {
                Debug.Log("FINISH TRIGGER: ProgressTracker null або не всі чекпоінти");
            }
        }
        else
        {
            Debug.Log("FINISH TRIGGER: Об'єкт не має тегу Player. Тег: " + other.tag);
        }
    }
}