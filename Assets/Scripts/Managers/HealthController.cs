using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Managers
{
    public class HealthController : IDisposable
    {
        private readonly CompositeDisposable _disposable = new();
        private readonly ReactiveProperty<int> _heath;

        private readonly Subject<bool> _invincibilityStream = new();
        private readonly int _maxHealth;
        private Tween _invincibilityTween;
        private bool _isInvincibility;

        public HealthController(int maxHealth)
        {
            _maxHealth = maxHealth;
            _heath = new ReactiveProperty<int>(maxHealth).AddTo(_disposable);
            _invincibilityStream.AddTo(_disposable);
            _heath.Where(value => value <= 0).Subscribe(_ => OnDeath()).AddTo(_disposable);
            _invincibilityStream.Subscribe(value => _isInvincibility = value).AddTo(_disposable);
        }

        public IObservable<int> OnHealthObservable => _heath;
        public IObservable<bool> InvincibilityObservable => _invincibilityStream;

        public void Dispose()
        {
            _heath?.Dispose();
            _disposable?.Dispose();
        }

        public event Action OnDeath = delegate { };
        public event Action OnHit = delegate { };

        public void TakeInvincibility(float time)
        {
            _invincibilityStream.OnNext(true);
            _invincibilityTween?.Kill();
            _invincibilityTween = DOVirtual.DelayedCall(time, () => _invincibilityStream.OnNext(false));
        }

        public void OnEndLevel()
        {
            _invincibilityTween?.Kill();
            _invincibilityStream.OnNext(false);
        }

        public void TakeHit()
        {
            if (_isInvincibility) return;
            _heath.Value--;
            OnHit?.Invoke();
        }

        public void AddHealth(int value)
        {
            _heath.Value = Mathf.Min(_heath.Value + value, _maxHealth);
        }

        public void ResetHealth()
        {
            _heath.Value = _maxHealth;
        }
    }
}