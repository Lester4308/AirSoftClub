using System;
using System.Globalization;

namespace Airsoft.Battle
{
    // Signed scaled integers; division truncates toward zero. Overflow throws.
    public readonly struct Fixed : IEquatable<Fixed>, IComparable<Fixed>
    {
        public const long Scale = 10_000;
        public long Raw { get; }
        private Fixed(long raw) { Raw = raw; }
        public static Fixed FromRaw(long raw) => new Fixed(raw);
        public static Fixed FromInt(long value) => new Fixed(checked(value * Scale));
        public static Fixed Ratio(long numerator, long denominator) =>
            new Fixed(checked(numerator * Scale) / denominator);
        public static Fixed Zero => FromRaw(0);
        public static Fixed One => FromRaw(Scale);
        public static Fixed operator +(Fixed a, Fixed b) => FromRaw(checked(a.Raw + b.Raw));
        public static Fixed operator -(Fixed a, Fixed b) => FromRaw(checked(a.Raw - b.Raw));
        public static Fixed operator *(Fixed a, Fixed b) => FromRaw(checked(a.Raw * b.Raw) / Scale);
        public static Fixed operator /(Fixed a, Fixed b) => FromRaw(checked(a.Raw * Scale) / b.Raw);
        public static Fixed Min(Fixed a, Fixed b) => a.Raw <= b.Raw ? a : b;
        public static Fixed Max(Fixed a, Fixed b) => a.Raw >= b.Raw ? a : b;
        public static Fixed Clamp(Fixed x, Fixed min, Fixed max) => Min(Max(x, min), max);
        public int CompareTo(Fixed other) => Raw.CompareTo(other.Raw);
        public bool Equals(Fixed other) => Raw == other.Raw;
        public override bool Equals(object? obj) => obj is Fixed other && Equals(other);
        public override int GetHashCode() => Raw.GetHashCode();
        public override string ToString() => Raw.ToString(CultureInfo.InvariantCulture) + "/" + Scale;
    }

    internal static class Require
    {
        public static int Range(int value, int min, int max, string name)
        {
            if (value < min || value > max) throw new ArgumentOutOfRangeException(name);
            return value;
        }
        public static Fixed Value(Fixed value, long maxUnits, string name, bool positive = false)
        {
            if (value.Raw < (positive ? 1 : 0) || value.Raw > checked(maxUnits * Fixed.Scale))
                throw new ArgumentOutOfRangeException(name);
            return value;
        }
        public static string Id(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 80)
                throw new ArgumentException("Expected nonempty identifier of at most 80 characters.", name);
            return value;
        }
    }
}
