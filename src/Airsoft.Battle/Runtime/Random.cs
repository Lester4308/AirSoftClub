using System;

namespace Airsoft.Battle
{
    public interface IRandomSource
    {
        uint NextUInt32();
        int NextInt(int exclusiveMax);
    }

    // SplitMix64, explicit unsigned wraparound; seed 0 is valid.
    public sealed class SeededRandom : IRandomSource
    {
        private ulong state;
        public SeededRandom(ulong seed) { state = seed; }
        public uint NextUInt32()
        {
            unchecked
            {
                ulong z = (state += 0x9E3779B97F4A7C15UL);
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                return (uint)((z ^ (z >> 31)) >> 32);
            }
        }
        public int NextInt(int exclusiveMax)
        {
            if (exclusiveMax <= 0) throw new ArgumentOutOfRangeException(nameof(exclusiveMax));
            // Multiply-high mapping: fixed one-draw cost, tiny documented quantization bias.
            return (int)(((ulong)NextUInt32() * (uint)exclusiveMax) >> 32);
        }
    }
}
