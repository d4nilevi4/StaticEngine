using System.Runtime.InteropServices;

namespace GravitySimulation;

// raylib's rlDrawVertexArrayElements is hardcoded to GL_TRIANGLES.
// We need GL_LINES, so call glDrawElements directly via the platform GL library.
internal static class GLNative
{
    public const int LINES = 0x0001;
    public const int UNSIGNED_INT = 0x1405;

    public static void DrawElementsLines(int count)
    {
        if (OperatingSystem.IsMacOS())
            Mac.glDrawElements(LINES, count, UNSIGNED_INT, IntPtr.Zero);
        else if (OperatingSystem.IsWindows())
            Win.glDrawElements(LINES, count, UNSIGNED_INT, IntPtr.Zero);
        else
            Lin.glDrawElements(LINES, count, UNSIGNED_INT, IntPtr.Zero);
    }

    private static class Mac
    {
        [DllImport("/System/Library/Frameworks/OpenGL.framework/OpenGL")]
        public static extern void glDrawElements(int mode, int count, int type, IntPtr indices);
    }

    private static class Win
    {
        [DllImport("opengl32.dll")]
        public static extern void glDrawElements(int mode, int count, int type, IntPtr indices);
    }

    private static class Lin
    {
        [DllImport("libGL.so.1")]
        public static extern void glDrawElements(int mode, int count, int type, IntPtr indices);
    }
}