using BeneficialStrategies.Iso20022.Components;
using BeneficialStrategies.Iso20022.SimpleTypes;

namespace MyCompany.Examples;

// <snippet class>
public sealed class MyIdentification : IGenericIdentification3
{
    public Max35Text Identification => "MINE-1";
    public Max35Text? Issuer => null;
}
// </snippet>
