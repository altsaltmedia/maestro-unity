/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{   
	[Serializable]
	public class BoolReference : VariableReferenceBase
    {
		public bool UseConstant = false;

        [PropertySpace]

        [ValueDropdown("boolValueList")]
		public bool ConstantValue;


        private ValueDropdownList<bool> boolValueList = new ValueDropdownList<bool>(){
            {"TRUE", true },
            {"FALSE", false }
        };

        [PropertySpace]

		public BoolVariable Variable;
		
		public BoolReference()
		{ }
		
		public BoolReference(bool value)
		{
			UseConstant = true;
			ConstantValue = value;
		}
		
		public bool Value {
			get { return UseConstant ? ConstantValue : Variable.value; }
		}
		
		public static implicit operator bool(BoolReference reference)
		{
			return reference.Value;
		}
	}	
}