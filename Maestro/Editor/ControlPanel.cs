using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AltSalt.Maestro
{
    public class ControlPanel : EditorWindow
    {
        public delegate void InspectorUpdateDelegate();
        public static InspectorUpdateDelegate inspectorUpdateDelegate = () => { };

        public delegate void SelectionChangedDelegate();
        public static SelectionChangedDelegate selectionChangedDelegate = () => { };

        private enum ElementNames
        {
            ConfigureWindow,
            ModuleButtons,
            RefreshButtons,
            Modules
        }

        private enum WindowButtonNames
        {
            ModulesButton,
            ConfigureWindowButton,
            RefreshModuleWindows,
            ResetControlPanel
        }

        private enum ModuleNames
        {
            LayoutObjects,
            EditObjectValues,
            TextPlacementUtils,
            SequencingStructures,
            JoinConfig,
            TouchConfig,
            AutorunConfig,
            TrackPlacement,
            TimelineAssetManipulation,
            ClipPlacement,
            PopulateAnimationClips,
            TimelineMonitor,
            SimpleAnimation,
            AnimationTracks,
            EditAnimationClips,
            AudioStructures,
            AudioTracks,
            SimpleLogicStructures,
            EventTracks,
            ComplexLogicStructures,
            SensorStructures,
            LayoutTools,
            TextTools,
            ObjectCreation,
            HierarchySelection,
            ResponsiveUtils,
            RegisterDependencies
        }

        private static Dictionary<ModuleNamespace, List<ModuleNames>> _moduleNamespaces = new Dictionary<ModuleNamespace, List<ModuleNames>>
        {
            {ModuleNamespace.Layout, new List<ModuleNames>
            {
                ModuleNames.LayoutObjects,
                ModuleNames.EditObjectValues,
                ModuleNames.TextPlacementUtils,
            }},
            {ModuleNamespace.Sequencing, new List<ModuleNames>
            {
                ModuleNames.SequencingStructures,
                ModuleNames.JoinConfig,
            }},
            {ModuleNamespace.Touch, new List<ModuleNames>
            {
                ModuleNames.TouchConfig,
            }},
            {ModuleNamespace.Autorun, new List<ModuleNames>
            {
                ModuleNames.AutorunConfig
            }},
            {ModuleNamespace.Animation, new List<ModuleNames>
            {
                ModuleNames.SimpleAnimation,
                ModuleNames.AnimationTracks,
                ModuleNames.EditAnimationClips
            }},
            {ModuleNamespace.Audio, new List<ModuleNames>
            {
                ModuleNames.AudioStructures,
                ModuleNames.AudioTracks
            }},
            {ModuleNamespace.Root, new List<ModuleNames>
            {
                ModuleNames.TrackPlacement,
                ModuleNames.TimelineAssetManipulation,
                ModuleNames.ClipPlacement,
                ModuleNames.TimelineMonitor,
                ModuleNames.SimpleLogicStructures,
                ModuleNames.EventTracks,
                ModuleNames.ObjectCreation,
                ModuleNames.HierarchySelection,
                ModuleNames.LayoutTools,
                ModuleNames.TextTools,
                ModuleNames.ResponsiveUtils,
                ModuleNames.RegisterDependencies
            }},
            {ModuleNamespace.Logic, new List<ModuleNames>
            {
                ModuleNames.ComplexLogicStructures,
            }},
            {ModuleNamespace.Sensors, new List<ModuleNames>
            {
                ModuleNames.SensorStructures
            }},
        };
        
        private static Dictionary<ModuleNamespace, List<ModuleNames>> moduleNamespaces => _moduleNamespaces;

        private static Dictionary<ModuleNames, List<ModuleNames>> _moduleDependencies = new Dictionary<ModuleNames, List<ModuleNames>>
        {
            { ModuleNames.ObjectCreation, new List<ModuleNames>()
            {
                ModuleNames.LayoutObjects,
                ModuleNames.SequencingStructures,
                ModuleNames.AutorunConfig,
                ModuleNames.TouchConfig,
                ModuleNames.TrackPlacement,
                ModuleNames.AudioStructures,
                ModuleNames.SimpleLogicStructures,
                ModuleNames.ComplexLogicStructures,
            }},
            { ModuleNames.TrackPlacement, new List<ModuleNames>()
            {    
                ModuleNames.AnimationTracks,
                ModuleNames.AudioTracks,
                ModuleNames.EventTracks,
            }},
            { ModuleNames.TimelineAssetManipulation, new List<ModuleNames>()
            {    
                ModuleNames.ClipPlacement
            }},
            { ModuleNames.ClipPlacement, new List<ModuleNames>()
            {    
                ModuleNames.PopulateAnimationClips
            }},
        };

        private static Dictionary<ModuleNames, List<ModuleNames>> moduleDependencies => _moduleDependencies;

        
        private VisualElement _modulesUXML;

        private VisualElement modulesUXML {
            get => _modulesUXML;
            set => _modulesUXML = value;
        }

        private Button _modulesButton;

        private Button modulesButton {
            get => _modulesButton;
            set => _modulesButton = value;
        }

        private Button _configureWindowButton;

        private Button configureWindowButton {
            get => _configureWindowButton;
            set => _configureWindowButton = value;
        }

        private ObjectCreation _objectCreation;

        public ObjectCreation objectCreation
        {
            get => _objectCreation;
            private set => _objectCreation = value;
        }
        
        private TrackPlacement _trackPlacement;

        public TrackPlacement trackPlacement
        {
            get => _trackPlacement;
            private set => _trackPlacement = value;
        }
        
        private ClipPlacement _clipPlacement;

        public ClipPlacement clipPlacement
        {
            get => _clipPlacement;
            private set => _clipPlacement = value;
        }

        private UQueryBuilder<Button> _moduleButtons;

        private UQueryBuilder<Button> moduleButtons
        {
            get => _moduleButtons;
            set => _moduleButtons = value;
        }

        private List<ModuleNames> _disabledButtonNames = new List<ModuleNames>();
        
        private List<ModuleNames> disabledButtonNames
        {
            get => _disabledButtonNames;
            set => _disabledButtonNames = value;
        }

        private Dictionary<Type, string> _enabledModuleWindows = new Dictionary<Type, string>();

        private Dictionary<Type, string> enabledModuleWindows => _enabledModuleWindows;

        private List<ModuleWindow> _createdModuleWindows = new List<ModuleWindow>();

        private List<ModuleWindow> createdModuleWindows => _createdModuleWindows;
        
        [MenuItem("Tools/Maestro/Control Panel")]
        public static void ShowWindow()
        {
            var window = CreateInstance<ControlPanel>();
            window.Show();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Maestro Story Engine");
            RenderLayout();
            Selection.selectionChanged += SelectionChangedCallback;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= SelectionChangedCallback;
        }

        private void SelectionChangedCallback()
        {
            if (selectionChangedDelegate == null) {
                RenderLayout();
            }
            selectionChangedDelegate.Invoke();
        }

        private void OnInspectorUpdate()
        {
            inspectorUpdateDelegate.Invoke();
        }

        public void RenderLayout()
        {
            Selection.objects = null;
            foreach (ModuleWindow moduleWindow in createdModuleWindows) {
                DestroyImmediate(moduleWindow);
            }
            enabledModuleWindows.Clear();
            disabledButtonNames.Clear();
            rootVisualElement.Clear();
            
            AssetDatabase.Refresh();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Utils.GetStylesheetPath());
            rootVisualElement.styleSheets.Add(styleSheet);

            // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
            var pageBuilderTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ProjectNamespaceData.namespaceData[ModuleNamespace.Root].editorPath + "ControlPanel_UXML.uxml");
            VisualElement pageBuilderStructure = pageBuilderTree.CloneTree();
            rootVisualElement.Add(pageBuilderStructure);

            VisualElement refreshButtonUXML = rootVisualElement.Query<VisualElement>(nameof(ElementNames.RefreshButtons));
            List<Button> windowButtons = refreshButtonUXML.Query<Button>().ToList();
            windowButtons.AddRange(rootVisualElement.Query<ToolbarButton>().ToList());
            windowButtons.ForEach(SetupWindowButton);
            
            VisualElement moduleButtonsUXML = rootVisualElement.Query<VisualElement>(nameof(ElementNames.ModuleButtons));
            moduleButtons = moduleButtonsUXML.Query<Button>();
            moduleButtons.ForEach(SetupModuleButton);
            
            modulesUXML = rootVisualElement.Query<ScrollView>(nameof(ElementNames.Modules));
            
            // To be used for eventual horizontal mode
            //var labels = rootVisualElement.Query<Label>();
            //labels.ForEach(ChangeLabel);
        }

        private void SetupWindowButton(Button button)
        {
            switch (button.name) {
                
                case nameof(WindowButtonNames.ModulesButton):
                    modulesButton = button;
                    button.clickable.clicked += () => {
                        modulesButton.AddToClassList("active");
                        configureWindowButton.RemoveFromClassList("active");
                        ShowVisualElement(rootVisualElement.Query<VisualElement>(nameof(ElementNames.Modules)));
                        HideVisualElement(rootVisualElement.Query<VisualElement>(nameof(ElementNames.ConfigureWindow)));
                    };
                    break;

                case nameof(WindowButtonNames.ConfigureWindowButton):
                    configureWindowButton = button;
                    button.clickable.clicked += () => {
                        configureWindowButton.AddToClassList("active");
                        modulesButton.RemoveFromClassList("active");
                        HideVisualElement(rootVisualElement.Query<VisualElement>(nameof(ElementNames.Modules)));
                        ShowVisualElement(rootVisualElement.Query<VisualElement>(nameof(ElementNames.ConfigureWindow)));
                    };
                    break;

                case nameof(WindowButtonNames.RefreshModuleWindows):
                    button.clickable.clicked += () => {
                        RefreshModuleWindows(this);
                    };
                    break;
                
                case nameof(WindowButtonNames.ResetControlPanel):
                    button.clickable.clicked += () => {
                        ResetControlPanel(this);
                    };
                    break;
            }
        }

        private Button SetupModuleButton(Button button)
        {

            switch (button.name) {

                case nameof(ModuleNames.RegisterDependencies):
                    button.clickable.clicked += RegisterDependencies.ShowWindow;
                    break;

                case nameof(ModuleNames.TextTools):
                    button.clickable.clicked += TextTools.ShowWindow;
                    break;
                
                case nameof(ModuleNames.LayoutTools):
                {
                    button.clickable.clicked += LayoutTools.ShowWindow;
                    break;
                }

                default:
                {
                    if (ModuleTypeExists(this, button.name, out Type moduleType) == false) {
                        button.SetEnabled(false);
                        button.text += " (module not installed)";
                    }
                    else {
                        button.clickable.clicked += () =>
                        {
                            TriggerCreateModuleWindow(this, button.name);
                            RefreshButtons(this);
                        };
                    }
                    break;
                }

            }

            return button;
        }

        private static bool ModuleTypeExists(ControlPanel controlPanel, string moduleString, out Type moduleType)
        {
            moduleType = null;
            
            if (ModuleNames.TryParse(moduleString, out ModuleNames module) == false) {
                //Debug.Log($"Unable to find data for module {moduleString}. Is your button named correctly?");
                return false;
            }
            
            ModuleNamespaceStrings moduleNamespaceStrings = GetNamespaceStrings(module);
            moduleType = Type.GetType($"{moduleNamespaceStrings.name}.{module}");

            if (moduleType != null) {
                return true;
            }

            controlPanel.disabledButtonNames.Add(module);
            return false;
        }

        private static ModuleWindow TriggerCreateModuleWindow(ControlPanel controlPanel, string moduleString)
        {
            ModuleWindow moduleWindow = null;
            
            if (ModuleNames.TryParse(moduleString, out ModuleNames moduleName) == false) {
                Debug.Log($"Unable to find data for module {moduleString}. Is your button named correctly?");   
            }
            
            else if (controlPanel.disabledButtonNames.Contains(moduleName) == true) {
                Debug.Log($"{moduleString} module already created.");
            }
            
            else {
                
                if (ModuleHasDependencies(moduleName, out ModuleNames moduleDependency)) {
                    TriggerCreateModuleWindow(controlPanel, moduleDependency.ToString());
                }
                
                ModuleNamespaceStrings moduleNamespaceStrings = GetNamespaceStrings(moduleName);

                if (CreateModuleWindow(controlPanel, moduleNamespaceStrings.name, moduleString,
                    moduleNamespaceStrings.editorPath + moduleString + "_UXML.uxml", out moduleWindow)) {
                    controlPanel.createdModuleWindows.Add(moduleWindow);
                    controlPanel.modulesUXML.Add(moduleWindow.moduleWindowUXML);
                    controlPanel.disabledButtonNames.Add(moduleName);
                    if (ControlPanel.moduleDependencies.ContainsKey(moduleName)) {
                        SaveDependency(controlPanel, moduleWindow);
                    }
                }
            }

            return moduleWindow;
        }
        
        private static bool CreateModuleWindow(ControlPanel controlPanel, string @namespace, string @class, string uxmlPath, out ModuleWindow moduleWindow)
        {
            var moduleClassType = Type.GetType($"{@namespace}.{@class}");

            if(moduleClassType != null) {
                
                if (controlPanel.enabledModuleWindows.ContainsKey(moduleClassType) == false) {
                    moduleWindow = ModuleWindow.CreateModuleWindow(moduleClassType, controlPanel, uxmlPath);
                    controlPanel.enabledModuleWindows.Add(moduleClassType, uxmlPath);
                    return true;
                }
            }
            else {
                Debug.Log("Unable to instantiate instance.");
            }

            moduleWindow = null;
            return false;
        }
        
        private static ModuleNamespaceStrings GetNamespaceStrings(ModuleNames moduleName)
        {
            foreach (KeyValuePair<ModuleNamespace, List<ModuleNames>> moduleNamespace in moduleNamespaces) {
                if (moduleNamespace.Value.Contains(moduleName)) {
                    return ProjectNamespaceData.namespaceData[moduleNamespace.Key];
                }
            }

            return null;
        }

        private static bool ModuleHasDependencies(ModuleNames moduleName, out ModuleNames moduleDependency)
        {
            foreach (KeyValuePair<ModuleNames, List<ModuleNames>> buttonDependencyData in moduleDependencies) {
                if (buttonDependencyData.Value.Contains(moduleName)) {
                    moduleDependency = buttonDependencyData.Key;
                    return true;
                }
            }

            moduleDependency = ModuleNames.ObjectCreation; // Default value 
            return false;
        }

        private static ModuleWindow SaveDependency(ControlPanel controlPanel, ModuleWindow moduleWindow)
        {
            switch (moduleWindow) {

                case ObjectCreation objectCreation:
                {
                    controlPanel.objectCreation = objectCreation;
                    break;
                }

                case TrackPlacement trackPlacement:
                {
                    controlPanel.trackPlacement = trackPlacement;
                    break;
                }
                
                case ClipPlacement clipPlacement:
                {
                    controlPanel.clipPlacement = clipPlacement;
                    break;
                }
            }

            return moduleWindow;
        }

        private static ControlPanel RefreshModuleWindows(ControlPanel controlPanel)
        {
            controlPanel.modulesUXML.Clear();
            foreach (ModuleWindow oldModuleWindow in controlPanel.createdModuleWindows) {
                DestroyImmediate(oldModuleWindow);
            }
            
            AssetDatabase.Refresh();
            
            foreach(KeyValuePair<Type, string> childWindow in controlPanel.enabledModuleWindows) {
                ModuleWindow newModuleWindow =
                    ModuleWindow.CreateModuleWindow(childWindow.Key, controlPanel, childWindow.Value);
                controlPanel.createdModuleWindows.Add(newModuleWindow);
                controlPanel.modulesUXML.Add(newModuleWindow.moduleWindowUXML);
            }

            RefreshButtons(controlPanel);
            
            return controlPanel;
        }

        private static UQueryBuilder<Button> RefreshButtons(ControlPanel controlPanel)
        {    
            controlPanel.moduleButtons.ForEach(button =>
            {
                if (ModuleNames.TryParse(button.name, out ModuleNames buttonName) != true) return;
                
                if (controlPanel.disabledButtonNames.Contains(buttonName)) {
                    button.SetEnabled(false);
                } else {
                    button.SetEnabled(true);
                }
            });

            return controlPanel.moduleButtons;
        }

        private static ControlPanel ResetControlPanel(ControlPanel controlPanel)
        {
            controlPanel.enabledModuleWindows.Clear();
            controlPanel.disabledButtonNames.Clear();
            controlPanel.RenderLayout();
            return controlPanel;
        }

        static VisualElement ShowVisualElement(VisualElement visualElement)
        {
            visualElement.style.display = DisplayStyle.Flex;
            return visualElement;
        }

        static VisualElement HideVisualElement(VisualElement visualElement)
        {
            visualElement.style.display = DisplayStyle.None;
            return visualElement;
        }
        
        // ** Will eventually be used for Horizontal Mode **
        // 
        //Label ChangeLabel(Label label)
        //{
        //    if(label.text == "X") {
        //        label.text = "Y";
        //    } else if(label.text == "Y") {
        //        label.text = "X";
        //    }
        //    return label;
        //}
    }
}
