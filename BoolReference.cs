/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{   
	[Serializable]
	public class BoolReference : VariableReference
    {
		[FormerlySerializedAs("ConstantValue")]
		[PropertySpace]
		[SerializeField]
		[ValueDropdown("boolValueList")]
		private bool _constantValue;

		private bool constantValue
		{
			get => _constantValue;
			set => _constantValue = value;
		}

        [FormerlySerializedAs("Variable")]
        [PropertySpace]
		[SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        private BoolVariable _variable;

        public BoolVariable GetVariable(Object callingObject)
		{
#if UNITY_EDITOR
			this.callingObject = callingObject;
			if (hasSearchedForAsset == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
				hasSearchedForAsset = true;
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

		public bool GetValue(UnityEngine.Object callingObject)
		{
			this.callingObject = callingObject;
			return useConstant ? constantValue : GetVariable(callingObject).value;
		}

		protected override void UpdateReferenceName()
		{
			if (GetVariable(callingObject) != null) {
				hasSearchedForAsset = false;
				referenceName = GetVariable(callingObject).name;
			}
//			else {
//				referenceName = "";
//			}
		}
		
    }	
}