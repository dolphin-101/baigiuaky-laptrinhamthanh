using UnityEngine;
using UnityEngine.SceneManagement; 
using FMODUnity;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private FMOD.Studio.EventInstance pauseSnapshot;

    [Header("Khai báo các Panel")]
    public GameObject settingsPanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (settingsPanel) settingsPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (victoryPanel) victoryPanel.SetActive(false);

        pauseSnapshot = RuntimeManager.CreateInstance("snapshot:/PauseSnapshot");
    }

    public void OpenSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(true);

        pauseSnapshot.start();
    }

    public void CloseSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(false);

        pauseSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void LoadHomeScene()
    {
        pauseSnapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        pauseSnapshot.release();

        SceneManager.LoadScene("home");
    }

    public void ShowGameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    public void ShowVictory()
    {
        if (victoryPanel) victoryPanel.SetActive(true);
    }

    void OnDestroy()
    {
        pauseSnapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        pauseSnapshot.release();
    }
}