using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonTexturePreview : MonoBehaviour
    {
        public DungeonTextureButton SelectButton { get; private set; }
        [field: SerializeField]
        public UnityEvent<DungeonTexturePreview> OnDelete { get; private set; }
        public SelectableMaterial Material { get; private set; }

        void Awake()
        {
            SelectButton = GetComponentInChildren<DungeonTextureButton>();
            Debug.Assert(SelectButton != null);
        }

        public void Delete() => OnDelete.Invoke(this);

        internal static DungeonTexturePreview Instantiate(DungeonTexturePreview prefab, Transform parent, SelectableMaterial material)
        {
            DungeonTexturePreview preview = Instantiate(prefab, parent);
            preview.SelectButton.TextureId = material.Id;
            preview.SelectButton.Image.texture = material.Unselected.mainTexture;
            preview.Material = material;
            return preview;
        }
    }
}