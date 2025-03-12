using System;

using CaptainCoder.Dungeoneering.Unity.Data;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Unity.UI
{
    public class ConfirmTexturePromptPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _inputField;
        [SerializeField]
        private Button _confirmButton;
        [SerializeField]
        private RawImage _textureImage;
        [SerializeField]
        private TextMeshProUGUI _errorLabel;
        private DungeonCrawlerData _dungeonCrawlerData;
        private Action<string, Texture2D> _onConfirm;
        private Action _onCancel;
        private Texture2D _texture;

        void Awake()
        {
            Assertion.NotNull(this, _errorLabel, _inputField, _confirmButton, _textureImage);
            _inputField.onValueChanged.AddListener(Validate);
        }

        private void Validate(string name)
        {
            if (name == string.Empty)
            {
                SetError("Texture must have a name");
            }
            else if (_dungeonCrawlerData.HasTexture(name))
            {
                SetError($"Conflicting texture name: {name}");
            }
            else
            {
                ClearError();
            }
        }

        private void ClearError()
        {
            _errorLabel.text = string.Empty;
            _errorLabel.gameObject.SetActive(false);
            _confirmButton.interactable = true;
        }

        private void SetError(string error)
        {
            _errorLabel.text = error;
            _errorLabel.gameObject.SetActive(true);
            _confirmButton.interactable = false;
        }

        public void Prompt(DungeonCrawlerData dungeonCrawlerData, Texture2D texture, string defaultName, Action<string, Texture2D> onConfirm, Action onCancel = null)
        {
            _dungeonCrawlerData = dungeonCrawlerData;
            _inputField.text = defaultName;
            _texture = texture;
            _textureImage.texture = texture;
            _onConfirm = onConfirm;
            _onCancel = onCancel;
            Validate(defaultName);
            gameObject.SetActive(true);
        }

        public void Confirm()
        {
            _onConfirm?.Invoke(_inputField.text, _texture);
            gameObject.SetActive(false);
        }

        public void Cancel()
        {
            _onCancel?.Invoke();
            gameObject.SetActive(false);
        }

    }
}