[![](https://img.shields.io/nuget/v/soenneker.utils.random.security.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.random.security/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.random.security/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.random.security/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.random.security.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.random.security/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.random.security/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.random.security/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.Random.Security
A utility library for security related random operations.

## Installation

```bash
dotnet add package Soenneker.Utils.Random.Security
```

## Quick start

```csharp
using Soenneker.Utils.Random.Security;
```

Call the static `RandomSecurityUtil` methods directly; no dependency-injection registration is required.

## Common operations

- `GetByteArray()` - Generates a cryptographically secure random byte array of the specified length. Returns a byte array filled with cryptographically secure random values.
- `Next()` - Generates a cryptographically secure random integer in the specified range [minValue, maxValue).
- `NextDecimal()` - Generates a cryptographically secure random decimal in the specified range [minValue, maxValue).
- `GetRandomFraction()` - Generates a cryptographically secure random fraction in the range [0, 1).
- `NextDouble()` - Generates a cryptographically secure random double in the range [0.0, 1.0).
