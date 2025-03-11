using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Unity;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonSelectorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private Dungeon _dungeon;
        public UnityEvent<Dungeon> OnSelected { get; private set; } = new();
        public UnityEvent<Dungeon> OnRemoved { get; private set; } = new();
        public UnityEvent<Dungeon> OnInfo { get; private set; } = new();
        public string Label => _label.text;
        [SerializeField]
        private Button _removeButton;
        [SerializeField]
        private TextMeshProUGUI _label;
        [SerializeField]
        private Image _selectionHighlighter;
        [SerializeField]
        private Color _highlightColor;
        private Color _hidden = new(0, 0, 0, 0);
        void Awake()
        {
            Assertion.NotNull(this, _removeButton, _label, _selectionHighlighter);
        }
        public void Initialize(Dungeon dungeon, bool isSelected)
        {
            _dungeon = dungeon;
            if (isSelected)
            {
                _label.text = $"<b>{_dungeon.Name} (Open)</b>";
                _removeButton.interactable = false;
            }
            else
            {
                _label.text = _dungeon.Name;
                _removeButton.interactable = true;
            }
        }
        private void Highlight() => _selectionHighlighter.color = _highlightColor;
        private void Unhighlight() => _selectionHighlighter.color = _hidden;
        public void Select() => OnSelected.Invoke(_dungeon);
        public void Remove() => OnRemoved.Invoke(_dungeon);
        public void Info() => OnInfo.Invoke(_dungeon);
        public void OnPointerEnter(PointerEventData eventData) => Highlight();
        public void OnPointerExit(PointerEventData eventData) => Unhighlight();
        public void OnPointerClick(PointerEventData eventData) => Select();
    }
}