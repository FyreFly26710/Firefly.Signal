namespace Firefly.Signal.SharedKernel.Domain;

public static class SnowflakeId
{
    private const int MachineIdBits = 4;
    private const int SequenceBits = 6;
    private const int TimestampPrecision = 5;
    private const long MaxSequence = (1L << SequenceBits) - 1;
    private static readonly object SyncRoot = new();
    private static readonly long Epoch = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();

    private static long _lastTimestamp = -1L;
    private static long _machineId = 1L;
    private static long _sequence;

    public static void Initialize(long machineId)
    {
        if (machineId < 0 || machineId >= (1L << MachineIdBits))
        {
            throw new ArgumentOutOfRangeException(nameof(machineId), $"Machine ID must be between 0 and {(1L << MachineIdBits) - 1}.");
        }

        _machineId = machineId;
    }

    public static long GenerateId()
    {
        lock (SyncRoot)
        {
            var timestamp = GetCurrentTimestamp();

            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & MaxSequence;
                if (_sequence == 0)
                {
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

            return (timestamp << (MachineIdBits + SequenceBits))
                   | (_machineId << SequenceBits)
                   | _sequence;
        }
    }

    private static long GetCurrentTimestamp()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Epoch;
        return timestamp >> TimestampPrecision;
    }
}
