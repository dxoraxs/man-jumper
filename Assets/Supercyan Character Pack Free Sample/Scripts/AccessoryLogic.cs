using UnityEngine;

namespace Supercyan.FreeSample
{
    public class AccessoryLogic : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer m_renderer;

        [SerializeField] private GameObject m_rig;
        public SkinnedMeshRenderer Renderer => m_renderer;

        private void Awake()
        {
            Destroy(m_rig);
        }
    }
}