using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Audio;

namespace AltSalt.Maestro
{
    [TrackColor(0.545149f, 0.895372f, 0.2679245f)]
    [TrackClipType(typeof(AudioLerpSnapshotClip))]
    [TrackBindingType(typeof(AudioMixer))]
    public class AudioLerpSnapshotTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            return ScriptPlayable<AudioLerpSnapshotMixerBehaviour>.Create(graph, inputCount);
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