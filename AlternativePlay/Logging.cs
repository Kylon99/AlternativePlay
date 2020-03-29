using System;

namespace AlternativePlay
{
    public static class Logging
    {
        public static void Info(string message) => Log("INFO", message);
        public static void Warning(string message) => Log("WARNING", message);
        private static void Log(string level, string message) => Console.WriteLine($"[{AlternativePlay.assemblyName} | {level}] {message}");
    }
}
