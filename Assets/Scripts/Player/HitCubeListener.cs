using System;
using System.Collections.Generic;
using System.Linq;
using Level;
using Level.ObstaclePatterns;
using Managers;
using UniRx;

namespace Player
{
    public class HitCubeListener : IDisposable
    {
        public static readonly List<CubeTypes> CubeHitFilter = new List<CubeTypes>()
            { CubeTypes.Saw, CubeTypes.Fence, CubeTypes.Space };

        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly Dictionary<int, PatternCubeResult> _filtersCube = new Dictionary<int, PatternCubeResult>();

        private readonly PlayerMoverController _playerMoverController;
        private readonly Dictionary<CubeTypes, List<PlayerMoverController.JumpState>> _triggerZone = new Dictionary<CubeTypes, List<PlayerMoverController.JumpState>>();
        private bool _isWaitHit;

        private CubeTypes _lastFilterCubeType;

        public HitCubeListener(PlayerCubeMoveListener playerCubeMoveListener,
            PlayerMoverController playerMoverController,
            IEnumerable<HitCubeValue> hitCubeSettings)
        {
            _playerMoverController = playerMoverController;

            foreach (var hitCubeSetting in hitCubeSettings)
                _triggerZone.Add(hitCubeSetting.Key,
                    new List<PlayerMoverController.JumpState>(hitCubeSetting.JumpState));

            playerCubeMoveListener.CubeIndexObservable.Subscribe(OnChangeCubeIndex).AddTo(_compositeDisposable);

            Observable.EveryUpdate()
                .Where(_ => _isWaitHit)
                .Subscribe(_ => OnWaitHit())
                .AddTo(_compositeDisposable);
        }

        public PatternCubeResult[] GetFiltersCube => _filtersCube.Values.ToArray();

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
            OnHit = null;
        }

        public event Action OnHit = delegate { };

        private void OnWaitHit()
        {
            if (!_triggerZone.TryGetValue(_lastFilterCubeType, out var triggerHeight)) return;

            if (triggerHeight.Contains(_playerMoverController.CurrentJumpState)) return;

            OnHit?.Invoke();
            _isWaitHit = false;
        }

        private void OnChangeCubeIndex(int index)
        {
            if (!_filtersCube.Any()) return;

            if (!_filtersCube.TryGetValue(index, out var currentCube) || !CubeHitFilter.Contains(currentCube.Type))
            {
                _isWaitHit = false;
                return;
            }

            _lastFilterCubeType = currentCube.Type;
            _isWaitHit = true;
        }

        public void SetNewLevelPath(IEnumerable<PatternLevelData> path)
        {
            _filtersCube.Clear();
            var indexCube = 0;
            foreach (var patternCubeResult in path)
            foreach (var patternCube in patternCubeResult.Cubes)
            {
                if (CubeHitFilter.Contains(patternCube.Type))
                    _filtersCube.Add(indexCube, patternCube);

                indexCube++;
            }
        }
    }
}