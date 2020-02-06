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
                
                if (inputWeight >= 1f) {
                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBinding.SetValue(trackAssetConfig.gameObject, Color.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, percentageComplete)));
                } else {
                    if (trackAssetConfig.currentTime >= input.endTime) {
                        trackBinding.SetValue(trackAssetConfig.gameObject, input.targetValue);
                    }
                    else if (i == 0 && trackAssetConfig.currentTime <= input.startTime) {
                        trackBinding.SetValue(trackAssetConfig.gameObject, input.initialValue);
                    }
                }
            }
        }
        
        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);

            if (trackBinding != null) {
                
                trackBinding.StoreCaller(trackBinding, input.parentTrack.name, input.parentTrack.name);
                
                if (Application.isPlaying == true && appUtilsRequested == true) {
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