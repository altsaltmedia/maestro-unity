namespace AltSalt.Maestro
{
    public interface IRegisterConditionResponse
    {
#if UNITY_EDITOR        
        string conditionEventTitle { get; set; }
        
        string eventDescription { get; set; }
#endif        
    }
}