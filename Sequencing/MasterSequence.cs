using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Sequencing
{
    [ExecuteInEditMode]
    public class MasterSequence : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        [Required]
        [InfoBox("This must be populated via a root config component")]
        private RootConfig _rootConfig;

        public RootConfig rootConfig
        {
            get => _rootConfig;
            set => _rootConfig = value;
        }

        [SerializeField]
        [ReadOnly]
        private ActiveInputModuleData _activeInputModule = new ActiveInputModuleData();

        private ActiveInputModuleData activeInputModule
        {
            get => _activeInputModule;
            set => _activeInputModule = value;
        }

        [FormerlySerializedAs("_sequenceConfigs")]
        [SerializeField]
        [ListDrawerSettings(Expanded = true)]
        private List<SequenceController> _sequenceControllers = new List<SequenceController>();

        public List<SequenceController> sequenceControllers => _sequenceControllers;

        [SerializeField]
        private List<MasterTimeData> _masterTimeDataList = new List<MasterTimeData>();

        public List<MasterTimeData> masterTimeDataList
        {
            get => _masterTimeDataList;
            private set => _masterTimeDataList = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private double _elapsedTime;

        public double elapsedTime
        {
            get => _elapsedTime;
            private set
            {
                if (masterTimeDataList.Count < 1) {
                    Init();
                }
                _elapsedTime = value;
            }
        }

        [ShowInInspector]
        [ReadOnly]
        private double _duration;
        
        public double duration {
            get {
                if (masterTimeDataList.Count < 1) {
                    Init();
                }
                return _duration;
            }
            private set => _duration = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private bool _hasActiveSequence;

        public bool hasActiveSequence
        {
            get => _hasActiveSequence;
            set => _hasActiveSequence = value;
        }

        public void Init()
        {
            // Generate master times for sequences
            masterTimeDataList = GenerateSequenceData(sequenceControllers);

            for (int i = 0; i < sequenceControllers.Count; i++) {
                sequenceControllers[i].Init(this);
            }
            
            // Get total time from master time of last sequence
            duration = masterTimeDataList[masterTimeDataList.Count - 1].masterTimeEnd;
        }

        public Sequence RequestModifySequenceTime(Sequence targetSequence, int requestPriority, string moduleName, float timeModifier)
        {
            if (rootConfig.appUtilsRequested == true) {
                return targetSequence;
            }
            
            SequenceController sequenceController = sequenceControllers.Find(x => x.sequence == targetSequence);

            if (string.IsNullOrEmpty(activeInputModule.name) || activeInputModule.name == moduleName || requestPriority > activeInputModule.priority)
            {
                activeInputModule = LockInputModule(activeInputModule, moduleName, requestPriority);
                
                sequenceController.ModifySequenceTime(timeModifier);
            }

            return targetSequence;
        }
        
        public Sequence RequestActivateForwardAutoplay(Sequence targetSequence, int requestPriority, string moduleName, float targetSpeed, out bool requestSuccessful)
        {
            requestSuccessful = false;
            
            if (rootConfig.appUtilsRequested == true) {
                return targetSequence;
            }
            
            SequenceController sequenceController = sequenceControllers.Find(x => x.sequence == targetSequence);

            if (string.IsNullOrEmpty(activeInputModule.name) || activeInputModule.name == moduleName || requestPriority > activeInputModule.priority)
            {
                activeInputModule = LockInputModule(activeInputModule, moduleName, requestPriority);
                
                sequenceController.ActivateForwardAutoplay(targetSpeed);
                requestSuccessful = true;
            }

            return targetSequence;
        }
        
        public void RequestDeactivateForwardAutoplay(Sequence targetSequence, int requestPriority, string moduleName)
        {
            if (rootConfig.appUtilsRequested == true) {
                return;
            }
            
            SequenceController sequenceController = sequenceControllers.Find(x => x.sequence == targetSequence);

            if (string.IsNullOrEmpty(activeInputModule.name) || activeInputModule.name == moduleName || requestPriority > activeInputModule.priority)
            {
                activeInputModule = LockInputModule(activeInputModule, moduleName, requestPriority);
                
                sequenceController.DeactivateForwardAutoplay();
            }
        }

        private static ActiveInputModuleData LockInputModule(ActiveInputModuleData activeInputModule, string moduleName, int priority)
        {
            activeInputModule.name = moduleName;
            activeInputModule.priority = priority;

            return activeInputModule;
        }

        public void UnlockInputModule(GameObject inputModuleObject)
        {
            if (activeInputModule.name != inputModuleObject.name) return;
            
            activeInputModule.name = string.Empty;
            activeInputModule.priority = 0;

            for (int i = 0; i < sequenceControllers.Count; i++) {
                sequenceControllers[i].SetSpeed(0);
            }
        }

        public MasterSequence RefreshElapsedTime(Sequence modifiedSequence)
        {
            MasterTimeData sequenceTimeData = this.masterTimeDataList.Find(x => x.sequence == modifiedSequence);
            
            if(sequenceTimeData != null) {
                this.elapsedTime = sequenceTimeData.masterTimeStart + modifiedSequence.currentTime;
            }

            return this;
        }

        public void RefreshHasActiveSequence()
        {
            if (SequenceActive(sequenceControllers) == true) {
                hasActiveSequence = true;
            }
            else {
                hasActiveSequence = false;
            }
        }

        private static bool SequenceActive(List<SequenceController> sequenceConfigs)
        {
            for (int i = 0; i < sequenceConfigs.Count; i++) {
                if (sequenceConfigs[i].sequence.active == true) {
                    return true;
                }
            }

            return false;
        }

        [Button(ButtonSizes.Large)]
        public void SetElapsedTime(GameObject caller, double targetTime)
        {
            _elapsedTime = targetTime;
            ApplyElapsedTimeToSequences(caller, targetTime, this.masterTimeDataList);
            rootConfig.sequenceModified.RaiseEvent(this.gameObject);
        }
        
        private static Sequence ApplyElapsedTimeToSequences(GameObject caller, double targetTime, List<MasterTimeData> sequenceData)
        {
            Sequence targetSequence = null;
            
            for (int targetID = 0; targetID < sequenceData.Count; targetID++) {
                
                // Since we're going through the sequences in order
                // from beginning to end, as soon as our target time
                // does not exceed the end time of a sequence, that
                // means we've found the sequence we'll want to target
                if (targetTime < sequenceData[targetID].masterTimeEnd) {

                    targetSequence = sequenceData[targetID].sequence;
                    targetSequence.active = true;
                    targetSequence.sequenceController.gameObject.SetActive(true);
                    
                    // If applicable, evaluate all of the preceding sequences at their last frame
                    if (targetID > 0) {
                        for (int j = 0; j < sequenceData.Count; j++) {
                            if (j < targetID) {
                                sequenceData[j].sequence.sequenceController
                                    .SetSequenceTime(caller, (float)sequenceData[j].sequence.sourcePlayable.duration);
                            }
                        }
                    }

                    // If applicable, evaluate all of the following sequences at their first frame
                    if (targetID < sequenceData.Count - 1) {
                        for (int j = sequenceData.Count - 1; j >= 0; j--) {
                            if (j > targetID) {
                                sequenceData[j].sequence.sequenceController.SetSequenceTime(caller, 0);
                            }
                        }
                    }

                    // Deactivate all other sequences
                    for (int z = 0; z < sequenceData.Count; z++) {

                        if (z != targetID) {
                            sequenceData[z].sequence.active = false;
                            sequenceData[z].sequence.sequenceController.gameObject.SetActive(false);
                        }
                    }
                    
                    // Finally, update our target sequence
                    if (targetID == 0) {
                        // If our target sequence is the first in the master list,
                        // then the target time is already our local time
                        targetSequence.sequenceController.SetSequenceTime(caller, (float)targetTime);
                    } else {
                        // Otherwise, we need to convert the target time to local time
                        // based on its position in the MasterSequence's sequence list
                        double localTime = targetTime - sequenceData[targetID - 1].masterTimeEnd;
                        targetSequence.sequenceController.SetSequenceTime(caller, (float)localTime);
                    }

                    break;
                }
            }

            return targetSequence;
        }

        private static List<MasterTimeData> GenerateSequenceData(List<SequenceController> sourceSequenceConfigs)
        {
            List<MasterTimeData> sequenceData = new List<MasterTimeData>();
            for (int i = 0; i < sourceSequenceConfigs.Count; i++)
            {
                double masterTimeStart = 0d;
                double masterTimeEnd = 0d;

                if (i == 0)  {
                    masterTimeEnd = sourceSequenceConfigs[i].sequence.sourcePlayable.duration;
                } else
                {
                    masterTimeStart = sequenceData[i - 1].masterTimeEnd;
                    masterTimeEnd = sourceSequenceConfigs[i].sequence.sourcePlayable.duration + sequenceData[i - 1].masterTimeEnd;
                }

                MasterTimeData newSequenceData = new MasterTimeData(sourceSequenceConfigs[i].sequence, masterTimeStart, masterTimeEnd);
                sequenceData.Add(newSequenceData);
            }

            return sequenceData;
        }

        public static double LocalToMasterTime(MasterSequence masterSequence, Sequence sourceSequence, double localTime)
        {
            if (masterSequence.masterTimeDataList.Count < 1) {
                masterSequence.Init();
            }
            List<MasterTimeData> masterTimeData = masterSequence.masterTimeDataList;
            for (int i = 0; i < masterTimeData.Count; i++)
            {
                if (masterTimeData[i].sequence == sourceSequence)
                {
                    return masterTimeData[i].masterTimeStart + localTime;
                }
            }
            
            throw new SystemException("Source sequence not found in sequence data.");
        }
    }

    [Serializable]
    public class MasterTimeData
    {
        [SerializeField]
        private Sequence _sequence;
        
        public Sequence sequence {
            get => _sequence;
            set => _sequence = value;
        }
        
        [SerializeField]
        private double _masterTimeStart;
        
        public double masterTimeStart {
            get => _masterTimeStart;
            set => _masterTimeStart = value;
        }

        [SerializeField]
        private double _masterTimeEnd;
        
        public double masterTimeEnd {
            get => _masterTimeEnd;
            set => _masterTimeEnd = value;
        }

        public MasterTimeData(Sequence sourceSequence, double masterTimeStart, double masterTimeEnd)
        {
            this.sequence = sourceSequence;
            this.masterTimeStart = masterTimeStart;
            this.masterTimeEnd = masterTimeEnd;
        }
    }

    [Serializable]
    public class ActiveInputModuleData
    {
        [SerializeField]
        private string _name;

        public string name
        {
            get => _name;
            set => _name = value;
        }

        [SerializeField]
        private int _priority;

        public int priority
        {
            get => _priority;
            set => _priority = value;
        }
    }
}