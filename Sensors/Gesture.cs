using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro.Sensors {

    public class Gesture {
        public Vector2 startPosition;
        public Vector2 position;
        public Vector2 deltaPosition;
        public float actionTime;
        public float deltaTime;
        /// <summary>
        /// The length of the swipe.
        /// </summary>
        public float swipeLength;
        /// <summary>
        /// The swipe vector direction.
        /// </summary>
        public Vector2 swipeVector;

    }

}
