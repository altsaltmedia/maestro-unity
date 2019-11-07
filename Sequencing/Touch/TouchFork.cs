using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace AltSalt.Sequencing.Touch
{
    [CreateAssetMenu(menuName = "AltSalt/Sequencing/Touch Fork")]
    public class TouchFork : Fork
    {
        [TitleGroup("Y North")]
        [SerializeField]
        [OnValueChanged(nameof(RefreshForkData))]
        private Sequence _yNorthBranch;

        private Sequence yNorthBranch => _yNorthBranch;
        
        [TitleGroup("Y North")]
        [SerializeField]
        [LabelText("Invert")]
        [OnValueChanged(nameof(RefreshForkData))]
        private bool _invertYNorth;

        private bool invertYNorth => _invertYNorth;

        [TitleGroup("Y North")]
        [SerializeField]
        [ReadOnly]
        private BranchKey _yNorthKey;

        private BranchKey yNorthKey
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
        [LabelText("Invert")]
        [OnValueChanged(nameof(RefreshForkData))]
        private bool _invertYSouth;

        private bool invertYSouth => _invertYSouth;
            
        [TitleGroup("Y South")]
        [SerializeField]
        [ReadOnly]
        private BranchKey _ySouthKey;

        private BranchKey ySouthKey
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
        [LabelText("Invert")]
        [OnValueChanged(nameof(RefreshForkData))]
        private bool _invertXEast;

        private bool invertXEast => _invertXEast;
            
        [TitleGroup("X East")]
        [SerializeField]
        [ReadOnly]
        private BranchKey _xEastKey;

        private BranchKey xEastKey
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
        [LabelText("Invert")]
        [OnValueChanged(nameof(RefreshForkData))]
        private bool _invertXWest;

        private bool invertXWest => _invertXWest;
            
        [TitleGroup("X West")]
        [SerializeField]
        [ReadOnly]
        private BranchKey _xWestKey;

        private BranchKey xWestKey
        {
            get => _xWestKey;
            set => _xWestKey = value;
        }
        
        protected override bool readonlyBranchingPaths => true;

        private List<BranchingPath> RefreshForkData()
        {
            branchingPaths.Clear();

#if UNITY_EDITOR
            if (yNorthKey == null) {
                yNorthKey = Utils.GetScriptableObject(nameof(VarDependencies.yNorthBranch)) as BranchKey;
            }
                
            if (yNorthBranch != null) {
                branchingPaths.Add(new BranchingPath(yNorthKey, yNorthBranch, invertYNorth));
            }
                
            if (ySouthKey == null) {
                ySouthKey = Utils.GetScriptableObject(nameof(VarDependencies.ySouthBranch)) as BranchKey;
            }
                
            if (ySouthBranch != null) {
                branchingPaths.Add(new BranchingPath(ySouthKey, ySouthBranch, invertYSouth));
            }
                
            if (xEastKey == null) {
                xEastKey = Utils.GetScriptableObject(nameof(VarDependencies.xEastBranch)) as BranchKey;
            }
                
            if (xEastBranch != null) {
                branchingPaths.Add(new BranchingPath(xEastKey, xEastBranch, invertXEast));
            }

            if (xWestKey == null) {
                xWestKey = Utils.GetScriptableObject(nameof(VarDependencies.xWestBranch)) as BranchKey;
            }
                
            if (xWestBranch != null) {
                branchingPaths.Add(new BranchingPath(xWestKey, xWestBranch, invertXWest));
            }
#endif

            return branchingPaths;
        }

        private static bool IsPopulated(List<BranchingPath> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}