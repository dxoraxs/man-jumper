using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public class UIBaseContainer : MonoBehaviour
    {
        private const float FADE_ANIMATE_DURATION = .5f;
        [ReadOnly] [SerializeField] private CanvasGroup _canvasGroup;

        protected virtual void OnValidate()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FastHide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        public void Hide()
        {
            _canvasGroup.DOFade(0, FADE_ANIMATE_DURATION);
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        public void Show()
        {
            _canvasGroup.DOFade(1, FADE_ANIMATE_DURATION);
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }
    }
}