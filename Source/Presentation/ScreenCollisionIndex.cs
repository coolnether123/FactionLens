using System;
using System.Collections.Generic;

namespace FactionLens.Presentation
{
    internal readonly struct ScreenBounds
    {
        internal ScreenBounds(
            float x,
            float y,
            float width,
            float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        internal float X { get; }
        internal float Y { get; }
        internal float Width { get; }
        internal float Height { get; }
        internal float XMax => X + Width;
        internal float YMax => Y + Height;

        internal bool Overlaps(ScreenBounds other)
        {
            return X < other.XMax &&
                XMax > other.X &&
                Y < other.YMax &&
                YMax > other.Y;
        }

        internal bool Contains(float x, float y)
        {
            return x >= X &&
                x < XMax &&
                y >= Y &&
                y < YMax;
        }

        internal ScreenBounds ShiftDown(float distance)
        {
            return new ScreenBounds(
                X,
                Y + distance,
                Width,
                Height);
        }
    }

    internal sealed class ScreenCollisionIndex
    {
        private const float DefaultCellSize = 64f;
        internal const int DefaultVerticalShifts = 3;
        private const float DefaultVerticalGap = 2f;

        private readonly float cellSize;
        private int maxVerticalShifts;
        private readonly float verticalGap;
        private readonly List<ScreenBounds> accepted =
            new List<ScreenBounds>();
        private readonly List<int> visitMarks =
            new List<int>();
        private readonly Dictionary<long, List<int>> buckets =
            new Dictionary<long, List<int>>();
        private readonly List<List<int>> activeBuckets =
            new List<List<int>>();
        private readonly Stack<List<int>> bucketPool =
            new Stack<List<int>>();
        private int visitGeneration;

        internal ScreenCollisionIndex(
            float cellSize = DefaultCellSize,
            int maxVerticalShifts = DefaultVerticalShifts,
            float verticalGap = DefaultVerticalGap)
        {
            if (cellSize <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(cellSize));
            }

            if (maxVerticalShifts < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxVerticalShifts));
            }

            if (verticalGap < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(verticalGap));
            }

            this.cellSize = cellSize;
            this.maxVerticalShifts = maxVerticalShifts;
            this.verticalGap = verticalGap;
        }

        internal int Count => accepted.Count;

        /// <summary>
        /// How many times a colliding candidate may be shifted down before it
        /// is refused. Set to zero to require that every label sit at its
        /// natural anchor: a candidate that collides is then dropped outright
        /// rather than displaced, and it reserves no screen space, so it can
        /// never push a neighbouring label out of its own anchored slot.
        /// </summary>
        internal int MaxVerticalShifts
        {
            get { return maxVerticalShifts; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                maxVerticalShifts = value;
            }
        }

        internal long ComparisonCount { get; private set; }

        internal bool TryPlace(
            ScreenBounds candidate,
            out ScreenBounds placed)
        {
            placed = candidate;
            if (!IsValid(candidate))
            {
                return false;
            }

            for (int shift = 0;
                shift <= maxVerticalShifts;
                shift++)
            {
                if (!Intersects(placed))
                {
                    Add(placed);
                    return true;
                }

                if (shift < maxVerticalShifts)
                {
                    placed = placed.ShiftDown(
                        placed.Height + verticalGap);
                }
            }

            return false;
        }

        internal void Clear()
        {
            for (int index = 0;
                index < activeBuckets.Count;
                index++)
            {
                List<int> bucket = activeBuckets[index];
                bucket.Clear();
                bucketPool.Push(bucket);
            }

            buckets.Clear();
            activeBuckets.Clear();
            accepted.Clear();
            visitMarks.Clear();
            visitGeneration = 0;
            ComparisonCount = 0;
        }

        private bool Intersects(ScreenBounds candidate)
        {
            BeginVisit();
            CellRange(
                candidate,
                out int minX,
                out int maxX,
                out int minY,
                out int maxY);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (!buckets.TryGetValue(
                        Key(x, y),
                        out List<int> bucket))
                    {
                        continue;
                    }

                    for (int index = 0;
                        index < bucket.Count;
                        index++)
                    {
                        int acceptedIndex = bucket[index];
                        if (visitMarks[acceptedIndex] ==
                            visitGeneration)
                        {
                            continue;
                        }

                        visitMarks[acceptedIndex] =
                            visitGeneration;
                        ComparisonCount++;
                        if (accepted[acceptedIndex].Overlaps(
                            candidate))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void Add(ScreenBounds bounds)
        {
            int acceptedIndex = accepted.Count;
            accepted.Add(bounds);
            visitMarks.Add(0);
            CellRange(
                bounds,
                out int minX,
                out int maxX,
                out int minY,
                out int maxY);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    long key = Key(x, y);
                    if (!buckets.TryGetValue(
                        key,
                        out List<int> bucket))
                    {
                        bucket = bucketPool.Count > 0
                            ? bucketPool.Pop()
                            : new List<int>();
                        buckets.Add(key, bucket);
                        activeBuckets.Add(bucket);
                    }

                    bucket.Add(acceptedIndex);
                }
            }
        }

        private void BeginVisit()
        {
            if (visitGeneration == int.MaxValue)
            {
                for (int index = 0;
                    index < visitMarks.Count;
                    index++)
                {
                    visitMarks[index] = 0;
                }

                visitGeneration = 1;
                return;
            }

            visitGeneration++;
        }

        private void CellRange(
            ScreenBounds bounds,
            out int minX,
            out int maxX,
            out int minY,
            out int maxY)
        {
            minX = (int)Math.Floor(bounds.X / cellSize);
            maxX = (int)Math.Ceiling(bounds.XMax / cellSize) - 1;
            minY = (int)Math.Floor(bounds.Y / cellSize);
            maxY = (int)Math.Ceiling(bounds.YMax / cellSize) - 1;
        }

        private static bool IsValid(ScreenBounds bounds)
        {
            return bounds.Width > 0f &&
                bounds.Height > 0f &&
                !float.IsNaN(bounds.X) &&
                !float.IsNaN(bounds.Y) &&
                !float.IsNaN(bounds.Width) &&
                !float.IsNaN(bounds.Height) &&
                !float.IsInfinity(bounds.X) &&
                !float.IsInfinity(bounds.Y) &&
                !float.IsInfinity(bounds.Width) &&
                !float.IsInfinity(bounds.Height) &&
                !float.IsInfinity(bounds.XMax) &&
                !float.IsInfinity(bounds.YMax);
        }

        private static long Key(int x, int y)
        {
            return ((long)x << 32) ^ (uint)y;
        }
    }
}
