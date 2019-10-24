﻿using AltSalt.Sequencing.Navigate;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Sequencing.Autorun
{
    [CustomStyle(nameof(AutoplayStart))]
    public class AutoplayStart : InputMarker, IVideoConfigurator, IMarkerDescription
    {
        [SerializeField]
        private string _description;

        public string description
        {
            get => _description;
        }

        [SerializeField]
        private bool _isVideoSequence;

        public bool isVideoSequence
        {
            get => _isVideoSequence;
        }
    }
}