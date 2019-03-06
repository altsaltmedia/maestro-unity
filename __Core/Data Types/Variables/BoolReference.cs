/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;

namespace AltSalt
{   
	[Serializable]
	public class BoolReference
	{
		public bool UseConstant = false;
		public bool ConstantValue;
		public BoolVariable Variable;
		
		public BoolReference()
		{ }
		
		public BoolReference(bool value)
		{
			UseConstant = true;
			ConstantValue = value;
		}
		
		public bool Value {
			get { return UseConstant ? ConstantValue : Variable.Value; }
		}
		
		public static implicit operator bool(BoolReference reference)
		{
			return reference.Value;
		}
	}	
}