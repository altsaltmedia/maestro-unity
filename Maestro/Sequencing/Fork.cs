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
        [ValidateInput(nameof(IsPopulated), "A fork must contain at least three paths.")]
        [DisableIf(nameof(readonlyBranchingPaths))]
        [PropertyOrder(5)]
        [TitleGroup("Branches")]
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

        public BranchingPath SetDestinationBranch(BranchKey branchKey)
        {
            BranchingPath branchingPath = branchingPaths.Find(x => x.branchKey == branchKey);
            destinationBranch = branchingPath;
            return branchingPath;
        }

        public BranchingPath GetDestinationBranch()
        {
            return destinationBranch;
        }
            
        private static bool IsPopulated(List<BranchingPath> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}