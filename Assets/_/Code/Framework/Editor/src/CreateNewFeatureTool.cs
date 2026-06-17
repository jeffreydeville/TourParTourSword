using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class CreateNewFeatureTool : EditorWindow
    {
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow<CreateNewFeatureTool>("Feature Creator");
            window.minSize = new Vector2(400, 400);
        }

        private void OnGUI()
        {
            GUILayout.Label("Feature Parameters", EditorStyles.boldLabel);

            _featureName = EditorGUILayout.TextField("Feature Name", _featureName);
            // _scriptName = EditorGUILayout.TextField("Script Name", _scriptName);

            EditorGUILayout.Space(5);

            _assemblyTpe = (AssemblyDefinitionType)EditorGUILayout.EnumPopup("Assembly Type", _assemblyTpe);
            _isEditorScope = EditorGUILayout.Toggle("Is Editor Scope", _isEditorScope);
            _isRuntimeScope = EditorGUILayout.Toggle("Is Runtime Scope", _isRuntimeScope);

            if (GUILayout.Button("Generate Feature"))
            {
                CreateStructure();
            }
        }

        private void CreateStructure()
        {
            if (string.IsNullOrEmpty(_featureName))
            {
                Debug.LogError("Feature Name can't be null or empty");
                return;
            }
            //
            // if (string.IsNullOrEmpty(_scriptName))
            // {
            //     Debug.LogError("Script Name can't be null or empty");
            //     return;
            // }

            if (_isRuntimeScope) CreateScope(AssemblyDefinitionScope.Runtime);
            if (_isEditorScope) CreateScope(AssemblyDefinitionScope.Editor);

            AssetDatabase.Refresh();
            Debug.Log($"{_featureName} successfully created");
        }

        private void CreateScope(AssemblyDefinitionScope assemblyDefinitionScope)
        {
            string assemblyPath = _assemblyTpe.ToString();

            string rootPath = $"Assets/_/Code/{assemblyPath}";
            string basePath = $"{rootPath}/{_featureName}";

            string subFolders = assemblyDefinitionScope.ToString();
            string subSubFolders = "src";

            if (!AssetDatabase.IsValidFolder(basePath))
                AssetDatabase.CreateFolder(rootPath, _featureName);

            if (!AssetDatabase.IsValidFolder($"{basePath}/{subFolders}"))
                AssetDatabase.CreateFolder(basePath, subFolders);

            if (!AssetDatabase.IsValidFolder($"{basePath}/{subFolders}/{subSubFolders}"))
                AssetDatabase.CreateFolder($"{basePath}/{subFolders}", subSubFolders);

            CreateAssemblyDefinition($"{basePath}/{subFolders}", assemblyDefinitionScope.ToString());
            // CreateMonoBehavior($"{basePath}/{subFolders}/{subSubFolders}", assemblyDefinitionScope.ToString());
        }

        private void CreateAssemblyDefinition(string path, string scope)
        {
            string fileName = _featureName + "." + scope + ".asmdef";


            AssemblyDefinitionData assemblyDefinitionData = new AssemblyDefinitionData
            {
                name = _featureName + "." + scope,
                rootNamespace = _featureName + "." + scope
            };

            if (scope.Equals(nameof(AssemblyDefinitionScope.Editor)))
            {
                string[] platforms = { "Editor" };
                assemblyDefinitionData.includePlatforms = platforms;
            }

            string jsonBody = JsonUtility.ToJson(assemblyDefinitionData);
            try
            {
                File.WriteAllText($"{path}/{fileName}", jsonBody);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

//         private void CreateMonoBehavior(string path, string scope)
//         {
//             string fileName = _scriptName + ".cs";
//
//             string scriptBody = $@"using UnityEngine;
//
// namespace {_featureName}.{scope}
// {{
//     public class {_scriptName} : MonoBehaviour
//     {{
//         #region Publics
//
//         #endregion
//     
//
//         #region UnityAPI
//
//         #endregion
//
//
//         #region Main API
//
//         #endregion
//
//
//         #region Private and Protected
//
//         #endregion
//     }}
// }}";
//
//             File.WriteAllText($"{path}/{fileName}", scriptBody);
//         }

        private string _featureName;
        // private string _scriptName;

        private bool _isEditorScope;
        private bool _isRuntimeScope;

        private AssemblyDefinitionType _assemblyTpe;
    }

    public class AssemblyDefinitionData
    {
        public string name;
        public string rootNamespace;
        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string[] precompiledReferences;
        public bool autoReferenced = true;
        public string[] defineConstraints;
        public string[] versionDefines;
        public bool noEngineReferences;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string RootNamespace
        {
            get => rootNamespace;
            set => rootNamespace = value;
        }

        public string[] IncludePlatforms
        {
            get => includePlatforms;
            set => includePlatforms = value;
        }

        // public AssemblyDefinitionData(string name, string rootNamespace)
        // {
        //     this.name = name;
        //     this.rootNamespace = rootNamespace;
        // }
        //
        // public AssemblyDefinitionData(string name, string rootNamespace, string[] includePlatforms)
        // {
        //     this.name = name;
        //     this.rootNamespace = rootNamespace;
        //     this.includePlatforms = includePlatforms;
        // }
    }

    public enum AssemblyDefinitionType
    {
        Game,
        Framework
    }

    public enum AssemblyDefinitionScope
    {
        Editor,
        Runtime
    }
}