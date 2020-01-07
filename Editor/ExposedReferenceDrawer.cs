using UnityEngine;
using Sirenix.OdinInspector.Editor.Drawers;

public sealed class ExposedReferenceDrawer<TValue> : DrawWithUnityBaseDrawer<ExposedReference<TValue>>
    where TValue : UnityEngine.Object
{
}