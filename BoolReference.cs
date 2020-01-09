/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{   
	[Serializable]
	public class BoolReference : VariableReference
    {
		[FormerlySerializedAs("ConstantValue")]
		[SerializeField]
		[ValueDropdown("boolValueList")]
		private bool _constantValue;

		private bool constantValue
		{
			get => _constantValue;
			set => _constantValue = value;
		}

        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        private BoolVariable _variable;

        public BoolVariable GetVariable(Object callingObject)
		{
#if UNITY_EDITOR
			this.parentObject = callingObject;
			if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
				searchAttempted = true;
				LogMissingReferenceMessage(GetType().Name);
				_variable = Utils.GetScriptableObject(referenceName) as BoolVariable;
				if (_variable != null) {
					LogFoundReferenceMessage(GetType().Name, _variable);
				}
			}
#endif
			return _variable;
		}
        
        public void SetVariable(BoolVariable value)
        {
	        _variable = value;
        }

		public BoolReference()
		{ }
		
		
		public BoolReference(bool value)
		{
			useConstant = true;
			constantValue = value;
		}
		
		public bool GetValue(UnityEngine.Object callingObject)
		{
			this.parentObject = callingObject;
			return useConstant ? constantValue : GetVariable(callingObject).value;
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