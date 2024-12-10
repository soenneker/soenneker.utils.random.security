using System;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;

namespace Soenneker.Utils.Random.Security;

public static class RandomSecurityUtil
{
    /// <summary>
    /// Generates a cryptographically secure random byte array of the specified length.
    /// </summary>
    /// <param name="length">The length of the byte array to generate. Must be non-negative.</param>
    /// <returns>A byte array filled with cryptographically secure random values.</returns>
    /// <remarks>
    /// This method uses <see cref="GC.AllocateUninitializedArray{T}"/> to optimize allocation for large arrays,
    /// and fills the array with secure random bytes using <see cref="RandomNumberGenerator.Fill(Span{byte})"/>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length"/> is negative.</exception>
    [Pure]
    public static byte[] GetByteArray(int length)
    {
        byte[] buffer = GC.AllocateUninitializedArray<byte>(length);
        RandomNumberGenerator.Fill(buffer.AsSpan());
        return buffer;
    }

    /// <summary>
    /// Generates a cryptographically secure random integer in the specified range.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number returned. Must be greater than <paramref name="minValue"/>.</param>
    /// <returns>A cryptographically secure random integer between <paramref name="minValue"/> (inclusive) and <paramref name="maxValue"/> (exclusive).</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="minValue"/> is greater than or equal to <paramref name="maxValue"/>.
    /// </exception>
    [Pure]
    public static int Next(int minValue, int maxValue)
    {
        if (minValue >= maxValue)
            throw new ArgumentOutOfRangeException(nameof(minValue), "minValue must be less than maxValue.");

        int range = maxValue - minValue;

        if (range <= byte.MaxValue) // Optimize for small ranges
        {
            Span<byte> smallBuffer = stackalloc byte[1];
            RandomNumberGenerator.Fill(smallBuffer);
            return minValue + smallBuffer[0] % range;
        }

        Span<byte> buffer = stackalloc byte[4];
        while (true)
        {
            RandomNumberGenerator.Fill(buffer);
            int randomInt = BitConverter.ToInt32(buffer) & int.MaxValue;
            if (randomInt < int.MaxValue - int.MaxValue % range)
                return minValue + randomInt % range;
        }
    }

    /// <summary>
    /// Generates a cryptographically secure random decimal in the specified range.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number returned. Must be greater than <paramref name="minValue"/>.</param>
    /// <returns>A cryptographically secure random decimal between <paramref name="minValue"/> (inclusive) and <paramref name="maxValue"/> (exclusive).</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="minValue"/> is greater than or equal to <paramref name="maxValue"/>.
    /// </exception>
    [Pure]
    public static decimal NextDecimal(decimal minValue, decimal maxValue)
    {
        if (minValue >= maxValue)
            throw new ArgumentOutOfRangeException(nameof(minValue), "minValue must be less than maxValue.");

        // Calculate the range
        decimal range = maxValue - minValue;

        // Generate a random fraction between 0 (inclusive) and 1 (exclusive)
        decimal randomFraction = GetRandomFraction();

        // Scale the random fraction to the desired range
        return minValue + randomFraction * range;
    }

    /// <summary>
    /// Generates a cryptographically secure random fraction between 0 (inclusive) and 1 (exclusive).
    /// </summary>
    /// <returns>A random decimal between 0 and 1.</returns>
    [Pure]
    public static decimal GetRandomFraction()
    {
        // Decimal has 96 bits for the mantissa (3 * 4 bytes)
        Span<byte> buffer = stackalloc byte[12]; // 12 bytes = 96 bits
        RandomNumberGenerator.Fill(buffer);

        // Convert the 96 bits into an integer array
        int[] bits =
        [
            BitConverter.ToInt32(buffer.Slice(0, 4)), // Low
            BitConverter.ToInt32(buffer.Slice(4, 4)), // Mid
            BitConverter.ToInt32(buffer.Slice(8, 4)), // High
            0 // Flags: Positive number, scale = 0
        ];

        // Create a decimal from the bits
        var result = new decimal(bits);

        // Ensure the value is between 0 and 1 by dividing by a large power of 10
        return result / (decimal)Math.Pow(10, 28);
    }

    /// <summary>
    /// Generates a cryptographically secure random double-precision floating-point number in the range [0.0, 1.0).
    /// </summary>
    /// <remarks>Use this for double use. Use <see cref="NextDecimal(decimal, decimal)"/> for decimal use.</remarks>
    /// <returns>A cryptographically secure random double between 0.0 (inclusive) and 1.0 (exclusive).</returns>
    [Pure]
    public static double NextDouble()
    {
        // Generate 8 bytes for a 64-bit integer
        Span<byte> buffer = stackalloc byte[8];
        RandomNumberGenerator.Fill(buffer);

        // Convert the 8 bytes into a 64-bit unsigned integer
        var randomInt = BitConverter.ToUInt64(buffer);

        // Normalize to the range [0.0, 1.0)
        return randomInt / (ulong.MaxValue + 1.0); // Use a double for the divisor
    }
}