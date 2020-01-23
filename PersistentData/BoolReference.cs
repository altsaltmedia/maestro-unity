/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using Sirenix.OdinInspector;
using UnityEditor;
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
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private BoolVariable _variable;
        
        public BoolReference()
        { }
		
        public BoolReference(bool value)
        {
	        useConstant = true;
	        constantValue = value;
        }

        public override ScriptableObject GetVariable()
        {
	        base.GetVariable();
	        return _variable;
		}

        protected override bool ShouldPopulateReference()
        {
	        if (useConstant == false && _variable == null) {
		        return true;
	        }

	        return false;
        }

        protected override ScriptableObject ReadVariable()
        {
	        return _variable;
        }
        
        public void SetVariable(BoolVariable value)
        {
	        _variable = value;
        }

        public bool GetValue()
		{
			return useConstant ? constantValue : (GetVariable() as BoolVariable).value;
		}

		public BoolVariable SetValue(GameObject callingObject, bool targetValue)
		{
			if (useConstant == true) {
				LogDefaultChangeError(callingObject);
				return null;
			}

			BoolVariable boolVariable = GetVariable() as BoolVariable;
			boolVariable.StoreCaller(callingObject);
			boolVariable.SetValue(targetValue);
			return boolVariable;
		}
		
		public BoolVariable SetValue(GameObject callingObject, BoolVariable targetValue)
		{
			if (useConstant == true) {
				LogDefaultChangeError(callingObject);
				return null;
			}

			BoolVariable boolVariable = GetVariable() as BoolVariable;
			boolVariable.StoreCaller(callingObject);
			boolVariable.SetValue(targetValue.value);
			return boolVariable;
		}

		public BoolVariable Toggle(GameObject callingObject)
		{
			if (useConstant == true) {
				LogDefaultChangeError(callingObject);
				return null;
			}

			BoolVariable boolVariable = GetVariable() as BoolVariable;
			boolVariable.StoreCaller(callingObject);
			boolVariable.Toggle();
			return boolVariable;
		}
		
		public BoolVariable SetToDefaultValue(Object callingObject, string sourceScene, string sourceName)
		{
			if (useConstant == true) {
				LogDefaultChangeError(callingObject);
				return null;
			}

			BoolVariable boolVariable = GetVariable() as BoolVariable;
			boolVariable.StoreCaller(callingObject, sourceScene, sourceName);
			boolVariable.SetToDefaultValue();
			return boolVariable;
		}
    }	
}