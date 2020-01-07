using System;
using AltSalt.Maestro.Layout;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace AltSalt.Maestro
{
    public abstract class ChildModuleWindow : ScriptableObject
    {
        private ParentModuleWindow _parentModuleWindow;

        private ParentModuleWindow parentModuleWindow
        {
            get => _parentModuleWindow;
            set => _parentModuleWindow = value;
        }
        
        private Foldout _moduleChildUXML;

        protected Foldout moduleChildUXML
        {
            get => _moduleChildUXML;
            set => _moduleChildUXML = value;
        }
        
        public static ChildModuleWindow CreateModuleChildWindow(Type targetType, ParentModuleWindow parentModuleWindow, string childRootUXMLName)
        {
            ChildModuleWindow moduleChildWindow = ScriptableObject.CreateInstance(targetType.Name) as ChildModuleWindow;
            moduleChildWindow.Configure(parentModuleWindow, childRootUXMLName);
            return moduleChildWindow;
        }

        protected virtual ChildModuleWindow Configure(ParentModuleWindow parentModuleWindow, string childRootUXMLName)
        {
            this.parentModuleWindow = parentModuleWindow;
            moduleChildUXML = parentModuleWindow.moduleWindowUXML.Query<Foldout>(this.GetType().Name);
            
            SerializedObject childWindowSerialized = new SerializedObject(this);
            moduleChildUXML.Bind(childWindowSerialized);

            return this;
        }
    }
}