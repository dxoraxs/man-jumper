using System.Linq;
using DG.Tweening;
using Helpers;
using Level;
using Managers.Booster;
using Player;
using Sirenix.OdinInspector;
using UI;
using UniRx;
using UnityEngine;

namespace Managers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameSettings _gameSettings;
        [SerializeField] private LevelGenerator _levelGenerator;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private UIController _uiController;
        private BoosterController _boosterController;
        private CubeAnimator _cubeAnimator;
        private HealthController _healthController;
        private HitCubeListener _hitCubeListener;
        private bool _isGameStarted;
        private PlayerCubeMoveListener _playerCubeMoveListener;

        private async void Start()
        {
            _healthController = new HealthController(_gameSettings.MaxHealth);
            _healthController.OnHit += _playerController.PlayerModelAnimator.OnHit;
            _healthController.InvincibilityObservable.Subscribe(_playerController.PlayerModelAnimator.OnInvincibility)
                .AddTo(this);
            _playerCubeMoveListener = new PlayerCubeMoveListener(_playerController.PlayerMoverController);
            _boosterController = new BoosterController(_gameSettings.BoosterSettings,
                _playerCubeMoveListener.CubeIndexObservable, _levelGenerator.LevelContainer);

            InitializeBoosterEvent();

            _uiController.SubscribeHealthObservable(_healthController.OnHealthObservable);
            _uiController.SubscribeEndLevelButtons(OnClickRestartOrNextLevel, OnClickContinue,
                OnClickRestartOrNextLevel);
            _uiController.SubscribeInputListener(OnClickGameInput);

            _playerController.PlayerMoverController.Initialize(_gameSettings.Speed);

            _healthController.OnDeath += () => OnEndLevel(false);
            _healthController.OnDeath += _playerController.PlayerMoverController.StopMove;
            _playerCubeMoveListener.OnEndPath += () => OnEndLevel(true);
            await _levelGenerator.Initialize();
            InitializeNewLevel();
        }

        private void InitializeBoosterEvent()
        {
            _boosterController.OnTakeBooster.Where(booster => booster.type == BoosterType.Health)
                .Subscribe(boosterValue => _healthController.AddHealth((int)boosterValue.value)).AddTo(this);

            _boosterController.OnTakeBooster.Where(booster => booster.type == BoosterType.Invincibility)
                .Subscribe(boosterValue => _healthController.TakeInvincibility(boosterValue.value)).AddTo(this);

            _boosterController.OnTakeBooster.Where(booster => booster.type == BoosterType.SpeedUp)
                .Subscribe(boosterValue => _playerController.PlayerMoverController.SetSpeedUp(boosterValue.value))
                .AddTo(this);
        }

        private void OnEndLevel(bool isWinValue)
        {
            _isGameStarted = false;
            _healthController.OnEndLevel();
            _playerController.PlayerMoverController.StopSpeedUp();
            _playerController.OnEndLevel(isWinValue);
            _uiController.OnEndLevel(new EndPanelData(isWinValue,
                _levelGenerator.GenerateEndLevelData(_playerCubeMoveListener.StayCubeIndex)));
        }

        private void OnClickGameInput()
        {
            if (!_isGameStarted) StartLevel();
            else _playerController.PlayerMoverController.JumpPlayer();
        }

        private void OnClickRestartOrNextLevel()
        {
            _uiController.OnStartLevel();
            _healthController.ResetHealth();
            RegenerateLevel();
        }

        private void OnClickContinue()
        {
            _isGameStarted = true;
            _healthController.AddHealth(1);
            MovePlayerToNearestSafeCube();
            _uiController.OnStartLevel();
        }

        private void MovePlayerToNearestSafeCube()
        {
            _hitCubeListener.OnHit -= _healthController.TakeHit;
            var cubeData = _levelGenerator.CubePosition(_playerCubeMoveListener.StayCubeIndex);
            var duration = (cubeData.position - _playerController.transform.position).magnitude / 2;

            _playerController.transform.DOMove(cubeData.position, duration)
                .SetEase(Ease.Linear)
                .OnComplete(OnPlayerEndMoveToSafeCube);

            void OnPlayerEndMoveToSafeCube()
            {
                _playerController.PlayerMoverController.ReincarnationPlayer(cubeData.indexOffset);
                _hitCubeListener.OnHit += _healthController.TakeHit;
            }
        }

        [Button]
        private void RegenerateLevel()
        {
            _cubeAnimator.Dispose();
            _hitCubeListener.Dispose();
            _levelGenerator.ClearLevel(_playerController.transform.position);
            InitializeNewLevel();
        }

        private void InitializeNewLevel()
        {
            _levelGenerator.StartGenerateLevel(_gameSettings.CubeCount);
            var newLevelPatterns = _levelGenerator.PatternsLevelData;
            var cubePath = newLevelPatterns.CubePath.ToArray();
            _playerController.PlayerMoverController.SetHorizontalPath(
                PlayerPathCalculator.Calculate(cubePath, _playerController.transform.position));
            _playerCubeMoveListener.SetNewLevelPath(newLevelPatterns.PatternLevelData);
            _hitCubeListener = new HitCubeListener(_playerCubeMoveListener, _playerController.PlayerMoverController,
                _gameSettings.HitCubeSettings.HitCubeValues);
            _hitCubeListener.OnHit += _healthController.TakeHit;
            _hitCubeListener.SetNewLevelPath(newLevelPatterns.PatternLevelData);
            _cubeAnimator = new CubeAnimator(_levelGenerator.UsedCubes.ToArray(),
                newLevelPatterns.PatternLevelData.ToArray(), _playerCubeMoveListener.CubeIndexObservable);
            _boosterController.SetCubePath(cubePath);
        }

        private void StartLevel()
        {
            _isGameStarted = true;
            _playerController.PlayerMoverController.StartMovePlayer();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                StartLevel();
        }

        private void OnDrawGizmosSelected()
        {
            if (_playerCubeMoveListener == null || _hitCubeListener == null) return;

            foreach (var patternCube in _hitCubeListener.GetFiltersCube)
            {
                if (patternCube.Type == CubeTypes.Default || patternCube.Type == CubeTypes.Turn)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.red;
                Gizmos.DrawWireCube(patternCube.Position - Vector3.up / 2, Vector3.one);
            }
        }
#endif
    }
}