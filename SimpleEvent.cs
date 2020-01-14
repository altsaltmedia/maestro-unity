using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Events/Simple Event")]
    public class SimpleEvent : SimpleSignal
    {
        protected override string title => nameof(SimpleEvent);
        
        [SerializeField]
        [FormerlySerializedAs("associatedVariables")]
        private List<ScriptableObject> _associatedVariables = new List<ScriptableObject>();

        private List<ScriptableObject> associatedVariables
        {
            set => _associatedVariables = value;
        }
    }
}