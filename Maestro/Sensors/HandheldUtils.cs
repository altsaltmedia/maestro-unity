using UnityEngine;

namespace AltSalt.Maestro.Sensors
{
    public class HandheldUtils : MonoBehaviour
    {
        public void TriggerVibrate()
        {
#if !UNITY_STANDALONE
            Handheld.Vibrate();
#endif
        }
    }
}
