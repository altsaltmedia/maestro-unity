using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Logic
{
    [Serializable]
    public class LerpVariable
    {
        [SerializeField]
        public VariableBase variableTarget;

        [SerializeField]
        [ShowIf(nameof(FloatPopulated))]
        public float targetFloat;

        [SerializeField]
        [ShowIf(nameof(ColorPopulated))]
        public Color targetColor;

        [SerializeField]
        [ShowIf(nameof(V3Populated))]
        public Vector3 targetV3;

        [SerializeField]
        [ShowIf(nameof(IntPopulated))]
        public int targetInt;

        [SerializeField]
        public float duration = float.NaN;

        public delegate void VariableCallbackDelegate();
        public static VariableCallbackDelegate variableCallbackDelegate = () => { };

        public LerpVariable(VariableBase variableTarget, object targetValue, float duration)
        {
            this.variableTarget = variableTarget;

            if(targetValue is float) {
                this.targetFloat = (float)targetValue;
            } else if(targetValue is Color) {
                this.targetColor = (Color)targetValue;
            } else if(targetValue is Vector3) {
                this.targetV3 = (Vector3)targetValue;
            } else if(targetValue is int) {
                this.targetInt = (int)targetValue;
            }

            this.duration = duration;
        }

        public void LerpToTargetValue()
        {
            switch(variableTarget.GetType().Name) {

                case nameof(FloatVariable): {
                        FloatVariable variable = variableTarget as FloatVariable;
                        DOTween.To(() => variable.value, x => variable.value = x, targetFloat, duration).OnComplete(() => {
                            variableCallbackDelegate.Invoke();
                        });
                        break;
                    }

                case nameof(ColorVariable): {
                        ColorVariable variable = variableTarget as ColorVariable;
                        DOTween.To(() => variable.value, x => variable.value = x, targetColor, duration).OnComplete(() => {
                            variableCallbackDelegate.Invoke();
                        });
                        break;
                    }

                case nameof(V3Variable): {
                        V3Variable variable = variableTarget as V3Variable;
                        DOTween.To(() => variable.value, x => variable.value = x, targetV3, duration).OnComplete(() => {
                            variableCallbackDelegate.Invoke();
                        });
                        break;
                    }

                case nameof(IntVariable): {
                        IntVariable variable = variableTarget as IntVariable;
                        DOTween.To(() => variable.value, x => variable.value = x, targetInt, duration).OnComplete(() => {
                            variableCallbackDelegate.Invoke();
                        });
                        break;
                    }
            }
        }

        bool FloatPopulated()
        {
            if (variableTarget is FloatVariable) {
                return true;
            }

            return false;
        }

        bool ColorPopulated()
        {
            if (variableTarget is ColorVariable) {
                return true;
            }

            return false;
        }

        bool V3Populated()
        {
            if (variableTarget is V3Variable) {
                return true;
            }

            return false;
        }

        bool IntPopulated()
        {
            if (variableTarget is IntVariable) {
                return true;
            }

            return false;
        }
    }

}
