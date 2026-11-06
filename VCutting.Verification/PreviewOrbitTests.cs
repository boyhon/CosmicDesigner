using System.Reflection;
using VCutting;

internal static class PreviewOrbitTests
{
    public static void Run()
    {
        var method = typeof(Preview3DView).GetMethod("AdvanceOrbitAngle", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Orbit angle calculation is missing");
        var advance = method
            .CreateDelegate<Func<double, double, double>>();

        foreach (var direction in new[] { 1, -1 })
        {
            var angle = -20.0;
            for (var i = 0; i < 1600; i++)
            {
                var next = advance(angle, direction);
                if (Math.Abs(Math.IEEERemainder(next - angle, 360) - direction * .5) > 1e-9)
                    throw new InvalidOperationException("Orbit drag stopped or reversed");
                angle = next;
            }

            if (Math.Abs(advance(angle, -direction * 1600) + 20) > 1e-9)
                throw new InvalidOperationException("Reverse drag did not restore the original orientation");
        }
    }
}
