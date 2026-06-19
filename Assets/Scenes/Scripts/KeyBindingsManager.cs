using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class KeyBindingsManager : MonoBehaviour
{
    public TMP_Text gasText, brakeText, leftText, rightText;

    private int waitingForInput = -1;
    private string[] keys = { "Gas", "Brake", "Left", "Right" };

    void Start()
    {
        LoadKeys();
    }

    void Update()
    {
        if (waitingForInput != -1)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    PlayerPrefs.SetString(keys[waitingForInput], key.ToString());
                    PlayerPrefs.Save();
                    waitingForInput = -1;
                    LoadKeys();
                    break;
                }
            }
        }
    }

    void LoadKeys()
    {
        gasText.text = PlayerPrefs.GetString("Gas", "W");
        brakeText.text = PlayerPrefs.GetString("Brake", "S");
        leftText.text = PlayerPrefs.GetString("Left", "A");
        rightText.text = PlayerPrefs.GetString("Right", "D");
    }

    public void SetGas() { waitingForInput = 0; }
    public void SetBrake() { waitingForInput = 1; }
    public void SetLeft() { waitingForInput = 2; }
    public void SetRight() { waitingForInput = 3; }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}