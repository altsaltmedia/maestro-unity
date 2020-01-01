using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class RectTransformRelativePosClip : GameObjectReferenceClip
    {
        [SerializeField]
        [FormerlySerializedAs("template")]
        private RectTransformRelativePosBehaviour _childTemplate = new RectTransformRelativePosBehaviour ();

        private RectTransformRelativePosBehaviour childTemplate
        {
            get => _childTemplate;
            set => _childTemplate = value;
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            childTemplate.startTime = startTime;
            childTemplate.endTime = endTime;
            childTemplate.originReferenceObject = originReferenceObject.Resolve(graph.GetResolver());
            childTemplate.targetReferenceObject = targetReferenceObject.Resolve(graph.GetResolver());

            var playable = ScriptPlayable<RectTransformRelativePosBehaviour>.Create (graph, childTemplate);
            return playable;
        }
    }   
}