using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.745149f, 0.495372f, 0.8679245f)]
    [TrackClipType(typeof(ColorClip))]
    [TrackBindingType(typeof(ColorVariable))]
    public class LerpColorVarTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<LerpColorVarMixerBehaviour> trackPlayable = ScriptPlayable<LerpColorVarMixerBehaviour>.Create(graph, inputCount);
            LerpColorVarMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);

            return trackPlayable;
        }
        
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {

#if UNITY_EDITOR

            
            // SYNTAX FOR MARKING PROPERTIES TO NOT BE SAVED IN EDIT MODE, CAN ITERATE
            // THROUGH ALL PROPERTIES DYNAMICALLY OR USE DEBUG.LOG STATEMENT BELOW TO FIND
            // NAMES OF SERIALIZED PROPERTIES AND MARK THEM EXPLICITY

            //ColorVariable trackBinding = director.GetGenericBinding(this) as ColorVariable;
            //if (trackBinding == null)
            //    return;

            //var serializedObject = new UnityEditor.SerializedObject(trackBinding);
            //var iterator = serializedObject.GetIterator();

            //while (iterator.NextVisible(true)) {
            //    if (iterator.hasVisibleChildren)
            //        continue;

            //    Debug.Log(iterator.propertyPath);
            //    driver.AddFromName(iterator.propertyPath);
            //}

#endif

            base.GatherProperties(director, driver);
        }
    }
    
}