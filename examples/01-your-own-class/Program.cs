// Example 01: a class of your own that implements an interface of the model.
// Shows:  that an interface is plain C#: implement it, and everything written against the interface accepts your class.
// Needs:  only BeneficialStrategies.Iso20022.Messages.
// Prints: a description made through the interface.
using BeneficialStrategies.Iso20022.Components;
using MyCompany.Examples;

// <snippet use>
static string Describe(IGenericIdentification3 id) => $"{id.Identification} / {id.Issuer?.ToString() ?? "no issuer"}";

Console.WriteLine(Describe(new MyIdentification()));      // MINE-1 / no issuer
// </snippet>
