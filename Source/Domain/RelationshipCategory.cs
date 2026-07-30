namespace FactionLens.Domain
{
    public enum RelationshipCategory
    {
        Unknown = 0,
        Hostile = 1,
        Neutral = 2,
        Allied = 3,
        Player = 4,
        Factionless = 5
    }

    public enum OwnershipKnowledge
    {
        Unknown = 0,
        Disclosed = 1
    }

    public enum PlayerRelation
    {
        Unknown = 0,
        Hostile = 1,
        Neutral = 2,
        Allied = 3
    }

    public enum WorldObjectKind
    {
        Settlement = 0,
        Site = 1,
        Other = 2
    }
}
