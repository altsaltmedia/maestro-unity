using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Sequencing.Touch
{
    public class ForkSwitchClip : LerpToTargetClip
    {
        private ForkSwitchBehaviour _template = new ForkSwitchBehaviour();

        public ForkSwitchBehaviour template
        {
            get => _template;
            set => _template = value;
        }
        
        [SerializeField]
        private List<BranchingPath> _branchingPaths;

        public List<BranchingPath> branchingPaths
        {
            get => _branchingPaths;
            set => _branchingPaths = value;
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.midPoint = (float)(endTime + startTime) / 2f;

            template.branchingPaths = branchingPaths;
            
            var playable = ScriptPlayable<ForkSwitchBehaviour>.Create(graph, template);
            return playable;
        }
    }
}