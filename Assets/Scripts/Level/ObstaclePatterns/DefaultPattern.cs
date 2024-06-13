using System.Collections.Generic;
using UnityEngine;

namespace Level.ObstaclePatterns
{
    public class DefaultPattern : BasePattern
    {
        public override int PatternLength => 1;

        public override IEnumerable<PatternCubeResult> GeneratePattern(
            Vector3 startPosition, ref Vector3 direction)
        {
            var result = new List<PatternCubeResult> { new PatternCubeResult(CubeTypes.Default, startPosition + direction) };

            return result;
        }
    }
}