using UnityEngine;

public class CarAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource engineSource;
    public AudioSource brakeSource;

    [Header("Sounds")]
    public AudioClip engineSound;   // Звук двигуна (loop)
    public AudioClip brakeSound;    // Звук гальм (one-shot)

    [Header("Car Controller")]
    public CarController car;

    void Start()
    {
        if (car == null) car = GetComponent<CarController>();

        if (engineSource != null && engineSound != null)
        {
            engineSource.loop = true;
            engineSource.clip = engineSound;
            engineSource.pitch = 1f;
            engineSource.volume = 0.3f;
            engineSource.Play();
        }

        if (brakeSource != null)
        {
            brakeSource.loop = false;
            brakeSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (car == null) return;

        float speed = car.GetCurrentSpeed();

        // ========== ГУЧНІСТЬ ДВИГУНА ЗАЛЕЖИТЬ ВІД ШВИДКОСТІ ==========
        float targetVolume = Mathf.Lerp(0.2f, 0.8f, speed / car.maxSpeed);
        if (engineSource != null)
            engineSource.volume = targetVolume;

        // ========== ЗВУК ГАЛЬМ ==========
        if (Input.GetKeyDown(KeyCode.S) && speed > 1f)
        {
            if (brakeSource != null && brakeSound != null)
                brakeSource.PlayOneShot(brakeSound);
        }
    }
}