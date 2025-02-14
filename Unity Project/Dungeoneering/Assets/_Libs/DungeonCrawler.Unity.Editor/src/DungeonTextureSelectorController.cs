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

        private System.Action<string> _onSelectedCallback;
        private System.Action _onCanceledCallback;

        void Awake()
        {
            InitializeGrid(Manifest);
        }

        [Button]
        private void InitializeGrid() => InitializeGrid(Manifest);

        public void InitializeGrid(DungeonManifestData manifest)
        {
            Grid.DestroyAllChildren();
            foreach (var textureEntry in manifest.MaterialCache)
            {
                DungeonTextureButton btn = Instantiate(ButtonPrefab, Grid);
                btn.TextureName = textureEntry.Key;
                btn.Image.texture = textureEntry.Value.mainTexture;
                btn.OnClick.AddListener(SelectTexture);
            }
        }

        public void Cancel()
        {
            gameObject.SetActive(false);
            _onCanceledCallback?.Invoke();
        }

        private void SelectTexture(DungeonTextureButton textureButton)
        {
            gameObject.SetActive(false);
            _onSelectedCallback?.Invoke(textureButton.TextureName);

        }

        public void ShowDialogue(System.Action<string> onSelected, System.Action onCanceled)
        {
            _onSelectedCallback = onSelected;
            _onCanceledCallback = onCanceled;
            gameObject.SetActive(true);
        }

    }
}