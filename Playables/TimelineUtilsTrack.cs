using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace AltSalt.Maestro
{
    [TrackColor(0.245149f, 0.895372f, 0.5679245f)]
    [TrackClipType(typeof(TimelineUtilsClip))]
    public class TimelineUtilsTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject gameObject, int inputCount)
        {
            StoreClipProperties(gameObject);

            ScriptPlayable<TimelineUtilsMixerBehaviour> trackPlayable = ScriptPlayable<TimelineUtilsMixerBehaviour>.Create(graph, inputCount);
            TimelineUtilsMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(gameObject, behaviour);
#if UNITY_EDITOR
            if(gameObject.TryGetComponent(typeof(TrackAssetConfig), out Component component) == false) {
                gameObject.AddComponent(typeof(TrackAssetConfig));
            }
            
            TrackAssetConfig trackAssetConfig = component as TrackAssetConfig;
            behaviour.trackAssetConfig = trackAssetConfig;
            
            if (Application.isPlaying == false && TimelineEditor.inspectedDirector != null) {
                TimelineEditor.inspectedDirector.time = trackAssetConfig.timelineDebugTime;
            }
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