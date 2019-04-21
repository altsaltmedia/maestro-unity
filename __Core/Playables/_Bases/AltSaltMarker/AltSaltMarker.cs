using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class AltSaltMarker : SignalEmitter, INotification, INotificationOptionProvider
    {
        
        public PropertyName id {
            get {
                return new PropertyName();
            }
            
        }
        
        public override void OnInitialize(TrackAsset aPent) {

        }
        
        NotificationFlags INotificationOptionProvider.flags {
            get {
                return NotificationFlags.TriggerInEditMode;
                //return (retroactive ? NotificationFlags.Retroactive : default(NotificationFlags)) |
                //(emitOnce ? NotificationFlags.TriggerOnce : default(NotificationFlags));
            }
        }
    }
}