﻿using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace AltSalt.Maestro
{
    [TrackColor(0.245149f, 0.895372f, 0.5679245f)]
    [TrackClipType(typeof(DebugTimelineClip))]
    public class DebugTimelineTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);

            ScriptPlayable<DebugTimelineMixerBehaviour> trackPlayable = ScriptPlayable<DebugTimelineMixerBehaviour>.Create(graph, inputCount);
            DebugTimelineMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
#if UNITY_EDITOR
            behaviour.timelineCurrentTime.Variable = Utils.GetFloatVariable(nameof(VarDependencies.TimelineCurrentTime));
            if (Application.isPlaying == false) {
                TimelineEditor.inspectedDirector.time = behaviour.timelineCurrentTime.Value;
            }
            behaviour.onGraphStart.SimpleEventTarget = Utils.GetSimpleEvent(nameof(VarDependencies.OnGraphStart));
#endif
            return trackPlayable;
        }
        
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            
            //#if UNITY_EDITOR
            
            //        // SYNTAX FOR MARKING PROPERTIES TO NOT BE SAVED IN EDIT MODE, CAN ITERATE
            //        // THROUGH ALL PROPERTIES DYNAMICALLY OR USE DEBUG.LOG STATEMENT BELOW TO FIND
            //        // NAMES OF SERIALIZED PROPERTIES AND MARK THEM EXPLICITY
            
            //        ColorVariable trackBinding = director.GetGenericBinding(this) as ColorVariable;
            //         if (trackBinding == null)
            //            return;
            
            //        var serializedObject = new UnityEditor.SerializedObject(trackBinding);
            //        var iterator = serializedObject.GetIterator();
            
            //        while (iterator.NextVisible(true)) {
            //            if (iterator.hasVisibleChildren)
            //                continue;
            
            //            Debug.Log(iterator.propertyPath);
            //            AnimationClip animationClip = new AnimationClip();
            //            driver.AddObjectProperties(trackBinding, animationClip);
            //        }
            
            //#endif
            
            base.GatherProperties(director, driver);
        }
    }
    
}