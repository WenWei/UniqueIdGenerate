using System;

namespace UniqueIdGenerate
{
    public class DistributedSystemUniqueIdGeneration
    {
        private readonly ushort _machineId;
        private long _lastTimestamp = -1L;
        private long _sequence = 0L;
        private static long _sequenceMask = -1L ^ -1L << 24;
        private static readonly ushort SequenceBits = 24;
        private static readonly ushort MachineIdBits = 8;
        private static readonly ushort MachineIdShift = SequenceBits;
        private static readonly ushort TimestampLeftShift = (ushort)(SequenceBits + MachineIdBits);

        public DistributedSystemUniqueIdGeneration(ushort machineId)
        {
            _machineId = machineId;
        }

        public long NextId()
        {
            var timestamp = this.GetTimestamp();
            if(_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & _sequenceMask;
                if(_sequence == 0)
                {
                    timestamp = GetNextTimestamp(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0;
            }

            if(timestamp < _lastTimestamp)
            {
                try
                {
                    throw new Exception($"Clock moved backwards.  Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");
                }
                catch(Exception)
                {
                    throw;
                }
            }

            _lastTimestamp = timestamp;

            long nextId = (timestamp << TimestampLeftShift) | (_machineId << MachineIdShift) | (_sequence);
            Console.WriteLine("timestamp:" + timestamp +
                ",timestampLeftShift:" + TimestampLeftShift +
                ",nextId:" + nextId + ",machineId:" + _machineId +
                ",sequence:" + _sequence);
            return nextId;
        }

        private long GetNextTimestamp(long lastTimestamp)
        {
            var timestamp = this.GetTimestamp();
            while(timestamp <= lastTimestamp)
            {
                timestamp = this.GetTimestamp();
            }
            return timestamp;
        }

        public long GetTimestamp()
        {
            return DateTime.UtcNow.ToUnixTimestamp();
        }

    }

    public static class DateTimeExtensions
    {
        private static readonly DateTime UnixTimestampStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimestamp(this DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value.ToUniversalTime() - UnixTimestampStart);

            //return the total seconds (which is a UNIX timestamp)
            return (long)span.TotalSeconds;
        }
    }
}
