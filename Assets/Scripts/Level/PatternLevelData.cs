using System.Collections.Generic;
using System.Linq;
using Level.ObstaclePatterns;

namespace Level
{
    public class PatternLevelDTO
    {
        private readonly List<PatternCubeResult> _cubePath = new List<PatternCubeResult>();
        private readonly List<PatternLevelData> _patternsLevelData = new List<PatternLevelData>();

        public IEnumerable<PatternLevelData> PatternLevelData => _patternsLevelData;
        public IEnumerable<PatternCubeResult> CubePath => _cubePath;

        public void Clear()
        {
            _patternsLevelData.Clear();
            _cubePath.Clear();
        }

        public void Add(PatternLevelData patternLevelData)
        {
            _patternsLevelData.Add(patternLevelData);
        }

        public void InitializeCubePath()
        {
            _cubePath.AddRange(_patternsLevelData.Select(data => data.Cubes).SelectMany(cubes => cubes));
        }
    }

    public class PatternLevelData
    {
        public readonly PatternCubeResult[] Cubes;
        public readonly ObstaclesTypes Type;

        public PatternLevelData(ObstaclesTypes type, params PatternCubeResult[] cubes)
        {
            Type = type;
            Cubes = cubes;
        }
    }
}