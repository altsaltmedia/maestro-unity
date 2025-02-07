using System;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing
{

    [CreateAssetMenu(menuName = "Maestro/Sequencing/Branch Key")]
    [Serializable]
    public class BranchKey : CustomKey
    {
#if UNITY_EDITOR
        protected override string title => "Branch Key";
#endif
    }
    
}