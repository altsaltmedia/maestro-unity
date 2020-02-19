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

        public void TriggerModifyRequest(Sequence targetSequence, int requestPriority, string moduleName, float timeModifier)
        {
            if (rootConfig.appUtilsRequested == true) {
                return;
            }
            
            SequenceController sequenceController = sequenceControllers.Find(x => x.sequence == targetSequence);

            if (string.IsNullOrEmpty(activeInputModule.name) || activeInputModule.name == moduleName || requestPriority > activeInputModule.priority)
            {
                activeInputModule = LockInputModule(activeInputModule, moduleName, requestPriority);
                
                sequenceController.ModifySequence(timeModifier);
            }
        }
        
        public bool TriggerAutoplayRequest(Sequence targetSequence, int requestPriority, string moduleName, float timeModifier)
        {
            if (rootConfig.appUtilsRequested == true) {
                return false;
            }
            
            SequenceController sequenceController = sequenceControllers.Find(x => x.sequence == targetSequence);

            if (string.IsNullOrEmpty(activeInputModule.name) || activeInputModule.name == moduleName || requestPriority > activeInputModule.priority)
            {
                activeInputModule = LockInputModule(activeInputModule, moduleName, requestPriority);
                
                sequenceController.ActivateAutoplay();
                return true;
            }

            return false;
        }
        
        public void TriggerAutoplayPause(Sequence targetSequence, int requestPriority, string moduleName, float timeModifier)
        {
            if (rootConfig.appUtilsRequested == true) {
                return;
            }
            
            SequenceController sequenceController = sequenceControllers.Find(x => x.sequence == targetSequence);

            if (string.IsNullOrEmpty(activeInputModule.name) || activeInputModule.name == moduleName || requestPriority > activeInputModule.priority)
            {
                activeInputModule = LockInputModule(activeInputModule, moduleName, requestPriority);
                
                sequenceController.DeactivateAutoplay();
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
        public void SetElapsedTime(double targetTime)
        {
            _elapsedTime = targetTime;
            ApplyElapsedTimeToSequences(targetTime, masterTimeDataList);
            rootConfig.sequenceModified.RaiseEvent(this.gameObject);
        }
        
        private static Sequence ApplyElapsedTimeToSequences(double targetTime, List<MasterTimeData> sequenceData)
        {
            Sequence activeSequence = null;
            
            for (int i = 0; i < sequenceData.Count; i++) {
                
                // Since we're going through the sequences in order,
                // as soon as our target time does not exceed the end time,
                // that means we've found the sequence we'll want to target
                if (targetTime < sequenceData[i].masterTimeEnd) {

                    activeSequence = sequenceData[i].sequence;
                    activeSequence.active = true;
                    activeSequence.sequenceController.gameObject.SetActive(true);

                    // Update the active sequence
                    if (i == 0) {
                        activeSequence.currentTime = targetTime;
                    } else {
                        activeSequence.currentTime = targetTime - sequenceData[i - 1].masterTimeEnd;
                    }
                    
                    //activeSequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();

                    // Prep preceding sequence, if applicable
                    if (i > 0) {
                        for (int j = 0; j < sequenceData.Count; j++) {
                            if (j < i) {
                                sequenceData[j].sequence.currentTime = sequenceData[j].sequence.sourcePlayable.duration;
                                sequenceData[j].sequence.sequenceController.RefreshPlayableDirector();
                            }
                        }
                    }

                    // Prep following sequence, if applicable
                    if (i < sequenceData.Count - 1) {
                        for (int j = sequenceData.Count - 1; j >= 0; j--) {
                            if (j > i) {
                                sequenceData[j].sequence.currentTime = 0;
                                sequenceData[j].sequence.sequenceController.RefreshPlayableDirector();
                            }
                        }
                    }

                    // Deactivate the other sequences
                    for (int z = 0; z < sequenceData.Count; z++) {

                        if (z == i) {
                            continue;
                        }

                        sequenceData[z].sequence.active = false;
                        sequenceData[z].sequence.sequenceController.gameObject.SetActive(false);
                    }
                    
                    activeSequence.sequenceController.RefreshPlayableDirector();
                    
                    break;
                }
            }

            return activeSequence;
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