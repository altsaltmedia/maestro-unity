using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.1981132f, 0.5f, 0.1065063f)]
    [TrackClipType(typeof(ResponsiveVector3Clip))]
    [TrackBindingType(typeof(Transform))]
    public class TransformRotationTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<TransformRotationMixerBehaviour> trackPlayable = ScriptPlayable<TransformRotationMixerBehaviour>.Create(graph, inputCount);
            TransformRotationMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            var comp = director.GetGenericBinding(this) as Transform;
            if (comp == null)
                return;

            driver.AddFromName<Transform>(comp.gameObject, "m_LocalRotation.w");
            driver.AddFromName<Transform>(comp.gameObject, "m_LocalRotation.x");
            driver.AddFromName<Transform>(comp.gameObject, "m_LocalRotation.y");
            driver.AddFromName<Transform>(comp.gameObject, "m_LocalRotation.z");
#endif
            base.GatherProperties(director, driver);
        }
    }   
}