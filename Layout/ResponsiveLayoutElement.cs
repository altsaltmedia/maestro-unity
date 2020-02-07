using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using SimpleJSON;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public abstract class ResponsiveLayoutElement : SerializableElement, IResponsiveBreakpoints, IDynamicLayoutElement
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        protected AppSettings appSettings => _appSettings.GetVariable() as AppSettings;

        [ShowInInspector]
        [ReadOnly]
        private float _sceneWidth = 4.65f;

        public float sceneWidth
        {
            get => _sceneWidth;
            set => _sceneWidth = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private float _sceneHeight = 4.594452f;

        public float sceneHeight
        {
            get => _sceneHeight;
            set => _sceneHeight = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private float _sceneAspectRatio = 1.33f;

        public float sceneAspectRatio
        {
            get => _sceneAspectRatio;
            set => _sceneAspectRatio = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _enableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger enableDynamicElement => _enableDynamicElement;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _disableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger disableDynamicElement => _disableDynamicElement;

        public string elementName => this.name;

        [FormerlySerializedAs("hasBreakpoints"),SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(PopulateDefaultBreakpointValues))]
#endif
        protected bool _hasBreakpoints;
        
        public bool hasBreakpoints {
            get => _hasBreakpoints;
            set => _hasBreakpoints = value;
        }

        [FormerlySerializedAs("priority"),SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(ResetResponsiveElementData))]
#endif
        protected int _priority;
        
        public int priority => _priority;

        [SerializeField]
        protected bool _logElementOnLayoutUpdate;
        
        public bool logElementOnLayoutUpdate {
            get
            {
                if (_logElementOnLayoutUpdate == true || AppSettings.logGlobalResponsiveElementActions == true) {
                    return true;
                }

                return false;
            }
        }

        public Scene parentScene => gameObject.scene;

        private int _breakpointIndex;

        protected int breakpointIndex
        {
            get => _breakpointIndex;
            private set => _breakpointIndex = value;
        }

        [FormerlySerializedAs("aspectRatioBreakpoints"),ShowIf(nameof(_hasBreakpoints))]
        [PropertySpace]
        [InfoBox("You should always have x+1 breakpoint values; e.g., for 1 breakpoint at 1.34, you must specify 2 breakpoint values - one for aspect ratios wider than 1.34, and another for aspect ratios narrower than or equal to 1.34.")]
        [InfoBox("Breakpoint examples: To target devices narrow than iPad (aspect ratio 1.33), specify a breakpoint of 1.4; to target narrower than iPhone (1.77), specify a breakpoint of 1.78.")]
        [ValidateInput(nameof(IsPopulated))]
#if UNITY_EDITOR
        [OnValueChanged(nameof(UpdateBreakpointDependencies))]
#endif
        [SerializeField]
        private List<float> _aspectRatioBreakpoints = new List<float>();
        
        public List<float> aspectRatioBreakpoints => _aspectRatioBreakpoints;

        private static bool overwriteAllLayoutFiles = false;
        private static bool cancelSaveData = false;

        protected override void OnEnable()
        {
            base.OnEnable();
#if UNITY_EDITOR
            _appSettings.PopulateVariable(this, nameof(_appSettings));
            if (string.IsNullOrEmpty(_enableDynamicElement.referenceName) == true) {
                _enableDynamicElement.referenceName = nameof(enableDynamicElement).Capitalize();
            }
            _enableDynamicElement.PopulateVariable(this, nameof(_enableDynamicElement));
            if (string.IsNullOrEmpty(_disableDynamicElement.referenceName) == true) {
                _disableDynamicElement.referenceName = nameof(disableDynamicElement).Capitalize();
            }
            _disableDynamicElement.PopulateVariable(this, nameof(_disableDynamicElement));
            
            PopulateNonSerializedProperties();
#endif
            enableDynamicElement.RaiseEvent(this.gameObject, this);
        }

#if UNITY_EDITOR
        //void OnDisable()
        //{
        //    responsiveElementDisable.RaiseEvent(this.gameObject, this);
        //}
#endif

        public void CallExecuteLayoutUpdate(UnityEngine.Object callingObject)
        {
            RefreshActiveLayout();
            if (logElementOnLayoutUpdate == true) {
                Debug.Log("CallExecuteLayoutUpdate triggered!");
                Debug.Log("Calling object : " + callingObject.name, callingObject);
                Debug.Log("Triggered object : " + elementName, gameObject);
                Debug.Log("Component : " + this.GetType().Name, gameObject);
                Debug.Log("--------------------------");
            }

            if (appSettings.modifyLayoutActive == true) {
                LoadData();
            }
            ExecuteResponsiveAction();
        }

        protected void GetBreakpointIndex()
        {
            if (_hasBreakpoints == true) {
                if (_aspectRatioBreakpoints.Count < 1) {
#if UNITY_EDITOR
                    LogBreakpointError();
#endif
                    return;
                } else {
                    breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio, aspectRatioBreakpoints);
                }
            } else {
                breakpointIndex = 0;
            }
        }

        [InfoBox("Trigger responsive action.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(8)]
        public virtual void ExecuteResponsiveAction()
        {
            GetBreakpointIndex();
        }

#if UNITY_EDITOR

        public List<float> AddBreakpoint(float targetBreakpoint)
        {
            Undo.RecordObject(this, "add breakpoint");
            return ResponsiveUtilsCore.AddBreakpointToResponsiveElement(this, targetBreakpoint);
        }

        void ResetResponsiveElementData()
        {
            disableDynamicElement.RaiseEvent(this.gameObject, this);
            enableDynamicElement.RaiseEvent(this.gameObject, this);
        }

        private void PopulateDefaultBreakpointValues()
        {
            PopulateDefaultBreakpoint();
            UpdateBreakpointDependencies();
        }

        private void PopulateDefaultBreakpoint()
        {
            if(hasBreakpoints == true && aspectRatioBreakpoints.Count == 0) {
                decimal tempVal = Convert.ToDecimal(sceneAspectRatio);
                tempVal = Math.Round(tempVal, 2);
                aspectRatioBreakpoints.Add((float)tempVal + .01f);
            }
        }

        protected virtual void UpdateBreakpointDependencies() {
            if(aspectRatioBreakpoints.Count == 0) {
                hasBreakpoints = false;
            }
        }

        public virtual void Reset()
        {
            PopulateNonSerializedProperties();
        }

        protected override void PopulateNonSerializedProperties()
        {
            base.PopulateNonSerializedProperties();
            nonserializedProperties.Add(nameof(_logElementOnLayoutUpdate));
            nonserializedProperties.Add(nameof(_appSettings));
            nonserializedProperties.Add(nameof(_enableDynamicElement));
            nonserializedProperties.Add(nameof(_disableDynamicElement));
        }

        protected virtual void OnRenderObject()
        {
            //Uncomment this if it's ever necessary to repopulate missing dependencies
            //PopulateDependencies();
            //PopulateNonSerializedProperties();
            
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            overwriteAllLayoutFiles = false;
            cancelSaveData = false;
        }

        [HorizontalGroup("Data Handler", 0.5f)]
        [InfoBox("Saves data based on current scene and active layout.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(7)]
        public void SaveData()
        {
            if (cancelSaveData == true) {
                return;
            }
            
            Initialize();
            RefreshActiveLayout();
            string data = JsonUtility.ToJson(this, true);
            var tempObject = JSON.Parse(data);
            for(int i=0; i<nonserializedProperties.Count; i++) {
                tempObject.Remove(nonserializedProperties[i]);
            }
            data = tempObject.ToString(2);

            if (activeLayout == null) {
                Debug.Log("No active layout found. Please populate a layout config.", this);
                return;
            }
            
            string directoryPath = Utils.GetProjectDirectory(new string[] { "Scenes", SceneManager.GetActiveScene().name, "Layout", "Resources", activeLayout.name });
            string fileName = this.name + id.ToString();
            string filePath = Utils.GetFilePath(directoryPath, fileName, ".json");

            if(File.Exists(filePath) && overwriteAllLayoutFiles == false) {

                int option = EditorUtility.DisplayDialogComplex("Overwrite existing file?",
                    "This will overwrite the existing data at " +
                    Utils.GetFilePath(directoryPath, fileName) + ".",
                    "Yes to All", "Cancel", "Yes (Just Once)");
                
                switch (option) {
                    
                    case 0:
                        overwriteAllLayoutFiles = true;
                        File.WriteAllText(filePath, data);
                        AssetDatabase.Refresh();
                        break;

                    case 1:
                        cancelSaveData = true;
                        Debug.Log("Save data cancelled");
                        return;
                    
                    case 2:
                        File.WriteAllText(filePath, data);
                        AssetDatabase.Refresh();
                        break;

                    default:
                        Debug.LogError("Option not recognized");
                        break;
                    
                }
                
            } else {
                File.WriteAllText(filePath, data);
                AssetDatabase.Refresh();
            }
        }

        public string LogAddBreakpointMessage(float targetBreakpoint)
        {
            string message = "Added breakpoint of " + targetBreakpoint.ToString("F2") + " to " + this.name + " " + this.GetType().Name;
            Debug.Log(message, this);
            return message;
        }
#endif
        protected virtual void LogBreakpointError()
        {
            Debug.LogError("Please specify either 1.) target values for saving OR 2.) breakpoints and corresponding values on " + this.name, this);
        }

        [HorizontalGroup("Data Handler", 0.5f)]
        [InfoBox("Load and populate data based on current scene and active layout.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(7)]
        public void LoadData()
        {
            RefreshActiveLayout();
            if(activeLayout == null) return;

            string path = activeLayout.name + "/" + this.name + id.ToString();
            var jsonTextFile = Resources.Load<TextAsset>(path);
            if (jsonTextFile != null) {
                if (AppSettings.logGlobalResponsiveElementActions == true) {
                    Debug.Log("Populating " + this.name + " with stored data for " + activeLayout.name + " layout", this);
                } 
                JsonUtility.FromJsonOverwrite(jsonTextFile.ToString(), this);
                ExecuteResponsiveAction();
            }
            else {
                Debug.Log("File not found at resources path " + path, this);
            }
        }

        protected static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        protected static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(List<float> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }

}