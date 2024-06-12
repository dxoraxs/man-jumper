using UnityEngine;

namespace Level
{
    public class CubeContainer : MonoBehaviour
    {
        [field: SerializeField] public CubeTypes Type { get; private set; }
    }
}