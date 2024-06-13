using System;
using System.Collections.Generic;
using System.Linq;
using Level;
using Level.ObstaclePatterns;
using UniRx;
using UnityEngine;

namespace Player
{
    public class PlayerCubeMoveListener
    {
        private readonly ReactiveProperty<int> _lastStayCubeIndex = new ReactiveProperty<int>(0);
        private readonly PlayerMoverController _playerMoverController;

        private PatternCubeResult[] _allPath = Array.Empty<PatternCubeResult>();

        public PlayerCubeMoveListener(PlayerMoverController playerMoverController)
        {
            _playerMoverController = playerMoverController;
            _lastStayCubeIndex.Where(value => value == _allPath.Length - 1).Subscribe(_ => OnEndPath())
                .AddTo(_playerMoverController);
            Observable.EveryUpdate().Subscribe(_ => FindNewCube()).AddTo(_playerMoverController);
        }

        public IObservable<int> CubeIndexObservable => _lastStayCubeIndex;
        public int StayCubeIndex => _lastStayCubeIndex.Value;
        public event Action OnEndPath = delegate { };

        private void FindNewCube()
        {
            if (!_allPath.Any()) return;

            var playerPosition = _playerMoverController.transform.position;
            playerPosition.y = 0;
            var currentIndex = _lastStayCubeIndex.Value;
            var nextIndex = Mathf.Clamp(currentIndex + 1, 0, _allPath.Length - 1);

            var currentCubePosition = _allPath[currentIndex].Position;
            var nextCubePosition = _allPath[nextIndex].Position;

            var currentDistanceToLastCube = (currentCubePosition - playerPosition).sqrMagnitude;
            var distanceToNextCube = (nextCubePosition - playerPosition).sqrMagnitude;

            if (currentDistanceToLastCube > distanceToNextCube) _lastStayCubeIndex.Value = nextIndex;
        }

        public void SetNewLevelPath(IEnumerable<PatternLevelData> path)
        {
            _lastStayCubeIndex.Value = 0;
            var allCubes = new List<PatternCubeResult>();

            foreach (var patternCubeResult in path) allCubes.AddRange(patternCubeResult.Cubes);

            _allPath = allCubes.ToArray();
        }
    }
}