using System;
using System.Collections.Generic;
using System.Linq;
using AltSalt.Maestro.Animation;
using DoozyUI.Internal;
using UnityEditor;
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

        private enum PanelNames
        {
            Modules,
            ConfigureWindow
        }

        private enum ButtonNames
        {
            ModulesButton,
            ConfigureWindowButton,
            LayoutObjects,
            EditObjectValues,
            TextPlacementUtils,
            SequencingStructures,
            JoinConfig,
            TouchConfig,
            AutorunConfig,
            TrackPlacement,
            TimelineAssetPlacement,
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
            RegisterDependencies,
            RefreshModuleWindows,
            ResetControlPanel
        }

        private static Dictionary<ModuleNamespace, List<ButtonNames>> _buttonNamespaces = new Dictionary<ModuleNamespace, List<ButtonNames>>
        {
            {ModuleNamespace.Layout, new List<ButtonNames>
            {
                ButtonNames.LayoutObjects,
                ButtonNames.EditObjectValues,
                ButtonNames.TextPlacementUtils,
            }},
            {ModuleNamespace.Sequencing, new List<ButtonNames>
            {
                ButtonNames.SequencingStructures,
                ButtonNames.JoinConfig,
            }},
            {ModuleNamespace.Touch, new List<ButtonNames>
            {
                ButtonNames.TouchConfig,
            }},
            {ModuleNamespace.Autorun, new List<ButtonNames>
            {
                ButtonNames.AutorunConfig
            }},
            {ModuleNamespace.Animation, new List<ButtonNames>
            {
                ButtonNames.SimpleAnimation,
                ButtonNames.AnimationTracks,
                ButtonNames.EditAnimationClips
            }},
            {ModuleNamespace.Audio, new List<ButtonNames>
            {
                ButtonNames.AudioStructures,
                ButtonNames.AudioTracks
            }},
            {ModuleNamespace.Root, new List<ButtonNames>
            {
                ButtonNames.TrackPlacement,
                ButtonNames.TimelineAssetPlacement,
                ButtonNames.TimelineMonitor,
                ButtonNames.SimpleLogicStructures,
                ButtonNames.EventTracks,
                ButtonNames.ObjectCreation,
                ButtonNames.HierarchySelection,
                ButtonNames.ResponsiveUtils,
                ButtonNames.RegisterDependencies
            }},
            {ModuleNamespace.Logic, new List<ButtonNames>
            {
                ButtonNames.ComplexLogicStructures,
            }},
            {ModuleNamespace.Sensors, new List<ButtonNames>
            {
                ButtonNames.SensorStructures
            }},
            {ModuleNamespace.Modify, new List<ButtonNames>
            {
                ButtonNames.LayoutTools,
                ButtonNames.TextTools
            }}
        };

        private static Dictionary<ButtonNames, List<ButtonNames>> _buttonDependencies = new Dictionary<ButtonNames, List<ButtonNames>>
        {
            { ButtonNames.ObjectCreation, new List<ButtonNames>()
            {
                ButtonNames.LayoutObjects,
                ButtonNames.SequencingStructures,
                ButtonNames.AutorunConfig,
                ButtonNames.TouchConfig,
                ButtonNames.TrackPlacement,
                ButtonNames.AudioStructures,
                ButtonNames.SimpleLogicStructures,
                ButtonNames.ComplexLogicStructures,
            }},
            { ButtonNames.TrackPlacement, new List<ButtonNames>()
            {    
                ButtonNames.AnimationTracks,
                ButtonNames.AudioTracks,
                ButtonNames.EventTracks
            }}
        };

        private static Dictionary<ButtonNames, List<ButtonNames>> buttonDependencies => _buttonDependencies;

        private static Dictionary<ModuleNamespace, List<ButtonNames>> buttonNamespaces
        {
            get => _buttonNamespaces;
            set => _buttonNamespaces = value;
        }

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

        private UQueryBuilder<Button> _buttons;

        private UQueryBuilder<Button> buttons
        {
            get => _buttons;
            set => _buttons = value;
        }

        private List<ButtonNames> _disabledButtonNames = new List<ButtonNames>();
        
        private List<ButtonNames> disabledButtonNames
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

        void OnEnable()
        {
            titleContent = new GUIContent("Maestro Story Engine");
            RenderLayout();
            Selection.selectionChanged += SelectionChangedCallback;
        }

        void OnDisable()
        {
            Selection.selectionChanged -= SelectionChangedCallback;
        }

        void SelectionChangedCallback()
        {
            if (selectionChangedDelegate == null) {
                RenderLayout();
            }
            selectionChangedDelegate.Invoke();
        }

        void OnInspectorUpdate()
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

            modulesUXML = rootVisualElement.Query<ScrollView>(nameof(PanelNames.Modules));
            
            buttons = rootVisualElement.Query<Button>();
            buttons.ForEach(SetupButton);
            
            // To be used for eventual horizontal mode
            //var labels = rootVisualElement.Query<Label>();
            //labels.ForEach(ChangeLabel);
        }
        
        Button SetupButton(Button button)
        {
            switch (button.name) {
                
                default:
                    if (ModuleTypeExists(this, button.name) == false) {
                        button.SetEnabled(false);
                        button.text += " (module not installed)";
                    }
                    button.clickable.clicked += () =>
                    {
                        TriggerCreateModuleWindow(this, button.name);
                        RefreshButtons(this);
                    };
                    break;
                
                case nameof(ButtonNames.ModulesButton):
                    modulesButton = button;
                    button.clickable.clicked += () => {
                        modulesButton.AddToClassList("active");
                        configureWindowButton.RemoveFromClassList("active");
                        ShowVisualElement(rootVisualElement.Query<VisualElement>(nameof(PanelNames.Modules)));
                        HideVisualElement(rootVisualElement.Query<VisualElement>(nameof(PanelNames.ConfigureWindow)));
                    };
                    break;

                case nameof(ButtonNames.ConfigureWindowButton):
                    configureWindowButton = button;
                    button.clickable.clicked += () => {
                        configureWindowButton.AddToClassList("active");
                        modulesButton.RemoveFromClassList("active");
                        HideVisualElement(rootVisualElement.Query<VisualElement>(nameof(PanelNames.Modules)));
                        ShowVisualElement(rootVisualElement.Query<VisualElement>(nameof(PanelNames.ConfigureWindow)));
                    };
                    break;

                case nameof(ButtonNames.RefreshModuleWindows):
                    button.clickable.clicked += () => {
                        RefreshModuleWindows(this);
                    };
                    break;
                
                case nameof(ButtonNames.ResetControlPanel):
                    button.clickable.clicked += () => {
                        ResetControlPanel(this);
                    };
                    break;
            }

            return button;
        }

        private static bool ModuleTypeExists(ControlPanel controlPanel, string buttonString)
        {
            if (ButtonNames.TryParse(buttonString, out ButtonNames buttonName) == false) {
                Debug.Log($"Unable to find data for button {buttonString}. Is your button named correctly?");
                return false;
            }
            
            ModuleNamespaceStrings moduleNamespaceStrings = GetNamespaceStrings(buttonName);
            Type moduleType = Type.GetType($"{moduleNamespaceStrings.name}.{buttonName}");

            if (moduleType != null) {
                return true;
            } else {
                controlPanel.disabledButtonNames.Add(buttonName);
                return false;
            }
        }

        private static ModuleWindow TriggerCreateModuleWindow(ControlPanel controlPanel, string buttonString)
        {
            ModuleWindow moduleWindow = null;
            
            if (ButtonNames.TryParse(buttonString, out ButtonNames buttonName) == false) {
                Debug.Log($"Unable to find data for button {buttonString}. Is your button named correctly?");   
            }
            
            else if (controlPanel.disabledButtonNames.Contains(buttonName) == true) {
                Debug.Log($"{buttonString} module already created.");
            }
            
            else {
                
                if (ButtonHasDependencies(buttonName, out ButtonNames buttonDependency)) {
                    TriggerCreateModuleWindow(controlPanel, buttonDependency.ToString());
                }
                
                ModuleNamespaceStrings moduleNamespaceStrings = GetNamespaceStrings(buttonName);

                if (CreateModuleWindow(controlPanel, moduleNamespaceStrings.name, buttonString,
                    moduleNamespaceStrings.editorPath + buttonString + "_UXML.uxml", out moduleWindow)) {
                    controlPanel.createdModuleWindows.Add(moduleWindow);
                    controlPanel.modulesUXML.Add(moduleWindow.moduleWindowUXML);
                    controlPanel.disabledButtonNames.Add(buttonName);
                    if (ControlPanel.buttonDependencies.ContainsKey(buttonName)) {
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
        
        private static ModuleNamespaceStrings GetNamespaceStrings(ButtonNames buttonName)
        {
            foreach (KeyValuePair<ModuleNamespace, List<ButtonNames>> buttonItem in buttonNamespaces) {
                if (buttonItem.Value.Contains(buttonName)) {
                    return ProjectNamespaceData.namespaceData[buttonItem.Key];
                }
            }

            return null;
        }

        private static bool ButtonHasDependencies(ButtonNames buttonName, out ButtonNames buttonDependency)
        {
            foreach (KeyValuePair<ButtonNames, List<ButtonNames>> buttonDependencyData in buttonDependencies) {
                if (buttonDependencyData.Value.Contains(buttonName)) {
                    buttonDependency = buttonDependencyData.Key;
                    return true;
                }
            }

            buttonDependency = ButtonNames.ObjectCreation; // Default value 
            return false;
        }

        private static ModuleWindow SaveDependency(ControlPanel controlPanel, ModuleWindow moduleWindow)
        {
            switch (moduleWindow) {

                case ObjectCreation gameObjectCreation:
                {
                    controlPanel.objectCreation = gameObjectCreation;
                    break;
                }

                case TrackPlacement trackPlacement:
                {
                    controlPanel.trackPlacement = trackPlacement;
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
            controlPanel.buttons.ForEach(button =>
            {
                if (ButtonNames.TryParse(button.name, out ButtonNames buttonName) != true) return;
                
                if (controlPanel.disabledButtonNames.Contains(buttonName)) {
                    button.SetEnabled(false);
                } else {
                    button.SetEnabled(true);
                }
            });

            return controlPanel.buttons;
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
