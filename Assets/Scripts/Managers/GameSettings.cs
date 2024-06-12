using System;
using System.Collections.Generic;
using Level;
using Managers.Booster;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class GameSettings : MonoBehaviour
    {
        [field: SerializeField] public int CubeCount { get; private set; } = 40;
        [field: SerializeField] public float Speed { get; private set; } = 2;
        [field: SerializeField] public int MaxHealth { get; private set; } = 3;
        [field: Space] [field: SerializeField] public BoosterSettings BoosterSettings { get; private set; }
        [field: Space] [field: SerializeField] public HitCubeSettings HitCubeSettings { get; private set; }

        private void OnValidate()
        {
            HitCubeSettings.OnValidate();
            BoosterSettings.OnValidate();
        }
    }

    [Serializable]
    public class HitCubeSettings
    {
        [SerializeField] private HitCubeValue[] _hitCubeValues;

        public IEnumerable<HitCubeValue> HitCubeValues => _hitCubeValues;

        public void OnValidate()
        {
            if (_hitCubeValues == null)
            {
                _hitCubeValues = new HitCubeValue[HitCubeListener.CubeHitFilter.Count];
                for (var i = 0; i < HitCubeListener.CubeHitFilter.Count; i++)
                    _hitCubeValues[i] = new HitCubeValue(HitCubeListener.CubeHitFilter[i],
                        PlayerMoverController.JumpState.FirstJump);
            }

            if (_hitCubeValues.Length != HitCubeListener.CubeHitFilter.Count)
                Array.Resize(ref _hitCubeValues, HitCubeListener.CubeHitFilter.Count);
        }
    }

    [Serializable]
    public class HitCubeValue
    {
        [ReadOnly] public CubeTypes Key;
        public PlayerMoverController.JumpState[] JumpState;

        public HitCubeValue(CubeTypes key, params PlayerMoverController.JumpState[] jumpState)
        {
            Key = key;
            JumpState = jumpState;
        }
    }
}