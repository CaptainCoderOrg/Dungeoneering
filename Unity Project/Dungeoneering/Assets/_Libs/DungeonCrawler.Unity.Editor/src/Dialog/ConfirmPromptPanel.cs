using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Unity.UI
{
    public class ConfirmPromptPanel : MonoBehaviour
    {
        [field: SerializeField]
        private TextMeshProUGUI _promptLabel;
        [field: SerializeField]
        private Button _confirmButton;
        private Action _onConfirm;
        private Action _onCancel;

        void Awake()
        {
            Assertion.NotNull(this, _promptLabel, _confirmButton);
        }
        public void Prompt(string prompt, Action onConfirm, Action onCancel = null)
        {
            _promptLabel.text = prompt;
            _onConfirm = onConfirm;
            _onCancel = onCancel;
            gameObject.SetActive(true);
        }

        public void Confirm()
        {
            _onConfirm?.Invoke();
            gameObject.SetActive(false);
        }

        public void Cancel()
        {
            _onCancel?.Invoke();
            gameObject.SetActive(false);
        }

    }
}