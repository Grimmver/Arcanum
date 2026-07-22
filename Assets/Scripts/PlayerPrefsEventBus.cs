using System;
using UnityEngine;

public static class PlayerPrefsEventBus
{
    public static event Action<string, string> OnStringChanged;
    public static event Action<string, int> OnIntChanged;
    public static event Action<string, float> OnFloatChanged;

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();

        OnStringChanged?.Invoke(key, value);
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();

        OnIntChanged?.Invoke(key, value);
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();

        OnFloatChanged?.Invoke(key, value);
    }

    public static string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);

    public static int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);

    public static float GetFloat(string key, float defaultValue = 0f) => PlayerPrefs.GetFloat(key, defaultValue);
}
