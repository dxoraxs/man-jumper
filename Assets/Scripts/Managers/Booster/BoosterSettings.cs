using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers.Booster
{
    [Serializable]
    public class BoosterSettings
    {
        [SerializeField] private BoosterSettingsValue[] _boosterValues;

        public IEnumerable<BoosterSettingsValue> BoosterSettingsValues => _boosterValues;

        public void OnValidate()
        {
            var length = Enum.GetNames(typeof(BoosterType)).Length;
            if (_boosterValues == null)
            {
                _boosterValues = new BoosterSettingsValue[length];
                for (var i = 0; i < length; i++)
                    _boosterValues[i] = new BoosterSettingsValue((BoosterType)i, new BoosterValue());
            }

            if (_boosterValues.Length != length)
                Array.Resize(ref _boosterValues, length);
            for (var index = 0; index < _boosterValues.Length; index++)
            {
                var boosterSettingsValue = _boosterValues[index];
                boosterSettingsValue.Key = (BoosterType)index;
            }
        }
    }

    [Serializable]
    public class BoosterSettingsValue
    {
        [ReadOnly] public BoosterType Key;
        public BoosterValue Value;

        public BoosterSettingsValue(BoosterType key, BoosterValue value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class BoosterValue
    {
        [field: SerializeField] public float SpawnChance { get; private set; } = 0.1f;
        [field: SerializeField] public float CustomValue { get; private set; } = 1;
        [field: SerializeField] public GameObject View { get; private set; }
    }
}