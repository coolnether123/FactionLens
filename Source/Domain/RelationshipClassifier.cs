namespace FactionLens.Domain
{
    public static class RelationshipClassifier
    {
        public static RelationshipCategory Classify(
            OwnershipKnowledge knowledge,
            bool hasFaction,
            bool isPlayer,
            PlayerRelation relation)
        {
            return Classify(
                knowledge,
                hasFaction,
                isPlayer,
                relation,
                false);
        }

        public static RelationshipCategory Classify(
            OwnershipKnowledge knowledge,
            bool hasFaction,
            bool isPlayer,
            PlayerRelation relation,
            bool explicitlyAbandoned)
        {
            if (knowledge != OwnershipKnowledge.Disclosed)
            {
                return RelationshipCategory.Unknown;
            }

            if (explicitlyAbandoned || !hasFaction)
            {
                return RelationshipCategory.Factionless;
            }

            if (isPlayer)
            {
                return RelationshipCategory.Player;
            }

            switch (relation)
            {
                case PlayerRelation.Hostile:
                    return RelationshipCategory.Hostile;
                case PlayerRelation.Neutral:
                    return RelationshipCategory.Neutral;
                case PlayerRelation.Allied:
                    return RelationshipCategory.Allied;
                default:
                    return RelationshipCategory.Unknown;
            }
        }
    }

    public static class WorldObjectKindPolicy
    {
        public static bool IsEnabled(
            WorldObjectKind kind,
            bool settlements,
            bool sites,
            bool other)
        {
            switch (kind)
            {
                case WorldObjectKind.Settlement:
                    return settlements;
                case WorldObjectKind.Site:
                    return sites;
                default:
                    return other;
            }
        }
    }
}
