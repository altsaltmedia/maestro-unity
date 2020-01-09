using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ResponsiveVector3Behaviour : ResponsiveLerpToTargetBehaviour
    {
        [ShowInInspector]
        private bool _editMode;

        private bool editMode
        {
            get => _editMode;
            set => _editMode = value;
        }

        // initialPosition and targetPosition don't refresh in inspector for
        // some reason, but they work as intended
        [ShowIf(nameof(editMode))]
        [ShowInInspector]
        private Vector3 _initialValue;

        public Vector3 initialValue
        {
            get => _initialValue;
            set => _initialValue = value;
        }

        [ShowIf(nameof(editMode))]
        [ShowInInspector]
        private Vector3 _targetValue;

        public Vector3 targetValue
        {
            get => _targetValue;
            set => _targetValue = value;
        }

        [FormerlySerializedAs("breakpointInitialValue")]
        [SerializeField]
        private List<Vector3> _breakpointInitialValue = new List<Vector3>();

        public List<Vector3> breakpointInitialValue
        {
            get => _breakpointInitialValue;
            set => _breakpointInitialValue = value;
        }

        [FormerlySerializedAs("breakpointTargetValue")]
        [SerializeField]
        private List<Vector3> _breakpointTargetValue = new List<Vector3>();

        public List<Vector3> breakpointTargetValue
        {
            get => _breakpointTargetValue;
            set => _breakpointTargetValue = value;
        }

#if UNITY_EDITOR
        public override object SetInitialValueToTarget()
        {
            breakpointInitialValue.Clear();
            breakpointInitialValue.AddRange(breakpointTargetValue);
            return initialValue;
        }
        
        [ShowIf(nameof(editMode))]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SaveInitial()
        {
            SaveNewInitialValue(initialValue);
            editMode = false;
        }

        [ShowIf(nameof(editMode))]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SaveTarget()
        {
            SaveNewTargetValue(targetValue);
            editMode = false;
        }

        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();

            if (hasBreakpoints == true) {

                int initialCount = breakpointInitialValue.Count;
                Utils.ExpandList(breakpointInitialValue, aspectRatioBreakpoints.Count);
                for(int i = initialCount; i<breakpointInitialValue.Count; i++) {
                    breakpointInitialValue[i] = breakpointInitialValue[initialCount - 1];
                }

                int targetCount = breakpointTargetValue.Count;
                Utils.ExpandList(breakpointTargetValue, aspectRatioBreakpoints.Count);
                for (int i = targetCount; i < breakpointTargetValue.Count; i++) {
                    breakpointTargetValue[i] = breakpointTargetValue[initialCount - 1];
                }

            }
        }

        public Vector3 GetInitialValueAtBreakpoint(float targetAspectRatio)
        {
            if(aspectRatioBreakpoints.Count > 0) {
                int breakpointIndex = Utils.GetValueIndexInList(targetAspectRatio, aspectRatioBreakpoints);
                return breakpointInitialValue[breakpointIndex];
            } else {
                return breakpointInitialValue[0];
            }
        }

        public List<Vector3> SaveNewInitialValue(Vector3 targetValue)
        {
            if (hasBreakpoints == true) {
                int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio, aspectRatioBreakpoints);
                breakpointInitialValue[breakpointIndex] = targetValue;
            } else {
                breakpointInitialValue[0] = targetValue;
            }
            UpdateBreakpointDependencies();
            CallExecuteLayoutUpdate(directorObject);
            return breakpointInitialValue;
        }


        public Vector3 GetTargetValueAtBreakpoint(float targetAspectRatio)
        {
            if (aspectRatioBreakpoints.Count > 0) {
                int breakpointIndex = Utils.GetValueIndexInList(targetAspectRatio, aspectRatioBreakpoints);
                return breakpointTargetValue[breakpointIndex];
            } else {
                return breakpointTargetValue[0];
            }
        }

        public List<Vector3> SaveNewTargetValue(Vector3 targetValue)
        {
            if (hasBreakpoints == true) {
                int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio, aspectRatioBreakpoints);
                breakpointTargetValue[breakpointIndex] = targetValue;
            } else {
                breakpointTargetValue[0] = targetValue;
            }
            UpdateBreakpointDependencies();
            CallExecuteLayoutUpdate(directorObject);
            return breakpointTargetValue;
        }
#endif

        protected override void SetValue(int activeIndex)
        {
#if UNITY_EDITOR
            if (activeIndex != 0 &&
               (activeIndex >= breakpointInitialValue.Count ||
                activeIndex >= breakpointTargetValue.Count)) {
                LogBreakpointWarning();
                return;
            }
#endif
            if(editMode == false) {
                initialValue = breakpointInitialValue[activeIndex];
                targetValue = breakpointTargetValue[activeIndex];
            }
        }
    }
}
