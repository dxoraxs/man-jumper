using System.Collections.Generic;
using UnityEngine;

namespace Level.ObstaclePatterns
{
    public struct PatternCubeResult
    {
        public readonly CubeTypes Type;
        public Vector3 Position;

        public PatternCubeResult(CubeTypes type, Vector3 position)
        {
            Type = type;
            Position = position;
        }
    }

    public abstract class BasePattern
    {
        public abstract int PatternLength { get; }

        public abstract IEnumerable<PatternCubeResult> GeneratePattern(
            Vector3 startPosition, ref Vector3 direction);
    }
}