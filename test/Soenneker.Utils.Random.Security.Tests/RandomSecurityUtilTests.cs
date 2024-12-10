using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Utils.Random.Security.Tests;

[Collection("Collection")]
public class RandomSecurityUtilTests : FixturedUnitTest
{
    public RandomSecurityUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {

    }
}
