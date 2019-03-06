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
        public BoolReference localizationActive;

        [ValidateInput("IsPopulated")]
        public BoolReference autoplayActive;

        [ValidateInput("IsPopulated")]
        public BoolReference pauseMomentum;

        [ValidateInput("IsPopulated")]
        public BoolReference editorDebugEventsActive;

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}