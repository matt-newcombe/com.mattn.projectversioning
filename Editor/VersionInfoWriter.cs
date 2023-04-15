using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// On post build, this will create a file under Assets/Resources
/// which allows the game to display version and build info at runtime
/// </summary>
public static class VersionInfoWriter
{
    [MenuItem("Version Info/Update Resource Test")]
    public static void UpdateVersionInfoResource()
    {
        string buildNumber = "0";

        VersionInfo current = VersionInfo.Asset;
        if (current != null)
        {
            if (Int32.TryParse(current.buildNumber, out int buildNum))
            {
                buildNumber = (buildNum + 1).ToString();
            }
        }

        UpdateVersionInfoResource(PlayerSettings.bundleVersion, buildNumber);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    // Used by compiled out code behind Unity Cloud Builder define
    public static void UpdateVersionInfoResource(string bundleVersion, string buildNumber)
    {
        VersionInfo versionInfo = VersionInfo.Asset;
        if (versionInfo == null)
        {
            versionInfo = ScriptableObject.CreateInstance<VersionInfo>();
        }

        Debug.Log($"VersionInfo: Is vInfo null - {versionInfo == null}");

        versionInfo.gitCommitShortHash = GitCommandLine.RunBlockingGitCommand(GitCommands.CURRENT_COMMIT_SHORT_HASH).Output;
        versionInfo.buildNumber = buildNumber;
        versionInfo.buildMachineName = SystemInfo.deviceName;
        versionInfo.buildTime = $"{DateTime.Now.ToShortDateString()} - {DateTime.Now.ToShortTimeString()}";

        versionInfo.version = $"{bundleVersion}.{buildNumber}.{versionInfo.gitCommitShortHash}";
        Debug.Log($"VersionInfo: version set to {versionInfo.version}");

        string fileName = $"{VersionInfo.VERSION_INFO_NAME}.asset";
        string folderPath = Path.Combine(VersionInfo.ASSETS, VersionInfo.FOLDER);

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(VersionInfo.ASSETS, VersionInfo.FOLDER);
        }
        string assetPath = Path.Combine(folderPath, fileName);
        CreateOrReplaceAsset(versionInfo, assetPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static T CreateOrReplaceAsset<T>(T asset, string path) where T : UnityEngine.Object
    {
        T existingAsset = null;
        var existingPath = AssetDatabase.GetAssetPath(asset);
        
        if (existingPath != path && !string.IsNullOrEmpty(existingPath) && !string.IsNullOrEmpty(path))
        {
            AssetDatabase.MoveAsset(existingPath, path);
            existingAsset = asset;
        }
        else
        {
            existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);
        }
        
        if (existingAsset == null)
        {
            AssetDatabase.CreateAsset(asset, path);
            existingAsset = asset;
        }
        else
        {
            EditorUtility.CopySerialized(asset, existingAsset);
        }

        return existingAsset;
    }
}