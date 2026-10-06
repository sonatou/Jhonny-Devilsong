using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubMusic : MonoBehaviour
{
    [Header("Evento FMOD")]
    [SerializeField] private EventReference hubMusicEvent;

    private FMOD.Studio.EventInstance _instance;
    private bool _isPlaying;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        HandleScene(SceneManager.GetActiveScene().name);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnDestroy()
    {
        StopMusic(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleScene(scene.name);
    }

    void HandleScene(string sceneName)
    {
        if (sceneName == "Hub")
            PlayMusic();
        else
            StopMusic(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void PlayMusic()
    {
        if (_isPlaying) return;

        if (hubMusicEvent.IsNull)
        {
            Debug.LogWarning("[HubMusic] Evento FMOD nao atribuido.", this);
            return;
        }

        _instance = RuntimeManager.CreateInstance(hubMusicEvent);
        _instance.start();
        _isPlaying = true;
    }

    void StopMusic(FMOD.Studio.STOP_MODE mode)
    {
        if (!_isPlaying) return;

        _instance.stop(mode);
        _instance.release();
        _isPlaying = false;
    }
}
