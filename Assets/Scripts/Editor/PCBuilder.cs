#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class PCBuilder
{
    private const string BuildOutputPath = "Builds/PC/ARCANUM.exe";

    [MenuItem("Build/Build PC Version (Windows 64-bit)")]
    public static void BuildWindowsPCFromMenu()
    {
        BuildWindowsPC();
    }

    public static void BuildWindowsPC()
    {
        ConfigurePCPlayerSettings();

        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        string directory = Path.GetDirectoryName(BuildOutputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = BuildOutputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"PC Build succeeded: {Path.GetFullPath(BuildOutputPath)} ({summary.totalSize} bytes)");
        }
        else
        {
            Debug.LogError($"PC Build failed with result: {summary.result}");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }
        }
    }

    [MenuItem("Build/Configure PC Phone-Window Settings")]
    public static void ConfigurePCPlayerSettings()
    {
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 540;
        PlayerSettings.defaultScreenHeight = 960;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.allowFullscreenSwitch = true;
        PlayerSettings.runInBackground = true;

        AssetDatabase.SaveAssets();
        Debug.Log("PC PlayerSettings configured for 540x960 phone-sized window.");
    }
}
#endif
