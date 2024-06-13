using System.Collections.Generic;
using UnityEngine;

namespace Level.ObstaclePatterns
{
    public class TurnPattern : BasePattern
    {
        public override int PatternLength => 1;

        public override IEnumerable<PatternCubeResult> GeneratePattern(
            Vector3 startPosition, ref Vector3 direction)
        {
            var position = startPosition + direction;
            var result = new List<PatternCubeResult> { new PatternCubeResult(CubeTypes.Turn, position) };
            var randomSide = Random.value < 0 ? -1 : 1;
            direction = Quaternion.Euler(0, randomSide * 90, 0) * direction;
            result.Add(new PatternCubeResult(CubeTypes.Default, position + direction));

            return result;
        }
    }
}