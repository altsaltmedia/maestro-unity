using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AltSalt.Maestro.Layout;
using UnityEngine;
using UnityEditor;

namespace AltSalt.Maestro
{

    public class LayoutTools : ModifyTools
    {
        private static LayoutTools _layoutTools;

        private static LayoutTools layoutTools
        {
            get => _layoutTools;
            set => _layoutTools = value;
        }

        LayoutConfig targetLayout;
        protected string textFamilyDependencyNames;

        [MenuItem("Tools/Maestro/Layout Tools")]
        public static void ShowWindow()
        {
            var window = GetWindow<LayoutTools>();
            layoutTools = window;
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(LayoutTools)).Show();
        }

        protected override void OnGUI()
        {
            base.CreateHeader();

            GUILayout.Space(10);

            targetLayout = EditorGUILayout.ObjectField("Target Layout", targetLayout, typeof(LayoutConfig), false) as LayoutConfig;

            GUILayout.Space(20);

            if (targetLayout != null) {
                if (targetLayout.textFamilyDependencies.Count > 0) {
                    bool triggerTextChange = true;
                    string textFamilyNames = "";
                    for (int i = 0; i < targetLayout.textFamilyDependencies.Count; i++) {
                        textFamilyNames += targetLayout.textFamilyDependencies[i].GetVariable().name;
                        if (i <= targetLayout.textFamilyDependencies.Count - 2) {
                            textFamilyNames += ", ";
                        }
                    }
                    textFamilyDependencyNames = textFamilyNames;
                } else {
                    textFamilyDependencyNames = "NONE";
                }
                GUILayout.Label("'" + targetLayout.name + "' layout loaded text families: " + textFamilyDependencyNames);
            }

            EditorGUI.BeginDisabledGroup(DisableLayoutUpdate());
            if (GUILayout.Button("Activate Layout")) {
                ActivateLayout();
            }
            if (GUILayout.Button("Deactivate Layout")) {
                DeactivateLayout();
            }
            EditorGUI.EndDisabledGroup();
            
            base.OnGUI();
        }

        private bool DisableLayoutUpdate()
        {
            if(targetLayout == null) {
                return true;
            } else {
                return false;
            }
        }

        private void ActivateLayout()
        {
            if (EditorUtility.DisplayDialog("Set new layout?", 
                "This will activate the layout " + targetLayout.name +
                "and update all responsive elements with corresponding data," +
                "and (if needed) trigger text family changes to " + textFamilyDependencyNames +
                " and update ALL texts.", "Proceed", "Cancel")) {
                TriggerLayoutUpdate(true);
            }
        }

        private void DeactivateLayout()
        {
            if (EditorUtility.DisplayDialog("Set new layout?",
                "This will deactivate the layout " + targetLayout.name +
                "and update all responsive elements with corresponding data," +
                "and (if needed) trigger text family changes to " + textFamilyDependencyNames +
                " and update ALL texts.", "Proceed", "Cancel")) {
                TriggerLayoutUpdate(false);
            }
        }

        private void TriggerLayoutUpdate(bool targetStatus)
        {
            bool triggerTextChange;
            
            if (targetStatus == true) {
                ModifyHandler.ActivateOriginLayout(targetLayout, this, out triggerTextChange);
            }
            else {
                ModifyHandler.DeactivateOriginLayout(targetLayout, this, out triggerTextChange);
            }
            LayoutUpdate();

            if (triggerTextChange == true) {
                TextTools.TextRefreshAll();
            }
        }

        public static void LayoutUpdate()
        {
            UnityEngine.Object[] responsiveObjects = Utils.FilterSelection(Utils.GetAllGameObjects(), typeof(ResponsiveLayoutElement));
            List<ResponsiveLayoutElement> responsiveElements = new List<ResponsiveLayoutElement>();
            
            for (int i = 0; i < responsiveObjects.Length; i++) {
                GameObject gameObject = responsiveObjects[i] as GameObject;
                if (gameObject.active == true) {
                    responsiveElements.Add(gameObject.GetComponent<ResponsiveLayoutElement>());
                }
            }
            responsiveElements.Sort(new ResponsiveUtilsCore.DynamicElementSort());

            for (int i = 0; i < responsiveElements.Count; i++) {
                if (responsiveElements[i].isActiveAndEnabled == true) {
                    responsiveElements[i].CallExecuteLayoutUpdate(LayoutTools.layoutTools);
                }
            }
            
            layoutUpdate.RaiseEvent(layoutTools, "Editor tools", "Layout tools");
        }
    }

}