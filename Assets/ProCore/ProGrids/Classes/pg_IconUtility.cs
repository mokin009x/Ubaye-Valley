#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ProGrids
{
    [InitializeOnLoad]
    public static class pg_IconUtility
    {
        private const string ICON_FOLDER_PATH = "ProGridsToggles";
        private static readonly string iconFolderPath = "Assets/ProCore/ProGrids/GUI/ProGridsToggles/";

        static pg_IconUtility()
        {
            if (!Directory.Exists(iconFolderPath))
            {
                var folder = FindFolder(ICON_FOLDER_PATH);

                if (Directory.Exists(folder))
                    iconFolderPath = folder;
            }
        }

        private static string FindFolder(string folder)
        {
#if !UNITY_WEBPLAYER
            var single = folder.Replace("\\", "/").Substring(folder.LastIndexOf('/') + 1);

            var matches = Directory.GetDirectories("Assets/", single, SearchOption.AllDirectories);

            foreach (var str in matches)
            {
                var path = str.Replace("\\", "/");

                if (path.Contains(folder))
                {
                    if (!path.EndsWith("/"))
                        path += "/";

                    return path;
                }
            }
#endif
            Debug.LogError(
                "Could not locate ProGrids/GUI/ProGridsToggles folder.  The ProGrids folder may be moved, but the contents of ProGrids must remain unmodified.");

            return "";
        }

        public static Texture2D LoadIcon(string iconName)
        {
            var iconPath = string.Format("{0}{1}", iconFolderPath, iconName);

            if (!File.Exists(iconPath))
            {
                Debug.LogError("ProGrids failed to locate menu image: " + iconName +
                               ".\nThis can happen if the GUI folder is moved or deleted.  Deleting and re-importing ProGrids will fix this error.");
                return null;
            }

            return LoadAssetAtPath<Texture2D>(iconPath);
        }

        private static T LoadAssetAtPath<T>(string path) where T : Object
        {
            return (T) AssetDatabase.LoadAssetAtPath(path, typeof(T));
        }
    }
}
#endif