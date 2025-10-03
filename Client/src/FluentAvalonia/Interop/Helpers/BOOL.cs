﻿// Adapted from TerraFX.Interop.Windows, MIT license

namespace FluentAvalonia.Interop.Win32;

internal readonly struct BOOL : IComparable, IComparable<BOOL>, IEquatable<BOOL>, IFormattable
{
    public BOOL(int value)
    {
        Value = value;
    }

    public readonly int Value;

    public static BOOL FALSE => new BOOL(0);

    public static BOOL TRUE => new BOOL(1);

    public static bool operator ==(BOOL left, BOOL right) => left.Value == right.Value;

    public static bool operator !=(BOOL left, BOOL right) => left.Value != right.Value;

    public static bool operator <(BOOL left, BOOL right) => left.Value < right.Value;

    public static bool operator <=(BOOL left, BOOL right) => left.Value <= right.Value;

    public static bool operator >(BOOL left, BOOL right) => left.Value > right.Value;

    public static bool operator >=(BOOL left, BOOL right) => left.Value >= right.Value;

    public static implicit operator bool(BOOL value) => value.Value != 0;

    public static implicit operator BOOL(bool value) => new BOOL(value ? 1 : 0);

    public static bool operator false(BOOL value) => value.Value == 0;

    public static bool operator true(BOOL value) => value.Value != 0;

    public static implicit operator BOOL(byte value) => new BOOL(value);

    public static explicit operator byte(BOOL value) => (byte)(value.Value);

    public static implicit operator BOOL(short value) => new BOOL(value);

    public static explicit operator short(BOOL value) => (short)(value.Value);

    public static implicit operator BOOL(int value) => new BOOL(value);

    public static implicit operator int(BOOL value) => value.Value;

    public static explicit operator BOOL(long value) => new BOOL((int)(value));

    public static implicit operator long(BOOL value) => value.Value;

    public static explicit operator BOOL(nint value) => new BOOL((int)(value));

    public static implicit operator nint(BOOL value) => value.Value;

    public static implicit operator BOOL(sbyte value) => new BOOL(value);

    public static explicit operator sbyte(BOOL value) => (sbyte)(value.Value);

    public static implicit operator BOOL(ushort value) => new BOOL(value);

    public static explicit operator ushort(BOOL value) => (ushort)(value.Value);

    public static explicit operator BOOL(uint value) => new BOOL((int)(value));

    public static explicit operator uint(BOOL value) => (uint)(value.Value);

    public static explicit operator BOOL(ulong value) => new BOOL((int)(value));

    public static explicit operator ulong(BOOL value) => (ulong)(value.Value);

    public static explicit operator BOOL(nuint value) => new BOOL((int)(value));

    public static explicit operator nuint(BOOL value) => (nuint)(value.Value);

    public int CompareTo(object obj)
    {
        if (obj is BOOL other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException("obj is not an instance of BOOL.");
    }

    public int CompareTo(BOOL other) => Value.CompareTo(other.Value);

    public override bool Equals(object obj) => (obj is BOOL other) && Equals(other);

    public bool Equals(BOOL other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string format, IFormatProvider formatProvider) => Value.ToString(format, formatProvider);
}
