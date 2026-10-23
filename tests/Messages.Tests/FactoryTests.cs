// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

using BeneficialStrategies.Iso20022.Components;
using BeneficialStrategies.Iso20022.Factory;

namespace BeneficialStrategies.Iso20022;

/// <summary>The interfaces are what the Factory builds objects for: one message of every business area, with everything it reaches.</summary>
public class FactoryTests
{
    private static readonly Assembly Model = typeof(IGenericIdentification3).Assembly;

    public static IEnumerable<object[]> OneMessagePerBusinessArea() =>
        Model.GetExportedTypes()
            .Where(t => t.IsInterface && typeof(IOuterRecord).IsAssignableFrom(t))
            .GroupBy(t => t.Namespace!)
            .OrderBy(g => g.Key)
            .Select(g => new object[] { g.OrderBy(t => t.Name).First().AssemblyQualifiedName! });

    [Theory]
    [MemberData(nameof(OneMessagePerBusinessArea))]
    public void TheFactoryBuildsTheMessageAndEverythingItReaches(string messageType)
    {
        var message = Type.GetType(messageType)!;
        var factory = new IsoDataFactory();

        factory.Warm(message);
        var made = factory.Create(message);

        Assert.True(message.IsInstanceOfType(made), message.FullName);
    }
}
