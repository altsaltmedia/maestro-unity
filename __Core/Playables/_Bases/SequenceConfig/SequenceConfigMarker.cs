using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt
{
    [HideInMenu]
    public class SequenceConfigMarker : Marker
    {
        [SerializeField]
        public string description;
    }
}
