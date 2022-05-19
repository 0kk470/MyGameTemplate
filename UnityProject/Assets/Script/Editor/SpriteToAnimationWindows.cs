using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace SaltyFish.Editor
{
    [Serializable]
    public class TextureListData
    {
        public List<Texture2D> TextureList = new List<Texture2D>();
    }

    public class SpriteToAnimationWindows : SerializeEditorWindow
    {

        [MenuItem("Tools/Animation/SpriteToAnimationWindows")]
        private static void ShowWindow()
        {
            Rect rect = new Rect(Screen.width, Screen.height, 560, 300);
            var window = GetWindowWithRect<SpriteToAnimationWindows>(rect);
            window.titleContent = new GUIContent("SpriteToAnimationWindows");
            window.Show();
        }

        [SerializeField]
        private bool m_CustomFrame = false;

        [Range(1, 60)]
        private int m_Frame = 10;


        [SerializeField]
        private TextureListData m_TextureData = new TextureListData();

        Vector2 scrollPos;

        public void ConvertSprite()
        {
            var m_SelectedTextures = m_TextureData.TextureList;
            if (m_SelectedTextures == null || m_SelectedTextures.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "You have not selected any textures", "ok");
                return;
            }
            string saveFolder = EditorUtility.SaveFolderPanel("Save textures to folder", "", "");
            if (string.IsNullOrEmpty(saveFolder))
            {
                EditorUtility.DisplayDialog("Error", "You have not specific any folders", "ok");
                return;
            }
            var idx = saveFolder.IndexOf("Assets");
            if(idx == -1)
            {
                EditorUtility.DisplayDialog("Error", "TargetFolder is not in asset folder", "ok");
                return;
            }
            saveFolder = saveFolder.Substring(idx, saveFolder.Length - idx);
            for (int i = 0;i < m_SelectedTextures.Count; ++i)
            {
                EditorUtility.DisplayProgressBar("Hold on", $"Converting {i + 1} of {m_SelectedTextures.Count}", ((float)(i + 1)) / m_SelectedTextures.Count);
                var filePath = AssetDatabase.GetAssetPath(m_SelectedTextures[i]);
                TextureImporter importer = TextureImporter.GetAtPath(filePath) as TextureImporter;
                if (importer != null && importer.spriteImportMode == SpriteImportMode.Multiple && importer.textureType == TextureImporterType.Sprite)
                {
                    CreateAnim(filePath, saveFolder);
                }
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Notice", "Job is done", "Ok");
        }

        private void CreateAnim(string spritePath, string SaveFolder)
        {
            if (string.IsNullOrEmpty(spritePath))
                return;
            var arrAsset = AssetDatabase.LoadAllAssetsAtPath(spritePath).ToList();
            if (arrAsset == null)
            {
                UnityEngine.Debug.LogError("No Sprites at path: " + spritePath);
                return;
            }
            var sprites = arrAsset.Where(obj => obj is Sprite).ToArray();
            AnimationClip clip = new AnimationClip();
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.type = typeof(SpriteRenderer);
            curveBinding.path = "";
            curveBinding.propertyName = "m_Sprite";
            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Length];

            //Frame
            float frameTime = m_CustomFrame ? 1.0f / m_Frame : 1.0f / sprites.Length;
            clip.frameRate = sprites.Length;
            for (int i = 0; i < sprites.Length; i++)
            {
                keyFrames[i] = new ObjectReferenceKeyframe();
                keyFrames[i].time = frameTime * i;
                keyFrames[i].value = sprites[i];
            }

            //Is Loop
            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);
            AnimationClipSettings animationClipSettings = new AnimationClipSettings();
            animationClipSettings.loopTime = true;
            animationClipSettings.startTime = 0;
            animationClipSettings.stopTime = 1;
            clip.wrapMode = WrapMode.Loop;
            AnimationUtility.SetAnimationClipSettings(clip, animationClipSettings);

            //Save
            string clipName = Path.GetFileNameWithoutExtension(spritePath);

            AssetDatabase.CreateAsset(clip, SaveFolder + "/" + clipName + ".anim");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            return;
        }


        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            m_CustomFrame = EditorGUILayout.Toggle("Use Custom Frame", m_CustomFrame);
            if (m_CustomFrame)
            {
                m_Frame = EditorGUILayout.IntSlider("Frame", m_Frame, 1, 60);
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            SerializedProperty target = m_SerializedObject.FindProperty("m_TextureData");
            EditorGUILayout.PropertyField(target);
            m_SerializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Begin Convert"))
            {
                ConvertSprite();
            }
            EditorGUILayout.EndVertical();
        }
    }
}