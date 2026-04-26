using System.Diagnostics;
using Raylib_cs;

namespace GravitySimulation;

public static class Profiler
{
    public const int HistorySize = 200;

    sealed class Timer(string name, Color color)
    {
        public readonly string Name = name;
        public readonly Color Color = color;
        public readonly float[] History = new float[HistorySize];
        public float Current;
    }

    static readonly List<Timer> _timers = [];
    static readonly Dictionary<string, Timer> _map = [];
    static int _head;

    public static void Register(string name, Color color)
    {
        var timer = new Timer(name, color);
        _timers.Add(timer);
        _map[name] = timer;
    }

    public static long Timestamp() => Stopwatch.GetTimestamp();

    public static float ElapsedMs(long start) =>
        (float)Stopwatch.GetElapsedTime(start).TotalMilliseconds;

    public static void Record(string name, float ms)
    {
        if (_map.TryGetValue(name, out var t)) t.Current = ms;
    }

    public static void EndFrame()
    {
        foreach (var t in _timers) t.History[_head] = t.Current;
        _head = (_head + 1) % HistorySize;
    }

    public static void Draw(int screenW, int screenH)
    {
        float total = 0f;
        int ty = 70;
        foreach (var t in _timers)
        {
            total += t.Current;
            Raylib.DrawText($"{t.Name,-10} {t.Current,6:F2} ms", 10, ty, 16, t.Color);
            ty += 18;
        }

        Raylib.DrawText($"{"Total",-10} {total,6:F2} ms", 10, ty, 16, Color.White);

        int graphX = screenW - HistorySize - 10;
        int graphY = screenH - 130;
        int graphH = 110;

        Raylib.DrawRectangle(graphX - 2, graphY - 2, HistorySize + 4, graphH + 4, new Color(0, 0, 0, 160));

        float maxTotal = 1f;
        for (int i = 0; i < HistorySize; i++)
        {
            float sum = 0f;
            foreach (var t in _timers) sum += t.History[i];
            if (sum > maxTotal) maxTotal = sum;
        }

        float targetY = graphY + graphH - (16.67f / maxTotal) * graphH;
        if (targetY >= graphY)
            Raylib.DrawLine(graphX, (int)targetY, graphX + HistorySize, (int)targetY, new Color(255, 255, 255, 80));

        for (int i = 0; i < HistorySize; i++)
        {
            int idx = (_head + i) % HistorySize;
            int x = graphX + i;
            float scale = graphH / maxTotal;
            int bottom = graphY + graphH;
            foreach (var t in _timers)
                DrawBar(x, ref bottom, (int)(t.History[idx] * scale), t.Color);
        }

        Raylib.DrawText($"{maxTotal:F1}ms", graphX - 2, graphY - 14, 12, new Color(200, 200, 200, 180));
        Raylib.DrawText("0", graphX - 2, graphY + graphH + 2, 12, new Color(200, 200, 200, 180));
    }

    static void DrawBar(int x, ref int bottom, int height, Color color)
    {
        if (height < 1) return;
        bottom -= height;
        Raylib.DrawLine(x, bottom, x, bottom + height, color);
    }
}