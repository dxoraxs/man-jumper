using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private bool _isEnabled = true;
        private readonly Subject<Unit> _onClickSubject = new Subject<Unit>();
        private readonly Subject<Unit> _onDownSubject = new Subject<Unit>();
        private readonly Subject<bool> _onEnableSubject = new Subject<bool>();
        private readonly Subject<Unit> _onPointerEnterSubject = new Subject<Unit>();
        private readonly Subject<Unit> _onPointerExitSubject = new Subject<Unit>();
        private readonly Subject<Unit> _onUpSubject = new Subject<Unit>();

        public IObservable<Unit> OnDownObservable => _onDownSubject;
        public IObservable<Unit> OnUpObservable => _onUpSubject;
        public IObservable<Unit> OnClickObservable => _onClickSubject;
        public IObservable<Unit> OnPointerEnterObservable => _onPointerEnterSubject;
        public IObservable<Unit> OnPointerExitObservable => _onPointerExitSubject;
        public IObservable<bool> OnEnableObservable => _onEnableSubject;

        public bool Enabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                _onEnableSubject.OnNext(value);
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (Enabled) _onClickSubject.OnNext(Unit.Default);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _onDownSubject.OnNext(Unit.Default);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Enabled) _onPointerEnterSubject.OnNext(Unit.Default);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _onPointerExitSubject.OnNext(Unit.Default);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _onUpSubject.OnNext(Unit.Default);
        }
    }
}