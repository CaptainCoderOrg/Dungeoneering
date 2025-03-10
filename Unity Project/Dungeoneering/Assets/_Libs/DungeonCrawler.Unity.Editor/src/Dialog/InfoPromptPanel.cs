using TMPro;

using UnityEngine;
namespace CaptainCoder.Unity.UI
{
    public class InfoPromptPanel : MonoBehaviour
    {
        [field: SerializeField]
        private TextMeshProUGUI _promptLabel;
        void Awake()
        {
            Assertion.NotNull(this, _promptLabel);
        }

        public void ShowInfo(string message)
        {
            _promptLabel.text = message;
            gameObject.SetActive(true);
        }

    }
}