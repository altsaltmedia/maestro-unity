using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AltSalt.Maestro.Layout
{
    public class ContentExtensionTools : EditorWindow
    {
        protected static ComplexEventManualTrigger textChanged = new ComplexEventManualTrigger();
        protected static SimpleEventTrigger layoutChanged = new SimpleEventTrigger();
        protected string layoutDependencyNames;

        protected void CreateHeader()
        {
            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = new Color32(209, 209, 209, 255);
            textStyle.padding = new RectOffset(5, 0, 0, 0);
            textStyle.fontStyle = FontStyle.Bold;

            GUILayout.Space(10);
        }

        protected virtual void OnGUI()
        {
            GUILayout.Space(20);
            
            GUILayout.Label("These values are automatically populated via script.");
            EditorGUI.BeginDisabledGroup(true);
            layoutChanged.SetVariable(Utils.GetSimpleEvent(nameof(VarDependencies.LayoutChanged)));
            layoutChanged.SetVariable(EditorGUILayout.ObjectField("Layout Update", layoutChanged.GetVariable(), typeof(SimpleEvent), false) as SimpleEvent);
            textChanged.SetVariable(Utils.GetComplexEvent(nameof(VarDependencies.TextChanged)));
            textChanged.SetVariable(EditorGUILayout.ObjectField("Text Update", textChanged.GetVariable(), typeof(ComplexEvent), false) as ComplexEvent);
            EditorGUI.EndDisabledGroup();
        }

    }
}