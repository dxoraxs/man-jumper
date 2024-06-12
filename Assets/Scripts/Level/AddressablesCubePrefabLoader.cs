using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Level
{
    public class AddressablesCubePrefabLoader : MonoBehaviour
    {
        [SerializeField] private List<AssetReference> _prefabReferences;
        private readonly Dictionary<CubeTypes, CubeContainer> _cubes = new();

        public IReadOnlyDictionary<CubeTypes, CubeContainer> Cubes => _cubes;

        private void OnDestroy()
        {
            foreach (var assetReference in _prefabReferences)
                assetReference.ReleaseAsset();
        }

        public async UniTask LoadCubes()
        {
            var tasks = Enumerable.Select(_prefabReferences, LoadCubeContainerAsync).ToList();

            var cubeContainers = await UniTask.WhenAll(tasks);

            foreach (var cube in cubeContainers)
                _cubes.Add(cube.Type, cube);
        }

        private static async UniTask<CubeContainer> LoadCubeContainerAsync(AssetReference prefabReference)
        {
            var prefab = await prefabReference.LoadAssetAsync<GameObject>().Task;
            return prefab.GetComponent<CubeContainer>();
        }
    }
}