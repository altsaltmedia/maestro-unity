using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Sequence List")]
    public class SequenceList : ScriptableObject
    {
        public List<Sequence> sequences = new List<Sequence>();
    }   
}