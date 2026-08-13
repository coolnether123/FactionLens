# Faction Lens Legacy Contract Fixture

Developer-only RimWorld 1.0 compatibility fixture. It is never included in a
Faction Lens release package.

Build the fixture against the authoritative 1.0 managed directory, add this
mod beside an isolated Faction Lens/Spine pair, then run:

```text
dev-run factionlens-legacy-contract run
dev-run factionlens-legacy-contract status
dev-run factionlens-legacy-contract load FactionLensLegacyContract-1.0
dev-run factionlens-legacy-contract verify
```

The fixture invokes Faction Lens' production settings write/read path and
world-label processing through a temporary developer window. It records
defaults, valid edge values, contextual registration, label visibility/output,
Harmony ownership, and the deterministic save name. The fixture assembly and
its About metadata are explicitly developer-only and are excluded by the
release package allowlist.
