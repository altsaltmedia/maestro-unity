using UnityEngine.SceneManagement;

namespace AltSalt.Maestro
{
    public interface IDynamicLayoutElement
    {
        string elementName {
            get;
        }
        
        ComplexEventManualTrigger enableDynamicElement { get; }

        ComplexEventManualTrigger disableDynamicElement { get; }
        
        Scene parentScene {
            get;
        }
        
        int priority {
            get;
        }
        
        bool logElementOnLayoutUpdate {
            get;
        }
        
        void CallExecuteLayoutUpdate(UnityEngine.Object callingObject);
    }
}