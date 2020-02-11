namespace AltSalt.Maestro
{
    public interface IRegisterConditionResponseActionData : IRegisterActionData
    {
        EventExecutionType eventExecutionType { get; set; }
    }
}