using System.Collections.Generic;
using UnityEngine;

namespace Level.ObstaclePatterns
{
    public class FencePattern : BasePattern
    {
        public override int PatternLength => 3;

        public override IEnumerable<PatternCubeResult> GeneratePattern(
            Vector3 startPosition, ref Vector3 direction)
        {
            var result = new List<PatternCubeResult>
            {
                new(CubeTypes.Fence, startPosition + direction),
                new(CubeTypes.Default, startPosition + direction * 2),
                new(CubeTypes.Default, startPosition + direction * 3)
            };

            return result;
        }
    }
}