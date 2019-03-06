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
        public BoolReference isReversing;

        protected static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }   
}