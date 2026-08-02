# Faction Lens Test Fixture

Developer-only harness extension. It is not included in release packages.

With the fixture explicitly added to an isolated harness lane, run:

```text
dev-run factionlens-collision-fixture run
dev-run factionlens-collision-fixture cleanup
```

`run` creates adjacent temporary settlements around the player settlement and
opens the world map so displaced-label connectors can be inspected. `cleanup`
removes only the fixture-owned world objects.
