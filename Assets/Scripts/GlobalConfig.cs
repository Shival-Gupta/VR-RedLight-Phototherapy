using UnityEngine;

/// <summary>
/// Global static configuration accessible across all scenes.
/// </summary>
public static class GlobalConfig
{
    // Global Session Data
    public static int SelectedPatternIndex = 0;
    public static string UserId = "guest";
    public static int SessionDurationSeconds = 180;

    // Scene Indexes & Names
    public const int SceneIndexSplash = 0;
    public const int SceneIndexMainMenu = 1;
    public const int SceneIndexHistory = 2;
    public const string SceneSplash = "0 Splash";
    public const string SceneMainMenu = "1 MainMenu";
    public const string SceneHistory = "2 History";

    // VR Therapy Scene Indexes & Names
    public static readonly int[] VRSceneIndexes = { 3, 4, 5 };
    public static readonly string[] VRSceneNames = {
        "3 VR Glow",
        "4 VR GridWave",
        "5 VR SplineTrack"
    };

    // Helper methods
    public static bool IsVRScene(string sceneName)
    {
        foreach (string vrScene in VRSceneNames)
        {
            if (sceneName == vrScene)
                return true;
        }
        return false;
    }

    public static bool IsVRSceneIndex(int index)
    {
        foreach (int vrIndex in VRSceneIndexes)
        {
            if (index == vrIndex)
                return true;
        }
        return false;
    }
}
