namespace AltSalt.Maestro
{
    public interface IRegisterActionTriggerBehaviour
    {
#if UNITY_EDITOR
        void SyncTriggerDescriptions();
#endif
    }
}