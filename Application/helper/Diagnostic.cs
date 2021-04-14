using System;
using System.Diagnostics;
namespace MA.Helper
{
    public static class Diagnostic
    {
        public static TimeSpan MeasureTime(Action action)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            action();
            watch.Stop();
            var name = action.GetType().Name;
            if (watch.Elapsed.TotalSeconds >= 1)
            {
                System.Console.WriteLine($"Operation took {watch.Elapsed.TotalSeconds} seconds.");
            }
            else
            {
                System.Console.WriteLine($"Operation took {watch.ElapsedMilliseconds} milliseconds.");
            }

            return watch.Elapsed;
        }
    }
}