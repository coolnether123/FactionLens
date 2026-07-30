using FactionLens.Api;
using FactionLens.Domain;
using RimWorld;
using RimWorld.Planet;

namespace FactionLens.Ownership
{
    internal static class OwnershipService
    {
        internal static bool TryClassify(
            WorldObject worldObject,
            out RelationshipCategory category,
            out WorldObjectKind kind)
        {
            kind = KindOf(worldObject);

            if (FactionLensApi.TryResolve(
                worldObject,
                out OwnershipResolution custom))
            {
                category = Classify(custom);
                return custom.Kind !=
                    OwnershipResolutionKind.NotHandled;
            }

            if (!IsVanillaOwnershipMeaningful(worldObject))
            {
                category = RelationshipCategory.Unknown;
                return false;
            }

            // Vanilla deliberately retains the former faction reference on an
            // abandoned settlement. Its visible state is nevertheless
            // abandoned, not a live diplomatic holding.
            if (worldObject is AbandonedSettlement)
            {
                category = RelationshipClassifier.Classify(
                    OwnershipKnowledge.Disclosed,
                    true,
                    false,
                    RelationOf(worldObject.Faction),
                    true);
                return true;
            }

            Faction faction = worldObject.Faction;
            OwnershipKnowledge knowledge =
                faction == null || worldObject.AppendFactionToInspectString
                    ? OwnershipKnowledge.Disclosed
                    : OwnershipKnowledge.Unknown;
            category = RelationshipClassifier.Classify(
                knowledge,
                faction != null,
                faction != null && faction.IsPlayer,
                RelationOf(faction));
            return true;
        }

        private static RelationshipCategory Classify(
            OwnershipResolution resolution)
        {
            if (resolution.Kind ==
                OwnershipResolutionKind.Unknown)
            {
                return RelationshipCategory.Unknown;
            }

            Faction faction = resolution.Faction;
            return RelationshipClassifier.Classify(
                OwnershipKnowledge.Disclosed,
                faction != null,
                faction != null && faction.IsPlayer,
                RelationOf(faction));
        }

        private static bool IsVanillaOwnershipMeaningful(
            WorldObject worldObject)
        {
            return worldObject is Settlement ||
                worldObject is Site ||
                worldObject.def?.canHaveFaction == true;
        }

        private static WorldObjectKind KindOf(WorldObject worldObject)
        {
            if (worldObject is Settlement)
            {
                return WorldObjectKind.Settlement;
            }

            if (worldObject is Site)
            {
                return WorldObjectKind.Site;
            }

            return WorldObjectKind.Other;
        }

        private static PlayerRelation RelationOf(Faction faction)
        {
            if (faction == null || faction.IsPlayer)
            {
                return PlayerRelation.Unknown;
            }

            switch (faction.PlayerRelationKind)
            {
                case FactionRelationKind.Hostile:
                    return PlayerRelation.Hostile;
                case FactionRelationKind.Ally:
                    return PlayerRelation.Allied;
                case FactionRelationKind.Neutral:
                    return PlayerRelation.Neutral;
                default:
                    return PlayerRelation.Unknown;
            }
        }
    }
}
