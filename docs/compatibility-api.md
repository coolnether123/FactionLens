# Compatibility API

Mods whose world objects store meaningful ownership somewhere other than
`WorldObject.Faction` can register a resolver after Faction Lens loads:

```csharp
using FactionLens.Api;
using RimWorld.Planet;

FactionLensApi.RegisterOwnershipResolver(
    "Author.ModId.SpecialOutpost",
    typeof(SpecialOutpost),
    worldObject =>
    {
        var outpost = (SpecialOutpost)worldObject;
        if (!outpost.PlayerHasDiscoveredOwner)
        {
            return OwnershipResolution.Unknown;
        }

        // A null faction here means disclosed factionless ownership.
        return OwnershipResolution.Disclosed(outpost.ActualFaction);
    },
    priority: 100);
```

Registration IDs must be stable and unique. Resolvers are evaluated by
descending priority and then ordinal ID. Return
`OwnershipResolution.NotHandled` to let a lower-priority resolver or the
vanilla fallback decide. Use
`FactionLensApi.UnregisterOwnershipResolver(id)` when unloading a dynamic
integration.

Do not return a faction until the player is allowed to know it. Returning
`Unknown` colors the already-visible object with the unknown color but does
not reveal a faction or relationship.

The public API is:

- `FactionLens.Api.FactionLensApi.RegisterOwnershipResolver`
- `FactionLens.Api.FactionLensApi.UnregisterOwnershipResolver`
- `FactionLens.Api.OwnershipResolver`
- `FactionLens.Api.OwnershipResolution`
- `FactionLens.Api.OwnershipResolutionKind`
