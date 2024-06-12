using System;
using System.Collections.Generic;
using Level.ObstaclePatterns;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;

namespace UI
{
    public class EndPanelContainer : UIBaseContainer
    {
        public static readonly List<ObstaclesTypes> FilterObstacles = new()
            { ObstaclesTypes.Fence, ObstaclesTypes.Saw, ObstaclesTypes.Space, ObstaclesTypes.DoubleSpace };

        [SerializeField] private TMP_Text _endWinText;
        [SerializeField] private TMP_Text _endLoseText;
        [SerializeField] private GameObject _loseButton;
        [SerializeField] private GameObject _winButton;
        [SerializeField] private CustomButton _restartButton;
        [SerializeField] private CustomButton _continueButton;
        [SerializeField] private CustomButton _nextLevelButton;
        [SerializeField] private ObstacleViewData[] _obstacleViewData;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_obstacleViewData.Length != FilterObstacles.Count)
                Array.Resize(ref _obstacleViewData, FilterObstacles.Count);
            for (var i = 0; i < FilterObstacles.Count; i++)
                _obstacleViewData[i].Type = FilterObstacles[i];
        }

        public void InitializeButtons(Action onRestart, Action onContinue, Action onNextLevel)
        {
            _restartButton.OnClickObservable.Subscribe(_ => onRestart?.Invoke()).AddTo(this);
            _continueButton.OnClickObservable.Subscribe(_ => onContinue?.Invoke()).AddTo(this);
            _nextLevelButton.OnClickObservable.Subscribe(_ => onNextLevel?.Invoke()).AddTo(this);
        }

        public void SetValue(EndPanelData endPanelData)
        {
            _nextLevelButton.gameObject.SetActive(endPanelData.IsWin);
            _endWinText.gameObject.SetActive(endPanelData.IsWin);
            _endLoseText.gameObject.SetActive(!endPanelData.IsWin);

            _loseButton.SetActive(!endPanelData.IsWin);
            _winButton.SetActive(endPanelData.IsWin);

            for (var index = 0; index < FilterObstacles.Count; index++)
            {
                var filterObstacle = FilterObstacles[index];
                var count = endPanelData.ObstaclesCount.GetValueOrDefault(filterObstacle, 0);
                _obstacleViewData[index].Text.text = count.ToString();
            }
        }

        [Serializable]
        private class ObstacleViewData
        {
            [ReadOnly] public ObstaclesTypes Type;
            public TMP_Text Text;
        }
    }

    public readonly struct EndPanelData
    {
        public readonly bool IsWin;
        public readonly Dictionary<ObstaclesTypes, int> ObstaclesCount;

        public EndPanelData(bool isWin, Dictionary<ObstaclesTypes, int> obstaclesCount)
        {
            IsWin = isWin;
            ObstaclesCount = obstaclesCount;
        }
    }
}