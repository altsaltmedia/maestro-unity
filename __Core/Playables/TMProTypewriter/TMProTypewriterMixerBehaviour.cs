/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt
{
    public class TMProTypewriterMixerBehaviour : LerpToTargetMixerBehaviour
    {
        float rawMaxCharactersVal;
        float rawMaxWordVal;
        
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
                        modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                        rawMaxCharactersVal = Mathf.Lerp(input.initialMaxVisibleCharacters, input.targetMaxVisibleCharacters, input.easingFunction(0f, 1f, modifier));
                        trackBindingComponent.maxVisibleCharacters = (int)Mathf.Round(rawMaxCharactersVal);

                        rawMaxWordVal = Mathf.Lerp(input.initialMaxVisibleWords, input.targetMaxVisibleWords, input.easingFunction(0f, 1f, modifier));
                        trackBindingComponent.maxVisibleWords = (int)Mathf.Round(rawMaxWordVal);
                    }
                }
                else {
                    if (currentTime >= input.endTime) {
                        trackBindingComponent.maxVisibleCharacters = input.targetMaxVisibleCharacters;
                    }
                    else if (i == 0 && currentTime <= input.startTime) {
                        trackBindingComponent.maxVisibleCharacters = input.initialMaxVisibleCharacters;
                    }
                }
            }
        }
    }   
}