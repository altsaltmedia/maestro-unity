using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace AltSalt.Sequencing.Touch
{
    [CreateAssetMenu(menuName = "AltSalt/Sequencing/Touch Fork")]
    public class AxisModifier_TouchFork : JoinTools_Fork
    {
        [TitleGroup("Y North")]
        [SerializeField]
        [OnValueChanged(nameof(RefreshForkData))]
        private Sequence _yNorthBranch;
            
        private Sequence yNorthBranch => _yNorthBranch;

        [TitleGroup("Y North")]
        [SerializeField]
        [ReadOnly]
        private JoinTools_BranchKey _yNorthKey;

        private JoinTools_BranchKey yNorthKey
        {
            get => _yNorthKey;
            set => _yNorthKey = value;
        }

        [TitleGroup("Y South")]
        [SerializeField]
        [OnValueChanged(nameof(RefreshForkData))]
        private Sequence _ySouthBranch;
            
        private Sequence ySouthBranch => _ySouthBranch;
            
        [TitleGroup("Y South")]
        [SerializeField]
        [ReadOnly]
        private JoinTools_BranchKey _ySouthKey;

        private JoinTools_BranchKey ySouthKey
        {
            get => _ySouthKey;
            set => _ySouthKey = value;
        }

        [TitleGroup("X East")]
        [SerializeField]
        [OnValueChanged(nameof(RefreshForkData))]
        private Sequence _xEastBranch;

        private Sequence xEastBranch => _xEastBranch;
            
        [TitleGroup("X East")]
        [SerializeField]
        [ReadOnly]
        private JoinTools_BranchKey _xEastKey;

        private JoinTools_BranchKey xEastKey
        {
            get => _xEastKey;
            set => _xEastKey = value;
        }

        [TitleGroup("X West")]
        [SerializeField]
        [OnValueChanged(nameof(RefreshForkData))]
        private Sequence _xWestBranch;
            
        private Sequence xWestBranch => _xWestBranch;
            
        [TitleGroup("X West")]
        [SerializeField]
        [ReadOnly]
        private JoinTools_BranchKey _xWestKey;

        private JoinTools_BranchKey xWestKey
        {
            get => _xWestKey;
            set => _xWestKey = value;
        }

//        [ReadOnly]
//        [SerializeField]
//        [ValidateInput(nameof(IsPopulated), "A fork must contain at least three paths.")]
//        protected new List<ForkTools_BranchingPath> _branchingPaths = new List<ForkTools_BranchingPath>();
//            
//        public List<ForkTools_BranchingPath> branchingPaths
//        {
//            get => _branchingPaths;
//            set => _branchingPaths = value;
//        }
            
        private List<JoinTools_BranchingPath> RefreshForkData()
        {
            branchingPaths.Clear();

            if (yNorthKey == null) {
                yNorthKey = Utils.GetScriptableObject(nameof(VarDependencies.yNorthBranch)) as JoinTools_BranchKey;
            }
                
            if (yNorthBranch != null) {
                branchingPaths.Add(new JoinTools_BranchingPath(yNorthKey, yNorthBranch, true));
            }
                
            if (ySouthKey == null) {
                ySouthKey = Utils.GetScriptableObject(nameof(VarDependencies.ySouthBranch)) as JoinTools_BranchKey;
            }
                
            if (ySouthBranch != null) {
                branchingPaths.Add(new JoinTools_BranchingPath(ySouthKey, ySouthBranch));
            }
                
            if (xEastKey == null) {
                xEastKey = Utils.GetScriptableObject(nameof(VarDependencies.xEastBranch)) as JoinTools_BranchKey;
            }
                
            if (xEastBranch != null) {
                branchingPaths.Add(new JoinTools_BranchingPath(xEastKey, xEastBranch));
            }

            if (xWestKey == null) {
                xWestKey = Utils.GetScriptableObject(nameof(VarDependencies.xWestBranch)) as JoinTools_BranchKey;
            }
                
            if (xWestBranch != null) {
                branchingPaths.Add(new JoinTools_BranchingPath(xWestKey, xWestBranch, true));
            }

            return branchingPaths;
        }

        private static bool IsPopulated(List<JoinTools_BranchingPath> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}