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
                    bool triggerTextChange = true;
                    for (int i = 0; i < targetLayout.supportedTextFamilies.Count; i++) {
                        if (modifySettings.activeTextFamily == targetLayout.supportedTextFamilies[i]) {
                            triggerTextChange = false;
                            loadedTextFamilyName = modifySettings.activeTextFamily.name;
                        }
                    }
                    if (triggerTextChange == true) {
                        loadedTextFamilyName = targetLayout.supportedTextFamilies[0].name;
                    }
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
                " in ModifySettings and update all responsive elements with corresponding data, and (if needed) trigger text family change to " + loadedTextFamilyName + ".", "Proceed", "Cancel")) {
                modifySettings.activeLayout = targetLayout;
                layoutUpdate.RaiseEvent(this, "layout tools", "editor window");
                if (targetLayout.supportedTextFamilies.Count == 0) {
                    modifySettings.activeTextFamily = modifySettings.defaultTextFamily;
                    textUpdate.RaiseEvent(this, "layout tools", "editor window");
                } else {
                    bool triggerTextChange = true;
                    for (int i = 0; i < targetLayout.supportedTextFamilies.Count; i++) {
                        if (modifySettings.activeTextFamily == targetLayout.supportedTextFamilies[i]) {
                            triggerTextChange = false;
                        }
                    }
                    if (triggerTextChange == true) {
                        modifySettings.activeTextFamily = targetLayout.supportedTextFamilies[0];
                        textUpdate.RaiseEvent(this, "layout tools", "editor window");
                    }
                }
            }
        }

    }

}