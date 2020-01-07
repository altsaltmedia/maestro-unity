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
        private bool _autoplayActive;

        public bool autoplayActive
        {
            get => _autoplayActive;
            set => _autoplayActive = value;
        }

        [ShowInInspector]
        private bool _isLerping;

        public bool isLerping
        {
            get => _isLerping;
            set => _isLerping = value;
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

        public EasingUtility easingUtility
        {
            get => _easingUtility;
        }

        private IEnumerator _lerpCoroutine;

        public IEnumerator lerpCoroutine {
            get => _lerpCoroutine;
            set => _lerpCoroutine = value;
        }

        protected override string dataTitle => sequence.name;

        public static Autorun_Data CreateInstance(Sequence sequence, List<AutorunExtents> autorunIntervals)
        {
            //var inputData = ScriptableObject.CreateInstance(typeof(AutorunData)) as AutorunData;
            var inputData = new Autorun_Data {sequence = sequence, autorunIntervals = autorunIntervals};

            return inputData;
        }
    }
}