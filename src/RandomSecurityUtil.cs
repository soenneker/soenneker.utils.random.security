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
}