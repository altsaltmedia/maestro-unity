using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveScale : ResponsiveRectTransform, IResponsiveSaveable
    {

        public List<Vector2> breakpointLocalScale = new List<Vector2>();

#if UNITY_EDITOR
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        public void SaveValue()
        {
            if (_aspectRatioBreakpoints.Count < 1) {
                Debug.LogError("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int targetBreakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, _aspectRatioBreakpoints);

            Utils.ExpandList(breakpointLocalScale, targetBreakpointIndex);
            breakpointLocalScale[targetBreakpointIndex] = rectTransform.localScale;
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            SetValue(breakpointIndex);
        }

        public void SetValue(int activeIndex)
        {
#if UNITY_EDITOR
            if (activeIndex >= breakpointLocalScale.Count) {
                LogBreakpointError();
                return;
            }
            Undo.RegisterCompleteObjectUndo(rectTransform, "set responsive scale");
#endif
            rectTransform.localScale = breakpointLocalScale[activeIndex];
        }

    }
    
}