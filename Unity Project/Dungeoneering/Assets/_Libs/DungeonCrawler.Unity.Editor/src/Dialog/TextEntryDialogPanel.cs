using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Unity.UI
{
    public class TextEntryDialogPanel : MonoBehaviour
    {
        [field: SerializeField]
        private TextMeshProUGUI _promptLabel;
        [field: SerializeField]
        private Button _confirmButton;
        [field: SerializeField]
        private TextMeshProUGUI _confirmLabel;
        [field: SerializeField]
        private TextMeshProUGUI _errorLabel;
        [field: SerializeField]
        private TextMeshProUGUI _cancelLabel;
        [field: SerializeField]
        private TMP_InputField _inputField;
        private Action<string> _onConfirm;
        private Action _onCancel;
        private Func<string, string> _validate;

        void Awake()
        {
            Assertion.NotNull(this, _promptLabel, _confirmButton, _confirmLabel, _cancelLabel);
            _inputField.onValueChanged.AddListener(HandleValueChanged);
        }

        private void HandleValueChanged(string value)
        {
            if (value.Trim() == string.Empty)
            {
                _confirmButton.interactable = false;
                _errorLabel.gameObject.SetActive(false);
                return;
            }
            string error = _validate?.Invoke(value);
            if (error != null)
            {
                _confirmButton.interactable = false;
                _errorLabel.text = error;
                _errorLabel.gameObject.SetActive(true);
                return;
            }
            _errorLabel.gameObject.SetActive(false);
            _confirmButton.interactable = true;
        }

        public void ShowError(string error)
        {
            gameObject.SetActive(true);
            _confirmButton.interactable = false;
            _errorLabel.text = error;
            _errorLabel.gameObject.SetActive(true);
        }

        public void Prompt(string prompt, string confirmText, string cancelText, Action<string> onConfirm, Func<string, string> validate, Action onCancel = null)
        {
            _errorLabel.gameObject.SetActive(false);
            _inputField.text = string.Empty;
            _confirmButton.interactable = false;
            _promptLabel.text = prompt;
            _confirmLabel.text = confirmText;
            _cancelLabel.text = cancelText;
            _validate = validate;
            _onConfirm = onConfirm;
            _onCancel = onCancel;
            gameObject.SetActive(true);
        }

        public void Confirm()
        {
            _onConfirm?.Invoke(_inputField.text);
            gameObject.SetActive(false);
        }

        public void Cancel()
        {
            _onCancel?.Invoke();
            gameObject.SetActive(false);
        }

    }
}