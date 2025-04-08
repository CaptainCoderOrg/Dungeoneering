using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterTileSelector : MonoBehaviour, IClickHandler, IMouseEnteredHandler
    {
        [SerializeField] private Color _highlightColor = Color.green;
        [SerializeField] private Color _attackRangeColor = Color.blue;
        [SerializeField] private Color _validAttackTarget = Color.red;
        [SerializeField] private Color _selectedColor = Color.yellow;
        [AssertIsSet][SerializeField] private MeshRenderer _highlightRenderer;
        public event System.Action OnClicked;
        public event System.Action OnMouseEntered;
        public event System.Action OnMouseExited;

        public void Highlight()
        {
            _highlightRenderer.enabled = true;
            _highlightRenderer.material.color = _highlightColor;
        }

        public void ShowAttackRange()
        {
            _highlightRenderer.enabled = true;
            _highlightRenderer.material.color = _attackRangeColor;
        }

        public void ValidAttackTarget()
        {
            _highlightRenderer.enabled = true;
            _highlightRenderer.material.color = _validAttackTarget;
        }

        internal void Selected()
        {
            _highlightRenderer.enabled = true;
            _highlightRenderer.material.color = _selectedColor;
        }

        public void Hide()
        {
            _highlightRenderer.enabled = false;
        }

        public void ClearEvents()
        {
            OnClicked = null;
            OnMouseEntered = null;
            OnMouseExited = null;
        }

        public void OnClick() => OnClicked?.Invoke();
        public void OnEnter() => OnMouseEntered?.Invoke();
        public void OnExit() => OnMouseExited?.Invoke();
    }
}