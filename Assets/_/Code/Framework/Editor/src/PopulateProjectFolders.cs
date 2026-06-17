using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class PopulateProjectFolders : EditorWindow
    {
        [MenuItem("Tools/Populate Folders")]
        public static void ShowWindow() => GetWindow<PopulateProjectFolders>("Populate Folders");

        private void OnGUI()
        {
            GUILayout.Label("", EditorStyles.boldLabel);

            if (GUILayout.Button("Populate")) PopulateFolders();
        }

        private void PopulateFolders()
        {
            string currentPath = "Assets";

            if (!AssetDatabase.IsValidFolder($"{currentPath}/{_subRoot}")) AssetDatabase.CreateFolder(currentPath, _subRoot);

            currentPath += $"/{_subRoot}";

            IterateThrough(_mainFolders, currentPath);
            IterateThrough(_codeSubFolders, currentPath + "/Code");
            IterateThrough(_contentSubFolders, currentPath + "/Content");
            IterateThrough(_databaseSubFolders, currentPath + "/Database");
        }

        private void IterateThrough(string[] foldersList, string path)
        {
            for (int i = 0; i < foldersList.Length; i++)
            {
                if (foldersList[i] == "") return;
                if (AssetDatabase.IsValidFolder($"{path}/{foldersList[i]}")) continue;
                
                if (AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(path, foldersList[i]);
            }
        }

        private string _subRoot = "_";
        private string[] _mainFolders = { "Code", "Content", "Database" };
        private string[] _codeSubFolders = { "Framework", "Game" };
        private string[] _contentSubFolders = { "" };
        private string[] _databaseSubFolders = { "Prefabs", "Scenes", "Settings" };
    }
}