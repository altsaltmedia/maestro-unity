using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [TrackColor(0.245149f, 0.895372f, 0.5679245f)]
    [TrackClipType(typeof(SimpleEventTimelineTriggerClip))]
    public class SimpleEventTimelineTriggerTrack : TrackAsset
    {
        public void StoreClipProperties(GameObject go)
        {
            foreach (var clip in GetClips()) {
                var myAsset = clip.asset as SimpleEventTimelineTriggerClip;
                if (myAsset) {
                    myAsset.startTime = clip.start;
                    myAsset.endTime = clip.end;
                    myAsset.trackAssetConfig = go.GetComponent<TrackAssetConfig>();
                    myAsset.parentTrack = this;
                }
            }
        }

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            return ScriptPlayable<SimpleEventTimelineTriggerMixerBehaviour>.Create (graph, inputCount);
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