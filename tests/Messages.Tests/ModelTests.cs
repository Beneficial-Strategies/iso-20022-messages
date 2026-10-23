// Copyright 2026 Jeff Ward, Beneficial Strategies. Usage subject to license of enclosing library.

using BeneficialStrategies.Iso20022.Components;

namespace BeneficialStrategies.Iso20022;

/// <summary>What every interface of the model must be, read from the assembly alone: no implementation is needed to say so.</summary>
public class ModelTests
{
    private static readonly Assembly Model = typeof(IGenericIdentification3).Assembly;

    private static readonly Type[] Interfaces = Model.GetExportedTypes().Where(t => t.IsInterface).ToArray();

    private static string Show(IEnumerable<string> problems) =>
        $"{problems.Count()} problem(s), the first ones: " + string.Join("; ", problems.Take(8));

    private static bool IsVariant(Type t) => typeof(IIsoChoiceVariant).IsAssignableFrom(t);

    [Fact]
    public void TheModelIsComplete_ThereAreInterfacesForTheMessagesComponentsAndChoices()
    {
        Assert.True(Interfaces.Count(t => typeof(IOuterRecord).IsAssignableFrom(t)) >= 3000, "messages");
        Assert.True(Interfaces.Count(t => typeof(IIsoComponent).IsAssignableFrom(t)) >= 10000, "components");
        Assert.True(Interfaces.Count(t => typeof(IIsoChoice).IsAssignableFrom(t) && !IsVariant(t)) >= 4000, "choices");
        Assert.True(Interfaces.Count(IsVariant) >= 10000, "choice variants");
    }

    [Fact]
    public void EveryInterface_CarriesTheMarkerOfExactlyOneKind()
    {
        var problems = new List<string>();
        foreach (var t in Interfaces)
        {
            var kinds = new[]
            {
                typeof(IOuterRecord).IsAssignableFrom(t),
                typeof(IIsoComponent).IsAssignableFrom(t),
                IsVariant(t),
                typeof(IIsoChoice).IsAssignableFrom(t) && !IsVariant(t),
            }.Count(b => b);
            if (kinds != 1)
                problems.Add($"{t.FullName} has {kinds} kinds");
        }
        Assert.True(problems.Count == 0, Show(problems));
    }

    [Fact]
    public void EveryInterface_HasOnlyReadablePropertiesAndNoMethods()
    {
        var problems = new List<string>();
        foreach (var t in Interfaces)
        {
            if (t.IsGenericTypeDefinition)
                problems.Add($"{t.FullName} is generic");
            foreach (var m in t.GetMethods().Where(m => !m.IsSpecialName))
                problems.Add($"{t.FullName} has the method {m.Name}");
            foreach (var p in t.GetProperties().Where(p => !p.CanRead || p.CanWrite))
                problems.Add($"{t.FullName}.{p.Name} is not get-only");
        }
        Assert.True(problems.Count == 0, Show(problems));
    }

    [Fact]
    public void EveryMemberOfAnInterface_NamesTheXmlElementItIsReadFrom()
    {
        var problems = new List<string>();
        foreach (var t in Interfaces)
            foreach (var p in t.GetProperties())
                if (p.GetCustomAttribute<IsoXmlTagAttribute>() is null)
                    problems.Add($"{t.FullName}.{p.Name}");
        Assert.True(problems.Count == 0, Show(problems));
    }

    [Fact]
    public void EveryMessageComponentAndChoice_NamesItsIsoIdentifier()
    {
        // A choice variant is a member of its choice in the ISO repository and may have no identifier of its own.
        var problems = Interfaces.Where(t => !IsVariant(t) && t.GetCustomAttribute<IsoIdAttribute>() is null).Select(t => t.FullName!).ToList();
        Assert.True(problems.Count == 0, Show(problems));
    }

    [Fact]
    public void EveryInterface_CarriesTheNameThatISOGivesItsType()
    {
        var problems = Interfaces.Where(t => t.GetCustomAttribute<IsoDisplayNameAttribute>() is null).Select(t => t.FullName!).ToList();
        Assert.True(problems.Count == 0, Show(problems));
    }

    [Fact]
    public void EveryMemberThatMayRepeat_SaysWhichKindOfListHoldsItsItems()
    {
        var problems = new List<string>();
        foreach (var t in Interfaces)
            foreach (var p in t.GetProperties())
                if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>) && p.GetCustomAttribute<IsoListAttribute>() is null)
                    problems.Add($"{t.FullName}.{p.Name}");
        Assert.True(problems.Count == 0, Show(problems));
    }

    [Fact]
    public void TheObsoleteTypesOfTheStandard_AreMarkedObsolete()
    {
        Assert.True(Interfaces.Count(t => t.GetCustomAttribute<ObsoleteAttribute>() is not null) >= 300, "obsolete interfaces");
    }

    [Fact]
    public void EveryMessage_SaysHowItIsReadAndWritten_AndItsIdentifierIsUnique()
    {
        var messages = Interfaces.Where(t => typeof(IOuterRecord).IsAssignableFrom(t)).ToList();
        var problems = new List<string>();
        var seen = new HashSet<string>();
        foreach (var t in messages)
        {
            var message = IsoMessageAttribute.Find(t);
            if (message is null)
            {
                problems.Add($"{t.FullName} has no [IsoMessage]");
                continue;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(message.IsoIdentifier, @"^[a-z]{4}\.\d{3}\.\d{3}\.\d{2}$"))
                problems.Add($"{t.FullName}: '{message.IsoIdentifier}' is not a message identifier");
            if (message.DocumentNamespace != "urn:iso:std:iso:20022:tech:xsd:" + message.IsoIdentifier)
                problems.Add($"{t.FullName}: namespace {message.DocumentNamespace}");
            if (!seen.Add(message.IsoIdentifier))
                problems.Add($"{t.FullName}: {message.IsoIdentifier} is used twice");
        }
        Assert.True(problems.Count == 0, Show(problems));
    }

    [Fact]
    public void EveryChoiceVariant_DerivesFromExactlyOneChoice()
    {
        var problems = new List<string>();
        foreach (var variant in Interfaces.Where(IsVariant))
        {
            var choices = variant.GetInterfaces().Where(i => i != typeof(IIsoChoice) && typeof(IIsoChoice).IsAssignableFrom(i) && !IsVariant(i)).ToList();
            if (choices.Count != 1)
                problems.Add($"{variant.FullName} derives from {choices.Count} choices");
        }
        Assert.True(problems.Count == 0, Show(problems));
    }

    [Fact]
    public void EveryChoice_HasAtLeastTwoVariants()
    {
        var variants = Interfaces.Where(IsVariant)
            .SelectMany(v => v.GetInterfaces().Where(i => typeof(IIsoChoice).IsAssignableFrom(i) && !IsVariant(i) && i != typeof(IIsoChoice)))
            .GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());
        // ISO defines these two with a single variant; any other choice with fewer than two is a mistake.
        var singleVariantInIso = new[] { "IAssetClassCommodityPolypropylene1Choice", "IRestrictedProprietaryChoice" };
        var problems = Interfaces.Where(t => typeof(IIsoChoice).IsAssignableFrom(t) && !IsVariant(t) && t != typeof(IIsoChoice))
            .Where(c => variants.GetValueOrDefault(c) < 2 && !singleVariantInIso.Contains(c.Name)).Select(c => $"{c.FullName} has {variants.GetValueOrDefault(c)} variant(s)").ToList();
        Assert.True(problems.Count == 0, Show(problems));
    }
}
