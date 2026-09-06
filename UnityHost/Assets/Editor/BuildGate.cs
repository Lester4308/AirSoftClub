using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;

public static class BuildGate
{
    public static void Managed() { Build(ScriptingImplementation.Mono2x, "Mono"); }
    public static void Il2Cpp() { Build(ScriptingImplementation.IL2CPP, "IL2CPP"); }
    private static void Build(ScriptingImplementation backend, string folder)
    {
        PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.Standalone, ApiCompatibilityLevel.NET_Standard);
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, backend);
        PlayerSettings.companyName = "AirsoftClub";
        PlayerSettings.productName = "AirsoftClubIntegration";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        if (!System.IO.Directory.Exists("Assets/Scenes")) System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/TechnicalBootstrap.unity");
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/TechnicalBootstrap.unity" },
            locationPathName = "../Artifacts/" + folder + "/AirsoftClubIntegration.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.Development
        });
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Build failed: " + report.summary.result);
        UnityEngine.Debug.Log("AIRSOFT_BUILD_PASSED backend=" + backend);
    }
}
