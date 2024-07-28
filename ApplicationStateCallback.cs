using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro {

    public class ApplicationStateCallback : MonoBehaviour
    {

        [SerializeField]
        private GameObjectGenericAction _onApplicationPause = new GameObjectGenericAction();

        private GameObjectGenericAction onApplicationPause
        {
            get => _onApplicationPause;
            set => _onApplicationPause = value;
        }

        [SerializeField]
        private GameObjectGenericAction _onApplicationQuit = new GameObjectGenericAction();

        private GameObjectGenericAction onApplicationQuit
        {
            get => _onApplicationQuit;
            set => _onApplicationQuit = value;
        }

        private void OnApplicationPause(bool paused)
        {
            if (Application.isPlaying == false) return;

            onApplicationPause.Invoke(this.gameObject);
        }

        private void OnApplicationQuit()
        {
            if (Application.isPlaying == false) return;

            onApplicationQuit.Invoke(this.gameObject);
        }
    }

}
