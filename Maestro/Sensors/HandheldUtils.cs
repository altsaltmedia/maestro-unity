using UnityEngine;

namespace AltSalt.Maestro
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
