namespace AltSalt.Maestro
{
    public interface IRegisterActionData
    {
#if UNITY_EDITOR        
        string actionDescription { get; set; }
#endif        
    }
}