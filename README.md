[![](https://img.shields.io/nuget/v/soenneker.utils.random.security.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.random.security/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.random.security/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.random.security/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.random.security.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.random.security/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.random.security/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.random.security/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.Random.Security
Generates cryptographically secure random bytes, bounded integers, decimals, and unit-interval doubles with `RandomNumberGenerator`.

## Installation

```bash
dotnet add package Soenneker.Utils.Random.Security
```

## Quick start

```csharp
using Soenneker.Utils.Random.Security;
```

Call the static `RandomSecurityUtil` methods directly; no dependency-injection registration is required.

The methods use `RandomNumberGenerator` and are appropriate when unpredictability matters. They do
not manage storage, encoding, expiration, comparison, or secret rotation for you.

## Secret bytes

```csharp
using System.Security.Cryptography;

byte[] secret = RandomSecurityUtil.GetByteArray(32); // 256 random bits

try
{
    UseSecret(secret);
}
finally
{
    CryptographicOperations.ZeroMemory(secret);
}
```

`GetByteArray(length)` allocates and fills a new array. A zero length returns `Array.Empty<byte>()`.
The caller owns the result and should clear it when practical. If you already own a buffer, call
the BCL's `RandomNumberGenerator.Fill(span)` directly to avoid another allocation.

Encode random bytes with an encoding designed for the destination when a textual token is needed;
do not decode arbitrary random bytes as UTF-8. Remember that the resulting managed string cannot
be securely erased.

## Bounded values

```csharp
int index = RandomSecurityUtil.Next(0, choices.Count); // [0, Count)
decimal amount = RandomSecurityUtil.NextDecimal(10m, 20m); // [10, 20)
```

`Next` delegates to the unbiased `RandomNumberGenerator.GetInt32` implementation and requires an
exclusive upper bound greater than the lower bound.

`NextDecimal` scales a 28-digit random fraction and rejects any result rounded to the exclusive
upper endpoint. Decimal range subtraction can still throw `OverflowException` when the requested
interval spans beyond decimal's representable arithmetic range. Use integer sampling when every
possible outcome must have a directly auditable discrete probability.

## Unit fractions

```csharp
decimal fraction = RandomSecurityUtil.GetRandomFraction(); // [0, 1), 28 decimal places
double sample = RandomSecurityUtil.NextDouble();            // [0, 1), 53 random bits
```

`GetRandomFraction` uses rejection sampling across the decimal coefficient range, without
floating-point conversion. `NextDouble` samples uniformly from values of the form `k / 2^53`.
Although their entropy source is cryptographic, floating-point and decimal samples are usually
better suited to randomized application behavior than to constructing keys or tokens; use random
bytes for those.
