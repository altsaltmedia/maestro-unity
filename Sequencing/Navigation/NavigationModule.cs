using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Navigation
{
    [Serializable]
    public abstract class NavigationModule : Input_Module
    {
        protected abstract NavigationController navigationController { get; set; }

        protected override Input_Controller inputController => navigationController;
    }
}