using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [CreateAssetMenu(menuName = "AltSalt/Events/Event Payload Key")]
    public class EventPayloadKey : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField]
        [Multiline]
        [Header("Event Payload Key")]
        [InfoBox("Use an Event Payload Key to allow a complex event packager/listener pair to communicate.")]
        string description;
#endif
    }

}
