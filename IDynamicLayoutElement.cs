using UnityEngine.SceneManagement;

namespace AltSalt.Maestro
{
    public interface IDynamicLayoutElement
    {
        string elementName {
            get;
        }
        
        ComplexEventTrigger dynamicElementEnable { get; }

        ComplexEventTrigger dynamicElementDisable { get; }
        
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