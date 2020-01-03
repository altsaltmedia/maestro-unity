using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class RectTransformRelativePosClip : LerpToTargetClip
    {
        [FormerlySerializedAs("_childTemplate")]
        [FormerlySerializedAs("template")]
        [SerializeField]
        private RectTransformRelativePosBehaviour _template = new RectTransformRelativePosBehaviour ();

        private RectTransformRelativePosBehaviour template
        {
            get => _template;
            set => _template = value;
        }
        
        public override LerpToTargetBehaviour templateReference => template;
        
        [SerializeField]
        [FormerlySerializedAs("originReferenceObject")]
        private ExposedReference<GameObject> _originReferenceObject;

        public ExposedReference<GameObject> originReferenceObject
        {
            get => _originReferenceObject;
            set => _originReferenceObject = value;
        }

        [SerializeField]
        [FormerlySerializedAs("targetReferenceObject")]
        private ExposedReference<GameObject> _targetReferenceObject;

        public ExposedReference<GameObject> targetReferenceObject
        {
            get => _targetReferenceObject;
            set => _targetReferenceObject = value;
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.originReferenceObject = originReferenceObject.Resolve(graph.GetResolver());
            template.targetReferenceObject = targetReferenceObject.Resolve(graph.GetResolver());

            var playable = ScriptPlayable<RectTransformRelativePosBehaviour>.Create (graph, template);
            return playable;
        }
    }   
}