using Soenneker.Tests.HostedUnit;


namespace Soenneker.Utils.Random.Security.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class RandomSecurityUtilTests : HostedUnitTest
{
    public RandomSecurityUtilTests(Host host) : base(host)
    {

    }

    [Test]
    public void Default()
    { }
}
