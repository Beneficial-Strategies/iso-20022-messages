// Example 02: a choice and its variants.
// Shows:  that a choice is an interface, each variant an interface that derives from it, and that you ask which one you have.
// Needs:  only BeneficialStrategies.Iso20022.Messages.
// Prints: a description of an account, by its variant.
using BeneficialStrategies.Iso20022.Choices;
using BeneficialStrategies.Iso20022.Choices.AccountIdentification4Choice;
using MyCompany.Examples;

// <snippet choice>
static string DescribeAccount(IAccountIdentification4Choice account) => account switch
{
    IIBAN iban => $"IBAN {iban.Value}",
    IOther other => $"account {other.Identification}",
    _ => "an account of another kind",
};

Console.WriteLine(DescribeAccount(new MyIban()));      // IBAN DE89370400440532013000
// </snippet>
