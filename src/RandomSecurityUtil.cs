using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Soenneker.Utils.Random.Security;

public static class RandomSecurityUtil
{
    // 10^28 fits in 96 bits (decimal uses 96-bit integer + scale).
    // 10^28 (hex) = 0x204FCE5E3E25026110000000
    // Little-endian 32-bit limbs:
    //   lo  = 0x10000000
    //   mid = 0x3E250261
    //   hi  = 0x204FCE5E
    private const uint Ten28_Lo = 0x10000000;
    private const uint Ten28_Mid = 0x3E250261;
    private const uint Ten28_Hi = 0x204FCE5E;

    private const byte DecimalScale = 28;

    /// <summary>
    /// Generates a cryptographically secure random byte array of the specified length.
    /// </summary>
    /// <param name="length">The length of the byte array to generate. Must be non-negative.</param>
    /// <returns>A byte array filled with cryptographically secure random values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length"/> is negative.</exception>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetByteArray(int length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));

        if (length == 0)
            return Array.Empty<byte>();

        byte[] buffer = GC.AllocateUninitializedArray<byte>(length);
        RandomNumberGenerator.Fill(buffer);
        return buffer;
    }

    /// <summary>
    /// Generates a cryptographically secure random integer in the specified range [minValue, maxValue).
    /// </summary>
    /// <param name="minValue">Inclusive lower bound.</param>
    /// <param name="maxValue">Exclusive upper bound (must be greater than <paramref name="minValue"/>).</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next(int minValue, int maxValue)
    {
        // Unbiased, handles edge cases (including overflow-y ranges) correctly.
        return RandomNumberGenerator.GetInt32(minValue, maxValue);
    }

    /// <summary>
    /// Generates a cryptographically secure random decimal in the specified range [minValue, maxValue).
    /// </summary>
    /// <param name="minValue">Inclusive lower bound.</param>
    /// <param name="maxValue">Exclusive upper bound (must be greater than <paramref name="minValue"/>).</param>
    [Pure]
    public static decimal NextDecimal(decimal minValue, decimal maxValue)
    {
        if (minValue >= maxValue)
            throw new ArgumentOutOfRangeException(nameof(minValue), "minValue must be less than maxValue.");

        // [0,1) * range + min
        return minValue + (GetRandomFraction() * (maxValue - minValue));
    }

    /// <summary>
    /// Generates a cryptographically secure random fraction in the range [0, 1).
    /// </summary>
    /// <remarks>
    /// Produces a decimal with 28 digits of scale using unbiased rejection sampling over [0, 10^28).
    /// Allocation-free and avoids floating point.
    /// </remarks>
    [Pure]
    public static decimal GetRandomFraction()
    {
        Span<byte> bytes = stackalloc byte[12];

        while (true)
        {
            RandomNumberGenerator.Fill(bytes);

            uint lo = ReadUInt32LE(bytes, 0);
            uint mid = ReadUInt32LE(bytes, 4);
            uint hi = ReadUInt32LE(bytes, 8);

            // Reject if (hi:mid:lo) >= 10^28 to keep result in [0, 1).
            if (IsGreaterOrEqual96(hi, mid, lo, Ten28_Hi, Ten28_Mid, Ten28_Lo))
                continue;

            // value / 10^28 (via scale=28) => [0,1)
            return new decimal((int)lo, (int)mid, (int)hi, isNegative: false, scale: DecimalScale);
        }
    }

    /// <summary>
    /// Generates a cryptographically secure random double in the range [0.0, 1.0).
    /// </summary>
    /// <remarks>
    /// Uses 53 random bits (the precision of IEEE 754 double mantissa) for a uniform distribution over k/2^53.
    /// </remarks>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NextDouble()
    {
        ulong r = RandomUInt64();
        r >>= 11; // keep top 53 bits
        return r * (1.0 / (1UL << 53));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsGreaterOrEqual96(uint aHi, uint aMid, uint aLo, uint bHi, uint bMid, uint bLo)
    {
        if (aHi != bHi) return aHi > bHi;
        if (aMid != bMid) return aMid > bMid;
        return aLo >= bLo;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong RandomUInt64()
    {
        Span<byte> b = stackalloc byte[8];
        RandomNumberGenerator.Fill(b);

        return
            ((ulong)b[0]) |
            ((ulong)b[1] << 8) |
            ((ulong)b[2] << 16) |
            ((ulong)b[3] << 24) |
            ((ulong)b[4] << 32) |
            ((ulong)b[5] << 40) |
            ((ulong)b[6] << 48) |
            ((ulong)b[7] << 56);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ReadUInt32LE(ReadOnlySpan<byte> bytes, int offset)
    {
        return
            (uint)bytes[offset] |
            ((uint)bytes[offset + 1] << 8) |
            ((uint)bytes[offset + 2] << 16) |
            ((uint)bytes[offset + 3] << 24);
    }
}
