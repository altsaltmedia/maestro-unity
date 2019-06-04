using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public abstract class SequenceController : MonoBehaviour
    {
        [Required]
        public SimpleEvent SequenceModified;

        [Required]
        public AppSettings appSettings;

        [Required]
        public SequenceList sequenceList;

        [ValidateInput("IsPopulated")]
        [BoxGroup("Android Dependencies")]
        public BoolReference isReversing;

        [SerializeField]
        [Required]
        [BoxGroup("Android Dependencies")]
        protected SimpleEvent triggerSpinnerShow;

        [SerializeField]
        [Required]
        [BoxGroup("Android Dependencies")]
        protected SimpleEvent triggerSpinnerHide;

        protected bool internalIsReversingVal = false;

        protected static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }   
}