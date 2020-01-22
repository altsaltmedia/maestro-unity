using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
	[Serializable]
    public class AxisReference : ReferenceBase
    {
	    [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private Axis _variable;

        public Axis GetVariable(Object callingObject)
		{
#if UNITY_EDITOR
			this.parentObject = callingObject;
			if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
				searchAttempted = true;
				LogMissingReferenceMessage(GetType().Name);
				var variableSearch = Utils.GetScriptableObject(referenceName) as Axis;
				if (variableSearch != null) {
					_variable = variableSearch;
					EditorUtility.SetDirty(callingObject);
					LogFoundReferenceMessage(GetType().Name, _variable);
				}
			}
#endif
			return _variable;
		}
        
        public void SetVariable(Axis value)
        {
	        _variable = value;
        }
        
		public bool IsActive(UnityEngine.Object callingObject)
		{
			this.parentObject = callingObject;
			return GetVariable(callingObject).active;
		}

		public Axis SetStatus(GameObject callingObject, bool targetValue)
		{
			Axis axis = GetVariable(callingObject);
			axis.StoreCaller(callingObject);
			axis.SetStatus(targetValue);
			return axis;
		}
		
		public Axis SetStatus(GameObject callingObject, BoolVariable targetValue)
		{
			Axis axis = GetVariable(callingObject);
			axis.StoreCaller(callingObject);
			axis.SetStatus(targetValue.value);
			return axis;
		}
		
		public bool IsInverted(UnityEngine.Object callingObject)
		{
			this.parentObject = callingObject;
			return GetVariable(callingObject).inverted;
		}

		public Axis SetInverted(GameObject callingObject, bool targetValue)
		{
			Axis axis = GetVariable(callingObject);
			axis.StoreCaller(callingObject);
			axis.SetInverted(targetValue);
			return axis;
		}
		
		public Axis SetInverted(GameObject callingObject, BoolVariable targetValue)
		{
			Axis axis = GetVariable(callingObject);
			axis.StoreCaller(callingObject);
			axis.SetInverted(targetValue.value);
			return axis;
		}
		
		protected override void UpdateReferenceName()
		{
			if (GetVariable(parentObject) != null) {
				searchAttempted = false;
				referenceName = GetVariable(parentObject).name;
			}
//			else {
//				referenceName = "";
//			}
		}
    }
}