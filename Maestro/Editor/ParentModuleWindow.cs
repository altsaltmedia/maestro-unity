using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AltSalt.Maestro
{
    public abstract class ParentModuleWindow : ModuleWindow
    {
        protected abstract Dictionary<Type, string> childWindowData { get; set; }

        private List<ChildModuleWindow> _childWindows = new List<ChildModuleWindow>();

        protected List<ChildModuleWindow> childWindows
        {
            get => _childWindows;
            set => _childWindows = value;
        }

        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            this.controlPanel = controlPanel;

            var uxmlTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            this.moduleWindowUXML = uxmlTree.CloneTree();
            
            SerializedObject childWindowSerialized = new SerializedObject(this);
            this.moduleWindowUXML.Bind(childWindowSerialized);
            
            foreach (KeyValuePair<Type, string> childWindow in childWindowData) {
                childWindows.Add(ChildModuleWindow.CreateModuleChildWindow(childWindow.Key, this, childWindow.Value));
            }

            return this;
        }
    }
}