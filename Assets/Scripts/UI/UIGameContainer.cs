using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace UI
{
    public class UIGameContainer : UIBaseContainer
    {
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private CustomButton _inputZone;

        public void SetHealth(int health)
        {
            _healthText.text = $"HEALTH: {health}";
        }

        public void SetInputListener(Action onClick)
        {
            _inputZone.OnDownObservable.Subscribe(_ => onClick.Invoke()).AddTo(this);
        }
    }
}