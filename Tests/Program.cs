using System;
using FactionLens.Domain;

namespace FactionLens.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            ClassifiesUndisclosedOwnershipAsUnknown();
            ClassifiesFactionlessBeforeRelationship();
            ClassifiesPlayerBeforeDiplomacy();
            ClassifiesEveryDiplomaticState();
            HonorsEveryObjectTypeSwitch();
            Console.WriteLine(
                "PASS: Faction Lens pure classification contracts");
            return 0;
        }

        private static void ClassifiesUndisclosedOwnershipAsUnknown()
        {
            Equal(
                RelationshipCategory.Unknown,
                RelationshipClassifier.Classify(
                    OwnershipKnowledge.Unknown,
                    true,
                    false,
                    PlayerRelation.Hostile),
                "Hidden ownership must not leak a hostile relationship.");
        }

        private static void ClassifiesFactionlessBeforeRelationship()
        {
            Equal(
                RelationshipCategory.Factionless,
                RelationshipClassifier.Classify(
                    OwnershipKnowledge.Disclosed,
                    false,
                    false,
                    PlayerRelation.Hostile),
                "A disclosed absence of ownership is factionless.");
            Equal(
                RelationshipCategory.Factionless,
                RelationshipClassifier.Classify(
                    OwnershipKnowledge.Disclosed,
                    true,
                    false,
                    PlayerRelation.Hostile,
                    true),
                "An explicit abandoned state must override a retained " +
                "former faction.");
        }

        private static void ClassifiesPlayerBeforeDiplomacy()
        {
            Equal(
                RelationshipCategory.Player,
                RelationshipClassifier.Classify(
                    OwnershipKnowledge.Disclosed,
                    true,
                    true,
                    PlayerRelation.Hostile),
                "Player ownership must take precedence over relation data.");
        }

        private static void ClassifiesEveryDiplomaticState()
        {
            Equal(
                RelationshipCategory.Hostile,
                Classify(PlayerRelation.Hostile),
                "Hostile relation");
            Equal(
                RelationshipCategory.Neutral,
                Classify(PlayerRelation.Neutral),
                "Neutral relation");
            Equal(
                RelationshipCategory.Allied,
                Classify(PlayerRelation.Allied),
                "Allied relation");
            Equal(
                RelationshipCategory.Unknown,
                Classify(PlayerRelation.Unknown),
                "Unrecognized relation must fail closed.");
        }

        private static void HonorsEveryObjectTypeSwitch()
        {
            if (!WorldObjectKindPolicy.IsEnabled(
                WorldObjectKind.Settlement,
                true,
                false,
                false))
            {
                throw new InvalidOperationException(
                    "Settlement switch did not enable settlements.");
            }

            if (!WorldObjectKindPolicy.IsEnabled(
                WorldObjectKind.Site,
                false,
                true,
                false))
            {
                throw new InvalidOperationException(
                    "Site switch did not enable sites.");
            }

            if (!WorldObjectKindPolicy.IsEnabled(
                WorldObjectKind.Other,
                false,
                false,
                true))
            {
                throw new InvalidOperationException(
                    "Other switch did not enable other objects.");
            }

            if (WorldObjectKindPolicy.IsEnabled(
                WorldObjectKind.Settlement,
                false,
                true,
                true))
            {
                throw new InvalidOperationException(
                    "Disabled settlement switch was ignored.");
            }
        }

        private static RelationshipCategory Classify(
            PlayerRelation relation)
        {
            return RelationshipClassifier.Classify(
                OwnershipKnowledge.Disclosed,
                true,
                false,
                relation);
        }

        private static void Equal<T>(
            T expected,
            T actual,
            string contract)
        {
            if (!Equals(expected, actual))
            {
                throw new InvalidOperationException(
                    contract + " Expected " + expected +
                    ", received " + actual + ".");
            }
        }
    }
}
