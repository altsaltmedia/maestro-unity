using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Navigation
{
    [Serializable]
    public abstract class NavigationModule : Input_Module
    {
        [ReadOnly]
        [ShowInInspector]
        private NavigationController _navigationController;

        protected NavigationController navigationController
        {
            get => _navigationController;
            set => _navigationController = value;
        }

        protected override Input_Controller inputController => navigationController;
    }
}