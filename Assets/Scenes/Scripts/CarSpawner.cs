using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs; // Масив з 3 префабів машин

    void Start()
    {
        // Отримуємо номер вибраної машини (1, 2 або 3)
        int selectedCar = PlayerPrefs.GetInt("SelectedCar", 2); // 2 = Balance за замовчуванням

        // Створюємо машину (номер -1 тому що масив починається з 0)
        Instantiate(carPrefabs[selectedCar - 1], transform.position, transform.rotation);
    }
}