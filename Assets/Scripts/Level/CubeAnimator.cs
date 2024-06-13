using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Level
{
    public class CubeAnimator : IDisposable
    {
        public const int OFFSET_INDEX_ANIMATION = 5;
        private const float ANIMATE_DURATION = .25f;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly Dictionary<int, CubeAnimatorStateData> _cubeContainers = new Dictionary<int, CubeAnimatorStateData>();
        private readonly Vector3 DISABLE_OFFSET_POSITION = Vector3.up * -3;

        public CubeAnimator(CubeContainer[] cubeContainers, PatternLevelData[] patternLevelData,
            IObservable<int> playerIndexObservable)
        {
            var cubeIndex = 0;
            var cubeIndexPattern = 0;
            foreach (var pattern in patternLevelData)
            foreach (var patternCube in pattern.Cubes)
            {
                if (patternCube.Type != CubeTypes.Space)
                {
                    _cubeContainers.Add(cubeIndexPattern, new CubeAnimatorStateData(cubeContainers[cubeIndex]));
                    cubeIndex++;
                }

                cubeIndexPattern++;
            }

            foreach (var cubeContainer in cubeContainers)
                cubeContainer.gameObject.SetActive(false);

            var limit = Math.Min(OFFSET_INDEX_ANIMATION, _cubeContainers.Count);
            for (var index = 0; index < limit; index++)
            {
                if (!_cubeContainers.TryGetValue(index, out var cubeContainer)) continue;

                cubeContainer.State = CubeContainerAnimationState.Enable;
                cubeContainer.Cube.gameObject.SetActive(true);
            }

            playerIndexObservable.Subscribe(OnChangeIndex).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        private void OnChangeIndex(int newIndex)
        {
            var startIndex = newIndex - OFFSET_INDEX_ANIMATION;
            var endIndex = newIndex + OFFSET_INDEX_ANIMATION;

            for (var index = startIndex; index < endIndex; index++)
            {
                if (!_cubeContainers.TryGetValue(index, out var cubeContainer)) continue;

                if (index < newIndex - 1)
                {
                    if (cubeContainer.State != CubeContainerAnimationState.Disable) DisableAnimation(cubeContainer);
                }
                else if (index >= newIndex)
                {
                    if (cubeContainer.State != CubeContainerAnimationState.Enable) EnableAnimation(cubeContainer);
                }
            }
        }

        private void EnableAnimation(CubeAnimatorStateData cubeContainer)
        {
            cubeContainer.State = CubeContainerAnimationState.Enable;
            cubeContainer.Cube.gameObject.SetActive(true);
            var defaultPosition = cubeContainer.Cube.transform.position;
            cubeContainer.Cube.transform.position += DISABLE_OFFSET_POSITION;
            cubeContainer.Cube.transform.DOMove(defaultPosition, ANIMATE_DURATION)
                .SetEase(Ease.OutBounce);
        }

        private void DisableAnimation(CubeAnimatorStateData cubeContainer)
        {
            cubeContainer.State = CubeContainerAnimationState.Disable;
            var defaultPosition = cubeContainer.Cube.transform.position;
            cubeContainer.Cube.transform.DOMove(defaultPosition + DISABLE_OFFSET_POSITION, ANIMATE_DURATION)
                .SetEase(Ease.OutBounce)
                .OnComplete(() => cubeContainer.Cube.gameObject.SetActive(false));
        }

        private enum CubeContainerAnimationState
        {
            Enable = 0,
            Disable = 1
        }

        private class CubeAnimatorStateData
        {
            public readonly CubeContainer Cube;
            public CubeContainerAnimationState State;

            public CubeAnimatorStateData(CubeContainer cube)
            {
                Cube = cube;
                State = CubeContainerAnimationState.Disable;
            }
        }
    }
}