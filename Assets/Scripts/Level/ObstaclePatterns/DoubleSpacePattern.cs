﻿using System.Collections.Generic;
using UnityEngine;

namespace Level.ObstaclePatterns
{
    public class DoubleSpacePattern : BasePattern
    {
        public override int PatternLength => 3;

        public override IEnumerable<PatternCubeResult> GeneratePattern(
            Vector3 startPosition, ref Vector3 direction)
        {
            var result = new List<PatternCubeResult>
            {
                new(CubeTypes.Space, startPosition + direction),
                new(CubeTypes.Space, startPosition + direction * 2),
                new(CubeTypes.Default, startPosition + direction * 3)
            };
            return result;
        }
    }
}