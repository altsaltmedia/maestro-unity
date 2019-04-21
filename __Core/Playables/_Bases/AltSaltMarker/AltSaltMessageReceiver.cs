using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class AltSaltMessageReceiver : MonoBehaviour, INotificationReceiver {

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        Debug.Log("Message Received");
    }

}