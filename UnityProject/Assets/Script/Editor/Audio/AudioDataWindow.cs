using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace SaltyFish.Editor
{

    public class AudioDataWindow : SerializeEditorWindow
    {

        [MenuItem("Tools/Audio/AudioDataWindow")]
        public static void OpenWindow()
        {
            var window = GetWindow<AudioDataWindow>("Keyboard Fighter AudioDataWindow", true);
            window.Show();
        }


        [SerializeField]
        private List<AudioClip> m_AudioClips = new List<AudioClip>();



        Vector2 scrollPos;

        public void OnSaveBtnClick()
        {
            Save();
        }

        private void SaveCodeIdentity(Dictionary<string, string> codeMap)
        {
            var savePath = Path.Combine(Application.dataPath, "Script/Data/AudioID.cs");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\tpublic static class AudioID");
            sb.AppendLine("\t{");
            foreach (var kv in codeMap)
            {
                sb.AppendFormat("\t\t\tpublic const string {0} = \"{1}\"; \n\n", kv.Key, kv.Value);
            }
            sb.AppendLine("\t}");
            File.WriteAllText(savePath, sb.ToString());
        }

        private void Save()
        {
            string FilePath = EditorUtility.SaveFilePanel("Save textures to folder", Application.streamingAssetsPath, "AudioConfig", "json");
            if (string.IsNullOrEmpty(FilePath))
            {
                EditorUtility.DisplayDialog("Error", "You have not specific any folders", "ok");
                return;
            }
            if (string.IsNullOrEmpty(FilePath))
            {
                EditorUtility.DisplayDialog("Error", "Path cannot be empty!", "ok");
                return;
            }
            if (Path.GetExtension(FilePath) != ".json")
            {
                EditorUtility.DisplayDialog("Error", "You selected a wrong File. File must end with extension '.json' ", "ok");
                return;
            }
            if (m_AudioClips == null || m_AudioClips.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "m_AudioClips is empty' ", "ok");
                return;
            }
            Dictionary<string, string> m_AudioConfigDic = new Dictionary<string, string>();
            foreach (var audioClip in m_AudioClips)
            {
                var path = AssetDatabase.GetAssetPath(audioClip);
                var name = Path.GetFileNameWithoutExtension(path);
                string id = Regex.Replace(name, @"[\\~#%&*\[\]{}()/:<>?| -]","");
                //uint id = GetUniqueAudioID(Path.GetDirectoryName(path));
                if (m_AudioConfigDic.ContainsKey(id))
                {
                    Debug.LogErrorFormat("id {0} is alreadyExisted, Path:{1}", id, path);
                    continue;
                }
                int extensionIndex = path.LastIndexOf('.');
                if(extensionIndex != -1)
                {
                    path = path.Substring(0, extensionIndex);
                }
                path = path.Replace("Assets/Resources/", "");
                m_AudioConfigDic.Add(id, path);
            }
            var json = JsonConvert.SerializeObject(m_AudioConfigDic, Formatting.Indented);
            File.WriteAllText(FilePath, json);
            SaveCodeIdentity(m_AudioConfigDic);
            EditorUtility.DisplayDialog("Success", "保存成功!", "ok");
        }


        private void FindAudioAssetsInFolder()
        {
            var path = EditorUtility.OpenFolderPanel("Select AudioFolder", "", "");
            var assetPath = Saltyfish.Util.UnityExtension.AbsolutePath2AssetPath(path);
            if (string.IsNullOrEmpty(assetPath))
                return;
            var assetGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { assetPath });
            var audioClips = assetGuids.Select(assetGuid => AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(assetGuid))).ToList();
            
            SerializedProperty target = m_SerializedObject.FindProperty("m_AudioClips");
            target.ClearArray();
            for (int i = 0; i < audioClips.Count; ++i)
            {
                target.InsertArrayElementAtIndex(i);
                var element = target.GetArrayElementAtIndex(i);
                element.objectReferenceValue = audioClips[i];
            }
        }


        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            SerializedProperty target = m_SerializedObject.FindProperty("m_AudioClips");
            EditorGUILayout.PropertyField(target);
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Find Existed Audio Clips"))
            {
                FindAudioAssetsInFolder();
            }
            if (GUILayout.Button("Save Audio"))
            {
                OnSaveBtnClick();
            }
            m_SerializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }
    }
}