using DoozyUI;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class TestLoadSceneButtonHandler : MonoBehaviour
    {
        [Required]
        [SerializeField]
        ComplexEvent sceneLoadTriggered;

        // Initiate fader sequence. Fader object in hierarchy fires LoadScene
        // on completion via DoozyUI
        public void LoadNewScene(string sceneName)
        {
            if (!sceneName.Contains("Load")) { return; }
            sceneLoadTriggered.Raise(sceneName.Replace("Load_", ""));
        }

        private static bool IsPopulated(StringReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
    
}