using UnityEngine;
using UnityEditor;

namespace AltSalt
{
    public abstract class ChildUIElementsWindow : ScriptableObject
    {
        public abstract ChildUIElementsWindow Init(EditorWindow parentWindow);
    }
}