using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using NaughtyAttributes;
using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonTextureSelectorController : MonoBehaviour
    {
        [field: SerializeField]
        public Transform Grid { get; private set; }
        [field: SerializeField]
        public DungeonManifestData Manifest { get; private set; }
        [field: SerializeField]
        public DungeonTextureButton ButtonPrefab { get; private set; }

        void Awake()
        {
            InitializeGrid(Manifest);
        }

        [Button]
        private void InitializeGrid() => InitializeGrid(Manifest);

        public void InitializeGrid(DungeonManifestData manifest)
        {
            Grid.DestroyAllChildren();
            foreach(var textureEntry in manifest.MaterialCache)
            {
                Debug.Log($"Adding: {textureEntry.Key}");
                DungeonTextureButton btn = Instantiate(ButtonPrefab, Grid);
                btn.Image.texture = textureEntry.Value.mainTexture;
            }
        }

    }
}