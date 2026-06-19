using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Finish()
    {
        Debug.Log("Гра завершена! Час: " + Time.time);
        // Тут можна завантажити сцену результатів або показати панель
    }
}