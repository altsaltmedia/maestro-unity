using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AltSalt.Maestro
{
    public class ModifyTools : EditorWindow
    {
//        protected ModifySettings modifySettings;
        protected static ComplexEventManualTrigger textUpdate = new ComplexEventManualTrigger();
        protected static SimpleEventTrigger layoutUpdate = new SimpleEventTrigger();
        protected string layoutDependencyNames;

        protected void CreateHeader()
        {
            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = new Color32(209, 209, 209, 255);
            textStyle.padding = new RectOffset(5, 0, 0, 0);
            textStyle.fontStyle = FontStyle.Bold;

            GUILayout.Space(10);

//            modifySettings = Utils.GetModifySettings();
        }

        protected virtual void OnGUI()
        {
            GUILayout.Space(20);
            
            GUILayout.Label("These values are automatically populated via script.");
            EditorGUI.BeginDisabledGroup(true);
//            modifySettings = EditorGUILayout.ObjectField("Modify Settings", modifySettings, typeof(ModifySettings), false) as ModifySettings;
            layoutUpdate.SetVariable(Utils.GetSimpleEvent(nameof(VarDependencies.LayoutUpdate)));
            layoutUpdate.SetVariable(EditorGUILayout.ObjectField("Layout Update", layoutUpdate.GetVariable(), typeof(SimpleEvent), false) as SimpleEvent);
            textUpdate.SetVariable(Utils.GetComplexEvent(nameof(VarDependencies.TextUpdate)));
            textUpdate.SetVariable(EditorGUILayout.ObjectField("Text Update", textUpdate.GetVariable(), typeof(ComplexEvent), false) as ComplexEvent);
            EditorGUI.EndDisabledGroup();
//
//            if (modifySettings == null) {
//                GUILayout.Label("WARNING: No Modify Settings found! Please create an instance of Modify Settings.");
//            }
        }

    }
}