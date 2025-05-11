namespace FF.Articles.Backend.Common.Utils;

// Consider using Guid instead, or maintain max id in redis.
// Requirement: Generate max safe long without needing to convert to string
public static class EntityUtil
{
    // |                                                                   m mmss ssss
    // 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000
    // Timestamp( 50+ bits)                                        | MachineId(3) | Sequence(6)
    // Max safe long is 2^53 - 1
    // (1000ms / (2^TimeStampPrecision)) * (2^SequenceBits) => id generated per second  => 1000/(2^5) * (2^6) = 2000 ids per second
    // avaialble bits for timestamp = 53 - MachineIdBits - SequenceBits = 44 bits
    // 41 bits, 1ms per tick lasts for 69 years
    private const int MachineIdBits = 3; // support 2^3-1 = 7 machines
    private const int SequenceBits = 6; // 2^6-1 = 63 ids per period
    private const int TimeStampPrecision = 5; // 2^5 = 32 ms a period

    private const long MaxSequence = (1L << SequenceBits) - 1;
    private static long _lastTimestamp = -1L;
    private static long _sequence = 0L;
    private static long _machineId = 1L;

    private static readonly object _lock = new object();
    public static long GenerateSnowflakeId()
    {
        lock (_lock)
        {
            long timestamp = GetCurrentTimestamp();
            if (timestamp < _lastTimestamp)
            {
                throw new InvalidOperationException(
                    $"Clock moved backwards. Refusing to generate ID for {(_lastTimestamp - timestamp) * 32} milliseconds.");
            }
            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & MaxSequence;
                if (_sequence == 0)
                {
                    // Wait for the next round
                    while (timestamp <= _lastTimestamp)
                    {
                        Thread.Sleep(1);
                        timestamp = GetCurrentTimestamp();
                    }
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;

            return (timestamp << (MachineIdBits + SequenceBits)) |
                   (_machineId << SequenceBits) |
                   _sequence;
        }
    }
    private static readonly long _epoch = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
    private static long GetCurrentTimestamp()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _epoch;
        return timestamp >> TimeStampPrecision;
    }
}
