using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

/// <summary>
/// Depending on whether we are building in Unity Cloud (PreExport) or
/// Editor - this will create the versioninfo file that is placed in
/// in Resources for runtime access
/// </summary>
public class BuildHookVersionInfo : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;
    
    // Note this env var is set automatically by unity cloud, we do not configure this on devops
    private static string CLOUD_BUILD_ENV_VAR = "UNITY_CLOUD_BUILD_NUMBER";

    public void OnPreprocessBuild(BuildReport report)
    {
        string buildNum = Environment.GetEnvironmentVariable(CLOUD_BUILD_ENV_VAR);
        
        if (!string.IsNullOrEmpty(buildNum))
        {
            VersionInfoWriter.UpdateVersionInfoResource(PlayerSettings.bundleVersion, buildNum);
        }
        else
        {
            VersionInfoWriter.UpdateVersionInfoResource();
        }
    }
}