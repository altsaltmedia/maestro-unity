namespace AltSalt.Maestro
{
    public interface ISceneDimensionListener
    {
        float sceneAspectRatio { get; set; }
        
        float sceneWidth { get; set;}
        
        float sceneHeight { get; set; }
    }
}