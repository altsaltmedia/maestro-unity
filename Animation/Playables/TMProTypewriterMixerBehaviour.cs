/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class TMProTypewriterMixerBehaviour : LerpToTargetMixerBehaviour
    {
        float rawMaxCharactersVal;
        float rawMaxWordVal;

        int originalCharactersVal = -1;
        int originalWordsVal = -1;
        
        TextMeshPro trackBinding;
        TextMeshPro trackBindingComponent;
        ScriptPlayable<TMProTypewriterBehaviour> inputPlayable;
        TMProTypewriterBehaviour input;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as TextMeshPro;

            if (!trackBinding)
                return;

            inputCount = playable.GetInputCount();

            if(originalCharactersVal == -1) {
                originalCharactersVal = trackBinding.maxVisibleCharacters;
            }

            if (originalWordsVal == -1) {
                originalWordsVal = trackBinding.maxVisibleWords;
            }

            for (int i = 0; i < inputCount; i++) {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<TMProTypewriterBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour();

                trackBindingComponent = trackBinding.GetComponent<TextMeshPro>();

                if (inputWeight >= 1f) {
                    if(input.setValuesImmediately == true) {
                        trackBindingComponent.maxVisibleCharacters = input.targetMaxVisibleCharacters;
                        trackBindingComponent.maxVisibleWords = input.targetMaxVisibleWords;
                    } else {
                        percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                        rawMaxCharactersVal = Mathf.Lerp(input.initialMaxVisibleCharacters, input.targetMaxVisibleCharacters, input.easingFunction(0f, 1f, percentageComplete));
                        trackBindingComponent.maxVisibleCharacters = (int)Mathf.Round(rawMaxCharactersVal);

                        rawMaxWordVal = Mathf.Lerp(input.initialMaxVisibleWords, input.targetMaxVisibleWords, input.easingFunction(0f, 1f, percentageComplete));
                        trackBindingComponent.maxVisibleWords = (int)Mathf.Round(rawMaxWordVal);
                    }
                }
                else {
                    if (timelineInstanceConfig.currentTime >= input.endTime) {
                        trackBindingComponent.maxVisibleCharacters = input.targetMaxVisibleCharacters;
                        trackBindingComponent.maxVisibleWords = input.targetMaxVisibleWords;
                    }
                    else if (i == 0 && timelineInstanceConfig.currentTime <= input.startTime) {
                        trackBindingComponent.maxVisibleCharacters = input.initialMaxVisibleCharacters;
                        trackBindingComponent.maxVisibleWords = input.initialMaxVisibleWords;
                    }
                }
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if(trackBinding != null) {
                trackBinding.maxVisibleCharacters = originalCharactersVal;
                trackBinding.maxVisibleWords = originalWordsVal;
            }
        }
    }   
}