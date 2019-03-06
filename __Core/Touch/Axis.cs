using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{   
    [CreateAssetMenu(menuName = "AltSalt/Axis")]
	public class Axis : ScriptableObject {

        [ValueDropdown("dropdownValues")]
        public string Name = "";

        private string[] dropdownValues = new string[]{
            "x",
            "y",
            "z",
            "transition"
        };

        public bool Active = false;
	}

}