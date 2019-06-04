using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/App Settings")]
    public class AppSettings : ScriptableObject
    {
        [ValidateInput("IsPopulated")]
        public BoolReference initDebugMenu;

        [ValidateInput("IsPopulated")]
        public BoolReference paused;

        [ValidateInput("IsPopulated")]
        public BoolReference modifyTextActive;

        [ValidateInput("IsPopulated")]
        public BoolReference modifyLayoutActive;

        [ValidateInput("IsPopulated")]
        public BoolReference saveDataActive;

        [ValidateInput("IsPopulated")]
        public BoolReference autoplayActive;

        [ValidateInput("IsPopulated")]
        public BoolReference volumeActive;

        [ValidateInput("IsPopulated")]
        public BoolReference pauseMomentum;

        [ValidateInput("IsPopulated")]
        public BoolReference lockAxis;

        [ValidateInput("IsPopulated")]
        public FloatReference timescale;

        [ValidateInput("IsPopulated")]
        public BoolReference editorDebugEventsActive;

        [ValidateInput("IsPopulated")]
        public BoolReference pillarBoxingEnabled;

        [ValidateInput("IsPopulated")]
        public BoolReference hasBeenOpened;

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}