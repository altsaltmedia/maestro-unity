using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro
{
    public static class ResponsiveUtilsCore
    {
        public class DynamicElementSort : Comparer<IDynamicLayoutElement>
        {
            public override int Compare(IDynamicLayoutElement x, IDynamicLayoutElement y)
            {
                return x.priority.CompareTo(y.priority);
            }
        }

#if UNITY_EDITOR
        public static List<float> AddBreakpointToResponsiveElement(IResponsiveBreakpoints targetElement, float targetBreakpoint)
        {
            List<float> aspectRatioBreakpoints = targetElement.aspectRatioBreakpoints;
            targetElement.hasBreakpoints = true;

            if (aspectRatioBreakpoints.Contains(targetBreakpoint)) {
                EditorUtility.DisplayDialog("Breakpoint already exists", "The breakpoint " + targetBreakpoint.ToString("F2") + " already exists on " + targetElement.elementName, "Okay");
                return aspectRatioBreakpoints;
            }

            if (aspectRatioBreakpoints.Count == 0) {
                targetElement.LogAddBreakpointMessage(targetBreakpoint);
                aspectRatioBreakpoints.Add(targetBreakpoint);
                return aspectRatioBreakpoints;
            }

            for (int i = 0; i < aspectRatioBreakpoints.Count; i++) {

                if (targetBreakpoint < aspectRatioBreakpoints[i]) {
                    aspectRatioBreakpoints.Insert(i, targetBreakpoint);
                    break;
                } else if (targetBreakpoint > aspectRatioBreakpoints[i] && aspectRatioBreakpoints.Count == i + 1) {
                    aspectRatioBreakpoints.Insert(i + 1, targetBreakpoint);
                    break;
                }
            }

            targetElement.LogAddBreakpointMessage(targetBreakpoint);
            return aspectRatioBreakpoints;
        }

        // =================================== //
        //     Utils for Responsive Clips      //
        // =================================== //

        // Due to the way Unity instantiates ScriptPlayables, there's no way that I've found to get around strong casts
        // to the behaviours in question to get the properties we need. The following functions are an admittedly bloated
        // way to get those values. Time wasted trying to improve this: 5 hours.

        public static IResponsiveBreakpoints GetResponsiveElementFromClipAsset(ResponsiveLerpToTargetClip responsiveLerpToTargetClip)
        {
            switch (responsiveLerpToTargetClip.GetType().Name) {

                case nameof(ResponsiveVector3Clip): {
                        ResponsiveVector3Clip clipAsset = responsiveLerpToTargetClip as ResponsiveVector3Clip;
                        return clipAsset.template;
                    }

                case nameof(ResponsiveFloatClip): {
                        ResponsiveFloatClip clipAsset = responsiveLerpToTargetClip as ResponsiveFloatClip;
                        return clipAsset.template;
                    }
            }

            return null;
        }

        public static PlayableAsset GetClipAssetFromResponsiveBehaviour(ResponsiveLerpToTargetBehaviour responsiveBehaviour)
        {
            switch (responsiveBehaviour.GetType().Name) {

                case nameof(ResponsiveVector3Behaviour): {
                        ResponsiveVector3Behaviour behaviourInstance = responsiveBehaviour as ResponsiveVector3Behaviour;
                        return behaviourInstance.clipAsset;
                    }

                case nameof(ResponsiveFloatBehaviour): {
                        ResponsiveFloatBehaviour behaviourInstance = responsiveBehaviour as ResponsiveFloatBehaviour;
                        return behaviourInstance.clipAsset;
                    }
            }

            return null;
        }

        public static TrackAsset GetParentTrackFromResponsiveBehaviour(ResponsiveLerpToTargetBehaviour responsiveBehaviour)
        {
            switch (responsiveBehaviour.GetType().Name) {

                case nameof(ResponsiveVector3Behaviour): {
                        ResponsiveVector3Behaviour behaviourInstance = responsiveBehaviour as ResponsiveVector3Behaviour;
                        return behaviourInstance.parentTrack;
                    }

                case nameof(ResponsiveFloatBehaviour): {
                        ResponsiveFloatBehaviour behaviourInstance = responsiveBehaviour as ResponsiveFloatBehaviour;
                        return behaviourInstance.parentTrack;
                    }
            }

            return null;
        }
#endif

    }
}
