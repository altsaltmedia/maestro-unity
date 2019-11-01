using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Serialization;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSalt.Sequencing.Touch
{

    [ExecuteInEditMode]
    public partial class AxisModifier : Touch_Module
    {
        [ShowInInspector]
        private List<BaseAxisSwitchLegacy2> _switchData = new List<BaseAxisSwitchLegacy2>();

        private List<BaseAxisSwitchLegacy2> switchData
        {
            get => _switchData;
            set => _switchData = value;
        }
        
        [SerializeField]
        private TouchExtentsCollection _touchExtentsCollection = new TouchExtentsCollection();

        private TouchExtentsCollection touchExtentsCollection
        {
            get => _touchExtentsCollection;
            set => _touchExtentsCollection = value;
        }

        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        [SerializeField]
        private ComplexEventTrigger _convertMomentum;

        public ComplexEventTrigger convertMomentum
        {
            get => _convertMomentum;
            set => _convertMomentum = value;
        }

        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        [SerializeField]
        private FloatReference _resetSpread;

        public float resetSpread
        {
            get => _resetSpread.Value;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        [SerializeField]
        private FloatReference _swipeTransitionSpread;

        public float swipeTransitionSpread
        {
            get => _swipeTransitionSpread.Value;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        [SerializeField]
        private FloatReference _momentumTransitionSpread;

        public float momentumTransitionSpread
        {
            get => _momentumTransitionSpread.Value;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        [SerializeField]
        private FloatReference _invertTransitionSpread;

        public float invertTransitionSpread
        {
            get => _invertTransitionSpread.Value;
        }
        
        [TitleGroup("Branch Keys")]
        [SerializeField]
        [ReadOnly]
        [Required]
        private JoinTools_BranchKey _yPositiveKey;

        public JoinTools_BranchKey yPositiveKey
        {
            get => _yPositiveKey;
            set => _yPositiveKey = value;
        }
        
        [TitleGroup("Branch Keys")]
        [SerializeField]
        [ReadOnly]
        [Required]
        private JoinTools_BranchKey _yNegativeKey;

        public JoinTools_BranchKey yNegativeKey
        {
            get => _yNegativeKey;
            set => _yNegativeKey = value;
        }

        [TitleGroup("Branch Keys")]
        [SerializeField]
        [ReadOnly]
        [Required]
        private JoinTools_BranchKey _xPositiveKey;

        public JoinTools_BranchKey xPositiveKey
        {
            get => _xPositiveKey;
            set => _xPositiveKey = value;
        }

        [TitleGroup("Branch Keys")]
        [SerializeField]
        [ReadOnly]
        [Required]
        private JoinTools_BranchKey _xNegativeKey;

        public JoinTools_BranchKey xNegativeKey
        {
            get => _xNegativeKey;
            set => _xNegativeKey = value;
        }

#if UNITY_EDITOR
        
        public void ConfigureData()
        {
            PopulateBranchKeys();
            touchExtentsCollection.Clear();

            for (int i = 0; i < touchController.touchDataList.Count; i++)
            {
                var touchData = touchController.touchDataList[i];
                
                Input_Track inputConfigTrack = touchData.inputConfigTrack;
                if (inputConfigTrack == null) continue;
                
                IEnumerable<IMarker> markers = inputConfigTrack.GetMarkers().OrderBy(s => s.time);

                List<AxisModifier_TouchExtents> rawExtents = CreateExtentsList(this, touchData, markers);
                List<AxisModifier_TouchExtents> configuredExtents = ConfigureAxisExtents(rawExtents);
                
                if (touchExtentsCollection.ContainsKey(touchData.sequence.sequenceConfig.masterSequence) == false) {
                    touchExtentsCollection.Add(touchData.sequence.sequenceConfig.masterSequence, configuredExtents);
                } else {
                    touchExtentsCollection[touchData.sequence.sequenceConfig.masterSequence].AddRange(configuredExtents);
                }
            }
            
            EditorUtility.SetDirty(this);
        }

        private void PopulateBranchKeys()
        {
            if (yPositiveKey == null) {
                yPositiveKey = Utils.GetScriptableObject(nameof(VarDependencies.yPositiveBranch)) as JoinTools_BranchKey;
            }

            if (yNegativeKey == null) {
                yNegativeKey = Utils.GetScriptableObject(nameof(VarDependencies.yNegativeBranch)) as JoinTools_BranchKey;
            }

            if (xPositiveKey == null) {
                xPositiveKey = Utils.GetScriptableObject(nameof(VarDependencies.xPositiveBranch)) as JoinTools_BranchKey;
            }

            if (xNegativeKey == null) {
                xNegativeKey = Utils.GetScriptableObject(nameof(VarDependencies.xNegativeBranch)) as JoinTools_BranchKey;
            }
        }

        private static List<AxisModifier_TouchExtents> CreateExtentsList(AxisModifier axisModifier, Touch_Data touchData,
            IEnumerable<IMarker> markers)
        {
            List<AxisModifier_TouchExtents> axisExtentsData = new List<AxisModifier_TouchExtents>();
            
            foreach (IMarker marker in markers)
            {
                if (marker is AxisModifier_AxisMarker axisMarker) {
                    axisExtentsData.Add(new AxisModifier_AxisExtents(axisModifier, touchData, axisMarker));
                }
            }

            if (GetForkExtents(axisModifier, touchData.sequence, touchData, out var forkExtents)) {
                axisExtentsData.Add(forkExtents);
            }
            
            axisExtentsData.Sort(new AxisExtentsSort());
            return axisExtentsData;
        }

        private static bool GetForkExtents(AxisModifier axisModifier, Sequence sequence, Touch_Data touchData, out AxisModifier_ForkExtents touchForkExtents)
        {
            touchForkExtents = null;
            List<JoinTools_ForkJoinData> forkDataList = axisModifier.touchController.joinTools.forkDataList; 
            
            for (int i = 0; i < forkDataList.Count; i++) {
                if (forkDataList[i].sequence == sequence) {
                    touchForkExtents = new AxisModifier_ForkExtents(axisModifier, touchData, forkDataList[i]);
                    return true;
                }
            }

            return false;
        }
        
        
        private class AxisExtentsSort : Comparer<AxisModifier_TouchExtents>
        {
            public override int Compare(AxisModifier_TouchExtents x, AxisModifier_TouchExtents y)
            {
                switch (x) {
                    case AxisModifier_AxisExtents x1 when y is AxisModifier_AxisExtents y1:
                        return x1.markerMasterTime.CompareTo(y1.markerMasterTime);
                    case AxisModifier_AxisExtents x2 when y is AxisModifier_ForkExtents y2:
                        return x2.markerMasterTime.CompareTo(y2.startTime);
                    case AxisModifier_ForkExtents x3 when y is AxisModifier_AxisExtents y3:
                        return x3.startTime.CompareTo(y3.markerMasterTime);
                }
                
                throw new System.Exception("Unable to sort axis extents");
            }
        }

        // Whereas we can populate Fork Extents with all of their data upon creation, we need to
        // know the order of Axis Extents first to set up the transitions correctly.
        // Given an ordered list, we then populate the adjacent extents, which allow us
        // to define start times, end times, and transition times between the axis intervals 
        private static List<AxisModifier_TouchExtents> ConfigureAxisExtents(List<AxisModifier_TouchExtents> touchExtentsData)
        {
            for (int j = 0; j < touchExtentsData.Count; j++) {

                if (touchExtentsData[j] is AxisModifier_AxisExtents axisExtents) {
                    
                    if (touchExtentsData.Count == 1) {
                        axisExtents.Configure(null, null);
                        break;
                    }
                        
                    if (j == 0) {
                        axisExtents.Configure(null, touchExtentsData[j + 1]);
                    }
                    else if (j == touchExtentsData.Count - 1)  {
                        axisExtents.Configure(touchExtentsData[j - 1], null);
                    }
                    else {
                        axisExtents.Configure( touchExtentsData[j - 1],  touchExtentsData[j + 1]);
                    }
                    
                }
            }

            return touchExtentsData;
        }
#endif
        
        public virtual void Update()
        {
            foreach (var touchExtents in touchExtentsCollection) {
                
                double masterTime = touchExtents.Key.elapsedTime;

                if (AxisModifier_TouchExtents.TimeWithinExtents(masterTime, touchExtents.Value, out var currentExtents)) {
                    
                    if (currentExtents is AxisModifier_AxisExtents axisExtents) {
                        AxisModifier_SimpleSwitch.ActivateSwitch(masterTime, axisExtents);
                    } else if (currentExtents is AxisModifier_ForkExtents forkExtents) {
                        AxisModifier_ForkSwitch.ActivateSwitch(masterTime, forkExtents);
                    }
                    
                }
            }
        }

        [Serializable]
        private class TouchExtentsCollection : SerializableDictionaryBase<MasterSequence, List<AxisModifier_TouchExtents>>
        { }
        
        [Serializable]
        [ExecuteInEditMode]
        public partial class BaseAxisSwitchLegacy2
        {
        }

        [Serializable]
        [ExecuteInEditMode]
        public partial class BiAxisSwitchLegacy2 : BaseAxisSwitchLegacy2
        {
        }

        [Serializable]
        [ExecuteInEditMode]
        public partial class SimpleSwitchLegacy2 : BiAxisSwitchLegacy2
        {
        }

        [Serializable]
        [ExecuteInEditMode]
        public partial class InvertSwitchLegacy2 : BiAxisSwitchLegacy2
        {
        }

        [Serializable]
        [ExecuteInEditMode]
        public partial class ForkSwitchLegacy2 : BaseAxisSwitchLegacy2
        {
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}