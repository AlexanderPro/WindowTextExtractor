//*****************************************************************************************************************
//https://github.com/gapotchenko/Gapotchenko.FX/blob/master/Source/Gapotchenko.FX.Diagnostics.Process/Pal/UniPtr.cs
//*****************************************************************************************************************

using System;

namespace WindowTextExtractor.Diagnostics
{
    /// <summary>
    /// Universal pointer that can hold both 32- and 64-bit values.
    /// </summary>
    readonly struct UniPtr : IEquatable<UniPtr>
    {
        public UniPtr(IntPtr value)
        {
            m_Value = value.ToInt64();
            m_Size = (byte)IntPtr.Size;
        }

        public UniPtr(int value)
        {
            m_Value = value;
            m_Size = sizeof(int);
        }

        public UniPtr(long value)
        {
            m_Value = value;
            m_Size = sizeof(long);
        }

        public UniPtr(ulong value) :
            this((long)value)
        {
        }

        UniPtr(long value, int size)
        {
            m_Value = value;
            m_Size = checked((byte)size);
        }

        readonly long m_Value;
        readonly byte m_Size;

        public int Size => m_Size;

        public long ToInt64() => m_Value;

        public ulong ToUInt64() => (ulong)m_Value;

        public bool FitsInNativePointer => m_Size <= IntPtr.Size;

        public bool CanBeRepresentedByNativePointer
        {
            get
            {
                int actualSize = m_Size;

                if (actualSize == 8)
                {
                    if (m_Value >> 32 == 0)
                        actualSize = 4;
                }

                return actualSize <= IntPtr.Size;
            }
        }

        public static implicit operator IntPtr(UniPtr p) => new IntPtr(p.ToInt64());

        public static implicit operator UniPtr(IntPtr p) => new UniPtr(p);

        public static UniPtr operator +(UniPtr a, long b) => new UniPtr(a.ToInt64() + b, a.Size);

        public static bool operator ==(UniPtr a, UniPtr b) => a.ToUInt64() == b.ToUInt64();

        public static bool operator !=(UniPtr a, UniPtr b) => a.ToUInt64() != b.ToUInt64();

        public override int GetHashCode() => m_Value.GetHashCode();

        public override bool Equals(object obj) => obj is UniPtr other && Equals(other);

        public bool Equals(UniPtr other) => m_Value == other.m_Value;

        public override string ToString() => ((ulong)m_Value).ToString();
    }
}
