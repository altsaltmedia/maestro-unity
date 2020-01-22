using Sirenix.OdinInspector.Editor.Drawers;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class TMProCharacterColorMixerBehaviour : LerpToTargetMixerBehaviour
    {
        TMP_Text trackBinding;
        ScriptPlayable<ColorBehaviour> inputPlayable;
        ColorBehaviour input;

        private TMP_TextInfo _textInfo;

        private TMP_TextInfo textInfo
        {
            get => _textInfo;
            set => _textInfo = value;
        }

        private int _characterCount;

        private int characterCount
        {
            get => _characterCount;
            set => _characterCount = value;
        }

        private double _baseAnimationLength;

        private double baseAnimationLength
        {
            get => _baseAnimationLength;
            set => _baseAnimationLength = value;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as TMP_Text;

            if (!trackBinding)
                return;

            textInfo = trackBinding.textInfo;
            characterCount = trackBinding.textInfo.characterCount;
            inputCount = playable.GetInputCount ();

            // We need to find the 
            float sanitizedCharacterCount = 0f;
            
            for (int i = 0; i < trackBinding.textInfo.characterCount; i++) {
                if (trackBinding.textInfo.characterInfo[i].isVisible == true) {
                    sanitizedCharacterCount++;
                }
            }

//            for (int i = 0; i < inputCount; i++)
//            {
//                inputWeight = playable.GetInputWeight(i);
//                inputPlayable = (ScriptPlayable<ColorBehaviour>)playable.GetInput(i);
//                input = inputPlayable.GetBehaviour ();
//                baseAnimationLength = 1f / sanitizedCharacterCount;
//
//                double intervalLength = 1f / sanitizedCharacterCount + (baseAnimationLength * input.blendValue);
//
//                int sanitizedCharacterIndex = 1;
//                
//                if(inputWeight >= 1f) {
//                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
//                    for (int j = 0; j < characterCount; j++) {
//                        
//                        if (textInfo.characterInfo[j].isVisible == false) {
//                            continue;
//                        }
//                        
//                        double intervalComplete = percentageComplete - (sanitizedCharacterIndex * baseAnimationLength - (baseAnimationLength * input.blendValue));
//                        sanitizedCharacterIndex++;
//
//                        if (Mathf.Abs((float)intervalComplete) <= input.blendValue)
//                        {
//                            double modifier = intervalComplete / intervalLength;
//
//                            Color targetColor;
//                            
//                            if (modifier >= 1f) {
//                                targetColor = input.targetValue;
//                            } else if (modifier <= 0) {
//                                targetColor = input.initialValue;
//                            }
//                            else {
//                                targetColor = Color.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, (float)modifier));
//                            }
//                            
//                            SetCharacterColor(trackBinding, targetColor, j);
//                        } else if (intervalComplete >= 0) {
//                            SetCharacterColor(trackBinding, input.targetValue, j);
//                        }
//                        else {
//                            SetCharacterColor(trackBinding, input.initialValue, j);
//                        }
//                    }
//                } else {
//                    if(currentTime >= input.endTime) {
//                        SetAllCharacterColors(trackBinding, input.targetValue);
//                    } else if (i == 0 && currentTime <= input.startTime) {
//                        SetAllCharacterColors(trackBinding, input.initialValue);
//                    }
//                }
//            }
        }

        private static TMP_Text SetAllCharacterColors(TMP_Text textComponent, Color targetColor)
        {
            for (int i = 0; i < textComponent.textInfo.characterCount; i++) {
                SetCharacterColor(textComponent, targetColor, i);
            }

            return textComponent;
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
            if (Application.isPlaying == true && scrubberActive == true) {
                if (trackBinding != null) {    
                    trackBinding.color = Utils.transparent;
                }
            }
        }
    }   
}