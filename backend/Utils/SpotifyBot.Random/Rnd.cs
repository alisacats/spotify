using System;
using System.Security.Cryptography;

namespace SpotifyBot.Random
{
    public static class Rnd
    {
        const int BufferSize = sizeof(int) * 1024;
        static readonly byte[] Buffer = new byte[BufferSize];
        static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        static int _bufferIndex = BufferSize;

        static T UsingBts<T>(int count, Func<ArraySegment<byte>, T> f)
        {
            lock (Buffer)
            {
                if (_bufferIndex + count > BufferSize)
                {
                    Rng.GetNonZeroBytes(Buffer);
                    _bufferIndex = 0;
                }

                var span = new ArraySegment<byte>(Buffer, _bufferIndex, count);
                _bufferIndex += count;
                return f(span);
            }
        }

        public static int NextInt32() => UsingBts(4, bts => BitConverter.ToInt32(bts.Array, bts.Offset));

        public static int NextInt32(int minValue, int maxValue)
        {
            if (minValue > maxValue) throw new ArgumentException("minValue > maxValue", nameof(minValue));

            // Cast to long is easier then magic with uint
            var diff = (long) maxValue - minValue;
            if (diff == 0) return minValue;

            var shift = NextUInt32() % (diff + 1);
            return (int) (minValue + shift);
        }

        public static uint NextUInt32() => UsingBts(4, bts => BitConverter.ToUInt32(bts.Array, bts.Offset));

        public static long NextInt64() => UsingBts(8, bts => BitConverter.ToInt64(bts.Array, bts.Offset));

        public static byte[] NextBytes(int count) => UsingBts(count, bts =>
        {
            var res = new byte[count];
            System.Buffer.BlockCopy(bts.Array, bts.Offset, res, 0, count);
            return res;
        });
    }
}
