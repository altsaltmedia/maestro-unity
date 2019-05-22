using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AltSalt
{

    public class LayoutTools : ModifyTools
    {
        Layout targetLayout;
        protected string loadedTextFamilyName;

        [MenuItem("Tools/AltSalt/Layout Tools")]
        public static void ShowWindow()
        {
            var window = GetWindow<LayoutTools>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(LayoutTools)).Show();
        }

        protected override void OnGUI()
        {
            base.CreateHeader();

            GUILayout.Space(10);

            targetLayout = EditorGUILayout.ObjectField("Target Layout", targetLayout, typeof(Layout), false) as Layout;

            GUILayout.Space(20);

            base.OnGUI();

            GUILayout.Space(20);

            if (targetLayout != null) {
                if (targetLayout.supportedTextFamilies.Count > 0) {
                    loadedTextFamilyName = targetLayout.supportedTextFamilies[0].name;
                } else {
                    loadedTextFamilyName = modifySettings.defaultTextFamily.name;
                }
                GUILayout.Label("'" + targetLayout.name + "' layout loaded text family: " + loadedTextFamilyName);
            }

            EditorGUI.BeginDisabledGroup(DisableLayoutUpdate());
            if (GUILayout.Button("Trigger Layout Update")) {
                TriggerLayoutUpdate();
            }
            EditorGUI.EndDisabledGroup();
        }

        bool DisableLayoutUpdate()
        {
            if(targetLayout == null) {
                return true;
            } else {
                return false;
            }
        }

        void TriggerLayoutUpdate()
        {
            if (EditorUtility.DisplayDialog("Set new layout?", "This will set the active layout to " + targetLayout.name +
                " in ModifySettings and update all responsive elements with corresponding data, and change language to " + loadedTextFamilyName + ".", "Proceed", "Cancel")) {
                modifySettings.activeLayout = targetLayout;
                layoutUpdate.Raise();
                if (modifySettings.activeLayout.supportedTextFamilies.Count == 0) {
                    modifySettings.activeTextFamily = modifySettings.defaultTextFamily;
                    textUpdate.Raise();
                } else {
                    bool triggerLayoutChange = true;
                    for (int i = 0; i < modifySettings.activeTextFamily.supportedLayouts.Count; i++) {
                        if (modifySettings.activeTextFamily == modifySettings.activeLayout.supportedTextFamilies[i]) {
                            triggerLayoutChange = false;
                        }
                    }
                    if (triggerLayoutChange == true) {
                        modifySettings.activeTextFamily = modifySettings.activeLayout.supportedTextFamilies[0];
                        textUpdate.Raise();
                    }
                }
            }
        }

    }

}