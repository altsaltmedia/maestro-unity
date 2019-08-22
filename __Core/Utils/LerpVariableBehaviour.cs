using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    public class LerpVariableBehaviour : MonoBehaviour
    {
        [SerializeField]
        List<LerpVariable> variableData = new List<LerpVariable>();

        [SerializeField]
        SimpleEventTrigger callback;

        //void LerpAllValues()
        //{
        //    for (int i = 0; i < variableData.Count; i++) {
        //        LerpToTargetValue(variableData[i]);
        //    }
        //}

        //void LerpSingleValue(VariableBase targetVariable)
        //{
        //    for (int i = 0; i < variableData.Count; i++) {
        //        if (variableData[i].variable == targetVariable) {
        //            LerpToTargetValue(variableData[i]);
        //        }
        //    }
        //}

        //public void SetTargetValue(float targetValue)
        //{
        //    this.targetFloat = targetValue;
        //}

        //public void SetTargetValue(Color targetValue)
        //{
        //    this.targetColor = targetValue;
        //}

        //public void SetTargetValue(Vector3 targetValue)
        //{
        //    this.targetV3 = targetValue;
        //}

        //public void SetTargetValue(int targetValue)
        //{
        //    this.targetInt = targetValue;
        //}

        //public void SetDuration(float duration)
        //{
        //    this.duration = duration;
        //}

    }
}

