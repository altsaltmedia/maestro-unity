/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine.Playables;

namespace AltSalt
{
    public class LerpToTargetMixerBehaviour : PlayableBehaviour
    {
        protected double currentTime;

        // Utility vars - specified here to prevent garbage collection
        protected int inputCount;
        protected float inputWeight;
        protected float modifier;

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            currentTime = playable.GetGraph().GetRootPlayable(0).GetTime();
        }

        // This ProcessFrame() method should be implemented and overridden in inheriting mixer behaviours
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // Do nothing
        }
    }   
}