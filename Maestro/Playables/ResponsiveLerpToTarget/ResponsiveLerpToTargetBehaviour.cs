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
    public abstract class ResponsiveLerpToTargetBehaviour : LerpToTargetBehaviour, IResponsive
    {
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        public FloatReference sceneAspectRatio = new FloatReference();

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventTrigger _responsiveElementEnable = new ComplexEventTrigger();

        private ComplexEventTrigger responsiveElementEnable
        {
            get => _responsiveElementEnable;
            set => _responsiveElementEnable = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventTrigger _responsiveElementDisable = new ComplexEventTrigger();

        private ComplexEventTrigger responsiveElementDisable
        {
            get => _responsiveElementDisable;
            set => _responsiveElementDisable = value;
        }

#if UNITY_EDITOR
        [OnValueChanged(nameof(PopulateDefaultBreakpointValues))]
#endif
        [FormerlySerializedAs("hasBreakpoints"),SerializeField]
        protected bool _hasBreakpoints;
        
        public bool hasBreakpoints {
            get => _hasBreakpoints;
            set => _hasBreakpoints = value;
        }

        [SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(ResetResponsiveElementData))]
#endif
        private int _priority;
        
        public int priority {
            get => _priority;
            set => _priority = value;
        }

        protected bool _logElementOnLayoutUpdate;
        
        public bool logElementOnLayoutUpdate {
            get
            {
                return false;
                if (appSettings.logResponsiveElementActions.Value == true) {
                    return true;
                } else {
                    return _logElementOnLayoutUpdate;
                }
            }
        }

        [FormerlySerializedAs("ease")]
        public EasingFunction.Ease responsiveEase = EasingFunction.Ease.EaseInOutQuad;

        [FormerlySerializedAs("aspectRatioBreakpoints"),PropertySpace]
        [InfoBox("You should always have x+1 breakpoint values; e.g., for 1 breakpoint at 1.34, you must specify 2 breakpoint values - one for aspect ratios wider than 1.34, and another for aspect ratios narrower than or equal to 1.34.")]
        [InfoBox("Breakpoint examples: To target devices narrow than iPad (aspect ratio 1.33), specify a breakpoint of 1.4; to target narrower than iPhone (1.77), specify a breakpoint of 1.78.")]
#if UNITY_EDITOR
        [OnValueChanged(nameof(UpdateBreakpointDependencies))]
#endif
        public List<float> _aspectRatioBreakpoints = new List<float>();
        
        public List<float> aspectRatioBreakpoints {
            get {
                return _aspectRatioBreakpoints;
            }
        }

        public string elementName {
            get {
                return this.ToString();
            }
        }

        public Scene parentScene {
            get {
                return directorObject.scene;
            }
        }
        
        public override void OnPlayableCreate(Playable playable)
        {
#if UNITY_EDITOR    
            if (sceneAspectRatio.Variable == null) {
                sceneAspectRatio.Variable = Utils.GetFloatVariable(nameof(VarDependencies.SceneAspectRatio));
            }
            
            UpdateBreakpointDependencies();

            if (responsiveElementEnable.ComplexEventTarget == null) {
                responsiveElementEnable.ComplexEventTarget = Utils.GetComplexEvent(nameof(VarDependencies.ResponsiveElementEnable));
            }

            if (responsiveElementDisable.ComplexEventTarget == null) {
                responsiveElementDisable.ComplexEventTarget = Utils.GetComplexEvent(nameof(VarDependencies.ResponsiveElementDisable));
            }
#endif
            base.OnPlayableCreate(playable);
            //responsiveElementEnable.RaiseEvent(clipAsset, parentScene.name, clipAsset.name, this);

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
                int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, aspectRatioBreakpoints);
                SetValue(breakpointIndex);
            }
        }

        protected abstract void SetValue(int activeIndex);

#if UNITY_EDITOR
        void ResetResponsiveElementData()
        {
            responsiveElementDisable.RaiseEvent(clipAsset, parentScene.name, clipAsset.name, this);
            responsiveElementEnable.RaiseEvent(clipAsset, parentScene.name, clipAsset.name, this);
        }

        protected void PopulateDefaultBreakpointValues()
        {
            PopulateDefaultBreakpoint();
            UpdateBreakpointDependencies();
        }

        void PopulateDefaultBreakpoint()
        {
            if (hasBreakpoints == true && _aspectRatioBreakpoints.Count == 0) {
                decimal tempVal = Convert.ToDecimal(sceneAspectRatio.Value);
                tempVal = Math.Round(tempVal, 2);
                _aspectRatioBreakpoints.Add((float)tempVal + .01f);
            }
        }

        protected virtual void UpdateBreakpointDependencies()
        {
            if (_aspectRatioBreakpoints.Count == 0) {
                _hasBreakpoints = false;
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