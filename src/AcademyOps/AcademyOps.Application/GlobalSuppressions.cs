using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Naming",
    "CA1715:Identifiers should have correct prefix",
    Justification = "Persistence marker names are part of the approved application architecture.",
    Scope = "type",
    Target = "~T:AcademyOps.Application.Persistence.AcademyOpsReadPersistence")]
[assembly: SuppressMessage(
    "Naming",
    "CA1715:Identifiers should have correct prefix",
    Justification = "Persistence marker names are part of the approved application architecture.",
    Scope = "type",
    Target = "~T:AcademyOps.Application.Persistence.AcademyOpsWritePersistence")]
