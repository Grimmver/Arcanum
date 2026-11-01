using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenHandler : MonoBehaviour
{
    private const int DefaultPhoneWidth = 540;
    private const int DefaultPhoneHeight = 960;

    private static bool hasInitializedWindow = false;

    private bool lastFullscreen;
    private int lastResolutionWidth = 0;
    private int lastResolutionHeight = 0;
    private int windowedWidth = DefaultPhoneWidth;
    private int windowedHeight = DefaultPhoneHeight;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializePhoneWindowOnPC()
    {
        if (hasInitializedWindow)
            return;

        hasInitializedWindow = true;

#if UNITY_STANDALONE
        int displayHeight = Display.main != null ? Display.main.systemHeight : Screen.currentResolution.height;
        int targetHeight = DefaultPhoneHeight;

        if (displayHeight > 0 && displayHeight < 1050)
        {
            targetHeight = Mathf.RoundToInt(displayHeight * 0.85f);
        }

        int targetWidth = Mathf.RoundToInt(targetHeight * (9f / 16f));
        Screen.SetResolution(targetWidth, targetHeight, FullScreenMode.Windowed);
#endif
    }

    private bool IsSupportedPlatform =>
        Application.platform == RuntimePlatform.WebGLPlayer ||
        Application.platform == RuntimePlatform.WindowsPlayer ||
        Application.platform == RuntimePlatform.OSXPlayer ||
        Application.platform == RuntimePlatform.LinuxPlayer;

    protected void Awake()
    {
        if (IsSupportedPlatform)
        {
            lastResolutionWidth = Screen.currentResolution.width;
            lastResolutionHeight = Screen.currentResolution.height;
            lastFullscreen = Screen.fullScreen;

            if (!Screen.fullScreen && Screen.width > 0 && Screen.height > 0)
            {
                windowedWidth = Screen.width;
                windowedHeight = Screen.height;
            }
        }
    }

    void Update()
    {
        if (!IsSupportedPlatform)
            return;

        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (Screen.fullScreen)
            {
                Screen.SetResolution(windowedWidth, windowedHeight, FullScreenMode.Windowed);
            }
            else
            {
                windowedWidth = Screen.width;
                windowedHeight = Screen.height;
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
            }
        }

        if (Screen.fullScreen != lastFullscreen)
        {
            if (Screen.fullScreen)
            {
                Screen.SetResolution(lastResolutionWidth, lastResolutionHeight, true);
            }
            lastFullscreen = Screen.fullScreen;
        }

        if (!Screen.fullScreen && (Screen.currentResolution.width != lastResolutionWidth || Screen.currentResolution.height != lastResolutionHeight))
        {
            lastResolutionWidth = Screen.currentResolution.width;
            lastResolutionHeight = Screen.currentResolution.height;
        }
    }
}
