#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace UnnyNet.Editor
{
    [ExecuteInEditMode]
    public class UnnyNet_Editor : EditorWindow
    {
        const string UnnyNetDefine = "UNNYNET";

        private static void AddUnnyNetSymbols(string newDefines)
        {
            var newDefineSymbols = newDefines
                .Split(';')
                .Select(d => d.Trim())
                .ToList();
            
            foreach (BuildTarget target in System.Enum.GetValues(typeof(BuildTarget)))
            {
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

                if (group == BuildTargetGroup.Unknown)
                {
                    continue;
                }

                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

                var defineSymbols = defines
                    .Split(';')
                    .Select(d => d.Trim())
                    .ToList();

                string definesToAdd = string.Empty;
                
                foreach (var newD in newDefineSymbols)
                {
                    bool found = defineSymbols.Any(symbol => newD.Equals(symbol));

                    if (!found)
                        definesToAdd += ";" + newD;
                }

                if (!string.IsNullOrEmpty(definesToAdd))
                {
                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(group,  defines + definesToAdd);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogErrorFormat("Could not add UnnyNet define symbols for build target: {0} group: {1}, {2}", target, group, e);
                    }
                }
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void OnScriptsReloaded()
        {
            AddUnnyNetSymbols(UnnyNetDefine);
        }

        #region Settings Window
        
        [MenuItem("Tools/UnnyNet", false, -100000)]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(UnnyNet_Editor));
            window.titleContent.text = "UnnyNet";
            window.titleContent.image = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UnnyNet/Editor/UnnyLogo.png");
        }

        private void Awake()
        {
            minSize = new Vector2(500, 500);
        }

        private static void OpenHash(string hash)
        {
            Application.OpenURL("https://docs.developers.unnynet.com/" + hash);
        }
        
        public class UnnyNetSettings
        {
            public string ApiGameId;
            public string PublicKey;
            public bool AutoDownloadDictionaries;
        }
        
        const string JSON_PATH = "Assets/UnnyNet/Resources/unnynet.data.json";
        readonly string[] SERVER_TYPE = {"Development", "Stage", "Production"};
        private UnnyNet_Plugins plugins;
        
        private static UnnyNetSettings _settings;
        private static string _apiGameId;
        private static string _publicKey;
        private static bool _autoDownloadDictionaries;
        private static int _selectedServer;
        private static bool _downloading;
        private static float _downloadingProgress;
        private static string _downloadingFileName;

        private UnnyNet_Plugins Plugins
        {
            get
            {
                if (plugins == null)
                    plugins = new UnnyNet_Plugins(this);
                return plugins;
            }
        }
        
        private static void SaveUnnyJson()
        {
            string finalString = JsonUtility.ToJson(_settings);

            Utils.CheckAndCreateDirectoryForFile(JSON_PATH);
            StreamWriter writer = new StreamWriter(JSON_PATH, false);
            writer.WriteLine(finalString);
            writer.Close();
            AssetDatabase.ImportAsset(JSON_PATH);
        }

        private void OnEnable()
        {
            PrepareSettings();
            EditorApplication.update += update;
        }
        
        private void OnDisable()
        {
            EditorApplication.update -= update;
        }

        private void update()
        {
            if (_downloading)
                Repaint();
        }

        private static void PrepareSettings()
        {
            _settings = CreateOrLoadUnnyJson();
            _apiGameId = _settings.ApiGameId;
            _publicKey = _settings.PublicKey;
            _autoDownloadDictionaries = _settings.AutoDownloadDictionaries;
        }

        public static UnnyNetSettings CreateOrLoadUnnyJson()
        {
            TextAsset textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(JSON_PATH, typeof(TextAsset));
            return textAsset == null ? new UnnyNetSettings() : JsonUtility.FromJson<UnnyNetSettings>(textAsset.text);
        }

        private void OnGUI()
        {
            GUI.enabled = !_downloading;
            
            RenderSettings();
            EditorGUILayout.Space();
            RenderLoader();
            EditorGUILayout.Space();
            Plugins.Render();
        }

        private void RenderSettings()
        {
            m_AnyChanges = false;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("UnnyNet Settings");

            SetColor(!string.Equals(_apiGameId,_settings.ApiGameId));
            _apiGameId = EditorGUILayout.TextField("Api Game ID", _apiGameId);
            
            SetColor(!string.Equals(_publicKey,_settings.PublicKey));
            _publicKey = EditorGUILayout.TextField("Public key", _publicKey);
            
            // SetColor(!string.Equals(_autoDownloadDictionaries,_settings.AutoDownloadDictionaries));
            // _autoDownloadDictionaries = EditorGUILayout.Toggle(new GUIContent
            // {
            //     text = "AutoDownload Game Data",
            //     tooltip = "Automatically update Game Data from UnnyNet servers before the build"
            // }, _autoDownloadDictionaries);

            SetColor(m_AnyChanges);
            GUI.enabled = m_AnyChanges && !_downloading;
            if (GUILayout.Button("Save"))
            {
                _settings.ApiGameId = _apiGameId;
                _settings.PublicKey = _publicKey;
                _settings.AutoDownloadDictionaries = _autoDownloadDictionaries;
                SaveUnnyJson();
            }

            GUILayout.EndVertical();
        }

        private void RenderLoader()
        {
            SetColor(false);
            GUI.enabled = !m_AnyChanges && !_downloading && !string.IsNullOrEmpty(_settings.PublicKey) && IsGameIdValid();
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Data Editor");
            _selectedServer = GUILayout.SelectionGrid(_selectedServer, SERVER_TYPE, SERVER_TYPE.Length, EditorStyles.radioButton);

            if (_downloading)
            {
                GUI.enabled = true;
                var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                EditorGUI.ProgressBar(rect, _downloadingProgress, _downloadingFileName);
                GUI.enabled = false;
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Generate Code"))
                    StartCodeGeneration();

                if (GUILayout.Button("Download Data"))
                    StartDownloading();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        private static void StartCodeGeneration()
        {
            _downloading = true;
            _downloadingProgress = 0.5f;
            _downloadingFileName = "Generating the code...";
            UnnyNet_CodeGeneration.StartGeneration(_settings.ApiGameId, _settings.PublicKey, (Constants.Environment) _selectedServer, () => { _downloading = false; });
        }

        private static void StartDownloading()
        {
            _downloading = true;
            _downloadingProgress = 0;

            var appConfig = new AppConfig
            {
                ApiGameId = _settings.ApiGameId,
                PublicKey = _settings.PublicKey,
                Environment = (Constants.Environment) _selectedServer
            };
            
            DicsHelper.LoadDocs(appConfig, responseData =>
            {
                _downloading = false;
                if (!responseData.Success)
                    EditorUtility.DisplayDialog("Error", responseData.Error.Message, "Ok");
            }, (fileName, progress) =>
            {
                _downloadingFileName = fileName;
                _downloadingProgress = progress;
            });
        }

        static bool IsGameIdValid()
        {
            if (string.IsNullOrEmpty(_settings.ApiGameId))
                return false;
            var withoutHyphens = _settings.ApiGameId.Replace("-", "");
            return withoutHyphens.Length == 32;
        }
        
        private bool m_AnyChanges;

        private void SetColor(bool anyChanges) {
            GUI.color = anyChanges ? Color.green : Color.white;
            if (anyChanges)
                m_AnyChanges = true;
        }
        #endregion
    }
}
#endif