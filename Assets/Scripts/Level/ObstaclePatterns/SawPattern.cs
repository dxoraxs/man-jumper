﻿using System.Collections.Generic;
using UnityEngine;

namespace Level.ObstaclePatterns
{
    public class SawPattern : BasePattern
    {
        public override int PatternLength => 2;

        public override IEnumerable<PatternCubeResult> GeneratePattern(
            Vector3 startPosition, ref Vector3 direction)
        {
            var result = new List<PatternCubeResult>
            {
                new(CubeTypes.Saw, startPosition + direction),
                new(CubeTypes.Default, startPosition + direction * 2)
            };

            return result;
        }
    }
}