﻿// Adapted from TerraFX.Interop.Windows, MIT license

namespace FluentAvalonia.Interop.Win32;

internal readonly unsafe struct HWND : IComparable, IEquatable<HWND>
{
    public readonly void* Value;

    public HWND(void* value)
    {
        Value = value;
    }

    public static HWND INVALID_VALUE => new HWND((void*)(-1));

    public static HWND NULL => new HWND(null);

    public static bool operator ==(HWND left, HWND right) => left.Value == right.Value;

    public static bool operator !=(HWND left, HWND right) => left.Value != right.Value;

    public static bool operator <(HWND left, HWND right) => left.Value < right.Value;

    public static bool operator <=(HWND left, HWND right) => left.Value <= right.Value;

    public static bool operator >(HWND left, HWND right) => left.Value > right.Value;

    public static bool operator >=(HWND left, HWND right) => left.Value >= right.Value;

    public static explicit operator HWND(void* value) => new HWND(value);

    public static implicit operator void*(HWND value) => value.Value;

    public static explicit operator HWND(byte value) => new HWND((void*)(value));

    public static explicit operator byte(HWND value) => (byte)(value.Value);

    public static explicit operator HWND(short value) => new HWND((void*)(value));

    public static explicit operator short(HWND value) => (short)(value.Value);

    public static explicit operator HWND(int value) => new HWND((void*)(value));

    public static explicit operator int(HWND value) => (int)(value.Value);

    public static explicit operator HWND(long value) => new HWND((void*)(value));

    public static explicit operator long(HWND value) => (long)(value.Value);

    public static explicit operator HWND(nint value) => new HWND((void*)(value));

    public static implicit operator nint(HWND value) => (nint)(value.Value);

    public static explicit operator HWND(sbyte value) => new HWND((void*)(value));

    public static explicit operator sbyte(HWND value) => (sbyte)(value.Value);

    public static explicit operator HWND(ushort value) => new HWND((void*)(value));

    public static explicit operator ushort(HWND value) => (ushort)(value.Value);

    public static explicit operator HWND(uint value) => new HWND((void*)(value));

    public static explicit operator uint(HWND value) => (uint)(value.Value);

    public static explicit operator HWND(ulong value) => new HWND((void*)(value));

    public static explicit operator ulong(HWND value) => (ulong)(value.Value);

    public static explicit operator HWND(nuint value) => new HWND((void*)(value));

    public static implicit operator nuint(HWND value) => (nuint)(value.Value);

    public int CompareTo(object obj)
    {
        if (obj is HWND other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException("obj is not an instance of HWND.");
    }

    //public int CompareTo(HWND other) => ((nuint)(Value)).CompareTo((nuint)(other.Value));

    public override bool Equals(object obj) => (obj is HWND other) && Equals(other);

    public bool Equals(HWND other) => ((nuint)(Value)).Equals((nuint)(other.Value));

    public override int GetHashCode() => ((nuint)(Value)).GetHashCode();

    //public override string ToString() => ((nuint)(Value)).ToString((sizeof(nint) == 4) ? "X8" : "X16");

    //public string ToString(string? format, IFormatProvider? formatProvider) => ((nuint)(Value)).ToString(format, formatProvider);
}



