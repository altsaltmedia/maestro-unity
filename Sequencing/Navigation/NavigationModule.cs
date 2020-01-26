using System;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Navigation
{
    [Serializable]
    public abstract class NavigationModule : Input_Module
    {
        [SerializeField]
        private NavigationController _navigationController;

        protected NavigationController navigationController => _navigationController;

        protected override Input_Controller inputController => navigationController;
    }
}