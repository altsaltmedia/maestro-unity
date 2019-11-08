using UnityEngine;
using UnityEditor;

namespace AltSalt.Maestro
{
    public abstract class ChildUIElementsWindow : ScriptableObject
    {
        public abstract ChildUIElementsWindow Init(EditorWindow parentWindow);
    }
}