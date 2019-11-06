using System;
using System.Collections.Generic;
using AltSalt.Sequencing.Touch;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing
{
    [Serializable]
    public class JoinTools_ForkJoinData : Input_Data
    {
        [SerializeField]
        [TitleGroup("$"+nameof(forkTitle))]
        private AxisModifier_TouchFork _fork;

        public AxisModifier_TouchFork fork
        {
            get => _fork;
            private set => _fork = value;
        }

        private string forkTitle
        {
            get => sequence.name + " - " + fork.name;
        }
            
        [SerializeField]
        [TitleGroup("$"+nameof(forkTitle))]
        private Input_Extents _extents;

        public Input_Extents extents
        {
            get => _extents;
            private set => _extents = value;
        }

        [SerializeField]
        private string _description;

        public string description
        {
            get => _description;
            private set => _description = value;
        }

        [SerializeField]
        [TitleGroup("$"+nameof(forkTitle))]
        private MarkerPlacement _markerPlacement;

        public MarkerPlacement markerPlacement
        {
            get => _markerPlacement;
            private set => _markerPlacement = value;
        }

        public JoinTools_ForkJoinData(Sequence sequence, JoinTools_ForkMarker forkMarker, double forkTransitionSpread)
        {
            this.sequence = sequence;
            this.markerPlacement = forkMarker.markerPlacement;
            this.description = forkMarker.description;
            
            if (forkMarker is JoinTools_ForkJoinPrevious) {
                this.extents = new Input_Extents(forkMarker.time, forkMarker.time + forkTransitionSpread);
            } else if (forkMarker is JoinTools_ForkJoinNext) {
                this.extents = new Input_Extents(forkMarker.time - forkTransitionSpread, forkMarker.time);
            }

            this.fork = forkMarker.joinDestination as AxisModifier_TouchFork;
        }
    }
}