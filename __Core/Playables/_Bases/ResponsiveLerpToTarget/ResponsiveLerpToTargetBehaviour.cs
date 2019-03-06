/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class ResponsiveLerpToTargetBehaviour : PlayableBehaviour {
    
        [ValidateInput("IsPopulated")]
        public FloatReference aspectRatio = new FloatReference();

        [PropertySpace]
        [InfoBox("You should always have x+1 breakpoint values; e.g., for 1 breakpoint at 1.34, you must specify 2 breakpoint values - one for aspect ratios wider than 1.34, and another for aspect ratios narrower than or equal to 1.34.")]
        [InfoBox("Breakpoint examples: To target devices narrow than iPad (aspect ratio 1.33), specify a breakpoint of 1.4; to target narrower than iPhone (1.77), specify a breakpoint of 1.78.")]
        public List<float> aspectRatioBreakpoints = new List<float>();

        public EasingFunction.Ease ease = EasingFunction.Ease.EaseInOutQuad;
        
        [HideInInspector]
        public double startTime;
        
        [HideInInspector]
        public double endTime;
        
        [HideInInspector]
        public EasingFunction.Function easingFunction;

        public override void OnGraphStart(Playable playable)
        {
#if UNITY_EDITOR
            if(aspectRatio.Variable == null) {
                aspectRatio.Variable = Utils.GetAspectRatio();
            }
#endif
            base.OnGraphStart(playable);
            easingFunction = EasingFunction.GetEasingFunction(ease);
            ExecuteResponsiveAction();
        }

        public virtual void ExecuteResponsiveAction() {
            // Override this in child classes to define custom responsive actions
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}