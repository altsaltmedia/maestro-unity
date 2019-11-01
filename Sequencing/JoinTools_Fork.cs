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
        [ValidateInput(nameof(IsPopulated), "A fork must contain at least three paths.")]
        protected List<JoinTools_BranchingPath> _branchingPaths;

        public List<JoinTools_BranchingPath> branchingPaths
        {
            get => _branchingPaths;
            set => _branchingPaths = value;
        }
            
        private static bool IsPopulated(List<JoinTools_BranchingPath> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}