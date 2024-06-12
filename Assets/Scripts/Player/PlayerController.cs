using Sirenix.OdinInspector;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [ReadOnly] [SerializeField] private Animator _animator;

        [field: SerializeField]
        [field: ReadOnly]
        public PlayerMoverController PlayerMoverController { get; private set; }

        [field: SerializeField]
        [field: ReadOnly]
        public PlayerModelAnimator PlayerModelAnimator { get; private set; }

        private PlayerAnimatorController _playerAnimatorController;

        private void Start()
        {
            _playerAnimatorController = new PlayerAnimatorController(_animator);

            PlayerMoverController.OnStartJump += () => _playerAnimatorController.SetJumpUp();
            PlayerMoverController.OnStopJump += () => _playerAnimatorController.SetJumpDown();
            PlayerMoverController.OnStartRun += () => _playerAnimatorController.SetRun();
            //PlayerMoverController.OnStopRun += () => _playerAnimatorController.SetVictory();
        }

        private void OnValidate()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
            if (PlayerMoverController == null)
                PlayerMoverController = GetComponent<PlayerMoverController>();
            if (PlayerModelAnimator == null)
                PlayerModelAnimator = GetComponent<PlayerModelAnimator>();
        }

        public void OnEndLevel(bool isWin)
        {
            if (isWin) _playerAnimatorController.SetVictory();
            else _playerAnimatorController.ResetAnimator();
        }
    }
}