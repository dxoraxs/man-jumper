using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Level;
using Level.ObstaclePatterns;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Managers.Booster
{
    public class BoosterController
    {
        private readonly Dictionary<int, BoosterLevelData> _boosterCubeDictionary = new Dictionary<int, BoosterLevelData>();
        private readonly Transform _boosterParent;
        private readonly Dictionary<BoosterType, BoosterValue> _boosterValueDictionary = new Dictionary<BoosterType, BoosterValue>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly Subject<(BoosterType type, float value)> _onTakeBooster = new Subject<(BoosterType type, float value)>();

        public BoosterController(BoosterSettings boosterSettings, IObservable<int> cubeIndexObservable,
            Transform boosterParent)
        {
            _boosterParent = boosterParent;

            foreach (var boosterSettingsValue in boosterSettings.BoosterSettingsValues)
                _boosterValueDictionary.Add(boosterSettingsValue.Key, boosterSettingsValue.Value);

            cubeIndexObservable.Subscribe(OnPlayerChangeCube).AddTo(_disposable);
            _onTakeBooster.AddTo(_disposable);
        }

        public IObservable<(BoosterType type, float value)> OnTakeBooster => _onTakeBooster;

        private void OnPlayerChangeCube(int newIndex)
        {
            if (_boosterCubeDictionary.TryGetValue(newIndex, out var boosterType))
            {
                var boosterValue = _boosterValueDictionary[boosterType.Type];
                _onTakeBooster.OnNext((boosterType.Type, boosterValue.CustomValue));
                _boosterCubeDictionary.Remove(newIndex);
                Object.Destroy(boosterType.View);
            }

            var startIndex = newIndex - CubeAnimator.OFFSET_INDEX_ANIMATION;
            var endIndex = newIndex + CubeAnimator.OFFSET_INDEX_ANIMATION;

            for (var index = startIndex; index < endIndex; index++)
            {
                if (!_boosterCubeDictionary.TryGetValue(index, out var cubeContainer)) continue;

                if (index < newIndex - 1)
                    cubeContainer.View.SetActive(false);
                else if (index >= newIndex) EnableBooster(cubeContainer);
            }
        }

        private static void EnableBooster(BoosterLevelData cubeContainer)
        {
            if (cubeContainer.View.activeSelf) return;

            cubeContainer.View.transform.localScale = Vector3.zero;
            cubeContainer.View.transform.DOScale(2, .25f);
            cubeContainer.View.SetActive(true);
        }

        public void SetCubePath(PatternCubeResult[] cubePath)
        {
            foreach (var boosterLevelData in _boosterCubeDictionary)
                Object.Destroy(boosterLevelData.Value.View);

            _boosterCubeDictionary.Clear();
            var valueCollection = _boosterValueDictionary.Values;
            var sumChance = valueCollection.Sum(value => value.SpawnChance);

            for (var index = 0; index < cubePath.Length; index++)
            {
                var patternCube = cubePath[index];
                if (patternCube.Type != CubeTypes.Default && patternCube.Type != CubeTypes.Turn) continue;

                var currentBooster = GetRandomBooster(_boosterValueDictionary, sumChance);

                if (currentBooster != BoosterType.None)
                    _boosterCubeDictionary.Add(index, CreateNewBooster(currentBooster, patternCube.Position));
            }

            OnPlayerChangeCube(0);
        }

        private BoosterLevelData CreateNewBooster(BoosterType boosterType, Vector3 position)
        {
            var view = Object.Instantiate(_boosterValueDictionary[boosterType].View, position, Quaternion.identity,
                _boosterParent);
            view.SetActive(false);
            return new BoosterLevelData(boosterType, view);
        }

        private BoosterType GetRandomBooster(Dictionary<BoosterType, BoosterValue> valueCollection, float sumChance)
        {
            var randomValue = Random.value * sumChance;
            for (var indexBooster = 0; indexBooster < valueCollection.Count; indexBooster++)
            {
                var obstacleContainer = valueCollection.ElementAt(indexBooster);
                randomValue -= obstacleContainer.Value.SpawnChance;

                if (randomValue <= 0)
                    return obstacleContainer.Key;
            }

            return BoosterType.None;
        }

        private struct BoosterLevelData
        {
            public readonly BoosterType Type;
            public readonly GameObject View;

            public BoosterLevelData(BoosterType type, GameObject view)
            {
                Type = type;
                View = view;
            }
        }
    }
}