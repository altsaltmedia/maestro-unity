using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    [Serializable]
    public class Autorun_Data : Input_Data
    {
        [SerializeField]
        [ReadOnly]
        private List<AutorunExtents> _autorunIntervals;

        public List<AutorunExtents> autorunIntervals
        {
            get => _autorunIntervals;
            set => _autorunIntervals = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private Autorun_Module _activeAutorunModule;

        public Autorun_Module activeAutorunModule
        {
            get => _activeAutorunModule;
            set => _activeAutorunModule = value;
        }

        [ShowInInspector]
        private AutorunExtents _activeInterval;

        public AutorunExtents activeInterval
        {
            get => _activeInterval;
            set => _activeInterval = value;
        }

        [ShowInInspector]
        private bool _forwardUpdateActive;

        public bool forwardUpdateActive
        {
            get => _forwardUpdateActive;
            set => _forwardUpdateActive = value;
        }
        
        [ShowInInspector]
        private bool _backwardUpdateActive;

        public bool backwardUpdateActive
        {
            get => _backwardUpdateActive;
            set => _backwardUpdateActive = value;
        }

        [ShowInInspector]
        private bool _eligibleForAutoplay;

        public bool eligibleForAutoplay
        {
            get => _eligibleForAutoplay;
            set => _eligibleForAutoplay = value;
        }

        [ShowInInspector]
        private bool _loop;

        public bool loop
        {
            get => _loop;
            set => _loop = value;
        }
        
        [SerializeField]
        private EasingUtility _easingUtility = new EasingUtility();

        public EasingUtility easingUtility => _easingUtility;

        protected override string dataTitle => sequence.name;

        public static Autorun_Data CreateInstance(Sequence sequence, List<AutorunExtents> autorunIntervals)
        {
            var inputData = new Autorun_Data {sequence = sequence, autorunIntervals = autorunIntervals};
            return inputData;
        }
    }
}