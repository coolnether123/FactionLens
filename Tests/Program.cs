using System;
using FactionLens.Domain;
using FactionLens.Presentation;
using static RimWorld.ModTestSupport.Test;

namespace FactionLens.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            Start("Faction Lens contracts");
            Run("undisclosed ownership stays unknown", ClassifiesUndisclosedOwnershipAsUnknown);
            Run("factionless precedes relationship", ClassifiesFactionlessBeforeRelationship);
            Run("player precedes diplomacy", ClassifiesPlayerBeforeDiplomacy);
            Run("all diplomatic states classify", ClassifiesEveryDiplomaticState);
            Run("object switches are honored", HonorsEveryObjectTypeSwitch);
            Run("collision placement semantics", PreservesCollisionPlacementSemantics);
            Run("bucket-boundary overlap", DetectsOverlapAcrossBucketBoundaries);
            Run("dense comparisons stay local", KeepsDenseLayoutComparisonsLocal);
            Run("label bounds are exact", LabelBoundsUseExactClickTarget);
            return Finish();
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

        private static void PreservesCollisionPlacementSemantics()
        {
            var index = new ScreenCollisionIndex();
            if (!index.TryPlace(
                new ScreenBounds(0f, 0f, 20f, 18f),
                out ScreenBounds first) ||
                first.Y != 0f)
            {
                throw new InvalidOperationException(
                    "The first label was not placed at its requested " +
                    "position.");
            }

            if (!index.TryPlace(
                new ScreenBounds(20f, 0f, 20f, 18f),
                out ScreenBounds touching) ||
                touching.Y != 0f)
            {
                throw new InvalidOperationException(
                    "Labels whose edges only touch must not overlap.");
            }

            Equal(
                20f,
                PlaceAtOrigin(index),
                "First overlap must shift down once.");
            Equal(
                40f,
                PlaceAtOrigin(index),
                "Second overlap must shift down twice.");
            Equal(
                60f,
                PlaceAtOrigin(index),
                "Third overlap must use the last allowed shift.");

            if (index.TryPlace(
                new ScreenBounds(0f, 0f, 20f, 18f),
                out ScreenBounds rejected))
            {
                throw new InvalidOperationException(
                    "A label blocked at all four tested positions must " +
                    "be rejected, but it was placed at " +
                    rejected.Y + ".");
            }
        }

        private static void DetectsOverlapAcrossBucketBoundaries()
        {
            var index = new ScreenCollisionIndex(
                cellSize: 32f);
            if (!index.TryPlace(
                new ScreenBounds(31f, 31f, 4f, 4f),
                out ScreenBounds first) ||
                !index.TryPlace(
                    new ScreenBounds(34f, 34f, 4f, 4f),
                    out ScreenBounds second))
            {
                throw new InvalidOperationException(
                    "Boundary fixtures could not be placed.");
            }

            Equal(
                40f,
                second.Y,
                "An overlap crossing grid cells must still shift.");
        }

        private static void KeepsDenseLayoutComparisonsLocal()
        {
            const int columns = 80;
            const int rows = 40;
            const int count = columns * rows;
            var index = new ScreenCollisionIndex();

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0;
                    column < columns;
                    column++)
                {
                    if (!index.TryPlace(
                        new ScreenBounds(
                            column * 24f,
                            row * 22f,
                            20f,
                            18f),
                        out ScreenBounds placed))
                    {
                        throw new InvalidOperationException(
                            "Dense non-overlapping label was rejected at " +
                            column + "," + row + ".");
                    }
                }
            }

            Equal(
                count,
                index.Count,
                "Dense fixture count");
            if (index.ComparisonCount >= count * 24L)
            {
                throw new InvalidOperationException(
                    "Dense layout performed too many overlap comparisons: " +
                    index.ComparisonCount + " for " + count + " labels.");
            }
        }

        private static void LabelBoundsUseExactClickTarget()
        {
            var bounds = new ScreenBounds(
                10f,
                20f,
                30f,
                18f);
            if (!bounds.Contains(10f, 20f) ||
                !bounds.Contains(39.99f, 37.99f))
            {
                throw new InvalidOperationException(
                    "Visible label pixels must be selectable.");
            }

            if (bounds.Contains(9.99f, 20f) ||
                bounds.Contains(40f, 20f) ||
                bounds.Contains(10f, 38f))
            {
                throw new InvalidOperationException(
                    "Clicks outside the label must not select it.");
            }
        }

        private static float PlaceAtOrigin(
            ScreenCollisionIndex index)
        {
            if (!index.TryPlace(
                new ScreenBounds(0f, 0f, 20f, 18f),
                out ScreenBounds placed))
            {
                throw new InvalidOperationException(
                    "Expected shifted label placement to succeed.");
            }

            return placed.Y;
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

    }
}
