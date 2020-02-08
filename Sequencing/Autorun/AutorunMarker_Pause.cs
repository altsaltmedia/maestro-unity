﻿using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    [CustomStyle(nameof(AutorunMarker_Pause))]
    public class AutorunMarker_Pause : ConfigMarker, IVideoConfigurator, IMarkerDescription
    {
        [SerializeField]
        private string _description;

        public string description => _description;

        [SerializeField]
        private bool _isVideoSequence;

        public bool isVideoSequence => _isVideoSequence;
    }
}