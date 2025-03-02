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
        public TextureReference Texture { get; private set; }

        void Awake()
        {
            SelectButton = GetComponentInChildren<DungeonTextureButton>();
            Debug.Assert(SelectButton != null);
        }

        public void Delete() => OnDelete.Invoke(this);

        internal static DungeonTexturePreview Instantiate(DungeonTexturePreview prefab, Transform parent, TextureReference texture)
        {
            DungeonTexturePreview preview = Instantiate(prefab, parent);
            preview.SelectButton.TextureId = texture.TextureId;
            preview.SelectButton.Image.texture = texture.Material.Unselected.mainTexture;
            preview.Texture = texture;
            return preview;
        }
    }
}