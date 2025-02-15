using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Unity;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonSelectorPanel : MonoBehaviour
    {
        [SerializeField]
        private DungeonController _dungeonController;
        [SerializeField]
        private Transform _buttonTransform;
        [SerializeField]
        private DungeonSelectorButton _dungeonButtonPrefab;

        void Awake()
        {
            Assertion.NotNull(this, _buttonTransform, _dungeonButtonPrefab, _dungeonController);
        }

        void OnEnable()
        {
            _dungeonController.ManifestData.AddListener(HandleManifestLoaded);
        }

        void OnDisable()
        {
            _dungeonController.ManifestData.RemoveListener(HandleManifestLoaded);
        }

        private void HandleManifestLoaded(DungeonCrawlerManifest manifest)
        {
            _buttonTransform.DestroyAllChildren();
            foreach ((string name, Dungeon d) in manifest.Dungeons)
            {
                DungeonSelectorButton button = Instantiate(_dungeonButtonPrefab, _buttonTransform);
                button.Label.text = name;
                button.Button.onClick.AddListener(() => BuildDungeon(d));
            }
        }

        private void BuildDungeon(Dungeon d)
        {
            Hide();
            _dungeonController.Build(d);
        }


        private void Hide() => gameObject.SetActive(false);
        public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);

    }
}