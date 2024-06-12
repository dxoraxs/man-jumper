using UnityEngine;

namespace Player
{
    public class PlayerAnimatorController
    {
        private const string RunKey = "Run";
        private const string JumpUpKey = "JumpUp";
        private const string JumpDownKey = "JumpDown";
        private const string VictoryKey = "Victory";

        private readonly Animator _animator;

        public PlayerAnimatorController(Animator animator)
        {
            _animator = animator;
        }

        public void SetRun()
        {
            ResetAnimator();
            _animator.SetTrigger(RunKey);
        }

        public void SetJumpUp()
        {
            _animator.SetTrigger(JumpUpKey);
        }

        public void SetJumpDown()
        {
            _animator.ResetTrigger(JumpUpKey);
            _animator.SetTrigger(JumpDownKey);
        }

        public void SetVictory()
        {
            ResetAnimator();
            _animator.SetTrigger(VictoryKey);
        }

        public void ResetAnimator()
        {
            _animator.Rebind();
            _animator.Update(0f);
        }
    }
}