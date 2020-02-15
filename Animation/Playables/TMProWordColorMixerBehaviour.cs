using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class TMProWordColorMixerBehaviour : LerpToTargetMixerBehaviour
    {
        TMP_Text trackBinding;
        ScriptPlayable<ColorBehaviour> inputPlayable;
        ColorBehaviour input;
        TMP_Text trackBindingComponent;
        Color originalValue;

        private TMP_TextInfo _textInfo;

        public TMP_TextInfo textInfo
        {
            get => _textInfo;
            set => _textInfo = value;
        }

        private int _wordCount;

        private int wordCount
        {
            get => _wordCount;
            set => _wordCount = value;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as TMP_Text;

            if (!trackBinding)
                return;

            if (trackBindingComponent == null) {
                trackBindingComponent = trackBinding.GetComponent<TMP_Text>();
                originalValue = trackBindingComponent.color;
            }

            textInfo = trackBinding.textInfo;
            wordCount = textInfo.wordCount;

            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ColorBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                trackBindingComponent = trackBinding.GetComponent<TMP_Text>();
                
                if(inputWeight >= 1f) {
                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.color = Color.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, percentageComplete));
                } else {
                    if(trackAssetConfig.currentTime >= input.endTime) {
                        trackBindingComponent.color = input.targetValue;
                    } else if (i == 0 && trackAssetConfig.currentTime <= input.startTime) {
                        trackBindingComponent.color = input.initialValue;
                    }
                }
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if (Application.isPlaying == true && isScrubbing == true) {
                if (trackBindingComponent != null) {    
                    trackBindingComponent.color = Utils.transparent;
                }
            }
        }
    }   
}