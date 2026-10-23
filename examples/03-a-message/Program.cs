// Example 03: what a reader needs to know about a message.
// Shows:  the identifier, the XML element and the namespace that a message interface carries, and the kind of any type of the model.
// Needs:  only BeneficialStrategies.Iso20022.Messages.
// Prints: the identification of pacs.008 and the kinds of four types.
using BeneficialStrategies.Iso20022;
using BeneficialStrategies.Iso20022.Choices;
using BeneficialStrategies.Iso20022.Choices.AccountIdentification4Choice;
using BeneficialStrategies.Iso20022.Components;
using BeneficialStrategies.Iso20022.Framework;
using BeneficialStrategies.Iso20022.pacs;

// <snippet message>
var message = IsoMessageAttribute.Find(typeof(IFIToFICustomerCreditTransferV14))!;
Console.WriteLine($"{message.IsoIdentifier}  <{message.XmlTag}>  {message.DocumentNamespace}");
// pacs.008.001.14  <FIToFICstmrCdtTrf>  urn:iso:std:iso:20022:tech:xsd:pacs.008.001.14

Console.WriteLine(Iso.KindOf(typeof(IFIToFICustomerCreditTransferV14)));   // Message
Console.WriteLine(Iso.KindOf(typeof(IGenericIdentification3)));            // Component
Console.WriteLine(Iso.KindOf(typeof(IAccountIdentification4Choice)));      // Choice
Console.WriteLine(Iso.KindOf(typeof(IIBAN)));                              // ChoiceVariant
// </snippet>
