using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing
{
    [CreateAssetMenu(menuName = "AltSalt/Sequencing/Fork")]
    public class JoinTools_Fork : ScriptableObject, JoinTools_IFork
    {
        [SerializeField]
        private string _description;

        private string description => _description;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated), "A fork must contain at least three paths.")]
        protected List<JoinTools_BranchingPath> _branchingPaths;

        public List<JoinTools_BranchingPath> branchingPaths
        {
            get => _branchingPaths;
            set => _branchingPaths = value;
        }

        [SerializeField]
        private JoinTools_BranchingPath _destinationBranch;

        public JoinTools_BranchingPath destinationBranch
        {
            get => _destinationBranch;
            private set => _destinationBranch = value;
        }

        public JoinTools_BranchingPath SetDestinationBranch(JoinTools_BranchingPath targetBranchingPath)
        {
            JoinTools_BranchingPath branchingPath = branchingPaths.Find(x => x.sequence == targetBranchingPath.sequence);
            destinationBranch = branchingPath;
            return branchingPath;
        }

        public JoinTools_BranchingPath GetDestinationBranch()
        {
            return destinationBranch;
        }
            
        private static bool IsPopulated(List<JoinTools_BranchingPath> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}