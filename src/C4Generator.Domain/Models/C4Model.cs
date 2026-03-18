namespace C4Generator.Domain.Models;

/// <summary>
/// Root C4 model stored as JSON in ArchitectureModel.ModelJson.
/// All four levels are embedded so each visualization query simply
/// reads the relevant section.
/// </summary>
public sealed record C4Model(
    C4ContextDiagram Context,
    C4ContainerDiagram Containers,
    C4ComponentDiagram Components,
    C4ClassDiagram Classes
);

// ── Level 1: Context ──────────────────────────────────────────────────────────

public sealed record C4ContextDiagram(
    string SystemName,
    string SystemDescription,
    IReadOnlyList<C4Actor> Actors,
    IReadOnlyList<C4ExternalSystem> ExternalSystems,
    IReadOnlyList<C4Relationship> Relationships
);

public sealed record C4Actor(string Id, string Name, string Description);

public sealed record C4ExternalSystem(string Id, string Name, string Description);

// ── Level 2: Containers ───────────────────────────────────────────────────────

public sealed record C4ContainerDiagram(
    IReadOnlyList<C4Container> Containers,
    IReadOnlyList<C4Relationship> Relationships
);

public sealed record C4Container(
    string Id,
    string Name,
    string Technology,
    string Description,
    string Type   // "Api" | "Database" | "Worker" | "UI" | "Queue" | "Other"
);

// ── Level 3: Components ───────────────────────────────────────────────────────

public sealed record C4ComponentDiagram(
    IReadOnlyList<C4Component> Components,
    IReadOnlyList<C4Relationship> Relationships
);

public sealed record C4Component(
    string Id,
    string ContainerId,
    string Name,
    string Technology,
    string Description,
    string Layer   // "Controller" | "Service" | "Repository" | "Handler" | "Other"
);

// ── Level 4: Classes ──────────────────────────────────────────────────────────

public sealed record C4ClassDiagram(
    IReadOnlyList<C4Class> Classes,
    IReadOnlyList<C4Relationship> Relationships
);

public sealed record C4Class(
    string Id,
    string ComponentId,
    string Name,
    string Namespace,
    string Kind,   // "Class" | "Interface" | "Record" | "Enum"
    IReadOnlyList<string> Methods
);

// ── Shared ────────────────────────────────────────────────────────────────────

public sealed record C4Relationship(
    string FromId,
    string ToId,
    string Label,
    string? Technology = null
);
