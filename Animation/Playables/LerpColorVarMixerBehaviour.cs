using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Animation
{    
    public class LerpColorVarMixerBehaviour : LerpToTargetMixerBehaviour
    {
        ColorVariable trackBinding;
        ScriptPlayable<ColorBehaviour> inputPlayable;
        ColorBehaviour input;
        bool originalValueSet;
        Color originalValue;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as ColorVariable;
            
            if (!trackBinding)
                return;

            if(originalValueSet == false) {
                originalValue = trackBinding.value;
                originalValueSet = true;
            }
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ColorBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                trackBinding.StoreCaller(trackAssetConfig.gameObject, trackAssetConfig.gameObject.scene.name, parentTrack.name);
                
                if (inputWeight >= 1f) {
                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBinding.SetValue(Color.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, percentageComplete)));
                } else {
                    if (currentTime >= input.endTime) {
                        trackBinding.SetValue(input.targetValue);
                    }
                    else if (i == 0 && currentTime <= input.startTime) {
                        trackBinding.SetValue(input.initialValue);
                    }
                }
            }
        }
        
        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);

            if (trackBinding != null) {
                
                trackBinding.StoreCaller(trackBinding, input.parentTrack.name, input.parentTrack.name);
                
                if (Application.isPlaying == true && scrubberActive == true) {
                    trackBinding.SetValue(Utils.transparent);
                }
#if UNITY_EDITOR
                if(Application.isPlaying == false) {
                    if (trackBinding.hasDefault == true) {
                        trackBinding.SetToDefaultValue();
                    }
                    else {
                        trackBinding.SetValue(originalValue);
                    }
                }
#endif
            }

        }
    }   
}