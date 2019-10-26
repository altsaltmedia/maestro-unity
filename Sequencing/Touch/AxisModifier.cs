using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Serialization;

#if UNITY_EDITOR
    using UnityEditor;
using UnityEditor.Timeline;

#endif

namespace AltSalt.Sequencing.Touch
{

    [ExecuteInEditMode]
    public partial class AxisModifier : TouchModule
    {
        [ShowInInspector]
        private List<BaseAxisSwitch> _switchData = new List<BaseAxisSwitch>();

        private List<BaseAxisSwitch> switchData
        {
            get => _switchData;
            set => _switchData = value;
        }
        
        [ShowInInspector]
        private List<AxisExtents> _axisExtentsData = new List<AxisExtents>();

        private List<AxisExtents> axisExtentsData
        {
            get => _axisExtentsData;
            set => _axisExtentsData = value;
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
        private FloatReference _swipeResetSpread;

        public float swipeResetSpread
        {
            get => _swipeResetSpread.Value;
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
        
        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        [SerializeField]
        private FloatReference _forkTransitionSpread;

        public float forkTransitionSpread
        {
            get => _forkTransitionSpread.Value;
        }

        protected override void Start()
        {
            base.Start();
            ConfigureData();
        }

#if UNITY_EDITOR
        
        public void ConfigureData()
        {
            switchData.Clear();

            for (int i = 0; i < touchController.touchDataList.Count; i++)
            {
                var touchData = touchController.touchDataList[i];
                
                AxisSwitchTrack axisSwitchTrack = touchData.axisSwitchTrack;
                if (axisSwitchTrack == null) continue;
                
                IEnumerable<IMarker> markers = axisSwitchTrack.GetMarkers().OrderBy(s => s.time);
                int count = 0;

                ConfigureAxisExtents(this, touchData, markers);

//                IEnumerable<TimelineClip> clips = axisSwitchTrack.GetClips();
//                
//                foreach (TimelineClip clip in clips)
//                {
//                    Type clipType = clip.asset.GetType();
//
//                    switch (clipType.Name)
//                    {
//                        case nameof(SimpleSwitchClip) :
//                        {
//                            switchData.Add(SimpleSwitch.CreateInstance(touchController, touchData, clip.asset as SimpleSwitchClip));
//                            break;
//                        }
//                    
//                        case nameof(InvertSwitchClip) :
//                        {
//                            switchData.Add(InvertSwitch.CreateInstance(touchController, touchData, clip.asset as InvertSwitchClip));
//                            break;
//                        }
//                    
//                        case nameof(ForkSwitchClip) :
//                        {
//                            switchData.Add(ForkSwitch.CreateInstance(touchController, touchData, clip.asset as ForkSwitchClip));
//                            break;
//                        }
//                    }
//                }
            }
            
            EditorUtility.SetDirty(this);
        }

        private static AxisModifier ConfigureAxisExtents(AxisModifier axisModifier, TouchController.TouchData touchData,
            IEnumerable<IMarker> markers)
        {
            List<AxisExtents> axisExtentsData = axisModifier.axisExtentsData;
            
            foreach (IMarker marker in markers)
            {
                if (marker is AxisModifierMarker axisModifierMarker) {
                        
                    switch (axisModifierMarker) {
                            
                        case SingleAxis singleAxis:
                            axisExtentsData.Add(new SingleAxisExtents(axisModifier, touchData, singleAxis));
                            break;
                            
                        case ForkAxis forkAxis:
                            axisExtentsData.Add(new ForkAxisExtents(axisModifier, touchData, forkAxis));
                            break;
                    }
                }
            }

            for (int j = 0; j < axisExtentsData.Count; j++)
            {
                if (axisExtentsData.Count == 1) {
                    AxisExtents.ConfigureAdjacentExtents(axisExtentsData[j], null, null);
                    break;
                }
                    
                if (j == 0) {
                    AxisExtents.ConfigureAdjacentExtents(axisExtentsData[j], null, axisExtentsData[j + 1]);
                }
                else if (j == axisExtentsData.Count - 1)  {
                    AxisExtents.ConfigureAdjacentExtents(axisExtentsData[j], axisExtentsData[j - 1], null);
                }
                else {
                    AxisExtents.ConfigureAdjacentExtents(axisExtentsData[j], axisExtentsData[j - 1],  axisExtentsData[j + 1]);
                }
            }

            return axisModifier;
        }
        
#endif
        
        public virtual void Update()
        {
            if (Application.isPlaying == false) return;
            
            for (int i = 0; i < touchController.masterSequences.Count; i++)
            {
                double masterTime = touchController.masterSequences[i].elapsedTime;
                
                for (int j = 0; j < switchData.Count; j++)
                {
                    if (touchController.masterSequences[i].sequenceConfigs
                        .Exists(x => x.sequence == switchData[j].touchData.sequence) == false) continue;
                    
                    Type switchType = switchData[j].GetType();
                    
                    switch (switchType.Name)
                    {
                        case nameof(SimpleSwitch) :
                        {
                            SimpleSwitch.CheckActivateSwitch(switchData[j], masterTime);
                            break;
                        }

                        case nameof(InvertSwitch):
                        {
                            InvertSwitch.CheckActivateSwitch(switchData[j], masterTime);
                            break;
                        }

                        case nameof(ForkSwitch) :
                        {
                            ForkSwitch.CheckActivateSwitch(switchData[j], masterTime);
                            break;
                        }
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                }
            }
        }

        public class AxisExtentsData
        {
            
        }
        
        [Serializable]
        [ExecuteInEditMode]
        public partial class BaseAxisSwitch
        {
        }

        [Serializable]
        [ExecuteInEditMode]
        public partial class BiAxisSwitch : BaseAxisSwitch
        {
        }

        [Serializable]
        [ExecuteInEditMode]
        public partial class SimpleSwitch : BiAxisSwitch
        {
        }

        [Serializable]
        [ExecuteInEditMode]
        public partial class InvertSwitch : BiAxisSwitch
        {
        }

        [Serializable]
        [ExecuteInEditMode]
        public partial class ForkSwitch : BaseAxisSwitch
        {
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}