# Changelog

The versions of this package have three parts, `design.breaking.compatible`: the first changes with a redesign, the second with a change that breaks existing users, the third with every other change. Newest first. Each entry is written from the issues closed since the previous release.

## 1.0.0-alpha (2026-10-23)

* Added: The first publication of the package, with the interfaces of 3,339 messages, their components and choices. It was prepared in a monorepo as `BeneficialStrategies.Iso20022.Interfaces` and never published under that name; it is now `BeneficialStrategies.Iso20022.Messages`, in its own repository with its history, and is versioned and released independently of the other packages. The namespaces are unchanged.
* Added: Versions have three parts, `design.breaking.compatible`: the first changes with a redesign, the second with a change that breaks existing users, the third with every other change. This is not Semantic Versioning.
* Added: Tests that read the assembly alone and say what every interface must be (one kind marker, get-only properties, an XML tag on every member, a unique identifier for every message, choices with their variants), and that the Factory builds one message of every business area.
* Added: The README is the package README, and its examples are projects under `examples/` that are built and run against the packed package.
