using System;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [ReadOnly] [SerializeField] private UIGameContainer _gameContainer;
        [ReadOnly] [SerializeField] private EndPanelContainer _endPanelContainer;

        private void Start()
        {
            _gameContainer.Show();
            _endPanelContainer.FastHide();
        }

        private void OnValidate()
        {
            _gameContainer = GetComponentInChildren<UIGameContainer>();
            _endPanelContainer = GetComponentInChildren<EndPanelContainer>();
        }

        public void SubscribeHealthObservable(IObservable<int> healthObservable)
        {
            healthObservable.Subscribe(_gameContainer.SetHealth).AddTo(this);
        }

        public void SubscribeInputListener(Action onInputListener)
        {
            _gameContainer.SetInputListener(onInputListener);
        }

        public void SubscribeEndLevelButtons(Action onRestart, Action onContinue, Action onNextLevel)
        {
            _endPanelContainer.InitializeButtons(onRestart, onContinue, onNextLevel);
        }

        public void OnEndLevel(EndPanelData endPanelData)
        {
            _gameContainer.FastHide();
            _endPanelContainer.Show();
            _endPanelContainer.SetValue(endPanelData);
        }

        public void OnStartLevel()
        {
            _gameContainer.Show();
            _endPanelContainer.Hide();
        }
    }
}