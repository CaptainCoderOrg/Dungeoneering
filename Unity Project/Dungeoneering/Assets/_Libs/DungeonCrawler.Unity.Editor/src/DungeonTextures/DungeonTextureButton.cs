using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonTextureButton : MonoBehaviour
    {
        [SerializeField]
        private RawImage _image;
        [field: SerializeField]
        public UnityEvent<DungeonTextureButton> OnClick { get; private set; }

        private Button _button;
        private TextureReference _texture;
        public TextureReference Texture
        {
            get => _texture;
            set
            {
                if (_texture != null)
                {
                    _texture.OnTextureChange -= Render;
                }
                _texture = value;
                if (_texture != null)
                {
                    _texture.OnTextureChange += Render;
                    Render(_texture);
                }
                else
                {
                    _image.texture = default;
                }
            }
        }

        void Awake()
        {
            _button = GetComponentInChildren<Button>();
            Debug.Assert(_button != null);
            _button.onClick.AddListener(Clicked);
            Debug.Assert(_image != null, "Image was not set");
        }

        void OnDisable()
        {
            if (_texture == null) { return; }
            _texture.OnTextureChange -= Render;
        }

        private void Render(TextureReference texture)
        {
            _image.texture = texture.Texture;
        }

        private void Clicked() => OnClick.Invoke(this);
    }
}