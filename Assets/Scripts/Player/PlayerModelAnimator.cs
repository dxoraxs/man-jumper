using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player
{
    public class PlayerModelAnimator : MonoBehaviour
    {
        [ReadOnly] [SerializeField] private SkinnedMeshRenderer _meshRenderer;
        [Space] [SerializeField] private Transform _camera;
        [SerializeField] private float _duration = .5f;
        [SerializeField] private float _strength = 4;
        [SerializeField] private int _vibrato = 10;

        private void OnValidate()
        {
            if (_meshRenderer == null)
                _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        public void OnHit()
        {
            _camera.DOShakeRotation(_duration, _strength, _vibrato);
            foreach (var material in _meshRenderer.materials)
            {
                material.color = Color.red;
                material.DOColor(Color.white, _duration);
            }
        }

        public void OnInvincibility(bool value)
        {
            var materialColor = value ? Color.green : Color.white;
            foreach (var material in _meshRenderer.materials)
                material.color = materialColor;
        }
    }
}