using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Level.ObstaclePatterns;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    public class LevelGenerator : MonoBehaviour
    {
        [ReadOnly] [SerializeField] private AddressablesCubePrefabLoader _addressablesCubePrefab;
        [SerializeField] private Transform _levelContainer;
        private readonly Dictionary<ObstaclesTypes, BasePattern> _patterns = new Dictionary<ObstaclesTypes, BasePattern>();
        private readonly List<CubeContainer> _usedCubes = new List<CubeContainer>();
        private Vector3 _direction = Vector3.forward;
        private Vector3 _startPosition = Vector3.zero;

        public Transform LevelContainer => _levelContainer;
        public PatternLevelDTO PatternsLevelData { get; } = new PatternLevelDTO();

        public IEnumerable<CubeContainer> UsedCubes => _usedCubes;

        private void OnValidate()
        {
            if (_addressablesCubePrefab == null)
                _addressablesCubePrefab = GetComponent<AddressablesCubePrefabLoader>();
        }

        public Dictionary<ObstaclesTypes, int> GenerateEndLevelData(int playerCubeIndex)
        {
            var result = new Dictionary<ObstaclesTypes, int>();
            foreach (var type in EndPanelContainer.FilterObstacles)
                result.Add(type, 0);

            var cubeIndex = 0;
            foreach (var pattern in PatternsLevelData.PatternLevelData)
            {
                if (EndPanelContainer.FilterObstacles.Contains(pattern.Type)) result[pattern.Type]++;

                cubeIndex += pattern.Cubes.Length;
                if (cubeIndex > playerCubeIndex)
                    break;
            }

            return result;
        }

        public (Vector3 position, int indexOffset) CubePosition(int playerCubeIndex)
        {
            var patternCubeResults = PatternsLevelData.CubePath.ToArray();
            for (var index = playerCubeIndex; index < patternCubeResults.Length; index++)
                if (patternCubeResults[index].Type == CubeTypes.Default ||
                    patternCubeResults[index].Type == CubeTypes.Turn)
                    return (patternCubeResults[index].Position, index - playerCubeIndex);

            return (patternCubeResults[playerCubeIndex + 1].Position, 1);
        }

        public async UniTask Initialize()
        {
            GeneratePattern();
            await _addressablesCubePrefab.LoadCubes();
        }

        public void ClearLevel(Vector3 position)
        {
            _startPosition = position;
            foreach (var cube in _usedCubes)
                Destroy(cube.gameObject);
            _usedCubes.Clear();
        }

        public void StartGenerateLevel(int count)
        {
            GenerateLevel(count);
        }

        private void GenerateLevel(int count)
        {
            PatternsLevelData.Clear();

            GenerateStartPosition(ref _startPosition, _direction, ref count);
            GenerateLevelPattern(count, _startPosition, ref _direction);
            PatternsLevelData.InitializeCubePath();
            InstantiateCubes();
            _startPosition = _usedCubes[_usedCubes.Count - 1].transform.position;
        }

        private void GenerateLevelPattern(int count, Vector3 startPosition, ref Vector3 direction)
        {
            while (count > 0)
            {
                var filteredPatterns = _patterns.Where(pattern
                    => pattern.Value.PatternLength <= count).ToArray();
                if (filteredPatterns.Length > 0)
                {
                    var randomPattern = filteredPatterns[Random.Range(0, filteredPatterns.Length)];
                    var cubes = randomPattern.Value.GeneratePattern(startPosition, ref direction);
                    var newPatternLevelData = new PatternLevelData(randomPattern.Key, cubes.ToArray());
                    PatternsLevelData.Add(newPatternLevelData);
                    count -= randomPattern.Value.PatternLength;
                    startPosition = cubes.Last().Position;
                }
                else
                {
                    Debug.LogError("error with find pattern for level generation");
                }
            }

            startPosition += direction;
            PatternsLevelData.Add(new PatternLevelData(ObstaclesTypes.Default,
                new PatternCubeResult(CubeTypes.Finish, startPosition)));
        }

        private void InstantiateCubes()
        {
            var listCubes = PatternsLevelData.CubePath.ToList();
            var direction = (listCubes[1].Position - listCubes[0].Position).normalized;
            for (var index = 0; index < listCubes.Count; index++)
            {
                var cubeData = listCubes[index];

                switch (cubeData.Type)
                {
                    case CubeTypes.Space:
                        continue;
                    case CubeTypes.Turn:
                        direction = (listCubes[index + 1].Position - cubeData.Position).normalized;
                        break;
                }

                SpawnCube(_addressablesCubePrefab.Cubes[cubeData.Type], cubeData.Position, direction);
            }
        }

        private void SpawnCube(CubeContainer cubePrefab, Vector3 position, Vector3 direction)
        {
            var cube = Instantiate(cubePrefab, position, Quaternion.identity, _levelContainer);
            cube.transform.LookAt(position + direction);
            _usedCubes.Add(cube);
        }

        private void GenerateStartPosition(ref Vector3 startPosition, Vector3 direction, ref int count)
        {
            PatternsLevelData.Add(new PatternLevelData(ObstaclesTypes.Default,
                new PatternCubeResult(CubeTypes.Finish, startPosition)));
            startPosition += direction;
            PatternsLevelData.Add(new PatternLevelData(ObstaclesTypes.Default,
                new PatternCubeResult(CubeTypes.Default, startPosition)));
            count -= 2;
        }

        private void GeneratePattern()
        {
            _patterns.Add(ObstaclesTypes.Default, new DefaultPattern());
            _patterns.Add(ObstaclesTypes.DoubleSpace, new DoubleSpacePattern());
            _patterns.Add(ObstaclesTypes.Fence, new FencePattern());
            _patterns.Add(ObstaclesTypes.Saw, new SawPattern());
            _patterns.Add(ObstaclesTypes.Space, new SpacePattern());
            _patterns.Add(ObstaclesTypes.Turn, new TurnPattern());
        }
    }
}