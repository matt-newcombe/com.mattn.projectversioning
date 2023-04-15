using System;
using UnityEngine;

namespace mattn
{
    public class VersionInfo : ScriptableObject
    {
        public string version;
        public string buildNumber;
        public string buildTime;
        public string buildMachineName;
        public string gitCommitShortHash;

        public const string VERSION_INFO_NAME = "VersionInfo";
        public const string FOLDER = "Resources";
        public const string ASSETS = "Assets";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _assetInfo = null;
        }

        private static VersionInfo _assetInfo;

        public static VersionInfo Asset
        {
            get
            {
                if (_assetInfo == null)
                {
                    _assetInfo = Resources.Load(VERSION_INFO_NAME) as VersionInfo;
                }

                if (_assetInfo == null)
                {
                    _assetInfo = BuildDummyVersionInfo();
                }

                return _assetInfo;
            }
        }

        private static VersionInfo BuildDummyVersionInfo()
        {
            VersionInfo info = CreateInstance<VersionInfo>();
            info.version = "unknown";
            info.buildNumber = "unknown";

            info.buildTime = $"{DateTime.Now.ToShortDateString()} - {DateTime.Now.ToShortTimeString()}";
            info.buildMachineName = SystemInfo.deviceName;

            return info;
        }
    }
}