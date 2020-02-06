using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.245149f, 0.895372f, 0.5679245f)]
    [TrackClipType(typeof(FloatClip))]
    [TrackBindingType(typeof(FloatVariable))]
    public class LerpFloatVarTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<LerpFloatVarMixerBehaviour> trackPlayable = ScriptPlayable<LerpFloatVarMixerBehaviour>.Create(graph, inputCount);
            LerpFloatVarMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }
        
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {

#if UNITY_EDITOR

            // SYNTAX FOR MARKING PROPERTIES TO NOT BE SAVED IN EDIT MODE, CAN ITERATE
            // THROUGH ALL PROPERTIES DYNAMICALLY OR USE DEBUG.LOG STATEMENT BELOW TO FIND
            // NAMES OF SERIALIZED PROPERTIES AND MARK THEM EXPLICITY

            //driver.AddFromName("Value");

            //FloatVariable trackBinding = director.GetGenericBinding(this) as FloatVariable;
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