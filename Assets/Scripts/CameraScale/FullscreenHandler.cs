using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenHandler : MonoBehaviour
{
    private bool lastFullscreen;
    private int lastResolutionWidth = 0;
    private int lastResolutionHeight = 0;

    protected void Awake()
    {
        // Only initialize on WebGL platform
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            lastResolutionWidth = Screen.currentResolution.width;
            lastResolutionHeight = Screen.currentResolution.height;
            lastFullscreen = Screen.fullScreen;
        }
    }

    void Update()
    {
        // Only execute on WebGL platform
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            return;

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
