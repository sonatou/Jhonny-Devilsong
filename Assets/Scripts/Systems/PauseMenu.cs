using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        IsPaused = false;
    }

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private OptionsMenu optionsMenu;
    [SerializeField] private string[] blockedScenes = { "MainMenu", "GameOver", "Win" };
    [SerializeField] private string mainMenuScene = "MainMenu";

    void Start()
    {
        pausePanel.SetActive(false);
        optionsMenu.Close();
    }

    void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;
        if (System.Array.IndexOf(blockedScenes, SceneManager.GetActiveScene().name) >= 0) return;

        if (optionsMenu.gameObject.activeSelf)
            optionsMenu.Close();
        else if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        SetAudioPaused(true);
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        SetAudioPaused(false);
        optionsMenu.Close();
        pausePanel.SetActive(false);
    }

    public void OpenOptions()
    {
        optionsMenu.Open();
    }

    public void GoToMainMenu()
    {
        Resume();
        ScoreManager.Instance?.ResetRun();
        GameManager.Instance?.ResetRooms();
        DestroyPersistentObjects();
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void SetAudioPaused(bool paused)
    {
        try
        {
            RuntimeManager.GetBus("bus:/").setPaused(paused);
        }
        catch (BusNotFoundException)
        {
        }
    }

    void DestroyPersistentObjects()
    {
        var player = FindAnyObjectByType<PlayerHealth>();
        if (player != null) Destroy(player.transform.root.gameObject);

        if (Camera.main != null && Camera.main.gameObject.scene.name == "DontDestroyOnLoad")
            Destroy(Camera.main.gameObject);

        if (BeatManager.Instance != null) Destroy(BeatManager.Instance.transform.root.gameObject);
    }
}
