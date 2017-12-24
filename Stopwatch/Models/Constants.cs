namespace Stopwatch.Models
{
    public static class Constants
    {
        public static string TimeSpanFormat { get; } = @"hh\:mm\:ss\""fff";

        public static string TimeSpanFormatNoMillsecond { get; } = @"hh\:mm\:ss\""";

        public static string DateTimeFormat { get; } = "yyyy/MM/dd HH:mm:ss";

        public static string StartLabel { get; } = "Start";
        public static string StopLabel { get; } = "Stop";
        public static string ResetLabel { get; } = "Reset";
    }
}