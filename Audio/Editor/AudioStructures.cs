using System.IO;
using UnityEditor;
using UnityEngine.Audio;
using UnityEngine.UIElements;

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
            //ControlPanel.selectionChangedDelegate += MoveAudioMixer;

            return this;
        }

        private void OnDestroy()
        {
            ControlPanel.inspectorUpdateDelegate -= UpdateDisplay;
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
            //ControlPanel.selectionChangedDelegate -= MoveAudioMixer;
        }

        private static VisualElementToggleData toggleData = new VisualElementToggleData();

        private string objectName => controlPanel.objectCreation.objectName;
        public bool selectOnCreation = true;

        private string selectedObjectDirectory => controlPanel.objectCreation.selectedObjectDirectory;

        private AudioMixer audioMixerTemplate => ModuleUtils.moduleReferences.audioMixerTemplate;
        
        // private bool _audioMixerCreated;
        //
        // private bool audioMixerCreated
        // {
        //     get => _audioMixerCreated;
        //     set => _audioMixerCreated = value;
        // }
        //
        // private bool _moveAudioMixerLoaded;
        //
        // private bool moveAudioMixerLoaded
        // {
        //     get => _moveAudioMixerLoaded;
        //     set => _moveAudioMixerLoaded = value;
        // }

        // private string _cachedMixerName;
        //
        // private string cachedMixerName
        // {
        //     get => _cachedMixerName;
        //     set => _cachedMixerName = value;
        // }
        //
        // private string _cachedDirectoryPath;
        //
        // private string cachedDirectoryPath
        // {
        //     get => _cachedDirectoryPath;
        //     set => _cachedDirectoryPath = value;
        // }


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
                        Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(AudioClipBundle), objectName, selectedObjectDirectory) };
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
