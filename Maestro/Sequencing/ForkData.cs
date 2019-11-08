using System;
using System.Collections.Generic;
using AltSalt.Maestro.Sequencing.Touch;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing
{
    [Serializable]
    public class ForkData : Input_Data
    {
        [SerializeField]
        private TouchFork _fork;

        public TouchFork fork
        {
            get => _fork;
            private set => _fork = value;
        }

        protected override string dataTitle
        {
            get => sequence.name + " - " + fork.name;
        }

        [SerializeField]
        private string _description;

        public string description
        {
            get => _description;
            private set => _description = value;
        }

        [SerializeField]
        private ForkMarker _forkMarker;
        
        public ForkMarker forkMarker
        {
            get => _forkMarker;
            private set => _forkMarker = value;
        }

        [TitleGroup("$"+nameof(dataTitle))]
        private MarkerPlacement _markerPlacement;

        public MarkerPlacement markerPlacement
        {
            get => _markerPlacement;
            private set => _markerPlacement = value;
        }

        public ForkData(Sequence sequence, ForkMarker forkMarker)
        {
            this.sequence = sequence;
            this.forkMarker = forkMarker;
            this.markerPlacement = forkMarker.markerPlacement;
            this.description = forkMarker.description;
            
//            if (forkMarker is ForkMarker_JoinPrevious) {
//                this.extents = new Extents(forkMarker.time, forkMarker.time + forkTransitionSpread);
//            } else if (forkMarker is ForkMarker_JoinNext) {
//                this.extents = new Extents(forkMarker.time - forkTransitionSpread, forkMarker.time);
//            }

            this.fork = forkMarker.joinDestination as TouchFork;
        }
    }
}