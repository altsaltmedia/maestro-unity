/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro
{
    [Serializable]
    public abstract class ResponsiveLerpToTargetBehaviour : LerpToTargetBehaviour, IResponsiveBreakpoints, IDynamicLayoutElement
    {
        [ShowInInspector]
        [ReadOnly]
        public float _sceneAspectRatio;

        public float sceneAspectRatio
        {
            get => _sceneAspectRatio;
            set => _sceneAspectRatio = value;
        }
        
        [ShowInInspector]
        [ReadOnly]
        private float _sceneWidth;

        public float sceneWidth
        {
            get => _sceneWidth;
            set => _sceneWidth = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private float _sceneHeight;

        public float sceneHeight
        {
            get => _sceneHeight;
            set => _sceneHeight = value;
        }

        [SerializeField]
        [FormerlySerializedAs("_responsiveElementEnable")]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventTrigger _enableDynamicElement = new ComplexEventTrigger();

        public ComplexEventTrigger enableDynamicElement
        {
            get => _enableDynamicElement;
            set => _enableDynamicElement = value;
        }

        [SerializeField]
        [FormerlySerializedAs("_responsiveElementDisable")]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventTrigger _disableDynamicElement = new ComplexEventTrigger();

        public ComplexEventTrigger disableDynamicElement
        {
            get => _disableDynamicElement;
            set => _disableDynamicElement = value;
        }

#if UNITY_EDITOR
        [OnValueChanged(nameof(PopulateDefaultBreakpointValues))]
#endif
        [FormerlySerializedAs("hasBreakpoints")]
        [SerializeField]
        protected bool _hasBreakpoints;
        
        public bool hasBreakpoints {
            get => _hasBreakpoints;
            set => _hasBreakpoints = value;
        }

        [FormerlySerializedAs("priority")]
        [SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(ResetResponsiveElementData))]
#endif
        private int _priority;
        
        public int priority {
            get => _priority;
            set => _priority = value;
        }

        private bool _logElementOnLayoutUpdate;
        
        public bool logElementOnLayoutUpdate {
            get
            {
                if (appSettings.logResponsiveElementActions == true) {
                    return true;
                }

                return _logElementOnLayoutUpdate;
            }
        }

        [FormerlySerializedAs("aspectRatioBreakpoints")]
        [PropertySpace]
        [SerializeField]
        [InfoBox("You should always have x+1 breakpoint values; e.g., for 1 breakpoint at 1.34, you must specify 2 breakpoint values - one for aspect ratios wider than 1.34, and another for aspect ratios narrower than or equal to 1.34.")]
        [InfoBox("Breakpoint examples: To target devices narrow than iPad (aspect ratio 1.33), specify a breakpoint of 1.4; to target narrower than iPhone (1.77), specify a breakpoint of 1.78.")]
#if UNITY_EDITOR
        [OnValueChanged(nameof(UpdateBreakpointDependencies))]
#endif
        private List<float> _aspectRatioBreakpoints = new List<float>();
        
        public List<float> aspectRatioBreakpoints => _aspectRatioBreakpoints;

        public string elementName => this.ToString();

        public Scene parentScene => directorObject.scene;

        public override void OnPlayableCreate(Playable playable)
        {
#if UNITY_EDITOR
            UpdateBreakpointDependencies();

            if (enableDynamicElement.complexEvent == null) {
                enableDynamicElement.complexEvent = Utils.GetComplexEvent(nameof(VarDependencies.EnableDynamicElement));
            }

            if (disableDynamicElement.complexEvent == null) {
                disableDynamicElement.complexEvent = Utils.GetComplexEvent(nameof(VarDependencies.DisableDynamicElement));
            }
#endif
            base.OnPlayableCreate(playable);
            enableDynamicElement.RaiseEvent(clipAsset, parentScene.name, clipAsset.name, this);

            easingFunction = EasingFunction.GetEasingFunction(ease);
            CallExecuteLayoutUpdate(directorObject);
        }

        public void CallExecuteLayoutUpdate(UnityEngine.Object callingObject)
        {
            if (logElementOnLayoutUpdate == true) {
                Debug.Log("CallExecuteLayoutUpdate triggered!");
                Debug.Log("Calling object : " + callingObject.name, callingObject);
                Debug.Log("Triggered timeline clip : " + elementName, clipAsset);
                Debug.Log("Track : " + parentTrack.name, parentTrack);
                Debug.Log("--------------------------");
            }

            // TO DO - write functionality to hook into modify
            ExecuteResponsiveAction();
        }

        protected virtual void ExecuteResponsiveAction()
        {
            if (aspectRatioBreakpoints.Count == 0 || hasBreakpoints == false) {
                SetValue(0);
            } else {
                int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio, aspectRatioBreakpoints);
                SetValue(breakpointIndex);
            }
        }

        protected abstract void SetValue(int activeIndex);

#if UNITY_EDITOR
        private void ResetResponsiveElementData()
        {
            disableDynamicElement.RaiseEvent(clipAsset, parentScene.name, clipAsset.name, this);
            enableDynamicElement.RaiseEvent(clipAsset, parentScene.name, clipAsset.name, this);
        }

        private void PopulateDefaultBreakpointValues()
        {
            PopulateDefaultBreakpoint();
            UpdateBreakpointDependencies();
        }

        private void PopulateDefaultBreakpoint()
        {
            if (hasBreakpoints == true && aspectRatioBreakpoints.Count == 0) {
                decimal tempVal = Convert.ToDecimal(sceneAspectRatio);
                tempVal = Math.Round(tempVal, 2);
                aspectRatioBreakpoints.Add((float)tempVal + .01f);
            }
        }

        protected virtual void UpdateBreakpointDependencies()
        {
            if (aspectRatioBreakpoints.Count == 0) {
                hasBreakpoints = false;
            }
        }

        public List<float> AddBreakpoint(float targetBreakpoint)
        {
            List<float> breakpointList = ResponsiveUtilsCore.AddBreakpointToResponsiveElement(this, targetBreakpoint);
            UpdateBreakpointDependencies();
            return breakpointList;
        }

        public string LogAddBreakpointMessage(float targetBreakpoint)
        {
            string message = "Added breakpoint of " + targetBreakpoint.ToString("F2") + " to " + this.ToString() + " " + this.GetType().Name;
            Debug.Log(message);
            return message;   
        }

        protected virtual void LogBreakpointWarning()
        {
            Debug.LogWarning("Please specify either 1.) target values for saving OR 2.) breakpoints and corresponding values on " + this.ToString());
        }
#endif

        private static bool IsPopulated(ComplexEventTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}