using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Sequencing.Touch
{

    public partial class AxisModifier : TouchModule
    {
        [SerializeField]
        private List<BaseAxisSwitch> _switchData;

        private List<BaseAxisSwitch> switchData
        {
            get => _switchData;
            set => _switchData = value;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        private ComplexEventTrigger _convertMomentum;

        public ComplexEventTrigger convertMomentum
        {
            get => _convertMomentum;
            set => _convertMomentum = value;
        }

        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        private FloatReference _swipeResetSpread;

        public float swipeResetSpread
        {
            get => _swipeResetSpread.Value;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        private FloatReference _swipeTransitionSpread;

        public float swipeTransitionSpread
        {
            get => _swipeTransitionSpread.Value;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        private FloatReference _momentumTransitionSpread;

        public float momentumTransitionSpread
        {
            get => _momentumTransitionSpread.Value;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        private FloatReference _invertTransitionSpread;

        public float invertTransitionSpread
        {
            get => _invertTransitionSpread.Value;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Switch Dependencies")]
        private FloatReference _forkTransitionSpread;

        public float forkTransitionSpread
        {
            get => _forkTransitionSpread.Value;
        }

        private void ConfigureData()
        {
            switchData.Clear();

            for (int i = 0; i < touchController.touchDataList.Count; i++)
            {
                var touchData = touchController.touchDataList[i];
                IEnumerable<TimelineClip> clips = touchData.axisSwitchTrack.GetClips();
                
                foreach (TimelineClip clip in clips)
                {
                    Type clipType = clip.asset.GetType();

                    switch (clipType.Name)
                    {
                        case nameof(SimpleSwitchClip) :
                        {
                            switchData.Add(SimpleSwitch.CreateInstance(touchController, touchData, clip.asset as SimpleSwitchClip));
                            break;
                        }
                    
                        case nameof(InvertSwitchClip) :
                        {
                            switchData.Add(InvertSwitch.CreateInstance(touchController, touchData, clip.asset as InvertSwitchClip));
                            break;
                        }
                    
                        case nameof(ForkSwitchClip) :
                        {
                            switchData.Add(ForkSwitch.CreateInstance(touchController, touchData, clip.asset as ForkSwitchClip));
                            break;
                        }
                    }
                }
            }
        }
        
        public virtual void UpdateAxes()
        {
            for (int i = 0; i < touchController.masterSequences.Count; i++)
            {
                double masterTime = touchController.masterSequences[i].ElapsedTime;
                
                for (int j = 0; j < switchData.Count; j++)
                {
                    if (touchController.masterSequences[i].sequenceConfigs
                        .Exists(x => x.sequence == switchData[j].touchData.sequence) == false) continue;
                    
                    Type clipType = switchData[j].GetType();
                    
                    switch (clipType.Name)
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
        
        [Serializable]
        public abstract partial class BaseAxisSwitch : ScriptableObject
        {
        }

        [Serializable]
        public partial class BiAxisSwitch : BaseAxisSwitch
        {
        }

        [Serializable]
        public partial class SimpleSwitch : BiAxisSwitch
        {
        }

        [Serializable]
        public partial class InvertSwitch : BiAxisSwitch
        {
        }

        [Serializable]
        public partial class ForkSwitch : BaseAxisSwitch
        {
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}