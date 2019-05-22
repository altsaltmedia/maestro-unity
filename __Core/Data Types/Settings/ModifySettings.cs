using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Modify/Modify Settings")]
    public class ModifySettings : ScriptableObject
    {
        [Header("Modify Settings")]
        [Required]
        public TextFamily defaultTextFamily;

        [Required]
        public TextFamily activeTextFamily;

        [Required]
        public Layout defaultLayout;

        [Required]
        public Layout activeLayout;
    }
}