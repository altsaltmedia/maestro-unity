using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class TMProColorMixerBehaviour : LerpToTargetMixerBehaviour
    {
        private TMP_Text _trackBinding;

        private TMP_Text trackBinding
        {
            get => _trackBinding;
            set => _trackBinding = value;
        }

        private ScriptPlayable<TMProColorBehaviour> _inputPlayable;

        private ScriptPlayable<TMProColorBehaviour> inputPlayable
        {
            get => _inputPlayable;
            set => _inputPlayable = value;
        }

        private TMProColorBehaviour _input;

        private TMProColorBehaviour input
        {
            get => _input;
            set => _input = value;
        }

        private int _sanitizedCharacterCount = 0;

        private int sanitizedCharacterCount
        {
            get => _sanitizedCharacterCount;
            set => _sanitizedCharacterCount = value;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as TMP_Text;

            if (trackBinding == null || trackBinding.gameObject.activeInHierarchy == false)
                return;
            
            inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<TMProColorBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();

                percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                
                if (inputWeight >= 1f) {

                    switch (input.textAnimationStyle) {

                        case TextAnimationStyle.Whole:
                            trackBinding.color = Color.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, percentageComplete));
                            break;

                        case TextAnimationStyle.Character:
                            if (sanitizedCharacterCount == 0) {
                                sanitizedCharacterCount = GetSanitizedCharacterCount(trackBinding);
                            }
                            AnimateCharacters(trackBinding);
                            break;

                    }
                }
                else {
                    if(timelineInstanceConfig.currentTime >= input.endTime) {
                        
                        switch (input.textAnimationStyle) {
                            
                            case TextAnimationStyle.Whole:
                                trackBinding.color = input.targetValue;
                                break;

                            case TextAnimationStyle.Character:
                                for (int j = 0; j < trackBinding.textInfo.characterCount; j++) {
                                    SetCharacterColor(trackBinding, input.targetValue, j);
                                }
                                break;
                        }
                        
                    } else if (i == 0 && timelineInstanceConfig.currentTime <= input.startTime) {
                        
                        switch (input.textAnimationStyle) {
                            
                            case TextAnimationStyle.Whole:
                                trackBinding.color = input.initialValue;
                                break;

                            case TextAnimationStyle.Character:
                                for (int j = 0; j < trackBinding.textInfo.characterCount; j++) {
                                    SetCharacterColor(trackBinding, input.initialValue, j);
                                }
                                break;
                        }
                    }
                }
            }
        }
        
        private void AnimateCharacters(TMP_Text textMeshPro)
        {
            TMP_TextInfo textInfo = textMeshPro.textInfo;
            int characterCount = textMeshPro.textInfo.characterCount;
            //int sanitizedCharacterIndex = 1;
            
            float baseAnimationLength = 1f / characterCount;
            double intervalLength = 1f / characterCount + (baseAnimationLength * input.blendValue);
            
            
            for (int j = 0; j < characterCount; j++) {
                        
                // if (textInfo.characterInfo[j].isVisible == false) {
                //     continue;
                // }
                        
                double intervalComplete = percentageComplete - (j * baseAnimationLength - (baseAnimationLength * input.blendValue));
                //sanitizedCharacterIndex++;

                if (Mathf.Abs((float)intervalComplete) <= input.blendValue)
                {
                    double modifier = intervalComplete / intervalLength;

                    Color targetColor;
                            
                    if (modifier >= 1f) {
                        targetColor = input.targetValue;
                    } else if (modifier <= 0) {
                        targetColor = input.initialValue;
                    }
                    else {
                        targetColor = Color.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, (float)modifier));
                    }
                            
                    SetCharacterColor(trackBinding, targetColor, j);
                } else if (intervalComplete >= 0) {
                    SetCharacterColor(trackBinding, input.targetValue, j);
                }
                else {
                    SetCharacterColor(trackBinding, input.initialValue, j);
                }
            }
        }

        // Need to set values using a sanitized list to
        // maximize the smoothness of our animation
        private static int GetSanitizedCharacterCount(TMP_Text textMeshPro)
        {
            int sanitizedCharacterCount = 0;
            
            for (int i = 0; i < textMeshPro.textInfo.characterCount; i++) {
                if (textMeshPro.textInfo.characterInfo[i].isVisible == true) {
                    sanitizedCharacterCount++;
                }
            }

            return sanitizedCharacterCount;
        }
        
        private static TMP_Text SetCharacterColor(TMP_Text textComponent, Color targetColor, int characterIndex)
        {
            TMP_TextInfo textInfo = textComponent.textInfo;
            
            if (characterIndex == -1 || characterIndex > textInfo.characterCount - 1) return textComponent;
            
            Color32 targetColor32 = targetColor;
            
            int materialIndex = textInfo.characterInfo[characterIndex].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[characterIndex].vertexIndex;
                
            Color32[] characterColors = textInfo.meshInfo[materialIndex].colors32;
            characterColors[vertexIndex + 0] = targetColor32;
            characterColors[vertexIndex + 1] = targetColor32;
            characterColors[vertexIndex + 2] = targetColor32;
            characterColors[vertexIndex + 3] = targetColor32;

            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            return textComponent;
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if (Application.isPlaying == true && isScrubbing == true) {
                if (trackBinding != null) {    
                    trackBinding.color = Utils.transparent;
                }
            }
        }
    }   
}