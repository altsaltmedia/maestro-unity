using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Audio
{
    public class AudioStructures : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);
            
            toggleData.Clear();

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);
            
            UpdateDisplay();
            ControlPanel.inspectorUpdateDelegate += UpdateDisplay;
            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        private void OnDestroy()
        {
            ControlPanel.inspectorUpdateDelegate -= UpdateDisplay;
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }

        private static VisualElementToggleData toggleData = new VisualElementToggleData();

        private string objectName => controlPanel.objectCreation.objectName;
        public bool selectOnCreation = true;

        private string selectedObjectDirectory => controlPanel.objectCreation.selectedObjectDirectory;

        private enum ButtonNames
        {
            AudioClipBundle,
            AudioSourceController,
            AudioMixer
        }

        private enum EnableCondition
        {
            DirectoryAndNamePopulated
        }

        private void UpdateDisplay()
        {
            if (string.IsNullOrEmpty(selectedObjectDirectory) == false && string.IsNullOrEmpty(objectName) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectoryAndNamePopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectoryAndNamePopulated, false);
            }
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {
                
                case nameof(ButtonNames.AudioClipBundle):
                    button.clickable.clicked += () =>  {
                        Selection.objects = CreateAudioClipBundle(Selection.objects, selectedObjectDirectory, objectName);
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectoryAndNamePopulated, button);
                    break;
                
                case nameof(ButtonNames.AudioSourceController):
                    button.clickable.clicked += () => {
                        Selection.activeGameObject =
                            ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.audioSourceController, objectName);
                    };
                    break;
                
                case nameof(ButtonNames.AudioMixer):
                    button.clickable.clicked += () =>  {
                        Selection.activeObject = CreateAudioMixer(ModuleUtils.moduleReferences.audioMixerTemplate, objectName,
                                selectedObjectDirectory);
                        EditorGUIUtility.PingObject( Selection.activeObject );
                        EditorUtility.FocusProjectWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectoryAndNamePopulated, button);
                    break;
            }

            return button;
        }

        private static AudioClipBundle[] CreateAudioClipBundle(Object[] selection, string targetPath, string objectName = "AudioClipBundle")
        {
            List<AudioClipBundle> newSelection = new List<AudioClipBundle>();
            AudioClip[] audioClips = Array.ConvertAll(Utils.FilterSelection(selection, typeof(AudioClip)), x => (AudioClip)x);
            if (audioClips.Length > 0) {
                for (int i = 0; i < audioClips.Length; i++) {
                    AudioClipBundle audioClipBundle = Utils.CreateScriptableObjectAsset(typeof(AudioClipBundle), audioClips[i].name, targetPath) as AudioClipBundle;
                    
                    AudioClipData audioClipData = new AudioClipData(audioClips[i]);
                    audioClipBundle.audioClipDataList.Add(audioClipData);
                    EditorUtility.SetDirty(audioClipBundle);
                    
                    newSelection.Add(audioClipBundle);
                }
            }
            else {
                newSelection.Add(Utils.CreateScriptableObjectAsset(typeof(AudioClipBundle), objectName, targetPath) as AudioClipBundle);
            }

            return newSelection.ToArray();
        }

        private static AudioMixer CreateAudioMixer(AudioMixer audioMixerTemplate, string targetName, string targetDirectory)
        {
            Selection.objects = new[] { audioMixerTemplate };
            EditorGUIUtility.PingObject( Selection.activeObject );
            EditorUtility.FocusProjectWindow();
            
            string templateMixerPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string newMixerPath = $"{targetDirectory}/{targetName}.mixer";
            
            AssetDatabase.CopyAsset(templateMixerPath, newMixerPath);
            return AssetDatabase.LoadAssetAtPath<AudioMixer>(newMixerPath);
        }

    }
}
