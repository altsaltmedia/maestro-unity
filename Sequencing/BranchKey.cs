using System;
using UnityEngine;

namespace AltSalt.Sequencing
{

    [CreateAssetMenu(menuName = "AltSalt/Sequencing/Branch Key")]
    [Serializable]
    public class BranchKey : CustomKey
    {
#if UNITY_EDITOR
        protected override string title => "Branch Key";
#endif
    }
    
}