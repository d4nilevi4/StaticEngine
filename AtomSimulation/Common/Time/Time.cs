namespace AtomSimulation;

public struct Time
{
    public float DeltaTime;
    public float TimeScale;
    public bool Paused;

    public Time()
    {
        DeltaTime = 0;
        TimeScale = 1;
        Paused = false;
    }
}