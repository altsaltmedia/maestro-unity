using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AltSalt.Maestro
{
    public abstract class ModuleWindow : EditorWindow
    {
        private ControlPanel _controlPanel;

        public ControlPanel controlPanel
        {
            get => _controlPanel;
            set => _controlPanel = value;
        }
        
        private VisualElement _moduleWindowUXML;

        public VisualElement moduleWindowUXML {
            get => _moduleWindowUXML;
            set => _moduleWindowUXML = value;
        }

        public static ModuleWindow CreateModuleWindow(Type targetType, ControlPanel controlPanel, string sourceUXMLPath)
        {
            var moduleWindow = ScriptableObject.CreateInstance(targetType) as ModuleWindow;
            moduleWindow.Configure(controlPanel, sourceUXMLPath);
            return moduleWindow;
        }
        
        protected virtual ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            rootVisualElement.viewDataKey = DateTime.Now.ToString();
            this.controlPanel = controlPanel;

            var uxmlTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            this.moduleWindowUXML = uxmlTree.CloneTree();
            
            SerializedObject moduleWindowSerialized = new SerializedObject(this);
            this.moduleWindowUXML.Bind(moduleWindowSerialized);

            return this;
        }

    }
}