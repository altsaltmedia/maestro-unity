using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Autorun
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

        private bool _autoplayActive;

        public bool autoplayActive
        {
            get => _autoplayActive;
            set => _autoplayActive = value;
        }

        private bool _isLerping;

        public bool isLerping
        {
            get => _isLerping;
            set => _isLerping = value;
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