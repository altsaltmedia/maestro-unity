using UnityEngine;

namespace AltSalt.Sequencing
{
    public class InputModule : MonoBehaviour
    {
        [SerializeField]
        private int _priority;

        public int priority
        {
            get => _priority;
        }
    }
}