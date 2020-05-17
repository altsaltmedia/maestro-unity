using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AltSalt.Maestro.Logic
{
    [RequireComponent(typeof(EventSystem))]
    public class EventSystemUtils : MonoBehaviour
    {
        private EventSystem _eventSystem;

        private EventSystem eventSystem
        {
            get => _eventSystem;
            set => _eventSystem = value;
        }

        [DisplayAsString]
        [ShowInInspector]
        private string scriptDescription => "This is a utility script for the EventSystem"; 

        private void Awake()
        {
            eventSystem = GetComponent<EventSystem>();
        }

        public void ResetSelectedGameObject()
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }
}