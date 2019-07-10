using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public abstract class SequenceController : MonoBehaviour
    {
        [Required]
        public SimpleEventTrigger sequenceModified;

        [Required]
        public AppSettings appSettings;

        [ValidateInput("IsPopulated")]
        public List<SequenceList> sequenceLists = new List<SequenceList>();

        [ValidateInput("IsPopulated")]
        [BoxGroup("Android Dependencies")]
        public BoolReference isReversing;

        [SerializeField]
        [Required]
        [BoxGroup("Android Dependencies")]
        protected SimpleEventTrigger triggerSpinnerShow;

        [SerializeField]
        [Required]
        [BoxGroup("Android Dependencies")]
        protected SimpleEventTrigger triggerSpinnerHide;

        protected bool internalIsReversingVal = false;

        protected static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        protected static bool IsPopulated(List<SequenceList> attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }   
}