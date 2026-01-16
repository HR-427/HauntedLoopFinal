using UnityEngine;

public class StartGamePopup : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject howToPlayPanel;

    void Start()
    {
        Time.timeScale = 0f;      // Pause game
        ShowStartMenu();
    }

    // ---------- PANEL CONTROL ----------

    void ShowStartMenu()
    {
        startPanel.SetActive(true);
        howToPlayPanel.SetActive(false);
    }

    void ShowHowToPlay()
    {
        startPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    // ---------- BUTTON EVENTS ----------

    public void StartGame()
    {
        startPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }

    public void OpenHowToPlay()
    {
        ShowHowToPlay();
    }

    public void BackToStartMenu()
    {
        ShowStartMenu();
    }
}
