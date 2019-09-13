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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt
{
    public class ResponsiveLerpToTargetBehaviour : PlayableBehaviour, IResponsive
    {
        [Required]
        [SerializeField]
        public AppSettings appSettings;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        public FloatReference sceneAspectRatio = new FloatReference();

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        ComplexEventTrigger responsiveElementEnable = new ComplexEventTrigger();

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        ComplexEventTrigger responsiveElementDisable = new ComplexEventTrigger();

        [SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(PopulateDefaultBreakpointValues))]
#endif
        protected bool hasBreakpoints;
        public bool HasBreakpoints {
            get {
                return hasBreakpoints;
            }
            set {
                hasBreakpoints = value;
            }
        }

        [SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(ResetResponsiveElementData))]
#endif
        int priority;
        public int Priority {
            get {
                return priority;
            }
        }

        [SerializeField]
        protected bool logElementOnLayoutUpdate;
        public bool LogElementOnLayoutUpdate {
            get {
                if (appSettings.logResponsiveElementActions.Value == true) {
                    return true;
                } else {
                    return logElementOnLayoutUpdate;
                }
            }
        }

        public EasingFunction.Ease ease = EasingFunction.Ease.EaseInOutQuad;

        [PropertySpace]
        [InfoBox("You should always have x+1 breakpoint values; e.g., for 1 breakpoint at 1.34, you must specify 2 breakpoint values - one for aspect ratios wider than 1.34, and another for aspect ratios narrower than or equal to 1.34.")]
        [InfoBox("Breakpoint examples: To target devices narrow than iPad (aspect ratio 1.33), specify a breakpoint of 1.4; to target narrower than iPhone (1.77), specify a breakpoint of 1.78.")]
#if UNITY_EDITOR
        [OnValueChanged(nameof(UpdateBreakpointDependencies))]
#endif
        public List<float> aspectRatioBreakpoints = new List<float>();
        public List<float> AspectRatioBreakpoints {
            get {
                return aspectRatioBreakpoints;
            }
        }

        public string Name {
            get {
                return this.ToString();
            }
        }

        public Scene ParentScene {
            get {
                return directorObject.scene;
            }
        }

        [HideInInspector]
        public double startTime;

        [HideInInspector]
        public double endTime;

        [HideInInspector]
        public TrackAsset parentTrack;

        [HideInInspector]
        public PlayableAsset clipAsset;

        [HideInInspector]
        public GameObject directorObject;

        [HideInInspector]
        public EasingFunction.Function easingFunction;

        public override void OnPlayableCreate(Playable playable)
        {
#if UNITY_EDITOR    
            if (sceneAspectRatio.Variable == null) {
                sceneAspectRatio.Variable = Utils.GetFloatVariable(nameof(VarDependencies.SceneAspectRatio));
            }
            
            UpdateBreakpointDependencies();
            responsiveElementEnable.ComplexEventTarget = Utils.GetComplexEvent(nameof(VarDependencies.ResponsiveElementEnable));
            responsiveElementDisable.ComplexEventTarget = Utils.GetComplexEvent(nameof(VarDependencies.ResponsiveElementDisable));
#endif
            base.OnGraphStart(playable);
            responsiveElementEnable.RaiseEvent(clipAsset, ParentScene.name, clipAsset.name, this);

            easingFunction = EasingFunction.GetEasingFunction(ease);
            CallExecuteLayoutUpdate(directorObject);
        }

        public void CallExecuteLayoutUpdate(UnityEngine.Object callingObject)
        {
            if (LogElementOnLayoutUpdate == true) {
                Debug.Log("CallExecuteLayoutUpdate triggered!");
                Debug.Log("Calling object : " + callingObject.name, callingObject);
                Debug.Log("Triggered timeline clip : " + Name, clipAsset);
                Debug.Log("Track : " + parentTrack.name, parentTrack);
                Debug.Log("--------------------------");
            }

            // TO DO - write functionality to hook into modify
            ExecuteResponsiveAction();
        }

        protected virtual void ExecuteResponsiveAction()
        {
            if (aspectRatioBreakpoints.Count == 0 || HasBreakpoints == false) {
                SetValue(0);
            } else {
                int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, aspectRatioBreakpoints);
                SetValue(breakpointIndex);
            }
        }

        protected virtual void SetValue(int activeIndex)
        {
            // Override this in child classes
        }

#if UNITY_EDITOR
        void ResetResponsiveElementData()
        {
            responsiveElementDisable.RaiseEvent(clipAsset, ParentScene.name, clipAsset.name, this);
            responsiveElementEnable.RaiseEvent(clipAsset, ParentScene.name, clipAsset.name, this);
        }

        protected void PopulateDefaultBreakpointValues()
        {
            PopulateDefaultBreakpoint();
            UpdateBreakpointDependencies();
        }

        void PopulateDefaultBreakpoint()
        {
            if (HasBreakpoints == true && aspectRatioBreakpoints.Count == 0) {
                decimal tempVal = Convert.ToDecimal(sceneAspectRatio.Value);
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