using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Player
{
    public class PlayerMoverController : MonoBehaviour
    {
        public enum JumpState
        {
            Ready = 0,
            FirstJump = 1,
            SecondJump = 2
        }

        [SerializeField] private Transform _model;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _jumpDuration;
        [SerializeField] private float _jumpDownMultiplier = 1.25f;
        [SerializeField] private AnimationCurve _jumpCurve;
        private Vector3[] _calculatePath = Array.Empty<Vector3>();
        private int _index;
        private Coroutine _jumpCoroutine;
        private float _jumpVerticalPosition;
        private Coroutine _moveCoroutine;
        private float _speed;
        private float _speedMultiplier = 1f;
        private Tween _speedUpTween;

        public JumpState CurrentJumpState { get; private set; } = JumpState.Ready;

        public Vector3 Position
        {
            get
            {
                var position = transform.position;
                position.y = _jumpVerticalPosition;
                return position;
            }
        }

        public void SetSpeedUp(float timer)
        {
            _speedMultiplier = 2;
            _speedUpTween?.Kill();
            _speedUpTween = DOVirtual.DelayedCall(timer, () => _speedMultiplier = 1);
        }

        public void StopSpeedUp()
        {
            _speedUpTween?.Kill();
            _speedMultiplier = 1;
        }

        public void Initialize(float speed)
        {
            _speed = speed;
        }

        public event Action OnStartJump;
        public event Action OnStopJump;
        public event Action OnStartRun;
        public event Action OnStopRun;

        public void StopMove()
        {
            OnStopRun?.Invoke();
            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        }

        public void ReincarnationPlayer(int indexOffset)
        {
            _index += indexOffset;
            AccelerationPlayer();
        }

        private void AccelerationPlayer()
        {
            var oldSpeed = _speed;
            OnStartRun?.Invoke();
            _speed /= 2;
            DOVirtual.Float(_speed, oldSpeed, 2, value => _speed = value);
            _moveCoroutine = StartCoroutine(MovePlayer());
        }

        public void SetHorizontalPath(Vector3[] path)
        {
            _index = 0;
            _calculatePath = path;
        }

        public void StartMovePlayer()
        {
            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
            _moveCoroutine = StartCoroutine(MovePlayer());
        }

        public void JumpPlayer()
        {
            if (CurrentJumpState == JumpState.SecondJump) return;

            if (_jumpCoroutine != null) StopCoroutine(_jumpCoroutine);
            _jumpCoroutine = StartCoroutine(Jump());
        }

        private IEnumerator Jump()
        {
            CurrentJumpState++;
            OnStartJump?.Invoke();
            var timer = 0f;
            var startJumpOffset = _jumpVerticalPosition;
            while (timer < _jumpDuration)
            {
                timer += Time.deltaTime;
                _jumpVerticalPosition = startJumpOffset + _jumpForce * _jumpCurve.Evaluate(timer / _jumpDuration);
                _model.transform.localPosition = Vector3.up * _jumpVerticalPosition;
                yield return null;
            }

            timer = 0;
            while (_jumpVerticalPosition > 0)
            {
                timer += Time.deltaTime;
                _jumpVerticalPosition += _jumpDownMultiplier * Physics.gravity.y * timer * Time.deltaTime;
                _model.transform.localPosition = Vector3.up * _jumpVerticalPosition;
                yield return null;
            }

            _jumpVerticalPosition = 0;
            CurrentJumpState = JumpState.Ready;
            OnStopJump?.Invoke();
        }

        private IEnumerator MovePlayer()
        {
            OnStartRun?.Invoke();

            while (_index < _calculatePath.Length)
            {
                var targetPosition = _calculatePath[_index];

                while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    var newTransformPosition =
                        Vector3.MoveTowards(transform.position, targetPosition,
                            _speedMultiplier * _speed * Time.deltaTime);
                    var direction = newTransformPosition - transform.position;
                    if (direction != Vector3.zero)
                    {
                        var targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                            _rotationSpeed * Time.deltaTime);
                    }

                    transform.position = newTransformPosition;
                    yield return null;
                }

                _index++;
            }

            OnStopRun?.Invoke();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                JumpPlayer();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + Vector3.up * _jumpVerticalPosition, 0.25f);

            for (var index = 0; index < _calculatePath.Length - 1; index++)
            {
                var vector3 = _calculatePath[index];
                var nextVector3 = _calculatePath[index + 1];
                Gizmos.DrawLine(vector3, nextVector3);
            }
        }
#endif
    }
}