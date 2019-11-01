using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace AltSalt.Sequencing.Touch
{
    [CreateAssetMenu(menuName = "AltSalt/Sequencing/Touch Fork")]
    public class AxisModifier_TouchFork : JoinTools_Fork
    {
        [TitleGroup("Y Positive")]
        [SerializeField]
        [OnValueChanged(nameof(RefreshForkData))]
        private Sequence _yPositiveBranch;
            
        private Sequence yPositiveBranch => _yPositiveBranch;

        [TitleGroup("Y Positive")]
        [SerializeField]
        [ReadOnly]
        private JoinTools_BranchKey _yPositiveKey;

        private JoinTools_BranchKey yPositiveKey
        {
            get => _yPositiveKey;
            set => _yPositiveKey = value;
        }

        [TitleGroup("Y Negative")]
        [SerializeField]
        [OnValueChanged(nameof(RefreshForkData))]
        private Sequence _yNegativeBranch;
            
        private Sequence yNegativeBranch => _yNegativeBranch;
            
        [TitleGroup("Y Negative")]
        [SerializeField]
        [ReadOnly]
        private JoinTools_BranchKey _yNegativeKey;

        private JoinTools_BranchKey yNegativeKey
        {
            get => _yNegativeKey;
            set => _yNegativeKey = value;
        }

        [TitleGroup("X Positive")]
        [SerializeField]
        [OnValueChanged(nameof(RefreshForkData))]
        private Sequence _xPositiveBranch;

        private Sequence xPositiveBranch => _xPositiveBranch;
            
        [TitleGroup("X Positive")]
        [SerializeField]
        [ReadOnly]
        private JoinTools_BranchKey _xPositiveKey;

        private JoinTools_BranchKey xPositiveKey
        {
            get => _xPositiveKey;
            set => _xPositiveKey = value;
        }

        [TitleGroup("X Negative")]
        [SerializeField]
        [OnValueChanged(nameof(RefreshForkData))]
        private Sequence _xNegativeBranch;
            
        private Sequence xNegativeBranch => _xNegativeBranch;
            
        [TitleGroup("X Negative")]
        [SerializeField]
        [ReadOnly]
        private JoinTools_BranchKey _xNegativeKey;

        private JoinTools_BranchKey xNegativeKey
        {
            get => _xNegativeKey;
            set => _xNegativeKey = value;
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

            if (yPositiveKey == null) {
                yPositiveKey = Utils.GetScriptableObject(nameof(VarDependencies.yPositiveBranch)) as JoinTools_BranchKey;
            }
                
            if (yPositiveBranch != null) {
                branchingPaths.Add(new JoinTools_BranchingPath(yPositiveKey, yPositiveBranch));
            }
                
            if (yNegativeKey == null) {
                yNegativeKey = Utils.GetScriptableObject(nameof(VarDependencies.yNegativeBranch)) as JoinTools_BranchKey;
            }
                
            if (yNegativeBranch != null) {
                branchingPaths.Add(new JoinTools_BranchingPath(yNegativeKey, yNegativeBranch));
            }
                
            if (xPositiveKey == null) {
                xPositiveKey = Utils.GetScriptableObject(nameof(VarDependencies.xPositiveBranch)) as JoinTools_BranchKey;
            }
                
            if (xPositiveBranch != null) {
                branchingPaths.Add(new JoinTools_BranchingPath(xPositiveKey, xPositiveBranch));
            }

            if (xNegativeKey == null) {
                xNegativeKey = Utils.GetScriptableObject(nameof(VarDependencies.xNegativeBranch)) as JoinTools_BranchKey;
            }
                
            if (xNegativeBranch != null) {
                branchingPaths.Add(new JoinTools_BranchingPath(xNegativeKey, xNegativeBranch));
            }

            return branchingPaths;
        }

        private static bool IsPopulated(List<JoinTools_BranchingPath> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}