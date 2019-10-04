using UnityEngine;

namespace AltSalt
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
