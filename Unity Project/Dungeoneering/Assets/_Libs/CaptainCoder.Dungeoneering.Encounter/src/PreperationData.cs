using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/PreperationData")]
    public class PreperationData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Color BackgroundAlbedo { get; private set; } = new Color(0, 0, 0, 0.5f);
        [field: ShowAssetPreview][field: SerializeField] public Sprite Sprite { get; private set; }
        [TextArea(3, 5)][SerializeField] private string _description;
        public string Description => _description;
        private string _tooltip;
        public string Tooltip => _tooltip ??= $"<u>{Name}</u>\n{_description}";
        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            _tooltip = $"<u>{Name}</u>\n{_description}";
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            _tooltip = null;
        }
#endif

    }
}