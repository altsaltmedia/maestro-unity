using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sensors
{
    public class HandheldUtils : MonoBehaviour
    {
        [Button(ButtonSizes.Large)]
        public void TriggerVibrate()
        {
#if !UNITY_STANDALONE
            Handheld.Vibrate();
#endif
        }
    }
}
