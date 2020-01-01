using System;
using AltSalt.Maestro.Animation;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class GameObjectReferenceClip : LerpToTargetClip
    {
        [SerializeField]
        private GameObjectReferenceBehaviour _template = new GameObjectReferenceBehaviour ();

        public GameObjectReferenceBehaviour template
        {
            get => _template;
            set => _template = value;
        }

        //This allows you to use GameObjects in your Scene
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

            var playable = ScriptPlayable<GameObjectReferenceBehaviour>.Create (graph, template);
            return playable;
        }
    }
}