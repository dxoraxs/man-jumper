using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ColorCustomButton : CustomButton
    {
        [SerializeField] private Image _graphics;
        [Space] [SerializeField] private Color _defaultState = Color.white;
        [SerializeField] private Color _clickState = new(.5f, .5f, .5f, 1);
        [SerializeField] private Color _disableState = new(.35f, .35f, .35f, 1);

        private void Start()
        {
            OnDownObservable.Subscribe(_ => _graphics.color = _clickState).AddTo(this);
            OnUpObservable.Subscribe(_ => _graphics.color = _defaultState).AddTo(this);
            OnEnableObservable.Subscribe(value => _graphics.color = value ? _defaultState : _disableState).AddTo(this);
        }
    }
}