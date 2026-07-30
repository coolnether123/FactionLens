using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace FactionLens.Api
{
    /// <summary>
    /// The result returned by a compatibility ownership resolver.
    /// Unknown is deliberately distinct from a disclosed factionless object.
    /// </summary>
    public readonly struct OwnershipResolution
    {
        private OwnershipResolution(
            OwnershipResolutionKind kind,
            Faction faction)
        {
            Kind = kind;
            Faction = faction;
        }

        public OwnershipResolutionKind Kind { get; }

        public Faction Faction { get; }

        public static OwnershipResolution NotHandled =>
            new OwnershipResolution(
                OwnershipResolutionKind.NotHandled,
                null);

        public static OwnershipResolution Unknown =>
            new OwnershipResolution(
                OwnershipResolutionKind.Unknown,
                null);

        public static OwnershipResolution Disclosed(Faction faction)
        {
            return new OwnershipResolution(
                OwnershipResolutionKind.Disclosed,
                faction);
        }
    }

    public enum OwnershipResolutionKind
    {
        NotHandled = 0,
        Unknown = 1,
        Disclosed = 2
    }

    public delegate OwnershipResolution OwnershipResolver(
        WorldObject worldObject);

    /// <summary>
    /// Registration surface for world objects whose meaningful ownership is
    /// not exposed through WorldObject.Faction.
    /// </summary>
    public static class FactionLensApi
    {
        private sealed class Registration
        {
            public string Id;
            public Type WorldObjectType;
            public OwnershipResolver Resolver;
            public int Priority;
        }

        private static readonly object Sync = new object();
        private static Registration[] registrations =
            new Registration[0];

        public static void RegisterOwnershipResolver(
            string id,
            Type worldObjectType,
            OwnershipResolver resolver,
            int priority = 0)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    "A stable resolver ID is required.",
                    nameof(id));
            }

            if (worldObjectType == null ||
                !typeof(WorldObject).IsAssignableFrom(worldObjectType))
            {
                throw new ArgumentException(
                    "The resolver type must derive from WorldObject.",
                    nameof(worldObjectType));
            }

            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            lock (Sync)
            {
                if (registrations.Any(item =>
                    string.Equals(
                        item.Id,
                        id,
                        StringComparison.Ordinal)))
                {
                    throw new InvalidOperationException(
                        "Duplicate Faction Lens ownership resolver ID: " +
                        id);
                }

                var updated = new List<Registration>(registrations)
                {
                    new Registration
                    {
                        Id = id,
                        WorldObjectType = worldObjectType,
                        Resolver = resolver,
                        Priority = priority
                    }
                };
                registrations = updated
                    .OrderByDescending(item => item.Priority)
                    .ThenBy(item => item.Id, StringComparer.Ordinal)
                    .ToArray();
            }
        }

        public static bool UnregisterOwnershipResolver(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            lock (Sync)
            {
                int index = Array.FindIndex(
                    registrations,
                    item => string.Equals(
                        item.Id,
                        id,
                        StringComparison.Ordinal));
                if (index < 0)
                {
                    return false;
                }

                var updated = new List<Registration>(registrations);
                updated.RemoveAt(index);
                registrations = updated.ToArray();
                return true;
            }
        }

        internal static bool TryResolve(
            WorldObject worldObject,
            out OwnershipResolution result)
        {
            Registration[] snapshot = registrations;
            for (int index = 0; index < snapshot.Length; index++)
            {
                Registration registration = snapshot[index];
                if (!registration.WorldObjectType.IsInstanceOfType(
                    worldObject))
                {
                    continue;
                }

                try
                {
                    result = registration.Resolver(worldObject);
                    if (result.Kind !=
                        OwnershipResolutionKind.NotHandled)
                    {
                        return true;
                    }
                }
                catch (Exception exception)
                {
                    Log.ErrorOnce(
                        "[Faction Lens] Ownership resolver '" +
                        registration.Id +
                        "' failed. Ownership will remain unknown for this " +
                        "object. " + exception,
                        ("FactionLens.Api." + registration.Id).GetHashCode());
                    result = OwnershipResolution.Unknown;
                    return true;
                }
            }

            result = OwnershipResolution.NotHandled;
            return false;
        }
    }
}
