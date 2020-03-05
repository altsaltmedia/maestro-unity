using System;
using System.IO;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Sequencing
{
    public class JoinConfig : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);
            
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
        
        private enum ButtonNames
        {
            JoinNext,
            JoinPrevious,
            ForkJoinNext,
            ForkJoinPrevious,
            SimpleFork
        }

        private static VisualElementToggleData toggleData = new VisualElementToggleData();
        
        private enum EnableCondition
        {
            DirectoryAndNamePopulated
        }
        
        private string selectedObjectDirectory => controlPanel.objectCreation.selectedObjectDirectory;

        private string objectName => controlPanel.objectCreation.objectName;
        
        private void UpdateDisplay()
        {
            if (string.IsNullOrEmpty(selectedObjectDirectory) == false && string.IsNullOrEmpty(objectName) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectoryAndNamePopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectoryAndNamePopulated, false);
            }
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.JoinNext):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(JoinMarker_JoinNext), TimelineEditor.inspectedAsset.duration);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;

                case nameof(ButtonNames.JoinPrevious):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(JoinMarker_JoinPrevious), 0);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;

                case nameof(ButtonNames.ForkJoinNext):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(ForkMarker_JoinNext), TimelineEditor.inspectedAsset.duration);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;
                
                case nameof(ButtonNames.ForkJoinPrevious):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(ForkMarker_JoinPrevious), 0);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;
                
                case nameof(ButtonNames.SimpleFork):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = Utils.CreateScriptableObjectAsset(typeof(Fork), selectedObjectDirectory, objectName);
                        EditorUtility.FocusProjectWindow();
                        EditorGUIUtility.PingObject(Selection.activeObject);
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectoryAndNamePopulated, button);
                    break;
            }

            return button;
        }

        public static ConfigMarker TriggerCreateMarker(ConfigTrack configTrack, Type markerType, double targetTime)
        {
            ConfigMarker configMarker = null;
            string markerTypeName = markerType.Name; 
            
            switch (markerTypeName) {

                case nameof(JoinMarker_JoinNext):
                {
                    configMarker = configTrack.CreateMarker<JoinMarker_JoinNext>(targetTime);
                }
                    break;

                case nameof(JoinMarker_JoinPrevious):
                {
                    configMarker = configTrack.CreateMarker<JoinMarker_JoinPrevious>(targetTime);
                }
                    break;

                case nameof(ForkMarker_JoinNext):
                {
                    configMarker = configTrack.CreateMarker<ForkMarker_JoinNext>(targetTime);
                }
                    break;
                
                case nameof(ForkMarker_JoinPrevious):
                {
                    configMarker = configTrack.CreateMarker<ForkMarker_JoinPrevious>(targetTime);
                }
                    break;
            }

            configMarker.name = markerTypeName;

            return configMarker;
        }

        public static Fork TriggerCreateSimpleFork(string targetDirectory)
        {
            if (targetDirectory.Length > 0) {
                
                Fork newAsset = ScriptableObject.CreateInstance(typeof(Fork)) as Fork;
                string finalPath = targetDirectory + "/New" + nameof(Fork) + ".asset";
                
                if (File.Exists(Path.GetFullPath(finalPath))) {
                    if (EditorUtility.DisplayDialog("Overwrite existing file?", "This will overwrite the existing file at " + finalPath, "Proceed", "Cancel")) {
                        AssetDatabase.CreateAsset(newAsset, finalPath);
                    } else {
                        return null;
                    }
                } else {
                    AssetDatabase.CreateAsset(newAsset, finalPath);
                }
                
                return newAsset;
            } else {
                Debug.Log("Creation cancelled");
            }

            return null;
        }
    }
}