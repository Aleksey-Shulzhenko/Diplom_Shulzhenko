using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    private int selectedTrack = 1;
    private int selectedCar = 1;

    public void SelectTrack1() { selectedTrack = 1; }
    public void SelectTrack2() { selectedTrack = 2; }
    public void SelectTrack3() { selectedTrack = 3; }

    public void SelectCar1() { selectedCar = 1; }
    public void SelectCar2() { selectedCar = 2; }
    public void SelectCar3() { selectedCar = 3; }

    public void StartRace()
    {
        PlayerPrefs.SetInt("SelectedTrack", selectedTrack);
        PlayerPrefs.SetInt("SelectedCar", selectedCar);
        PlayerPrefs.Save();

        string sceneName = selectedTrack switch
        {
            1 => "Track1",
            2 => "Track2",
            3 => "Track3",
            _ => "Track1"
        };
        SceneManager.LoadScene(sceneName);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}