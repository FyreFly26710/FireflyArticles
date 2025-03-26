using System.Security.Principal;
using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.Common.Utils;
public static class EntityUtil
{
    private static readonly object _lock = new object();

    private static readonly long _epoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / TimeSpan.TicksPerMillisecond;

    private const int TimestampShift = 22;
    private const int MachineIdShift = 12;
    private const long MaxSequence = 4095; // 12 bits

    private static long _lastTimestamp = -1L;
    private static long _sequence = 0L;

    public static long MachineId { get; set; } = 1; // set this per instance

    public static long GenerateSnowflakeId()
    {
        lock (_lock)
        {
            long timestamp = GetCurrentTimestamp();

            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & MaxSequence;
                if (_sequence == 0)
                {
                    // Wait for the next millisecond
                    while (timestamp <= _lastTimestamp)
                    {
                        timestamp = GetCurrentTimestamp();
                    }
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;

            return (timestamp << TimestampShift) |
                   (MachineId << MachineIdShift) |
                   _sequence;
        }
    }

    private static long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _epoch;
    }
}
