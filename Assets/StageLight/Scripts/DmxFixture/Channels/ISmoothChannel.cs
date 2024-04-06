namespace StageLight.DmxFixture.Channels
{
    public interface ISmoothChannel
    {
        bool IsSmooth { get; set; }
        float SmoothTime { get; set; }
        float SmoothMaxSpeed { get; set; }
    }
}
