using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AltSalt
{
    public class ModifyTools : EditorWindow
    {
        protected ModifySettings modifySettings;
        protected ComplexEvent textUpdate;
        protected SimpleEvent layoutUpdate;
        protected string loadedLayoutName;

        protected void CreateHeader()
        {
            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = new Color32(209, 209, 209, 255);
            textStyle.padding = new RectOffset(5, 0, 0, 0);
            textStyle.fontStyle = FontStyle.Bold;

            GUILayout.Space(10);

            modifySettings = Utils.GetModifySettings();
            GUILayout.Label("Current text family: " + modifySettings.activeTextFamily.name, textStyle);
            GUILayout.Label("Current layout: " + modifySettings.activeLayout.name, textStyle);
        }

        protected virtual void OnGUI()
        {
            GUILayout.Label("These values are automatically populated via script.");
            EditorGUI.BeginDisabledGroup(true);
            modifySettings = EditorGUILayout.ObjectField("Modify Settings", modifySettings, typeof(ModifySettings), false) as ModifySettings;
            layoutUpdate = EditorGUILayout.ObjectField("Layout Update", layoutUpdate, typeof(SimpleEvent), false) as SimpleEvent;
            layoutUpdate = Utils.GetSimpleEvent("LayoutUpdate");
            textUpdate = EditorGUILayout.ObjectField("Text Update", textUpdate, typeof(ComplexEvent), false) as ComplexEvent;
            textUpdate = Utils.GetComplexEvent("TextUpdate");
            EditorGUI.EndDisabledGroup();

            if (modifySettings == null) {
                GUILayout.Label("WARNING: No Modify Settings found! Please create an instance of Modify Settings.");
            }
        }

    }
}