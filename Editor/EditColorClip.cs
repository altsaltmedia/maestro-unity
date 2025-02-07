using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Timeline;
using UnityEditor.Timeline;

namespace AltSalt.Maestro.Animation
{
    public class EditColorClip : ChildModuleWindow
    {
        protected override ChildModuleWindow Configure(ParentModuleWindow parentModuleWindow, string childRootUXMLName)
        {
            base.Configure(parentModuleWindow, childRootUXMLName);

            moduleChildUXML = parentModuleWindow.moduleWindowUXML.Query<Foldout>("EditColorClip", ModuleUtils.toggleableGroup);

            var propertyFields = moduleChildUXML.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = moduleChildUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }

        public Color initialValue = new Color(1, 1, 1, 1);
        public Color targetValue = new Color(1, 1, 1, 1);
        static bool populateButtonPressed = false;

        enum PropertyFieldNames
        {
            InitialValue,
            TargetValue
        }

        enum ButtonNames
        {
            PopulateInitialValueFromSelection,
            PopulateInitialValueFromTarget,
            SetInitialValue,
            SetInitialTransparent,
            SetInitialOpaque,
            PopulateTargetValueFromSelection,
            PopulateTargetValueFromInit,
            SetTargetValue,
            SetTargetTransparent,
            SetTargetOpaque
        }

        void UpdateDisplay()
        {
            bool dependencySelected = false;

            for (int i = 0; i < TimelineEditor.selectedClips.Length; i++) {
                if (TimelineEditor.selectedClips[i].asset is LerpToTargetClip lerpToTargetClip &&
                    lerpToTargetClip.templateReference is ColorBehaviour) {
                    dependencySelected = true;
                    break;
                }
            }

            moduleChildUXML.SetEnabled(dependencySelected);
            moduleChildUXML.value = dependencySelected;
        }

        PropertyField SetupPropertyField(PropertyField propertyField)
        {
            switch (propertyField.name) {

                case nameof(PropertyFieldNames.InitialValue):
                    propertyField.RegisterCallback<ChangeEvent<Color>>((ChangeEvent<Color> evt) => {
                        if (populateButtonPressed == false) {
                            SetInitialValue(TimelineEditor.selectedClips, initialValue);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.TargetValue):
                    propertyField.RegisterCallback<ChangeEvent<Color>>((ChangeEvent<Color> evt) => {
                        if (populateButtonPressed == false) {
                            SetTargetValue(TimelineEditor.selectedClips, targetValue);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                        populateButtonPressed = false;
                    });
                    break;
            }

            return propertyField;
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.PopulateInitialValueFromSelection):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        initialValue = GetInitialValueFromSelection(TimelineEditor.selectedClips);
                    };
                    break;

                case nameof(ButtonNames.PopulateInitialValueFromTarget):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        initialValue = targetValue;
                    };
                    break;

                case nameof(ButtonNames.SetInitialValue):
                    button.clickable.clicked += () => {
                        SetInitialValue(TimelineEditor.selectedClips, initialValue);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;


                case nameof(ButtonNames.SetInitialTransparent):
                    button.clickable.clicked += () => {
                        SetInitialAlpha(TimelineEditor.selectedClips, 0);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetInitialOpaque):
                    button.clickable.clicked += () => {
                        SetInitialAlpha(TimelineEditor.selectedClips, 1);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.PopulateTargetValueFromSelection):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        targetValue = GetTargetValueFromSelection(TimelineEditor.selectedClips);
                    };
                    break;

                case nameof(ButtonNames.PopulateTargetValueFromInit):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        targetValue = initialValue;
                    };
                    break;

                case nameof(ButtonNames.SetTargetValue):
                    button.clickable.clicked += () => {
                        SetTargetValue(TimelineEditor.selectedClips, targetValue);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetTargetTransparent):
                    button.clickable.clicked += () => {
                        SetTargetAlpha(TimelineEditor.selectedClips, 0);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetTargetOpaque):
                    button.clickable.clicked += () => {
                        SetTargetAlpha(TimelineEditor.selectedClips, 1);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;
            }

            return button;
        }

        public static Color GetInitialValueFromSelection(TimelineClip[] clipSelection)
        {
            Color value = new Color(0, 0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (TimelineEditor.selectedClips[i].asset is LerpToTargetClip lerpToTargetClip &&
                    lerpToTargetClip.templateReference is ColorBehaviour colorBehaviour) {
                    value = colorBehaviour.initialValue;
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetInitialValue(TimelineClip[] clipSelection, Color targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (TimelineEditor.selectedClips[i].asset is LerpToTargetClip lerpToTargetClip &&
                    lerpToTargetClip.templateReference is ColorBehaviour colorBehaviour) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    Undo.RecordObject(lerpToTargetClip, "set clip(s) initial color");
                    colorBehaviour.initialValue = targetValue;
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] SetInitialAlpha(TimelineClip[] clipSelection, float targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpToTargetClip lerpToTargetClip &&
                    lerpToTargetClip.templateReference is ColorBehaviour colorBehaviour) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    Undo.RecordObject(lerpToTargetClip, "set clip(s) initial color");
                    Color originalColor = colorBehaviour.initialValue;
                    colorBehaviour.initialValue = new Color(originalColor.r, originalColor.g, originalColor.b, targetValue);
                }
            }

            return changedClips.ToArray();
        }

        public static Color GetTargetValueFromSelection(TimelineClip[] clipSelection)
        {
            Color value = new Color(0, 0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpToTargetClip lerpToTargetClip &&
                    lerpToTargetClip.templateReference is ColorBehaviour colorBehaviour) {
                    value = colorBehaviour.targetValue;
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetTargetValue(TimelineClip[] clipSelection, Color targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpToTargetClip lerpToTargetClip &&
                    lerpToTargetClip.templateReference is ColorBehaviour colorBehaviour) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    Undo.RecordObject(lerpToTargetClip, "set clip(s) target color");
                    colorBehaviour.targetValue = targetValue;
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] SetTargetAlpha(TimelineClip[] clipSelection, float targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpToTargetClip lerpToTargetClip &&
                    lerpToTargetClip.templateReference is ColorBehaviour colorBehaviour) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    Undo.RecordObject(lerpToTargetClip, "set clip(s) target color");
                    Color originalColor = colorBehaviour.targetValue;
                    colorBehaviour.targetValue = new Color(originalColor.r, originalColor.g, originalColor.b, targetValue);
                }
            }

            return changedClips.ToArray();
        }
        
    }
}
