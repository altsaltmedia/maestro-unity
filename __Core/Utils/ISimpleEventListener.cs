namespace AltSalt
{
    public interface ISimpleEventListener
    {
        void OnEventRaised();
        void LogName(string callingInfo);
    }
}