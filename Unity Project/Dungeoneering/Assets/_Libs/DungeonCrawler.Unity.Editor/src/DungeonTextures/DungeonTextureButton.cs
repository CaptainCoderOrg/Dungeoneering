using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonTextureButton : MonoBehaviour
    {
        [field: SerializeField]
        public RawImage Image { get; private set; }
        public TextureId TextureId { get; set; }

        [field: SerializeField]
        public UnityEvent<DungeonTextureButton> OnClick { get; private set; }

        private Button _button;

        void Awake()
        {
            _button = GetComponentInChildren<Button>();
            Debug.Assert(_button != null);
            _button.onClick.AddListener(Clicked);
        }

        private void Clicked() => OnClick.Invoke(this);
    }
}