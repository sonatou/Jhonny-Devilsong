using System;
using FMODUnity;
using UnityEngine;

public enum BeatBarMode { AbovePlayer, FixedScreen }

public static class GameSettings
{
    private const string BeatBarKey = "BeatBarMode";
    private const string MasterVolumeKey = "MasterVolume";
    private const string AudioOffsetKey = "AudioOffsetMs";

    public static event Action Changed;

    public static BeatBarMode BeatBar
    {
        get => (BeatBarMode)PlayerPrefs.GetInt(BeatBarKey, (int)BeatBarMode.AbovePlayer);
        set
        {
            PlayerPrefs.SetInt(BeatBarKey, (int)value);
            Save();
        }
    }

    public static float MasterVolume
    {
        get => PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        set
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, Mathf.Clamp01(value));
            ApplyVolume();
            Save();
        }
    }

    public static int AudioOffsetMs
    {
        get => PlayerPrefs.GetInt(AudioOffsetKey, 0);
        set
        {
            PlayerPrefs.SetInt(AudioOffsetKey, value);
            Save();
        }
    }

    public static void ApplyVolume()
    {
        try
        {
            RuntimeManager.GetBus("bus:/").setVolume(MasterVolume);
        }
        catch (BusNotFoundException)
        {
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        ApplyVolume();
    }

    static void Save()
    {
        PlayerPrefs.Save();
        Changed?.Invoke();
    }
}
