using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing
{
    [CreateAssetMenu(menuName = "AltSalt/Sequencing/Fork")]
    public class Fork : JoinerDestination
    {
        [SerializeField]
        
        private string _description;

        private string description => _description;

        [SerializeField]
        private bool _active = true;

        public bool active
        {
            get => _active;
            private set => _active = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated), "A fork must contain at least three paths.")]
        [DisableIf(nameof(readonlyBranchingPaths))]
        [PropertyOrder(5)]
        [TitleGroup("Branches")]
        [InfoBox("Remember: A fork MUST contain a definition for its origin branch, " +
                 "so that it can be disabled when advancing to a subsequent branch.", InfoMessageType.Warning)]
        protected List<BranchingPath> _branchingPaths;

        public List<BranchingPath> branchingPaths
        {
            get => _branchingPaths;
            set => _branchingPaths = value;
        }

        [SerializeField]
        [InfoBox("This value should be set dynamically via an input module")]
        private BranchingPath _destinationBranch;

        private BranchingPath destinationBranch
        {
            get => _destinationBranch;
            set => _destinationBranch = value;
        }

        protected virtual bool readonlyBranchingPaths => false;

        public void SetDestinationBranch(BranchKey branchKey)
        {
            BranchingPath branchingPath = branchingPaths.Find(x => x.branchKey == branchKey);
            destinationBranch = branchingPath;
        }
        
        public void SetDestinationBranch(Sequence sequence)
        {
            BranchingPath branchingPath = branchingPaths.Find(x => x.sequence == sequence);
            destinationBranch = branchingPath;
        }

        public void ActivateFork()
        {
            active = true;
        }
        
        public void DeactivateFork()
        {
            active = false;
        }

        public bool TryGetDestinationBranch(out BranchingPath branchingPath)
        {
            if (destinationBranch.sequence != null) {
                branchingPath = destinationBranch;
                return true;
            }
            
            branchingPath = null;
            return false;
        }
            
        private static bool IsPopulated(List<BranchingPath> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}